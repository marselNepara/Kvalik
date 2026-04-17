using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private async void RegisterBtn_Click(object? sender, RoutedEventArgs e)
    {
        var login = this.FindControl<TextBox>("LoginBox")?.Text ?? "";
        var password = this.FindControl<TextBox>("PasswordBox")?.Text ?? "";
        var email = this.FindControl<TextBox>("EmailBox")?.Text ?? "";
        var phone = this.FindControl<TextBox>("PhoneBox")?.Text ?? "";

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Логин и пароль обязательны.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var db = new AppDbContext();
        var auth = new AuthController(db);
        var newUser = new User { login = login, password = password, email = email, phone = phone };
        try
        {
            auth.Register(newUser);
            await MessageBoxManager.GetMessageBoxStandard("Успех", "Регистрация прошла успешно!", ButtonEnum.Ok, Icon.Success).ShowAsync();
            App.Navigation.Navigate(new LoginView());
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok, Icon.Error).ShowAsync();
        }
    }

    private void BackToLogin_Click(object? sender, RoutedEventArgs e)
    {
        App.Navigation.Navigate(new LoginView());
    }
}
