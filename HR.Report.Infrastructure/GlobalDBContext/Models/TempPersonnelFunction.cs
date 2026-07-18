using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Temp_Personnel_Function", Schema = "Payroll")]
public partial class TempPersonnelFunction
{
    [Key]
    public long Id { get; set; }

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

    public long EmployeeId { get; set; }

    public long? ArearsStatusId { get; set; }

    public long OrganisationChartId { get; set; }

    public long? CostCenterId { get; set; }

    public long? OrganizationUnitId { get; set; }

    public long? PersonnelFunctionExcelFileId { get; set; }

    public long? WorkPlaceId { get; set; }

    public int? FunctionDay { get; set; }

    public int? PersonnelFunctionDay { get; set; }

    public int? PersonnelHourPresent { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelNoEnter { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelAbsenceDay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelIllnessDay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelMissionHours { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelOverTime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelOverTimeMinutes { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelNightWork { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelWorkingHolidayHours { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? RemoteWorkHours { get; set; }

    public bool? IsConfirmed { get; set; }

    [StringLength(128)]
    public string? PayRollApproveUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PayRollAproveDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RealFunctionDay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? HolidayFunctionDay { get; set; }

    public long? FunctionTypeId { get; set; }

    public int? PersonnelMissionDay { get; set; }

    public int? PaylessDay { get; set; }

    public int? PaylessHour { get; set; }

    public int? ShiftWork10Percent { get; set; }

    public int? ShiftWork15Percent { get; set; }

    public int? ShiftWork22Point5Percent { get; set; }

    public int? RewardsDay { get; set; }

    public int? PostType { get; set; }

    public int? DeservedFunctionInHoliday { get; set; }

    public int? DeservedFunctionOutHoliday { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelNightWorkDay { get; set; }

    public float? PersonnelWorkingHolidaysDay { get; set; }

    public long? LinearFunctionDay { get; set; }

    [StringLength(2048)]
    public string? Comment { get; set; }

    public DateOnly? ConfirmDate { get; set; }

    public bool? IsModir { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelCeillingOvertime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelOverTimeFixed { get; set; }

    public long? CarServiceDeduction { get; set; }

    public long? AttendanceId { get; set; }

    public DateTime? ReceiveDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AccordReward { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Arear1 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Arear2 { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Food { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Reward { get; set; }

    public int? ShiftCount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ShiftWorkAllowance { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BasijOverTime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Karaneh { get; set; }

    public int? PaylessMinutes { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelHourlyWork { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelHourlyWorkMinutes { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PersonnelNightWorkMinutes { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BonusCeiling { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DebtToTheCompany { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal HekmatDeductions { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal IndividualBonusCeiling { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LastMonthDemand { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MaximumAmountOfAllowancePayable { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal MissionExpenses { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal OtherBenefits { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal OtherDeductions { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PersonnelWorkingHolidayMinutes { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal RequestForAdditionalInsuranceForEntry { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TravelExpenses { get; set; }

    [StringLength(1012)]
    public string? Description { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ApprovedEfficiency { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ApprovedEfficiencyReserve { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ApprovedOvertimeHours { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CashOvertime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DisciplinaryOvertime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Efficiency100Percent { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? EfficiencyAndBonusRight { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MissionAndShift { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OtherPaymentsAndDeductions { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OvertimeOutsideUnit { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? OvertimePerCapita { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ServiceRight { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ShiftReplacementOvertime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TotalOvertime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? FridayWorkHours { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? FridayWorkAllowance { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NightWorkAllowance { get; set; }
}
