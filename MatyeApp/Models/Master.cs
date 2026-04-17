using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("masters")]
public class Master
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int masterId { get; set; }
    public int userId { get; set; }
    [ForeignKey("userId")] public User User { get; set; } = null!;
    public int qualificationLevel { get; set; } = 1;
    public string bio { get; set; } = string.Empty;
}
