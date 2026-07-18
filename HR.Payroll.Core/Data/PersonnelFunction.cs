using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HR.Payroll.Core.Data;

[Table("Personnel_Function", Schema = "Payroll")]
public class PersonnelFunction : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("ArearsStatus")]
    public long? ArearsStatusId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual ArearsStatus? ArearsStatus { get; set; }


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
    [ForeignKey("PersonnelFunctionExcelFile")]
    public long? PersonnelFunctionExcelFileId { get; set; }
    public virtual PersonnelFunctionExcelFile? PersonnelFunctionExcelFile { get; set; }
    [ForeignKey("WorkPlace")]
    public long? WorkPlaceId { get; set; }
    public virtual OrganisationChart? WorkPlace { get; set; }
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
    public int? RemoteWorkHours { get; set; }
    public bool? IsConfirmed { get; set; }
    [StringLength(128)]
    public string? PayRollApproveUser { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? PayRollAproveDate { get; set; }
    public decimal? RealFunctionDay { get; set; }
    public decimal? HolidayFunctionDay { get; set; }
    public virtual BaseTableValue? FunctionType { get; set; }
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
    public decimal? PersonnelNightWorkDay { get; set; }
    public float? PersonnelWorkingHolidaysDay { get; set; }
    public long? LinearFunctionDay { get; set; }
    [StringLength(2048)]
    public string? Comment { get; set; }
    [StringLength(1012)]
    public string? Description { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? ConfirmDate { get; set; }
    public bool? IsModir { get; set; }
    public decimal? PersonnelCeillingOvertime { get; set; }
    public decimal? PersonnelOverTimeFixed { get; set; }
    public long? CarServiceDeduction { get; set; }
    public long? AttendanceId { get; set; }
    public DateTime? ReceiveDate { get; set; }

    // New fields
    public decimal? ShiftWorkAllowance { get; set; } // فوق العاده نوبت کاری
    public int? ShiftCount { get; set; } // تعداد شیفت
    public decimal? Food { get; set; } // غذا
    public decimal? AccordReward { get; set; } // پاداش آکورد
    public decimal? Reward { get; set; } // پاداش
    public decimal? Arear1 { get; set; } // معوقه1
    public decimal? Arear2 { get; set; } // معوقه 2

    // Requested new fields
    /// <summary>
    /// کارکرد ساعتی (به ساعت)
    /// </summary>
    public decimal? PersonnelHourlyWork { get; set; }
    /// <summary>
    /// دقیقه در کارکرد ساعتی
    /// </summary>
    public decimal? PersonnelHourlyWorkMinutes { get; set; }
    /// <summary>
    /// دقیقه کسر کار
    /// </summary>
    public int? PaylessMinutes { get; set; }
    /// <summary>
    /// کارانه
    /// </summary>
    public decimal? Karaneh { get; set; }
    /// <summary>
    /// دقیقه شبکاری
    /// </summary>
    public decimal? PersonnelNightWorkMinutes { get; set; }
    /// <summary>
    /// اضافه کاری بسیج
    /// </summary>
    public decimal? BasijOverTime { get; set; }

    /// <summary>
    /// هزینه ایاب و ذهاب
    /// </summary>
    public decimal TravelExpenses { get; set; }
    /// <summary>
    /// هزینه های مامویرت
    /// </summary>
    public decimal MissionExpenses { get; set; }
    /// <summary>
    /// طلب بیمه تکمیلی ورودی
    /// </summary>
    public decimal RequestForAdditionalInsuranceForEntry { get; set; }
    /// <summary>
    /// سقف مساعده قابل پرداخت
    /// </summary>
    public decimal MaximumAmountOfAllowancePayable { get; set; }
    /// <summary>
    /// سایر کسورات
    /// </summary>
    public decimal OtherDeductions { get; set; }
    /// <summary>
    /// کسورات حکمت
    /// </summary>
    public decimal HekmatDeductions { get; set; }
    /// <summary>
    /// دقیقه تعطیل کاری
    /// </summary>
    public decimal PersonnelWorkingHolidayMinutes { get; set; }
    /// <summary>
    /// بدهی به شرکت
    /// </summary>
    public decimal DebtToTheCompany { get; set; }
    /// <summary>
    /// طلب ماه قبل
    /// </summary>
    public decimal LastMonthDemand { get; set; }
    /// <summary>
    /// سقف پاداش
    /// </summary>
    public decimal BonusCeiling { get; set; }
    /// <summary>
    /// سقف فردی پاداش
    /// </summary>
    public decimal IndividualBonusCeiling { get; set; }
    /// <summary>
    /// سایر مزایا
    /// </summary>
    public decimal OtherBenefits { get; set; }

    /// <summary>
    /// سرانه اضافه کار
    /// </summary>
    public decimal? OvertimePerCapita { get; set; }
    /// <summary>
    /// اضافه کار انتظامات
    /// </summary>
    public decimal? DisciplinaryOvertime { get; set; }
    /// <summary>
    /// ساعت اضافه کار تاییدی
    /// </summary>
    public decimal? ApprovedOvertimeHours { get; set; }
    /// <summary>
    /// اضافه کار خارج از یگان
    /// </summary>
    public decimal? OvertimeOutsideUnit { get; set; }
    /// <summary>
    /// حق سرویس
    /// </summary>
    public decimal? ServiceRight { get; set; }
    /// <summary>
    /// اضافه کار جایگزین شیفت
    /// </summary>
    public decimal? ShiftReplacementOvertime { get; set; }
    /// <summary>
    /// اضافه کار تنخواه
    /// </summary>
    public decimal? CashOvertime { get; set; }
    /// <summary>
    /// مجموع اضافه کاری
    /// </summary>
    public decimal? TotalOvertime { get; set; }
    /// <summary>
    /// حق کارایی و پاداش
    /// </summary>
    public decimal? EfficiencyAndBonusRight { get; set; }
    /// <summary>
    /// ماموریت و شیفت
    /// </summary>
    public decimal? MissionAndShift { get; set; }
    /// <summary>
    /// سایر پرداختی و کسور
    /// </summary>
    public decimal? OtherPaymentsAndDeductions { get; set; }
    /// <summary>
    /// کارایی 100 درصد
    /// </summary>
    public decimal? Efficiency100Percent { get; set; }
    /// <summary>
    /// کارایی مصوب
    /// </summary>
    public decimal? ApprovedEfficiency { get; set; }
    /// <summary>
    /// ذخیره کارایی مصوب
    /// </summary>
    public decimal? ApprovedEfficiencyReserve { get; set; }
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

    [NotMapped]
    public new string title { get; set; }

}
