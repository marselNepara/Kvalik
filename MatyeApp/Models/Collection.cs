using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("collections")]
public class Collection
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int collectionId { get; set; }
    public string collectionName { get; set; } = string.Empty;
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
