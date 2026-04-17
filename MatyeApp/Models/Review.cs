using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("reviews")]
public class Review
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int reviewId { get; set; }
    public int userId { get; set; } 
    [ForeignKey("userId")] public User User { get; set; } = null!;
    public int? serviceId { get; set; }
    [ForeignKey("serviceId")] public Service? Service { get; set; }
    public int? masterId { get; set; }
    [ForeignKey("masterId")] public Master? Master { get; set; }
    public int rating { get; set; }
    public string comment { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
}
