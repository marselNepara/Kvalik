using System;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class PaymentController
{
    private readonly AppDbContext _db;

    public PaymentController(AppDbContext db) => _db = db;

    public void TopUpBalance(int userId, decimal amount, string cardNumber)
    {
        var user = _db.Users.Find(userId) ?? throw new Exception("Пользователь не найден.");
        user.balance += amount;

        // Store only last 4 digits
        var last4 = cardNumber.Length >= 4 ? cardNumber[^4..] : cardNumber;
        _db.Payments.Add(new Payment
        {
            userId = userId,
            amount = amount,
            cardNumber = $"**** **** **** {last4}",
            paymentDate = DateTime.UtcNow
        });
        _db.SaveChanges();

        // Update session
        if (MatyeApp.Services.SessionService.currentUser?.userId == userId)
            MatyeApp.Services.SessionService.currentUser.balance = user.balance;
    }
}
