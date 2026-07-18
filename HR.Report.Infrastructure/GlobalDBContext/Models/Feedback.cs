using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Feedback", Schema = "bas")]
public partial class Feedback
{
    [Key]
    public long Id { get; set; }

    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [StringLength(50)]
    public string? FeedbackType { get; set; }

    public long? SubmittedByUserId { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [StringLength(2000)]
    public string? Response { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ResponseDate { get; set; }

    public long? RespondedByUserId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }
}
