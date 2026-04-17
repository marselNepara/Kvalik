using System;
using System.Collections.Generic;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class ServicesController
{
    private readonly AppDbContext _db;
    private const int PageSize = 10;

    public ServicesController(AppDbContext db) => _db = db;

    public List<Service> GetServices(int page, string filter = "", string sortOrder = "asc",
        int? categoryId = null, int? collectionId = null)
    {
        var query = _db.Services
            .Include(s => s.Category)
            .Include(s => s.Collection)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(s => s.serviceName.ToLower().Contains(filter.ToLower()));
        if (categoryId.HasValue)
            query = query.Where(s => s.categoryId == categoryId.Value);
        if (collectionId.HasValue)
            query = query.Where(s => s.collectionId == collectionId.Value);

        query = sortOrder == "desc"
            ? query.OrderByDescending(s => s.serviceName)
            : query.OrderBy(s => s.serviceName);

        return query.Skip((page - 1) * PageSize).Take(PageSize).ToList();
    }

    public int GetTotalCount(string filter = "", int? categoryId = null, int? collectionId = null)
    {
        var query = _db.Services.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(s => s.serviceName.ToLower().Contains(filter.ToLower()));
        if (categoryId.HasValue)
            query = query.Where(s => s.categoryId == categoryId.Value);
        if (collectionId.HasValue)
            query = query.Where(s => s.collectionId == collectionId.Value);
        return query.Count();
    }

    public void AddService(Service service)
    {
        service.createdAt = DateTime.UtcNow;
        service.updatedAt = DateTime.UtcNow;
        _db.Services.Add(service);
        _db.SaveChanges();
    }

    public void UpdateService(Service service)
    {
        service.updatedAt = DateTime.UtcNow;
        _db.Services.Update(service);
        _db.SaveChanges();
    }

    public void DeleteService(int serviceId)
    {
        var s = _db.Services.Find(serviceId);
        if (s != null) { _db.Services.Remove(s); _db.SaveChanges(); }
    }

    public List<Category> GetCategories() => _db.Categories.OrderBy(c => c.categoryName).ToList();
    public List<Collection> GetCollections() => _db.Collections.OrderBy(c => c.collectionName).ToList();
}
