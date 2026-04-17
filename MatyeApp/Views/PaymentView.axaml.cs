using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MatyeApp.Controllers;
using MatyeApp.Data;
using MatyeApp.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MatyeApp.Views;

public partial class PaymentView : UserControl
{
    public PaymentView()
    {
        InitializeComponent();
    }

    private async void PayBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (SessionService.currentUser == null) return;

        var card = this.FindControl<TextBox>("CardBox")?.Text?.Replace(" ", "") ?? "";
        var amountStr = this.FindControl<TextBox>("AmountBox")?.Text ?? "";

        if (card.Length != 16 || !card.All(char.IsDigit))
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введите корректный номер карты (16 цифр).", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }
        if (!decimal.TryParse(amountStr, out var amount) || amount <= 0)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введите корректную сумму.", ButtonEnum.Ok, Icon.Warning).ShowAsync();
            return;
        }

        try
        {
            var ctrl = new PaymentController(new AppDbContext());
            ctrl.TopUpBalance(SessionService.currentUser.userId, amount, card);
            await MessageBoxManager.GetMessageBoxStandard("Успех", $"Баланс пополнен на {amount:N2} ₽", ButtonEnum.Ok, Icon.Success).ShowAsync();
            App.Navigation.Navigate(new ProfileView());
        }
        catch (System.Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok, Icon.Error).ShowAsync();
        }
    }

    private void BackBtn_Click(object? sender, RoutedEventArgs e)
        => App.Navigation.Navigate(new ProfileView());
}
