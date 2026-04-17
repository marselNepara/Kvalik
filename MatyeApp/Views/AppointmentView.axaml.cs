using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class AppointmentView : UserControl
{
    private readonly AppointmentController _ctrl;
    private readonly int _serviceId;
    private List<MasterService> _masterServices = new();

    public AppointmentView(int serviceId = 0)
    {
        InitializeComponent();
        _ctrl = new AppointmentController(new AppDbContext());
        _serviceId = serviceId;
        LoadMasters();
    }

    private void LoadMasters()
    {
        if (_serviceId == 0) return;
        _masterServices = _ctrl.GetAvailableMasterServices(_serviceId);
        var box = this.FindControl<ComboBox>("MasterBox")!;
        box.ItemsSource = _masterServices;
        box.DisplayMemberBinding = new Avalonia.Data.Binding("Master.User.login");
    }

    private async void BookBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionService.currentUser == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Необходимо войти в систему.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var masterService = this.FindControl<ComboBox>("MasterBox")?.SelectedItem as MasterService;
        var datePicker = this.FindControl<DatePicker>("DatePicker")!;

        if (masterService == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите мастера.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }
        if (datePicker.SelectedDate == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите дату.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var date = datePicker.SelectedDate.Value.DateTime;
        var appt = _ctrl.CreateAppointment(SessionService.currentUser.userId, masterService.masterServiceId, date);
        await MessageBoxManager.GetMessageBoxStandard("Успех",
            $"Вы записаны! Ваш номер в очереди: {appt.queueNumber}", ButtonEnum.Ok, Icon.Success).ShowAsync();
        App.Navigation.Navigate(new ServicesView());
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new ServicesView());
}
