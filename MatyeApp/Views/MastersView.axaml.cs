using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class MastersView : UserControl
{
    private readonly MastersController _ctrl;
    private readonly ServicesController _svcCtrl;
    private int _currentPage = 1;

    public MastersView()
    {
        InitializeComponent();
        _ctrl = new MastersController(new AppDbContext());
        _svcCtrl = new ServicesController(new AppDbContext());
        UpdateRoleVisibility();
        LoadData();
    }

    private void UpdateRoleVisibility()
    {
        var role = SessionService.currentRole;
        bool canEdit = role == "Модератор" || role == "Администратор";
        this.FindControl<Button>("AssignBtn")!.IsVisible = canEdit;
        this.FindControl<Button>("UnassignBtn")!.IsVisible = canEdit;
        this.FindControl<Button>("UpgradeBtn")!.IsVisible = canEdit;
    }

    private void LoadData()
    {
        var data = _ctrl.GetMasters(_currentPage);
        this.FindControl<Avalonia.Controls.DataGrid>("MastersGrid")!.ItemsSource = data;
        int total = _ctrl.GetTotalCount();
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / 10.0));
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_currentPage} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Записей: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _currentPage > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _currentPage < totalPages;
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _currentPage--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _currentPage++; LoadData(); }

    private async void AssignBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("MastersGrid")!;
        if (grid.SelectedItem is not Master master) { await ShowError("Выберите мастера."); return; }

        var services = _svcCtrl.GetServices(1, "", "asc");
        if (!services.Any()) { await ShowError("Нет доступных услуг."); return; }

        // Simple dialog: show list of services
        var dialog = new Window
        {
            Title = "Выбор услуги", Width = 400, Height = 400,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var lb = new ListBox { ItemsSource = services, DisplayMemberBinding = new Avalonia.Data.Binding("serviceName") };
        var btn = new Button { Content = "Привязать", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
        var panel = new StackPanel { Spacing = 10, Margin = new Avalonia.Thickness(16) };
        panel.Children.Add(lb); panel.Children.Add(btn);
        dialog.Content = panel;
        btn.Click += (_, _) =>
        {
            if (lb.SelectedItem is Service svc)
            {
                _ctrl.AssignService(master.masterId, svc.serviceId);
                dialog.Close();
            }
        };
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel != null) await dialog.ShowDialog(topLevel);
        LoadData();
    }

    private async void UnassignBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("MastersGrid")!;
        if (grid.SelectedItem is not Master master) { await ShowError("Выберите мастера."); return; }

        var masterServices = _ctrl.GetMasterServices(master.masterId);
        if (!masterServices.Any()) { await ShowError("У мастера нет привязанных услуг."); return; }

        var dialog = new Window { Title = "Отвязать услугу", Width = 400, Height = 400, WindowStartupLocation = WindowStartupLocation.CenterOwner };
        var lb = new ListBox { ItemsSource = masterServices, DisplayMemberBinding = new Avalonia.Data.Binding("Service.serviceName") };
        var btn = new Button { Content = "Отвязать", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
        var panel = new StackPanel { Spacing = 10, Margin = new Avalonia.Thickness(16) };
        panel.Children.Add(lb); panel.Children.Add(btn);
        dialog.Content = panel;
        btn.Click += (_, _) =>
        {
            if (lb.SelectedItem is MasterService ms)
            {
                _ctrl.UnassignService(ms.masterServiceId);
                dialog.Close();
            }
        };
        var topLevel = TopLevel.GetTopLevel(this) as Window;
        if (topLevel != null) await dialog.ShowDialog(topLevel);
        LoadData();
    }

    private async void UpgradeBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("MastersGrid")!;
        if (grid.SelectedItem is not Master master) { await ShowError("Выберите мастера."); return; }

        var result = await MessageBoxManager.GetMessageBoxStandard(
            "Подтверждение", $"Повысить квалификацию мастера {master.User?.login}?", ButtonEnum.YesNo, Icon.Question).ShowAsync();
        if (result == ButtonResult.Yes)
        {
            _ctrl.UpgradeQualification(master.masterId);
            await MessageBoxManager.GetMessageBoxStandard("Успех", "Квалификация повышена.", ButtonEnum.Ok, Icon.Success).ShowAsync();
            LoadData();
        }
    }

    private System.Threading.Tasks.Task ShowError(string msg)
        => MessageBoxManager.GetMessageBoxStandard("Ошибка", msg, ButtonEnum.Ok, Icon.Warning).ShowAsync();
}
