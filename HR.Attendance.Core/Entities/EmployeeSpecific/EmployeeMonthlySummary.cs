using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities.EmployeeSpecific;

/// <summary>
/// خلاصه ماهانه حضور و کارکرد کارمند
/// </summary>
[Table("Employee_Monthly_Summary", Schema = "Attendance")]
public class EmployeeMonthlySummary : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("CostCenter")]
    public long? CostCenterId { get; set; }
    public virtual OrganisationChart? CostCenter { get; set; }

    [ForeignKey("OrganizationUnit")]
    public long? OrganizationUnitId { get; set; }
    public virtual OrganisationChart? OrganizationUnit { get; set; }

    [ForeignKey("WorkPlace")]
    public long? WorkPlaceId { get; set; }
    public virtual OrganisationChart? WorkPlace { get; set; }

    [Comment("روزهای ماه")]
    public int? FunctionDay { get; set; }

    [Comment("روزهای کارکرد")]
    public int? PersonnelFunctionDay { get; set; }

    [Comment("ساعات حضور ساعتی")]
    public int? PersonnelHourPresent { get; set; }

    public decimal? PersonnelNoEnter { get; set; }
    public decimal? PersonnelAbsenceDay { get; set; }
    public decimal? PersonnelIllnessDay { get; set; }
    public decimal? PersonnelMissionHours { get; set; }
    public decimal? PersonnelOverTime { get; set; }
    public decimal? PersonnelOverTimeMinutes { get; set; }
    public decimal? PersonnelNightWork { get; set; }
    public decimal? PersonnelWorkingHolidayHours { get; set; }

    [Comment("سال")]
    public int? Year { get; set; }

    [Comment("ماه")]
    public int? Month { get; set; }

    [Comment("ساعت دورکاری")]
    public int? RemoteWorkHours { get; set; }

    [Comment("تأیید شده")]
    public bool? IsConfirmed { get; set; }

    public decimal? RealFunctionDay { get; set; }
    public decimal? HolidayFunctionDay { get; set; }
    public int? PersonnelMissionDay { get; set; }
    public int? PaylessDay { get; set; }
    public int? PaylessHour { get; set; }
    public int? ShiftWork10Percent { get; set; }
    public int? ShiftWork15Percent { get; set; }
    public int? ShiftWork22Point5Percent { get; set; }
    public int? DeservedFunctionInHoliday { get; set; }
    public int? DeservedFunctionOutHoliday { get; set; }
    public decimal? PersonnelNightWorkDay { get; set; }
    public float? PersonnelWorkingHolidaysDay { get; set; }
    public long? LinearFunctionDay { get; set; }

    [StringLength(2048)]
    public string? Comment { get; set; }

    [StringLength(1012)]
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ConfirmDate { get; set; }

    public decimal? PersonnelCeillingOvertime { get; set; }
    public decimal? PersonnelOverTimeFixed { get; set; }
    public long? AttendanceId { get; set; }
    public DateTime? ReceiveDate { get; set; }

    [Comment("فوق‌العاده نوبت‌کاری")]
    public decimal? ShiftWorkAllowance { get; set; }

    [Comment("تعداد شیفت")]
    public int? ShiftCount { get; set; }

    [Comment("کارکرد ساعتی (ساعت)")]
    public decimal? PersonnelHourlyWork { get; set; }

    [Comment("کارکرد ساعتی (دقیقه)")]
    public decimal? PersonnelHourlyWorkMinutes { get; set; }

    [Comment("دقیقه کسر کار")]
    public int? PaylessMinutes { get; set; }

    [Comment("دقیقه شب‌کاری")]
    public decimal? PersonnelNightWorkMinutes { get; set; }

    [Comment("اضافه‌کاری بسیج")]
    public decimal? BasijOverTime { get; set; }

    [Comment("دقیقه تعطیل‌کاری")]
    public decimal? PersonnelWorkingHolidayMinutes { get; set; }

    [Comment("سرانه اضافه‌کار")]
    public decimal? OvertimePerCapita { get; set; }

    [Comment("اضافه‌کار انتظامات")]
    public decimal? DisciplinaryOvertime { get; set; }

    [Comment("ساعت اضافه‌کار تأییدی")]
    public decimal? ApprovedOvertimeHours { get; set; }

    [Comment("اضافه‌کار خارج از یگان")]
    public decimal? OvertimeOutsideUnit { get; set; }

    [Comment("اضافه‌کار جایگزین شیفت")]
    public decimal? ShiftReplacementOvertime { get; set; }

    [Comment("اضافه‌کار تنخواه")]
    public decimal? CashOvertime { get; set; }

    [Comment("مجموع اضافه‌کاری")]
    public decimal? TotalOvertime { get; set; }

    [Comment("مأموریت و شیفت")]
    public decimal? MissionAndShift { get; set; }

    [Comment("جمعه کاری")]
    public decimal? FridayWorkHours { get; set; }

    [Comment("فوق العاده جمعه کاری")]
    public decimal? FridayWorkAllowance { get; set; }

    [Comment("فوق العاده شبکاری")]
    public decimal? NightWorkAllowance { get; set; }

    [NotMapped]
    public new string title { get; set; } = string.Empty;
}
