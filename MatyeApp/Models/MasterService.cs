using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("masterServices")]
public class MasterService
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int masterServiceId { get; set; }
    public int masterId { get; set; }
    [ForeignKey("masterId")] public Master Master { get; set; } = null!;
    public int serviceId { get; set; }
    [ForeignKey("serviceId")] public Service Service { get; set; } = null!;
}
