using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("roles")]
public class Role
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int roleId { get; set; }
    public string roleName { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}
