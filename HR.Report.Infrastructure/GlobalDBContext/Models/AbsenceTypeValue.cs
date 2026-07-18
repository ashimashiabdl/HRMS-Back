using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Absence_Type_Value", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("AbsenceTypeId", Name = "IX_Absence_Type_Value_AbsenceTypeId")]
public partial class AbsenceTypeValue
{
    [Key]
    public long Id { get; set; }

    public long AbsenceTypeId { get; set; }

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

    [InverseProperty("AbsenceTypeValue")]
    public virtual ICollection<AbsenceRecord> AbsenceRecords { get; set; } = new List<AbsenceRecord>();

    [ForeignKey("AbsenceTypeId")]
    [InverseProperty("AbsenceTypeValues")]
    public virtual AbsenceType AbsenceType { get; set; } = null!;
}
