using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Shift", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Attendance_Shift_OrganisationChartId")]
public partial class AttendanceShift
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    /// <summary>
    /// کد شیفت
    /// </summary>
    [StringLength(32)]
    public string Code { get; set; } = null!;

    /// <summary>
    /// رنگ نمایشی (hex)
    /// </summary>
    [StringLength(7)]
    public string Color { get; set; } = null!;

    /// <summary>
    /// فعال
    /// </summary>
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
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("Shift")]
    public virtual ICollection<AttendanceEmployeeShiftAssignment> AttendanceEmployeeShiftAssignments { get; set; } = new List<AttendanceEmployeeShiftAssignment>();

    [InverseProperty("Shift")]
    public virtual ICollection<AttendanceShiftDetail> AttendanceShiftDetails { get; set; } = new List<AttendanceShiftDetail>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("AttendanceShifts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
