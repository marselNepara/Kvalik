using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("services")]
public class Service
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int serviceId { get; set; }
    public string serviceName { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; }
    public int categoryId { get; set; }
    [ForeignKey("categoryId")] public Category Category { get; set; } = null!;
    public int collectionId { get; set; }
    [ForeignKey("collectionId")] public Collection Collection { get; set; } = null!;
    public string imageUrl { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}
