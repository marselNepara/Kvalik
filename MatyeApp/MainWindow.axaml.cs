using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MatyeApp.Controllers;
using MatyeApp.Services;
using MatyeApp.Views;

namespace MatyeApp;

public partial class MainWindow : Window
{
    private NavigationController _nav;

    public MainWindow()
    {
        InitializeComponent();
        _nav = App.Navigation;
        _nav.SetContentArea(this.FindControl<ContentControl>("ContentArea")!);
        BuildNavMenu();
        _nav.Navigate(new LoginView());
    }

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void MinimizeBtn_Click(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private async void CloseBtn_Click(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Выход", "Вы уверены, что хотите выйти?",
            ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
        var result = await box.ShowAsync();
        if (result == ButtonResult.Yes)
            Close();
    }

    public void BuildNavMenu()
    {
        var panel = this.FindControl<StackPanel>("NavPanel")!;
        panel.Children.Clear();
        var role = SessionService.currentRole;

        void AddBtn(string text, System.Action action)
        {
            var btn = new Button
            {
                Content = text,
                Classes = { "nav" }
            };
            btn.Click += (_, _) => action();
            panel.Children.Add(btn);
        }

        // Услуги и Мастера — для всех
        AddBtn("Услуги", () => _nav.Navigate(new ServicesView()));
        AddBtn("Мастера", () => _nav.Navigate(new MastersView()));
        AddBtn("Отзывы", () => _nav.Navigate(new ReviewsView()));

        if (role == "Пользователь")
        {
            AddBtn("Профиль", () => _nav.Navigate(new ProfileView()));
            AddBtn("Оставить отзыв", () => _nav.Navigate(new ReviewView()));
        }
        if (role == "Мастер")
        {
            AddBtn("Моё расписание", () => _nav.Navigate(new MasterScheduleView()));
        }
        if (role == "Администратор")
        {
            AddBtn("Записи", () => _nav.Navigate(new AppointmentsManageView()));
            AddBtn("Квалификация", () => _nav.Navigate(new QualificationRequestsView()));
            AddBtn("Пользователи", () => _nav.Navigate(new AdminUsersView()));
            AddBtn("Добавить сотрудника", () => _nav.Navigate(new AdminStaffView()));
        }
        if (role == "Модератор")
        {
            AddBtn("Записи", () => _nav.Navigate(new AppointmentsManageView()));
        }
        if (role == "Гость")
        {
            AddBtn("Войти", () => _nav.Navigate(new LoginView()));
        }
        else
        {
            AddBtn("Выйти", () =>
            {
                SessionService.logout();
                BuildNavMenu();
                _nav.Navigate(new LoginView());
            });
        }
    }
}
