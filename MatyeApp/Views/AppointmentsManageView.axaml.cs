using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class AppointmentsManageView : UserControl
{
    private List<Appointment> _all = new();
    private int _page = 1;
    private const int PageSize = 10;
    private string? _statusFilter = null;

    private static readonly string[] Statuses = { "Ожидание", "В процессе", "Завершено", "Отменено" };

    public AppointmentsManageView()
    {
        InitializeComponent();

        var filterBox = this.FindControl<ComboBox>("StatusFilter")!;
        filterBox.ItemsSource = new[] { "Все статусы" }.Concat(Statuses).ToList();
        filterBox.SelectedIndex = 0;

        LoadData();
    }

    private void LoadData()
    {
        var db = new AppDbContext();
        var query = db.Appointments
            .Include(a => a.User)
            .Include(a => a.MasterService).ThenInclude(ms => ms.Service)
            .Include(a => a.MasterService).ThenInclude(ms => ms.Master).ThenInclude(m => m.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(_statusFilter))
            query = query.Where(a => a.status == _statusFilter);

        _all = query.OrderByDescending(a => a.appointmentDate).ToList();

        int total = _all.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        var page = _all.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

        this.FindControl<Avalonia.Controls.DataGrid>("AppointmentsGrid")!.ItemsSource = page;
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_page} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Записей: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _page > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _page < totalPages;
    }

    private void StatusFilter_Changed(object? sender, SelectionChangedEventArgs e)
    {
        var box = this.FindControl<ComboBox>("StatusFilter")!;
        var selected = box.SelectedItem?.ToString();
        _statusFilter = selected == "Все статусы" ? null : selected;
        _page = 1;
        LoadData();
    }

    private async void ChangeStatusBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("AppointmentsGrid")!;
        if (grid.SelectedItem is not Appointment appt)
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Выберите запись.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        // Show status picker dialog
        var dialog = new Window
        {
            Title = "Изменить статус",
            Width = 300, Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var lb = new ListBox { ItemsSource = Statuses, Margin = new Avalonia.Thickness(16, 12) };
        lb.SelectedItem = appt.status;

        var btn = new Button
        {
            Content = "Применить",
            Classes = { "primary" },
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 0, 0, 12)
        };

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(lb);
        panel.Children.Add(btn);
        dialog.Content = panel;

        btn.Click += async (_, _) =>
        {
            if (lb.SelectedItem is not string newStatus) return;

            var db = new AppDbContext();
            var record = db.Appointments.Find(appt.appointmentId);
            if (record != null)
            {
                record.status = newStatus;
                db.SaveChanges();
            }
            dialog.Close();
            LoadData();
            await MessageBoxManager.GetMessageBoxStandard("Готово", $"Статус изменён на «{newStatus}».", ButtonEnum.Ok, Icon.Success).ShowAsync();
        };

        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel != null) await dialog.ShowDialog(topLevel);
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _page--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _page++; LoadData(); }
}
