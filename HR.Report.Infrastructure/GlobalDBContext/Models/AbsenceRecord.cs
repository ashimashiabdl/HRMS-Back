using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Absence_Record", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("AbsenceTypeValueId", Name = "IX_Absence_Record_AbsenceTypeValueId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Absence_Record_EmployeeId")]
public partial class AbsenceRecord
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

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

    public bool FirstApprove { get; set; }

    public bool SecondApprove { get; set; }

    public long AbsenceTypeValueId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("AbsenceTypeValueId")]
    [InverseProperty("AbsenceRecords")]
    public virtual AbsenceTypeValue AbsenceTypeValue { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("AbsenceRecords")]
    public virtual Employee Employee { get; set; } = null!;
}
