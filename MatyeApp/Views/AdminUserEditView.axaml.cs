using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class AdminUserEditView : UserControl
{
    private readonly User _user;
    private readonly Action _onSaved;
    private readonly AdminController _ctrl;

    public AdminUserEditView(User user, Action onSaved)
    {
        InitializeComponent();
        _user = user;
        _onSaved = onSaved;
        _ctrl = new AdminController(new AppDbContext());
        LoadForm();
    }

    private void LoadForm()
    {
        this.FindControl<TextBox>("LoginBox")!.Text = _user.login;
        this.FindControl<TextBox>("EmailBox")!.Text = _user.email;
        this.FindControl<TextBox>("PhoneBox")!.Text = _user.phone;
        this.FindControl<TextBox>("BalanceBox")!.Text = _user.balance.ToString();

        var roles = _ctrl.GetRoles();
        var roleBox = this.FindControl<ComboBox>("RoleBox")!;
        roleBox.ItemsSource = roles;
        roleBox.DisplayMemberBinding = new Avalonia.Data.Binding("roleName");
        roleBox.SelectedItem = roles.FirstOrDefault(r => r.roleId == _user.roleId);
    }

    private async void SaveBtn_Click(object? sender, RoutedEventArgs e)
    {
        _user.login = this.FindControl<TextBox>("LoginBox")?.Text ?? _user.login;
        _user.email = this.FindControl<TextBox>("EmailBox")?.Text ?? _user.email;
        _user.phone = this.FindControl<TextBox>("PhoneBox")?.Text ?? _user.phone;
        if (decimal.TryParse(this.FindControl<TextBox>("BalanceBox")?.Text, out var bal))
            _user.balance = bal;
        if (this.FindControl<ComboBox>("RoleBox")?.SelectedItem is Role role)
            _user.roleId = role.roleId;

        _ctrl.UpdateUser(_user);
        await MessageBoxManager.GetMessageBoxStandard("Успех", "Сохранено.", ButtonEnum.Ok, Icon.Success).ShowAsync();
        _onSaved();
        App.Navigation.Navigate(new AdminUsersView());
    }

    private void CancelBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new AdminUsersView());
}
