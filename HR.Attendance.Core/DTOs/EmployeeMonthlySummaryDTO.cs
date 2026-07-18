using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class EmployeeMonthlySummaryDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long? CostCenterId { get; set; }
    public string? CostCenter { get; set; }

    public long? OrganizationUnitId { get; set; }
    public string? OrganizationUnit { get; set; }

    public long? WorkPlaceId { get; set; }
    public string? WorkPlace { get; set; }

    public int? FunctionDay { get; set; }
    public int? PersonnelFunctionDay { get; set; }
    public int? PersonnelHourPresent { get; set; }
    public decimal? PersonnelNoEnter { get; set; }
    public decimal? PersonnelAbsenceDay { get; set; }
    public decimal? PersonnelIllnessDay { get; set; }
    public decimal? PersonnelMissionHours { get; set; }
    public decimal? PersonnelOverTime { get; set; }
    public decimal? PersonnelOverTimeMinutes { get; set; }
    public decimal? PersonnelNightWork { get; set; }
    public decimal? PersonnelWorkingHolidayHours { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public string? MonthTitle { get; set; }
    public int? RemoteWorkHours { get; set; }
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

    public DateTime? ConfirmDate { get; set; }
    public decimal? PersonnelCeillingOvertime { get; set; }
    public decimal? PersonnelOverTimeFixed { get; set; }
    public long? AttendanceId { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public decimal? ShiftWorkAllowance { get; set; }
    public int? ShiftCount { get; set; }
    public decimal? PersonnelHourlyWork { get; set; }
    public decimal? PersonnelHourlyWorkMinutes { get; set; }
    public int? PaylessMinutes { get; set; }
    public decimal? PersonnelNightWorkMinutes { get; set; }
    public decimal? BasijOverTime { get; set; }
    public decimal? PersonnelWorkingHolidayMinutes { get; set; }
    public decimal? OvertimePerCapita { get; set; }
    public decimal? DisciplinaryOvertime { get; set; }
    public decimal? ApprovedOvertimeHours { get; set; }
    public decimal? OvertimeOutsideUnit { get; set; }
    public decimal? ShiftReplacementOvertime { get; set; }
    public decimal? CashOvertime { get; set; }
    public decimal? TotalOvertime { get; set; }
    public decimal? MissionAndShift { get; set; }
    /// <summary>
    /// جمعه کاری
    /// </summary>
    public decimal? FridayWorkHours { get; set; }
    /// <summary>
    /// فوق العاده جمعه کاری
    /// </summary>
    public decimal? FridayWorkAllowance { get; set; }
    /// <summary>
    /// فوق العاده شبکاری
    /// </summary>
    public decimal? NightWorkAllowance { get; set; }
}
