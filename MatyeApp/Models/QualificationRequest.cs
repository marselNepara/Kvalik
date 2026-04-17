using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatyeApp.Models;

[Table("qualificationRequests")]
public class QualificationRequest
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int requestId { get; set; }
    public int masterId { get; set; }
    [ForeignKey("masterId")] public Master Master { get; set; } = null!;
    public DateTime requestDate { get; set; }
    public string status { get; set; } = "На рассмотрении";
}
