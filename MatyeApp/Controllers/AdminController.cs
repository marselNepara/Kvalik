using System;
using System.Collections.Generic;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class AdminController
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    public List<User> GetAllUsers()
        => _db.Users.Include(u => u.Role).OrderBy(u => u.login).ToList();

    public void UpdateUser(User user)
    {
        _db.Users.Update(user);
        _db.SaveChanges();
    }

    public void DeleteUser(int userId)
    {
        var user = _db.Users.Find(userId);
        if (user != null) { _db.Users.Remove(user); _db.SaveChanges(); }
    }

    public List<Role> GetRoles() => _db.Roles.ToList();

    public void CreateStaff(User user, string roleName)
    {
        var role = _db.Roles.FirstOrDefault(r => r.roleName == roleName);
        if (role == null)
        {
            role = new Role { roleName = roleName };
            _db.Roles.Add(role);
            _db.SaveChanges();
        }
        user.roleId = role.roleId;
        user.createdAt = DateTime.UtcNow;
        _db.Users.Add(user);
        _db.SaveChanges();

        if (roleName == "Мастер")
        {
            _db.Masters.Add(new Master { userId = user.userId, qualificationLevel = 1, bio = "" });
            _db.SaveChanges();
        }
    }
}
