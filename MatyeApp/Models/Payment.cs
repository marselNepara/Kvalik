using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("payments")]
public class Payment
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int paymentId { get; set; }
    public int userId { get; set; }
    [ForeignKey("userId")] public User User { get; set; } = null!;
    public decimal amount { get; set; }
    public string cardNumber { get; set; } = string.Empty;
    public DateTime paymentDate { get; set; }
}
