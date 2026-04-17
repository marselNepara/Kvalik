using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Services;

namespace MatyeApp.Views;

public partial class ProfileView : UserControl
{
    public ProfileView()
    {
        InitializeComponent();
        LoadProfile();
    }

    private void LoadProfile()
    {
        if (SessionService.currentUser == null) return;
        var ctrl = new ProfileController(new AppDbContext());
        var user = ctrl.GetProfile(SessionService.currentUser.userId);
        if (user == null) return;

        this.FindControl<TextBlock>("LoginLabel")!.Text = user.login;
        this.FindControl<TextBlock>("EmailLabel")!.Text = user.email;
        this.FindControl<TextBlock>("PhoneLabel")!.Text = user.phone;
        this.FindControl<TextBlock>("RoleLabel")!.Text = user.Role?.roleName ?? "-";
        this.FindControl<TextBlock>("BalanceLabel")!.Text = $"{user.balance:N2} ₽";

        var appointments = ctrl.GetUserAppointments(user.userId);
        this.FindControl<Avalonia.Controls.DataGrid>("AppointmentsGrid")!.ItemsSource = appointments;
    }

    private void TopUpBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new PaymentView());
}
