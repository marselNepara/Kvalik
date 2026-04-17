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

public partial class ServiceEditView : UserControl
{
    private readonly Service? _service;
    private readonly Action _onSaved;
    private readonly ServicesController _ctrl;

    public ServiceEditView(Service? service, Action onSaved)
    {
        InitializeComponent();
        _service = service;
        _onSaved = onSaved;
        _ctrl = new ServicesController(new AppDbContext());
        LoadForm();
    }

    private void LoadForm()
    {
        this.FindControl<TextBlock>("TitleLabel")!.Text = _service == null ? "Добавить услугу" : "Редактировать услугу";

        var cats = _ctrl.GetCategories();
        var cols = _ctrl.GetCollections();

        var catBox = this.FindControl<ComboBox>("CategoryBox")!;
        catBox.ItemsSource = cats;
        catBox.DisplayMemberBinding = new Avalonia.Data.Binding("categoryName");

        var colBox = this.FindControl<ComboBox>("CollectionBox")!;
        colBox.ItemsSource = cols;
        colBox.DisplayMemberBinding = new Avalonia.Data.Binding("collectionName");

        if (_service != null)
        {
            this.FindControl<TextBox>("NameBox")!.Text = _service.serviceName;
            this.FindControl<TextBox>("DescBox")!.Text = _service.description;
            this.FindControl<TextBox>("PriceBox")!.Text = _service.price.ToString();
            this.FindControl<TextBox>("ImageBox")!.Text = _service.imageUrl;
            catBox.SelectedItem = cats.FirstOrDefault(c => c.categoryId == _service.categoryId);
            colBox.SelectedItem = cols.FirstOrDefault(c => c.collectionId == _service.collectionId);
        }
    }

    private async void SaveBtn_Click(object? sender, RoutedEventArgs e)
    {
        var name = this.FindControl<TextBox>("NameBox")?.Text ?? "";
        var desc = this.FindControl<TextBox>("DescBox")?.Text ?? "";
        var priceStr = this.FindControl<TextBox>("PriceBox")?.Text ?? "";
        var imageUrl = this.FindControl<TextBox>("ImageBox")?.Text ?? "";
        var cat = this.FindControl<ComboBox>("CategoryBox")?.SelectedItem as Category;
        var col = this.FindControl<ComboBox>("CollectionBox")?.SelectedItem as Collection;

        if (string.IsNullOrWhiteSpace(name) || cat == null || col == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Заполните обязательные поля.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }
        if (!decimal.TryParse(priceStr, out var price))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Некорректная цена.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        if (_service == null)
        {
            _ctrl.AddService(new Service
            {
                serviceName = name, description = desc, price = price,
                categoryId = cat.categoryId, collectionId = col.collectionId, imageUrl = imageUrl
            });
        }
        else
        {
            _service.serviceName = name; _service.description = desc; _service.price = price;
            _service.categoryId = cat.categoryId; _service.collectionId = col.collectionId; _service.imageUrl = imageUrl;
            _ctrl.UpdateService(_service);
        }

        await MessageBoxManager.GetMessageBoxStandard("Успех", "Сохранено.", ButtonEnum.Ok, Icon.Success).ShowAsync();
        _onSaved();
        App.Navigation.Navigate(new ServicesView());
    }

    private void CancelBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new ServicesView());
}
