using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private async void LoginBtn_Click(object? sender, RoutedEventArgs e)
    {
        var login = this.FindControl<TextBox>("LoginBox")?.Text ?? "";
        var password = this.FindControl<TextBox>("PasswordBox")?.Text ?? "";

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните все поля.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var db = new AppDbContext();
        var auth = new AuthController(db);
        if (auth.Login(login, password) != null)
        {
            var mainWindow = (MainWindow)TopLevel.GetTopLevel(this)!;
            mainWindow.BuildNavMenu();
            App.Navigation.Navigate(new ServicesView());
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Неверный логин или пароль.", ButtonEnum.Ok, Icon.Error).ShowAsync();
        }
    }

    private void RegisterLink_Click(object? sender, RoutedEventArgs e)
    {
        App.Navigation.Navigate(new RegisterView());
    }
}
