using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("categories")]
public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int categoryId { get; set; }
    public string categoryName { get; set; } = string.Empty;
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
