using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

[Table("Attendance_Calendar", Schema = "Attendance")]
public class AttendanceCalendar : BaseEntity, IignoreDateRangeValidation , IOrganisationChartId
{


    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }


    [Column(TypeName = "datetime")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("تاریخ")]
    public DateTime Date { get; set; }

    [Comment("تعطیل")]
    public bool IsHoliday { get; set; }

    [ForeignKey("Holiday")]
    [Comment("تعطیلات مرتبط")]
    public long? HolidayId { get; set; }
    public virtual AttendanceHoliday? Holiday { get; set; }

    [Comment("روز هفته (مطابق DayOfWeek)")]
    public int WeekDay { get; set; }
}
