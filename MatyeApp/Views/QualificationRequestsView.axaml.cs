using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class QualificationRequestsView : UserControl
{
    private List<QualificationRequest> _all = new();
    private int _page = 1;
    private const int PageSize = 10;
    private string? _statusFilter = null;

    public QualificationRequestsView()
    {
        InitializeComponent();

        var filterBox = this.FindControl<ComboBox>("StatusFilter")!;
        filterBox.ItemsSource = new[] { "Все статусы", "На рассмотрении", "Одобрено", "Отклонено" };
        filterBox.SelectedIndex = 0;

        LoadData();
    }

    private void LoadData()
    {
        var db = new AppDbContext();
        var query = db.QualificationRequests
            .Include(r => r.Master).ThenInclude(m => m.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(_statusFilter))
            query = query.Where(r => r.status == _statusFilter);

        _all = query.OrderByDescending(r => r.requestDate).ToList();

        int total = _all.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        var page = _all.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

        this.FindControl<Avalonia.Controls.DataGrid>("RequestsGrid")!.ItemsSource = page;
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_page} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Записей: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _page > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _page < totalPages;
    }

    private void StatusFilter_Changed(object? sender, SelectionChangedEventArgs e)
    {
        var box = this.FindControl<ComboBox>("StatusFilter")!;
        var selected = box.SelectedItem?.ToString();
        _statusFilter = selected == "Все статусы" ? null : selected;
        _page = 1;
        LoadData();
    }

    private async void ApproveBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("RequestsGrid")!;
        if (grid.SelectedItem is not QualificationRequest req)
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Выберите заявку.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }
        if (req.status != "На рассмотрении")
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Можно одобрить только заявку со статусом «На рассмотрении».", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var confirm = await MessageBoxManager.GetMessageBoxStandard(
            "Подтверждение", $"Одобрить заявку мастера «{req.Master?.User?.login}»?\nКвалификация будет повышена с {req.Master?.qualificationLevel} до {req.Master?.qualificationLevel + 1}.",
            ButtonEnum.YesNo, Icon.Question).ShowAsync();

        if (confirm != ButtonResult.Yes) return;

        var db = new AppDbContext();
        var request = db.QualificationRequests.Find(req.requestId);
        if (request != null)
        {
            request.status = "Одобрено";
            // Повышаем квалификацию мастера
            var master = db.Masters.Find(req.masterId);
            if (master != null) master.qualificationLevel++;
            db.SaveChanges();
        }

        LoadData();
        await MessageBoxManager.GetMessageBoxStandard("Готово", "Заявка одобрена, квалификация повышена.", ButtonEnum.Ok, Icon.Success).ShowAsync();
    }

    private async void RejectBtn_Click(object? sender, RoutedEventArgs e)
    {
        var grid = this.FindControl<Avalonia.Controls.DataGrid>("RequestsGrid")!;
        if (grid.SelectedItem is not QualificationRequest req)
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Выберите заявку.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }
        if (req.status != "На рассмотрении")
        {
            await MessageBoxManager.GetMessageBoxStandard("Внимание", "Можно отклонить только заявку со статусом «На рассмотрении».", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var confirm = await MessageBoxManager.GetMessageBoxStandard(
            "Подтверждение", $"Отклонить заявку мастера «{req.Master?.User?.login}»?",
            ButtonEnum.YesNo, Icon.Warning).ShowAsync();

        if (confirm != ButtonResult.Yes) return;

        var db = new AppDbContext();
        var request = db.QualificationRequests.Find(req.requestId);
        if (request != null)
        {
            request.status = "Отклонено";
            db.SaveChanges();
        }

        LoadData();
        await MessageBoxManager.GetMessageBoxStandard("Готово", "Заявка отклонена.", ButtonEnum.Ok, Icon.Success).ShowAsync();
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _page--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _page++; LoadData(); }
}
