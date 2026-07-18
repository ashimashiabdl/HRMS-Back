using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDisketteWhDTO : BaseDTO
    {
        public long TaxDisketteId { get; set; }
        public  string? TaxDiskette { get; set; }
        public string? ActiveName { get; set; }
        public string? NationalNo { get; set; }
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? IdentityNo { get; set; }
       
    
        public long InterdictOrderId { get; set; }
        /// <summary>
        ///  نحوه پرداخت
        /// </summary>
    
        public long PaymentTypeId { get; set; }

        public string? PaymentType { get; set; }
        /// <summary>
        /// تعداد ماه های کارکرد واقعی از ابتدای سال جاری
        /// </summary>
        [Comment("تعداد ماه های کارکرد واقعی از ابتدای سال جاری")]
        public int RealWorkMonthCount { get; set; }
        /// <summary>
        /// آیا این ماه آخرین ماه فعالیت کاری حقوق بگیر می باشد ؟
        /// </summary>
        [Comment("آیا این ماه آخرین ماه فعالیت کاری حقوق بگیر می باشد ؟")]
        public bool IsthisMonthLastMonthOfWork { get; set; }
        /// <summary>
        /// نوع ارز - جدول شماره 13 سند
        /// </summary>
        [Comment("نوع ارز - جدول شماره 13 سند")]
        public int CurrencyCode { get; set; }
        /// <summary>
        /// نرخ تسعیر ارز
        /// </summary>
        [Comment("نرخ تسعیر ارز")]
        public int ExchangeRate { get; set; }
        /// <summary>
        /// تاریخ شروع ب کار
        /// </summary>
        [Comment("تاریخ شروع ب کار")]
        public string? StartWorkDate { get; set; }
        /// <summary>
        /// تاریخ پایان کار
        /// </summary>
        [Comment("تاریخ پایان کار")]
        public string? EndWorkDate { get; set; }
        /// <summary>
        /// وضعیت کارمند جدول شماره 9
        /// </summary>
        [Comment("وضعیت کارمند جدول شماره 9")]
        public int EmployeeStatusTable9 { get; set; }
        /// <summary>
        /// جدول شماره 7
        /// </summary>
        [Comment("جدول شماره 7")]
        public int WorkPlaceTypeCode { get; set; }
        /// <summary>
        /// نا خالص حقوق و دستمزد مستمر نقدی ماه جاری - ریالی
        /// </summary>
        [Comment("نا خالص حقوق و دستمزد مستمر نقدی ماه جاری - ریالی")]
        public long NotNetCountiniousPaymentOCurrentMonthRiali { get; set; }
        /// <summary>
        /// پرداخت های مستمر معوق که مالیاتی برای آن ها محاسبه نشده است - ریالی
        /// </summary>
        [Comment("پرداخت های مستمر معوق که مالیاتی برای آن ها محاسبه نشده است - ریالی")]
        public long SumOfDelayedCountiniousPaymentInCurrentMonth { get; set; }
        /// <summary>
        /// مسکن - از جدول Employee پر می شود
        /// </summary>
        [Comment("مسکن - از جدول Employee پر می شود")]
        public int HouseCode { get; set; }
        /// <summary>
        /// مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری
        /// </summary>
        [Comment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری")]
        public long HouseAmount { get; set; }
        /// <summary>
        /// کد وسیله نقلیه سازمانی
        /// </summary>
        [Comment("کد وسیله نقلیه سازمانی")]
        public int CarCode { get; set; }
        /// <summary>
        /// مبلغ کسر شده از حقوق کارمند بابت وسیله نقلیه ماه جاری
        /// </summary>
        [Comment("مبلغ کسر شده از حقوق کارمند بابت وسیله نقلیه ماه جاری")]
        public long CarAmount { get; set; }
        /// <summary>
        /// جمع پرداخت های مستمر غیر نقدی ماه جاری - ریالی
        /// </summary>
        [Comment("جمع پرداخت های مستمر غیر نقدی ماه جاری - ریالی")]
        public long SumNonCashTaxCoveredAndCountinious { get; set; }
        /// <summary>
        /// هزینه های درمانی موضوع ماده 137 قانون مالیات های مستقیم
        /// </summary>
        [Comment("هزینه های درمانی موضوع ماده 137 قانون مالیات های مستقیم")]
        public long MedicalExpensesArticle137 { get; set; }
        /// <summary>
        /// حق بیمه پرداختی موضوع ماده 137 قانون مالیات های مستقیم
        /// </summary>
        [Comment("حق بیمه پرداختی موضوع ماده 137 قانون مالیات های مستقیم")]
        public long InsurancePaymentArticle137 { get; set; }
        /// <summary>
        /// تسهیلات اعتباری مسکن از بانک ها( موضوع بند الف ماده 139 قانون برنامه سوم )
        /// </summary>
        [Comment("تسهیلات اعتباری مسکن از بانک ها( موضوع بند الف ماده 139 قانون برنامه سوم")]
        public long HousingCreditFacilitiesFromBanks { get; set; }
        /// <summary>
        /// سایر معافیت ها
        /// </summary>
        [Comment("سایر معافیت ها")]
        public long OtherExemptions { get; set; }
        /// <summary>
        /// نا خالص اضافه کاری ماه جاری - ریالی
        /// </summary>
        [Comment("نا خالص اضافه کاری ماه جاری - ریالی")]
        public long NotNetCurrentMonthOverTimePayment { get; set; }
        /// <summary>
        /// سایر پرداخت های غیر مستمر نقدی ماه جاری - ریالی
        /// </summary>
        [Comment("سایر پرداخت های غیر مستمر نقدی ماه جاری - ریالی")]
        public long SumOfOtherCurrentMonthNonCountiniousCashPayments { get; set; }
        /// <summary>
        /// پاداش های موردی ماه جاری - ریالی
        /// </summary>
        [Comment("پاداش های موردی ماه جاری - ریالی")]
        public long CurrentMonthCaseBonusRiali { get; set; }
        /// <summary>
        /// پرداخت های غیر مستمر نقدی معوقه ماه جاری - ریالی
        /// </summary>
        [Comment("پرداخت های غیر مستمر نقدی معوقه ماه جاری - ریالی")]
        public long SumOfCurrentMonthNonCountiniousDelayedCash { get; set; }
        /// <summary>
        /// کسر می شود : معافیت های غیر مستمر نقدی ( شامل بند 6 ماده 91 )
        /// </summary>
        [Comment("کسر می شود : معافیت های غیر مستمر نقدی ( شامل بند 6 ماده 91")]
        public long SumOfExemptionNonCountiniousCash { get; set; }
        /// <summary>
        /// پرداخت مزایای غیر مستمر غیر نقدی ماه جاری - ریالی
        /// </summary>
        [Comment("پرداخت مزایای غیر مستمر غیر نقدی ماه جاری - ریالی")]
        public long SumNonCashTaxCoveredAndNotCountinious { get; set; }
        /// <summary>
        /// عیدی و مزایای پایان سال
        /// </summary>
        [Comment("عیدی و مزایای پایان سال")]
        public long EndYearRewardAndAdvantagesRiali { get; set; }
        /// <summary>
        /// بازخرید مرخصی و بازخرید سنوات
        /// </summary>
        [Comment("بازخرید مرخصی و بازخرید سنوات")]
        public long LeaveAndYearsRedemptionRiali { get; set; }
        /// <summary>
        /// کسر می شود : معافیت ( فقط برای بند 5 ماده 91 )
        /// </summary>
        [Comment("کسر می شود : معافیت ( فقط برای بند 5 ماده 91 )")]
        public long Exemption { get; set; }
        /// <summary>
        /// معافیت مربوط به مناطق آزاد تجاری
        /// </summary>
        [Comment("معافیت مربوط به مناطق آزاد تجاری")]
        public long FreeZoneExemption { get; set; }
        /// <summary>
        /// معافیت موضوع قانون اجتناب از اخذ مالیات مضاعف
        /// </summary>
        [Comment("معافیت موضوع قانون اجتناب از اخذ مالیات مضاعف")]
        public long DoubleTaxationAvoidance { get; set; }
        /// <summary>
        /// مالیات متعلقه حقوق و دستمزد مستمر نقدی، درآمد ها و مزایای غیر نقدی، پرداخت های غیر مستمر نقدی و غیر نقدی، عیدی و مزایا، بازخرید مرخصی و سنوات ماه جاری
        /// </summary>
        [Comment("مالیات متعلقه حقوق و دستمزد مستمر نقدی، درآمد ها و مزایای غیر نقدی، پرداخت های غیر مستمر نقدی و غیر نقدی، عیدی و مزایا، بازخرید مرخصی و سنوات ماه جاری")]
        public long SalaryTax { get; set; }
        /// <summary>
        /// جمع خالص مالیات متعلقه ماه جاری
        /// </summary>
        [Comment("جمع خالص مالیات متعلقه ماه جاری")]
        public long NetSalaryTax { get; set; }
        // New fields per TaxDisketteWH entity
        public long PurePaymentAmount { get; set; }
        public long WorkplaceStatusId { get; set; }
        public long ExceptionsSubjectToTheBudgetLawOf1404 { get; set; }
        public long ExchangeRateOfCurrency { get; set; }
        public long GrossContinuousCashCurrentMonth { get; set; }
        public long ContinuousCashArearsNoTax { get; set; }
        public int HouseStatusId { get; set; }
        public long EmployeeHousingDeductionCurrentMonth { get; set; }
        public int CarStatusId { get; set; }
        public long EmployeeCarDeductionCurrentMonth { get; set; }
        public long ContinuousNonCashOtherBenefitsCost { get; set; }
        public long ContinuousNonCashArearsNoTax { get; set; }
        public long ConsultingFeesAndSimilar { get; set; }
        public long ResearchContracts { get; set; }
        public long Overtime { get; set; }
        public long TravelExpense { get; set; }
        public long MissionAllowance { get; set; }
        public long Karaneh { get; set; }
        public long BonusExceptYearEndServiceEndProductivity { get; set; }
        public long YearEndBonus { get; set; }
        public long AnnualEydi { get; set; }
        public long EndOfServiceBonus { get; set; }
        public long DismissalCompensation { get; set; }
        public long ServiceBuyback { get; set; }
        public long SeverancePay { get; set; }
        public long UnusedLeavePay { get; set; }
        public long NonContinuousCashCurrentMonth { get; set; }
        public long NonContinuousCashArearsNoTax { get; set; }
        public long NonContinuousNonCashCostCurrentMonth { get; set; }
        public long NonContinuousNonCashArearsNoTax { get; set; }
        public long MedicalInsuranceArticle137 { get; set; }
        public long LifeInsuranceArticle137 { get; set; }
        public long TeachingResearchFees { get; set; }
        public long OnCallPay { get; set; }
        public long WelfareMotivationProductivity { get; set; }
        public long WorkEffortExcludingWageSalaryBonus { get; set; }

    }
}
