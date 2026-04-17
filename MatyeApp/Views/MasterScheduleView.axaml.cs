using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class MasterScheduleView : UserControl
{
    public MasterScheduleView()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        if (SessionService.currentUser == null) return;
        var db = new AppDbContext();

        var master = db.Masters.FirstOrDefault(m => m.userId == SessionService.currentUser.userId);
        if (master == null) return;

        var appointments = db.Appointments
            .Include(a => a.User)
            .Include(a => a.MasterService).ThenInclude(ms => ms.Service)
            .Where(a => a.MasterService.masterId == master.masterId)
            .OrderBy(a => a.appointmentDate)
            .ToList();

        this.FindControl<Avalonia.Controls.DataGrid>("AppointmentsGrid")!.ItemsSource = appointments;

        var masterServices = new MastersController(db).GetMasterServices(master.masterId);
        this.FindControl<Avalonia.Controls.DataGrid>("ServicesGrid")!.ItemsSource = masterServices;
    }

    private async void RequestUpgradeBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionService.currentUser == null) return;
        var db = new AppDbContext();
        var master = db.Masters.FirstOrDefault(m => m.userId == SessionService.currentUser.userId);
        if (master == null) { await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Профиль мастера не найден.", ButtonEnum.Ok, Icon.Error).ShowAsync(); return; }

        db.QualificationRequests.Add(new QualificationRequest
        {
            masterId = master.masterId,
            requestDate = DateTime.UtcNow,
            status = "На рассмотрении"
        });
        db.SaveChanges();

        await MessageBoxManager.GetMessageBoxStandard("Успех", "Заявка на повышение квалификации отправлена.", ButtonEnum.Ok, Icon.Success).ShowAsync();
    }
}
