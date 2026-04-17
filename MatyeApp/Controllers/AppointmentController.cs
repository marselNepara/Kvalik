using System;
using System.Collections.Generic;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class AppointmentController
{
    private readonly AppDbContext _db;

    public AppointmentController(AppDbContext db) => _db = db;

    public List<MasterService> GetAvailableMasterServices(int serviceId)
        => _db.MasterServices
            .Include(ms => ms.Master).ThenInclude(m => m.User)
            .Include(ms => ms.Service)
            .Where(ms => ms.serviceId == serviceId)
            .ToList();
    public Appointment CreateAppointment(int userId, int masterServiceId, DateTime date)
    {
        // Validate masterService exists
        if (!_db.MasterServices.Any(ms => ms.masterServiceId == masterServiceId))
            throw new Exception($"Мастер-услуга с id={masterServiceId} не найдена.");

        int maxQueue = _db.Appointments
            .Where(a => a.masterServiceId == masterServiceId && a.appointmentDate.Date == date.Date)
            .Select(a => (int?)a.queueNumber)
            .Max() ?? 0;

        var appointment = new Appointment
        {
            userId = userId,
            masterServiceId = masterServiceId,
            appointmentDate = date,
            queueNumber = maxQueue + 1,
            status = "Ожидание",
            createdAt = DateTime.UtcNow
        };
        _db.Appointments.Add(appointment);
        _db.SaveChanges();
        return appointment;
    }

    public List<Appointment> GetUserAppointments(int userId)
        => _db.Appointments
            .Include(a => a.MasterService).ThenInclude(ms => ms.Service)
            .Include(a => a.MasterService).ThenInclude(ms => ms.Master).ThenInclude(m => m.User)
            .Where(a => a.userId == userId)
            .OrderByDescending(a => a.appointmentDate)
            .ToList();
}
