using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Personnel_Function_Visible", Schema = "Payroll")]
public class PersonnelFunctionVisible : BaseEntity, IOrganisationChartId,IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    // All fields mirror PersonnelFunction names (except excluded/date fields), but all are boolean flags
    public bool? FunctionDay { get; set; }
    public bool? PersonnelFunctionDay { get; set; }
    public bool? PersonnelHourPresent { get; set; }
    public bool? PersonnelNoEnter { get; set; }
    public bool? PersonnelAbsenceDay { get; set; }
    public bool? PersonnelIllnessDay { get; set; }
    public bool? PersonnelMissionHours { get; set; }
    public bool? PersonnelOverTime { get; set; }
    public bool? PersonnelOverTimeMinutes { get; set; }
    public bool? PersonnelNightWork { get; set; }
    public bool? PersonnelWorkingHolidayHours { get; set; }
    public bool? Year { get; set; }
    public bool? Month { get; set; }
    public bool? RemoteWorkHours { get; set; }
    public bool? IsConfirmed { get; set; }
    public bool? RealFunctionDay { get; set; }
    public bool? HolidayFunctionDay { get; set; }
    public bool? PersonnelMissionDay { get; set; }
    public bool? PaylessDay { get; set; }
    public bool? PaylessHour { get; set; }
    public bool? ShiftWork10Percent { get; set; }
    public bool? ShiftWork15Percent { get; set; }
    public bool? ShiftWork22Point5Percent { get; set; }
    public bool? RewardsDay { get; set; }
    public bool? PostType { get; set; }
    public bool? DeservedFunctionInHoliday { get; set; }
    public bool? Description { get; set; }
    public bool? DeservedFunctionOutHoliday { get; set; }
    public bool? PersonnelNightWorkDay { get; set; }
    public bool? PersonnelWorkingHolidaysDay { get; set; }
    public bool? LinearFunctionDay { get; set; }
    public bool? IsModir { get; set; }
    public bool? PersonnelCeillingOvertime { get; set; }
    public bool? PersonnelOverTimeFixed { get; set; }
    public bool? CarServiceDeduction { get; set; }
    public bool? AttendanceId { get; set; }
    public bool? ShiftWorkAllowance { get; set; }
    public bool? ShiftCount { get; set; }
    public bool? Food { get; set; }
    public bool? AccordReward { get; set; }
    public bool? Reward { get; set; }
    public bool? Arear1 { get; set; }
    public bool? Arear2 { get; set; }
    public bool? PersonnelHourlyWork { get; set; }
    public bool? PersonnelHourlyWorkMinutes { get; set; }
    public bool? PaylessMinutes { get; set; }
    public bool? Karaneh { get; set; }
    public bool? PersonnelNightWorkMinutes { get; set; }
    public bool? BasijOverTime { get; set; }

    /// <summary>
    /// هزینه ایاب و ذهاب
    /// </summary>
    public bool TravelExpenses { get; set; }
    /// <summary>
    /// هزینه های مامویرت
    /// </summary>
    public bool MissionExpenses { get; set; }
    /// <summary>
    /// طلب بیمه تکمیلی ورودی
    /// </summary>
    public bool RequestForAdditionalInsuranceForEntry { get; set; }
    /// <summary>
    /// سقف مساعده قابل پرداخت
    /// </summary>
    public bool MaximumAmountOfAllowancePayable { get; set; }
    /// <summary>
    /// سایر کسورات
    /// </summary>
    public bool OtherDeductions { get; set; }
    /// <summary>
    /// کسورات حکمت
    /// </summary>
    public bool HekmatDeductions { get; set; }
    /// <summary>
    /// دقیقه تعطیل کاری
    /// </summary>
    public bool PersonnelWorkingHolidayMinutes { get; set; }
    /// <summary>
    /// بدهی به شرکت
    /// </summary>
    public bool DebtToTheCompany { get; set; }
    /// <summary>
    /// طلب ماه قبل
    /// </summary>
    public bool LastMonthDemand { get; set; }
    /// <summary>
    /// سقف پاداش
    /// </summary>
    public bool BonusCeiling { get; set; }
    /// <summary>
    /// سقف فردی پاداش
    /// </summary>
    public bool IndividualBonusCeiling { get; set; }
    /// <summary>
    /// سایر مزایا
    /// </summary>
    public bool OtherBenefits { get; set; }
    /// <summary>
    /// سرانه اضافه کار
    /// </summary>
    public bool OvertimePerCapita { get; set; }
    /// <summary>
    /// اضافه کار انتظامات
    /// </summary>
    public bool DisciplinaryOvertime { get; set; }
    /// <summary>
    /// ساعت اضافه کار تاییدی
    /// </summary>
    public bool ApprovedOvertimeHours { get; set; }
    /// <summary>
    /// اضافه کار خارج از یگان
    /// </summary>
    public bool OvertimeOutsideUnit { get; set; }
    /// <summary>
    /// حق سرویس
    /// </summary>
    public bool ServiceRight { get; set; }
    /// <summary>
    /// اضافه کار جایگزین شیفت
    /// </summary>
    public bool ShiftReplacementOvertime { get; set; }
    /// <summary>
    /// اضافه کار تنخواه
    /// </summary>
    public bool CashOvertime { get; set; }
    /// <summary>
    /// مجموع اضافه کاری
    /// </summary>
    public bool TotalOvertime { get; set; }
    /// <summary>
    /// حق کارایی و پاداش
    /// </summary>
    public bool EfficiencyAndBonusRight { get; set; }
    /// <summary>
    /// ماموریت و شیفت
    /// </summary>
    public bool MissionAndShift { get; set; }
    /// <summary>
    /// سایر پرداختی و کسور
    /// </summary>
    public bool OtherPaymentsAndDeductions { get; set; }
    /// <summary>
    /// کارایی 100 درصد
    /// </summary>
    public bool Efficiency100Percent { get; set; }
    /// <summary>
    /// کارایی مصوب
    /// </summary>
    public bool ApprovedEfficiency { get; set; }
    /// <summary>
    /// ذخیره کارایی مصوب
    /// </summary>
    public bool ApprovedEfficiencyReserve { get; set; }
    /// <summary>
    /// جمعه کاری
    /// </summary>
    public bool FridayWorkHours { get; set; }
    /// <summary>
    /// فوق العاده جمعه کاری
    /// </summary>
    public bool FridayWorkAllowance { get; set; }
    /// <summary>
    /// فوق العاده شبکاری
    /// </summary>
    public bool NightWorkAllowance { get; set; }
}


