using HR.Attendance.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

[Table("Employee_Attendance_Daily_Result", Schema = "Attendance")]
public class EmployeeAttendanceDailyResult : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
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

    [ForeignKey("Shift")]
    public long ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("اولین ورود")]
    public DateTime? FirstIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود دوم")]
    public DateTime? SecondIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود سوم")]
    public DateTime? ThirdIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود چهارم")]
    public DateTime? FourthIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود پنجم")]
    public DateTime? FifthIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود ششم")]
    public DateTime? SixthIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("ورود هفتم")]
    public DateTime? SeventhIn { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج دوم")]
    public DateTime? SecondOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج سوم")]
    public DateTime? ThirdOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج چهارم")]
    public DateTime? FourthOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج پنجم")]
    public DateTime? FifthOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج ششم")]
    public DateTime? SixthOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("خروج هفتم")]
    public DateTime? SeventhOut { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("آخرین خروج")]
    public DateTime? LastOut { get; set; }

    [Comment("ثانیه کارکرد")]
    public int WorkedSeconds { get; set; }

    [Comment("ثانیه مورد نیاز")]
    public int RequiredSeconds { get; set; }

    [Comment("ثانیه تأخیر")]
    public int DelaySeconds { get; set; }

    [Comment("ثانیه تعجیل در خروج")]
    public int EarlyLeaveSeconds { get; set; }

    [Comment("ثانیه غیبت")]
    public int AbsentSeconds { get; set; }

    [Comment("ثانیه اضافه‌کار")]
    public int OvertimeSeconds { get; set; }

    [Comment("ثانیه کار شب")]
    public int NightWorkSeconds { get; set; }

    [Comment("ثانیه کار در تعطیل")]
    public int HolidayWorkSeconds { get; set; }

    [Comment("ثانیه مأموریت")]
    public int MissionSeconds { get; set; }

    [Comment("ثانیه مرخصی")]
    public int LeaveSeconds { get; set; }

    [Comment("ثانیه استراحت")]
    public int BreakSeconds { get; set; }

    [Comment("ثانیه استراحت با حقوق")]
    public int PaidBreakSeconds { get; set; }

    [Comment("ثانیه استراحت بدون حقوق")]
    public int UnpaidBreakSeconds { get; set; }

    [Comment("نسخه محاسبه")]
    public int CalculationVersion { get; set; }

    [Column(TypeName = "datetime")]
    [Comment("تاریخ محاسبه")]
    public DateTime? CalculateDate { get; set; }
}
