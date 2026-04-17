using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using MatyeApp.Models;
using MatyeApp.Services;

namespace MatyeApp.Views;

public partial class ServiceDetailView : UserControl
{
    private readonly Service _service;

    public ServiceDetailView(Service service)
    {
        InitializeComponent();
        _service = service;
        LoadService();
    }

    private void LoadService()
    {
        this.FindControl<TextBlock>("NameLabel")!.Text = _service.serviceName;
        this.FindControl<TextBlock>("DescLabel")!.Text = _service.description;
        this.FindControl<TextBlock>("PriceLabel")!.Text = $"{_service.price:N2} ₽";
        this.FindControl<TextBlock>("CategoryLabel")!.Text = _service.Category?.categoryName ?? "-";
        this.FindControl<TextBlock>("CollectionLabel")!.Text = _service.Collection?.collectionName ?? "-";
        this.FindControl<TextBlock>("CreatedLabel")!.Text = _service.createdAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        this.FindControl<TextBlock>("UpdatedLabel")!.Text = _service.updatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

        // Load image via Avalonia asset loader
        if (!string.IsNullOrWhiteSpace(_service.imageUrl))
        {
            var img = MatyeApp.Services.ImagePathConverter.Instance
                .Convert(_service.imageUrl, typeof(Bitmap), null, System.Globalization.CultureInfo.InvariantCulture)
                as Bitmap;
            if (img != null)
                this.FindControl<Image>("ServiceImage")!.Source = img;
        }

        // Show book button only for Пользователь
        this.FindControl<Button>("BookBtn")!.IsVisible = SessionService.currentRole == "Пользователь";
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new ServicesView());

    private void BookBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new AppointmentView(_service.serviceId));
}
