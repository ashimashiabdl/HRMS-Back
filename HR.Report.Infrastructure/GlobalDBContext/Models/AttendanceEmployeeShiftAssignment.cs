using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Attendance_Employee_Shift_Assignment", Schema = "Attendance")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Attendance_Employee_Shift_Assignment_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Attendance_Employee_Shift_Assignment_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("ShiftId", Name = "IX_Attendance_Employee_Shift_Assignment_ShiftId")]
public partial class AttendanceEmployeeShiftAssignment
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long ShiftId { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    [StringLength(512)]
    public string? Description { get; set; }

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

    [ForeignKey("EmployeeId")]
    [InverseProperty("AttendanceEmployeeShiftAssignments")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("AttendanceEmployeeShiftAssignments")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("ShiftId")]
    [InverseProperty("AttendanceEmployeeShiftAssignments")]
    public virtual AttendanceShift Shift { get; set; } = null!;
}
