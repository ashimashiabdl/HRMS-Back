using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Field_Operator", Schema = "rpt")]
[Microsoft.EntityFrameworkCore.Index("FieldDataTypeId", Name = "IX_Field_Operator_FieldDataTypeId")]
public partial class FieldOperator
{
    [Key]
    public long Id { get; set; }

    public long FieldDataTypeId { get; set; }

    public string? Operator { get; set; }

    public string? FriendlyName { get; set; }

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

    [ForeignKey("FieldDataTypeId")]
    [InverseProperty("FieldOperators")]
    public virtual FieldDataType FieldDataType { get; set; } = null!;
}
