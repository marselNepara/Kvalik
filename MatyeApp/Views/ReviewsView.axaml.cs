using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Views;

public class ReviewRow
{
    public User? User { get; set; }
    public string ReviewType { get; set; } = "";
    public string TargetName { get; set; } = "";
    public string RatingStars { get; set; } = "";
    public string comment { get; set; } = "";
    public string createdAt { get; set; } = "";
    public int? serviceId { get; set; }
    public int? masterId { get; set; }
}

public partial class ReviewsView : UserControl
{
    private List<ReviewRow> _all = new();
    private int _page = 1;
    private const int PageSize = 10;
    private string? _typeFilter = null;
    private string? _targetFilter = null;

    private List<Service> _services = new();
    private List<Master> _masters = new();

    public ReviewsView()
    {
        InitializeComponent();
        LoadFilters();
        LoadData();
    }

    private void LoadFilters()
    {
        var db = new AppDbContext();
        _services = db.Services.OrderBy(s => s.serviceName).ToList();
        _masters = db.Masters.Include(m => m.User).OrderBy(m => m.User.login).ToList();

        var typeBox = this.FindControl<ComboBox>("TypeFilter")!;
        typeBox.ItemsSource = new[] { "Все отзывы", "Об услуге", "О мастере" };
        typeBox.SelectedIndex = 0;
    }

    private void LoadData()
    {
        var db = new AppDbContext();
        var reviews = db.Reviews
            .Include(r => r.User)
            .Include(r => r.Service)
            .Include(r => r.Master).ThenInclude(m => m!.User)
            .AsQueryable();

        if (_typeFilter == "Об услуге")
            reviews = reviews.Where(r => r.serviceId != null);
        else if (_typeFilter == "О мастере")
            reviews = reviews.Where(r => r.masterId != null);

        var rows = reviews.OrderByDescending(r => r.createdAt).ToList()
            .Select(r => new ReviewRow
            {
                User = r.User,
                ReviewType = r.serviceId != null ? "Услуга" : "Мастер",
                TargetName = r.serviceId != null
                    ? (r.Service?.serviceName ?? "-")
                    : (r.Master?.User?.login ?? "-"),
                RatingStars = new string('★', r.rating) + new string('☆', 5 - r.rating),
                comment = r.comment,
                createdAt = r.createdAt.ToLocalTime().ToString("dd.MM.yyyy"),
                serviceId = r.serviceId,
                masterId = r.masterId
            }).ToList();

        if (!string.IsNullOrEmpty(_targetFilter))
            rows = rows.Where(r => r.TargetName == _targetFilter).ToList();

        _all = rows;

        int total = _all.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        var page = _all.Skip((_page - 1) * PageSize).Take(PageSize).ToList();

        this.FindControl<Avalonia.Controls.DataGrid>("ReviewsGrid")!.ItemsSource = page;
        this.FindControl<TextBlock>("PageLabel")!.Text = $"Страница {_page} из {totalPages}";
        this.FindControl<TextBlock>("StatusLabel")!.Text = $"Отзывов: {total}";
        this.FindControl<Button>("PrevBtn")!.IsEnabled = _page > 1;
        this.FindControl<Button>("NextBtn")!.IsEnabled = _page < totalPages;
    }

    private void TypeFilter_Changed(object? sender, SelectionChangedEventArgs e)
    {
        var box = this.FindControl<ComboBox>("TypeFilter")!;
        var selected = box.SelectedItem?.ToString();
        _typeFilter = selected == "Все отзывы" ? null : selected;
        _targetFilter = null;

        var targetBox = this.FindControl<ComboBox>("TargetFilter")!;
        if (_typeFilter == "Об услуге")
        {
            targetBox.ItemsSource = new[] { "Все" }.Concat(_services.Select(s => s.serviceName)).ToList();
        }
        else if (_typeFilter == "О мастере")
        {
            targetBox.ItemsSource = new[] { "Все" }.Concat(_masters.Select(m => m.User?.login ?? "")).ToList();
        }
        else
        {
            targetBox.ItemsSource = new[] { "Все" };
        }
        targetBox.SelectedIndex = 0;

        _page = 1;
        LoadData();
    }

    private void TargetFilter_Changed(object? sender, SelectionChangedEventArgs e)
    {
        var box = this.FindControl<ComboBox>("TargetFilter")!;
        var selected = box.SelectedItem?.ToString();
        _targetFilter = selected == "Все" ? null : selected;
        _page = 1;
        LoadData();
    }

    private void PrevBtn_Click(object? sender, RoutedEventArgs e) { _page--; LoadData(); }
    private void NextBtn_Click(object? sender, RoutedEventArgs e) { _page++; LoadData(); }
}
