using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class ServicesView : UserControl
{
    private readonly ServicesController _ctrl;
    private int _currentPage = 1;
    private string _sortOrder = "asc";
    private string _filter = "";
    private int? _categoryId = null;
    private int? _collectionId = null;
    private List<Category> _categories = new();
    private List<Collection> _collections = new();

    public ServicesView()
    {
        InitializeComponent();
        _ctrl = new ServicesController(new AppDbContext());
        LoadFilters();
        LoadData();
        UpdateRoleVisibility();
    }

    private void UpdateRoleVisibility()
    {
        var role = SessionService.currentRole;
        bool canEdit = role == "Модератор" || role == "Администратор";
        this.FindControl<Button>("AddBtn")!.IsVisible = canEdit;
        this.FindControl<Button>("EditBtn")!.IsVisible = canEdit;
        this.FindControl<Button>("DeleteBtn")!.IsVisible = canEdit;
    }

    private void LoadFilters()
    {
        _categories = _ctrl.GetCategories();
        _collections = _ctrl.GetCollections();

        var catBox = this.FindControl<ComboBox>("CategoryFilter")!;
        catBox.ItemsSource = new[] { new Category { categoryId = 0, categoryName = "Все категории" } }.Concat(_categories).ToList();
        catBox.DisplayMemberBinding = new Avalonia.Data.Binding("categoryName");
        catBox.SelectedIndex = 0;

        var colBox = this.FindControl<ComboBox>("CollectionFilter")!;
        colBox.ItemsSource = new[] { new Collection { collectionId = 0, collectionName = "Все коллекции" } }.Concat(_collections).ToList();
        colBox.DisplayMemberBinding = new Avalonia.Data.Binding("collectionName");
        colBox.SelectedIndex = 0;
    }

    private void LoadData()
    {
        var data = _ctrl.GetServices(_currentPage, _filter, _sortOrder, _categoryId, _collectionId);
        this.FindControl<Avalonia.Controls.DataGrid>("ServicesGrid")!.ItemsSource = data;

        int total = _ctrl.GetTotalCount(_filter, _categoryId, _collectionId);
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / 10.0));
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_currentPage} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Записей: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _currentPage > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _currentPage < totalPages;
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        _filter = this.FindControl<TextBox>("SearchBox")?.Text ?? "";
        _currentPage = 1;
        LoadData();
    }

    private void Filter_Changed(object? sender, SelectionChangedEventArgs e)
    {
        var cat = this.FindControl<ComboBox>("CategoryFilter")?.SelectedItem as Category;
        var col = this.FindControl<ComboBox>("CollectionFilter")?.SelectedItem as Collection;
        _categoryId = (cat?.categoryId == 0) ? null : cat?.categoryId;
        _collectionId = (col?.collectionId == 0) ? null : col?.collectionId;
        _currentPage = 1;
        LoadData();
    }

    private void SortBtn_Click(object? sender, RoutedEventArgs e)
    {
        _sortOrder = _sortOrder == "asc" ? "desc" : "asc";
        var btn = this.FindControl<Button>("SortBtn")!;
        btn.Content = _sortOrder == "asc" ? "Сортировка А-Я" : "Сортировка Я-А";
        _currentPage = 1;
        LoadData();
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _currentPage--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _currentPage++; LoadData(); }

    private void ServicesGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("ServicesGrid")!;
        if (grid.SelectedItem is Service service)
            App.Navigation.Navigate(new ServiceDetailView(service));
    }

    private void AddBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new ServiceEditView(null, () => LoadData()));

    private void EditBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("ServicesGrid")!;
        if (grid.SelectedItem is Service service)
            App.Navigation.Navigate(new ServiceEditView(service, () => LoadData()));
        else
            MessageBoxManager.GetMessageBoxStandard("Внимание", "Выберите услугу в таблице.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
    }

    private async void DeleteBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("ServicesGrid")!;
        if (grid.SelectedItem is not Service service) return;

        var result = await MessageBoxManager.GetMessageBoxStandard(
            "Удаление", $"Удалить услугу «{service.serviceName}»?", ButtonEnum.YesNo, Icon.Warning).ShowAsync();
        if (result == ButtonResult.Yes)
        {
            _ctrl.DeleteService(service.serviceId);
            LoadData();
        }
    }
}
