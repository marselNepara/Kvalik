using System;
using MatyeApp.Data;
using MatyeApp.Models;

namespace MatyeApp.Controllers;

public class ReviewController
{
    private readonly AppDbContext _db;

    public ReviewController(AppDbContext db) => _db = db;

    public void AddReview(Review review)
    {
        review.createdAt = DateTime.UtcNow;
        _db.Reviews.Add(review);
        _db.SaveChanges();
    }
}
