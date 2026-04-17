using System;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using MatyeApp.Services;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class AuthController
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }
    public User? Login(string login, string password)
    {
        var user = _db.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.login == login && u.password == password);

        if (user == null) return null;
        SessionService.currentUser = user;
        return user;
    }
    public void Register(User newUser)
    {
        if (_db.Users.Any(u => u.login == newUser.login))
            throw new Exception("Пользователь с таким логином уже существует.");

        var role = _db.Roles.FirstOrDefault(r => r.roleName == "Пользователь");
        if (role == null)
        {
            role = new Role { roleName = "Пользователь" };
            _db.Roles.Add(role);
            _db.SaveChanges();
        }

        newUser.roleId = role.roleId;
        newUser.createdAt = DateTime.UtcNow;
        _db.Users.Add(newUser);
        _db.SaveChanges();
    }

    public void Logout()
    {
        SessionService.logout();
    }
}
