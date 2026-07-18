using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Reportable_Field", Schema = "rpt")]
[Microsoft.EntityFrameworkCore.Index("FieldDataTypeId", Name = "IX_Reportable_Field_FieldDataTypeId")]
[Microsoft.EntityFrameworkCore.Index("ReportableEntityId", Name = "IX_Reportable_Field_ReportableEntityId")]
public partial class ReportableField
{
    [Key]
    public long Id { get; set; }

    public long ReportableEntityId { get; set; }

    public string? TechnicalName { get; set; }

    public string? FriendlyName { get; set; }

    public long FieldDataTypeId { get; set; }

    public string? NavigationPath { get; set; }

    public bool IsFilterable { get; set; }

    public bool IsSelectable { get; set; }

    public bool IsSortable { get; set; }

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

    public int Priority { get; set; }

    public long? BaseTableId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("FieldDataTypeId")]
    [InverseProperty("ReportableFields")]
    public virtual FieldDataType FieldDataType { get; set; } = null!;

    [ForeignKey("ReportableEntityId")]
    [InverseProperty("ReportableFields")]
    public virtual ReportableEntity ReportableEntity { get; set; } = null!;
}
