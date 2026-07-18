using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Reportable_Entity", Schema = "rpt")]
public partial class ReportableEntity
{
    [Key]
    public long Id { get; set; }

    public string TechnicalName { get; set; } = null!;

    public string FriendlyName { get; set; } = null!;

    public string? Schema { get; set; }

    public string? TableName { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("ReportableEntity")]
    public virtual ICollection<ReportableField> ReportableFields { get; set; } = new List<ReportableField>();
}
