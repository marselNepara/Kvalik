using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("appointments")]
public class Appointment
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int appointmentId { get; set; }
    public int userId { get; set; }
    [ForeignKey("userId")] public User User { get; set; } = null!;
    public int masterServiceId { get; set; }
    [ForeignKey("masterServiceId")] public MasterService MasterService { get; set; } = null!;
    public DateTime appointmentDate { get; set; }
    public int queueNumber { get; set; }
    public string status { get; set; } = "Ожидание";
    public DateTime createdAt { get; set; }
}
