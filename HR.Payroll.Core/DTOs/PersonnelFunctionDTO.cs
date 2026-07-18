using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelFunctionDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public string? Employee { get; set; }
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? NationalNo { get; set; }
        public long? CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }
        /// <summary>
        /// روز های ماه
        /// </summary>
        public int? FunctionDay { get; set; }
        /// <summary>
        /// روز های کارکرد
        /// </summary>
        public int? PersonnelFunctionDay { get; set; }
        public int? PersonnelHourPresent { get; set; }
        /// <summary>
        /// غیبت
        /// </summary>
        public decimal? PersonnelNoEnter { get; set; }
        public decimal? PersonnelAbsenceDay { get; set; }
        /// <summary>
        /// روز های استعلاجی
        /// </summary>
        public decimal? PersonnelIllnessDay { get; set; }
        /// <summary>
        /// تعداد ساعت ماموریت
        /// </summary>
        public decimal? PersonnelMissionHours { get; set; }
        /// <summary>
        /// ساعت اضافه کاری
        /// </summary>
        public decimal? PersonnelOverTime { get; set; }
        /// <summary>
        /// تعداد شب کاری به ساعت
        /// </summary>
        public decimal? PersonnelNightWork { get; set; }
        /// <summary>
        /// ساعات تعطیل کاری
        /// </summary>
        public decimal? PersonnelWorkingHolidayHours { get; set; }
        /// <summary>
        /// سال
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// ماه
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// وضعیت تایید
        /// </summary>
        public bool? IsConfirmed { get; set; }
        /// <summary>
        /// کاربر تاییدکننده حقوق
        /// </summary>
        [StringLength(128)]
        public string? PayRollApproveUser { get; set; }
        /// <summary>
        /// وضعیت معوقه
        /// </summary>
        public long? ArearsStatusId { get; set; }
        public int? RemoteWorkHours { get; set; }
        public string? ArearsStatus { get; set; }
        public decimal? RealFunctionDay { get; set; }
        public decimal? HolidayFunctionDay { get; set; }
        public long? FunctionTypeId { get; set; }
        public string? FunctionType { get; set; }
        /// <summary>
        /// روز های ماموریت
        /// </summary>
        public int? PersonnelMissionDay { get; set; }
        public int? PaylessDay { get; set; }
        public int? RewardsDay { get; set; }
        public int? PostType { get; set; }
        public int? DeservedFunctionInHoliday { get; set; }
        public int? DeservedFunctionOutHoliday { get; set; }
        /// <summary>
        /// تعداد شب کاری
        /// </summary>
        public decimal? PersonnelNightWorkDay { get; set; }
        /// <summary>
        /// تعداد روز های تعطیل کاری
        /// </summary>
        public float? PersonnelWorkingHolidaysDay { get; set; }
        public long? LinearFunctionDay { get; set; }
        [StringLength(2048)]
        public string? Comment { get; set; }
        [StringLength(1012)]
        public string? Description { get; set; }
        /// <summary>
        /// زمان تایید رکورد در سیستم حقوق
        /// </summary>
        public DateTime? ConfirmDate { get; set; }
        /// <summary>
        /// تاریخ تایید حقوق و دستمزد
        /// </summary>
        public DateTime? PayRollAproveDate { get; set; }
        /// <summary>
        /// وضعیت مدیر بودن
        /// </summary>
        public bool? IsModir { get; set; }
        public decimal? PersonnelCeillingOvertime { get; set; }
        public decimal? PersonnelOverTimeFixed { get; set; }
        public long? CarServiceDeduction { get; set; }
        public long? AttendanceId { get; set; }
        public DateTime? ReceiveDate { get; set; }
        /// <summary>
        /// دقیقه اضافه کاری
        /// </summary>
        public decimal? PersonnelOverTimeMinutes { get; set; }

        /// <summary>
        /// ساعت کسر کار
        /// </summary>
        public int? PaylessHour { get; set; }
        /// <summary>
        /// نوبت کاری 10 درصد
        /// </summary>
        public int? ShiftWork10Percent { get; set; }
        /// <summary>
        /// نوبت کاری 15 درصد
        /// </summary>
        public int? ShiftWork15Percent { get; set; }

        /// <summary>
        /// نوبت کاری 22.5 درصد
        /// </summary>
        public int? ShiftWork22Point5Percent { get; set; }

        // New fields
        /// <summary>
        /// فوق العاده نوبت کاری
        /// </summary>
        public decimal? ShiftWorkAllowance { get; set; }
        /// <summary>
        /// تعداد شیفت
        /// </summary>
        public int? ShiftCount { get; set; }
        /// <summary>
        /// غذا
        /// </summary>
        public decimal? Food { get; set; }
        /// <summary>
        /// پاداش آکورد
        /// </summary>
        public decimal? AccordReward { get; set; }
        /// <summary>
        /// پاداش
        /// </summary>
        public decimal? Reward { get; set; }
        /// <summary>
        /// معوقه1
        /// </summary>
        public decimal? Arear1 { get; set; }
        /// <summary>
        /// معوقه 2
        /// </summary>
        public decimal? Arear2 { get; set; }

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
        public decimal? TravelExpenses { get; set; }
        /// <summary>
        /// هزینه های مامویرت
        /// </summary>
        public decimal? MissionExpenses { get; set; }
        /// <summary>
        /// طلب بیمه تکمیلی ورودی
        /// </summary>
        public decimal? RequestForAdditionalInsuranceForEntry { get; set; }
        /// <summary>
        /// سقف مساعده قابل پرداخت
        /// </summary>
        public decimal? MaximumAmountOfAllowancePayable { get; set; }
        /// <summary>
        /// سایر کسورات
        /// </summary>
        public decimal? OtherDeductions { get; set; }
        /// <summary>
        /// کسورات حکمت
        /// </summary>
        public decimal? HekmatDeductions { get; set; }
        /// <summary>
        /// دقیقه تعطیل کاری
        /// </summary>
        public decimal? PersonnelWorkingHolidayMinutes { get; set; }
        /// <summary>
        /// بدهی به شرکت
        /// </summary>
        public decimal? DebtToTheCompany { get; set; }
        /// <summary>
        /// طلب ماه قبل
        /// </summary>
        public decimal? LastMonthDemand { get; set; }
        /// <summary>
        /// سقف پاداش
        /// </summary>
        public decimal? BonusCeiling { get; set; }
        /// <summary>
        /// سقف فردی پاداش
        /// </summary>
        public decimal? IndividualBonusCeiling { get; set; }
        /// <summary>
        /// سایر مزایا
        /// </summary>
        public decimal? OtherBenefits { get; set; }
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
    }
}
