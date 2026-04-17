using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class AdminStaffView : UserControl
{
    public AdminStaffView()
    {
        InitializeComponent();
        this.FindControl<ComboBox>("RoleBox")!.SelectedIndex = 0;
    }

    private async void CreateBtn_Click(object? sender, RoutedEventArgs e)
    {
        var login = this.FindControl<TextBox>("LoginBox")?.Text ?? "";
        var password = this.FindControl<TextBox>("PasswordBox")?.Text ?? "";
        var email = this.FindControl<TextBox>("EmailBox")?.Text ?? "";
        var phone = this.FindControl<TextBox>("PhoneBox")?.Text ?? "";
        var roleItem = this.FindControl<ComboBox>("RoleBox")?.SelectedItem as ComboBoxItem;
        var roleName = roleItem?.Content?.ToString() ?? "Мастер";

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Логин и пароль обязательны.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var ctrl = new AdminController(new AppDbContext());
        var user = new User { login = login, password = password, email = email, phone = phone };
        ctrl.CreateStaff(user, roleName);

        await MessageBoxManager.GetMessageBoxStandard("Успех", $"Сотрудник «{login}» создан как {roleName}.", ButtonEnum.Ok, Icon.Success).ShowAsync();
        App.Navigation.Navigate(new AdminUsersView());
    }
}
