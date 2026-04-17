using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MatyeApp.Controllers;
using MatyeApp.Data;
using Microsoft.Extensions.DependencyInjection;

namespace MatyeApp;

public partial class App : Application
{
    public static NavigationController Navigation { get; private set; } = new NavigationController();
    public static ServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<AppDbContext>();
        serviceCollection.AddSingleton<NavigationController>(_ => Navigation);
        serviceCollection.AddTransient<AuthController>();
        serviceCollection.AddTransient<ServicesController>();
        serviceCollection.AddTransient<MastersController>();
        serviceCollection.AddTransient<AppointmentController>();
        serviceCollection.AddTransient<ProfileController>();
        serviceCollection.AddTransient<PaymentController>();
        serviceCollection.AddTransient<ReviewController>();
        serviceCollection.AddTransient<AdminController>();
        Services = serviceCollection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
