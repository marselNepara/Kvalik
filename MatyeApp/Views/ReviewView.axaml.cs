using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class ReviewView : UserControl
{
    private readonly ReviewController _ctrl;
    private List<Service> _services = new();
    private List<Master> _masters = new();

    public ReviewView()
    {
        InitializeComponent();
        _ctrl = new ReviewController(new AppDbContext());
        var db = new AppDbContext();
        _services = db.Services.OrderBy(s => s.serviceName).ToList();
        _masters = db.Masters.Include(m => m.User).OrderBy(m => m.User.login).ToList();

        var typeBox = this.FindControl<ComboBox>("TypeBox")!;
        typeBox.SelectedIndex = 0;
    }

    private void TypeBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var typeBox = this.FindControl<ComboBox>("TypeBox")!;
        var targetBox = this.FindControl<ComboBox>("TargetBox")!;

        if (typeBox.SelectedIndex == 0)
        {
            targetBox.ItemsSource = _services;
            targetBox.DisplayMemberBinding = new Avalonia.Data.Binding("serviceName");
        }
        else
        {
            targetBox.ItemsSource = _masters;
            targetBox.DisplayMemberBinding = new Avalonia.Data.Binding("User.login");
        }
        targetBox.SelectedIndex = -1;
    }

    private async void SubmitBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionService.currentUser == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Необходимо войти.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        var typeBox = this.FindControl<ComboBox>("TypeBox")!;
        var targetBox = this.FindControl<ComboBox>("TargetBox")!;
        var rating = (int)(this.FindControl<NumericUpDown>("RatingBox")?.Value ?? 5);
        var comment = this.FindControl<TextBox>("CommentBox")?.Text ?? "";

        var review = new Review
        {
            userId = SessionService.currentUser.userId,
            rating = rating,
            comment = comment
        };

        if (typeBox.SelectedIndex == 0)
        {
            if (targetBox.SelectedItem is not Service svc)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите услугу.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
                return;
            }
            review.serviceId = svc.serviceId;
        }
        else
        {
            if (targetBox.SelectedItem is not Master master)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите мастера.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
                return;
            }
            review.masterId = master.masterId;
        }

        _ctrl.AddReview(review);
        await MessageBoxManager.GetMessageBoxStandard("Успех", "Отзыв отправлен.", ButtonEnum.Ok, Icon.Success).ShowAsync();
        App.Navigation.Navigate(new ServicesView());
    }
}
