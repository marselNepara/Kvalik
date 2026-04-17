using System.Collections.Generic;
using System.Linq;
using MatyeApp.Data;
using MatyeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatyeApp.Controllers;

public class MastersController
{
    private readonly AppDbContext _db;
    private const int PageSize = 10;

    public MastersController(AppDbContext db) => _db = db;

    public List<Master> GetMasters(int page = 1)
        => _db.Masters.Include(m => m.User)
            .OrderBy(m => m.User.login)
            .Skip((page - 1) * PageSize).Take(PageSize).ToList();

    public int GetTotalCount() => _db.Masters.Count();

    public List<MasterService> GetMasterServices(int masterId)
        => _db.MasterServices.Include(ms => ms.Service)
            .Where(ms => ms.masterId == masterId).ToList();

    public void AssignService(int masterId, int serviceId)
    {
        if (_db.MasterServices.Any(ms => ms.masterId == masterId && ms.serviceId == serviceId)) return;
        _db.MasterServices.Add(new MasterService { masterId = masterId, serviceId = serviceId });
        _db.SaveChanges();
    }

    public void UnassignService(int masterServiceId)
    {
        var ms = _db.MasterServices.Find(masterServiceId);
        if (ms != null) { _db.MasterServices.Remove(ms); _db.SaveChanges(); }
    }

    public void UpgradeQualification(int masterId)
    {
        var master = _db.Masters.Find(masterId);
        if (master != null) { master.qualificationLevel++; _db.SaveChanges(); }
    }
}
