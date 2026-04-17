using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class AdminUsersView : UserControl
{
    private readonly AdminController _ctrl;
    private List<User> _allUsers = new();
    private int _currentPage = 1;
    private const int PageSize = 10;

    public AdminUsersView()
    {
        InitializeComponent();
        _ctrl = new AdminController(new AppDbContext());
        LoadData();
    }

    private void LoadData()
    {
        _allUsers = _ctrl.GetAllUsers();
        int total = _allUsers.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        var page = _allUsers.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();
        this.FindControl<Avalonia.Controls.DataGrid>("UsersGrid")!.ItemsSource = page;
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_currentPage} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Записей: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _currentPage > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _currentPage < totalPages;
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _currentPage--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _currentPage++; LoadData(); }

    private void EditBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("UsersGrid")!;
        if (grid.SelectedItem is User user)
            App.Navigation.Navigate(new AdminUserEditView(user, LoadData));
        else
            MessageBoxManager.GetMessageBoxStandard("Внимание", "Выберите пользователя в таблице.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
    }

    private void UsersGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("UsersGrid")!;
        if (grid.SelectedItem is User user)
            App.Navigation.Navigate(new AdminUserEditView(user, LoadData));
    }

    private async void DeleteBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("UsersGrid")!;
        if (grid.SelectedItem is not User user) return;

        var result = await MessageBoxManager.GetMessageBoxStandard(
            "Удаление", $"Удалить пользователя «{user.login}»?", ButtonEnum.YesNo, Icon.Warning).ShowAsync();
        if (result == ButtonResult.Yes)
        {
            _ctrl.DeleteUser(user.userId);
            LoadData();
        }
    }
}
