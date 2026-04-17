using System.Collections.Generic;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class ProfileController
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db) => _db = db;

    public User? GetProfile(int userId)
        => _db.Users.Include(u => u.Role).Where(u => u.userId == userId).FirstOrDefault();

    public List<Appointment> GetUserAppointments(int userId)
        => new AppointmentController(_db).GetUserAppointments(userId);
}
