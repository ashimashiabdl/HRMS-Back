using HR.Attendance.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

/// <summary>
/// بازه زمانی عدم حضور کارمند بر اساس شیفت کاری
/// </summary>
[Table("Employee_Attendance_Exception", Schema = "Attendance")]
public class EmployeeAttendanceException : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("Employee")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("AttendanceCalendar")]
    public long AttendanceCalendarId { get; set; }
    public virtual AttendanceCalendar? AttendanceCalendar { get; set; }

    [ForeignKey("AbsenceType")]
    public long AbsenceTypeId { get; set; }
    public virtual AbsenceType? AbsenceType { get; set; }



    [ForeignKey("Shift")]
    public long ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("لحظه آغاز")]
    public DateTime StartAt { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("لحظه پایان")]
    public DateTime EndAt { get; set; }

    [Comment("مدت عدم حضور (ثانیه)")]
    public int DurationSeconds { get; set; }

    [Comment("نسخه محاسبه")]
    public int CalculationVersion { get; set; }
}
