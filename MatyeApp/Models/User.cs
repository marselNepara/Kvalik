using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("users")]
public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int userId { get; set; }
    public string login { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public decimal balance { get; set; } = 0;
    public int roleId { get; set; }
    [ForeignKey("roleId")] public Role Role { get; set; } = null!;
    public DateTime createdAt { get; set; }
}
