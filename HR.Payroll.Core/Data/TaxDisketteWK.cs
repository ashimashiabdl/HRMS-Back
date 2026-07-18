using HR.Order.Core.Data;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

/// <summary>
/// سرجمع دیسکت مالیات
/// </summary>
[Table("Tax_DisketteWK", Schema = "Payroll")]
public class TaxDisketteWK : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("TaxDiskette")]
    public long TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }

    /// <summary>
    /// نوع پرداخت
    /// </summary>
    [ForeignKey("PaymentType")]
    public long PaymentTypeId { get; set; }
    public virtual PaymentType? PaymentType { get; set; }

    /// <summary>
    /// وضعیت محل خدمت
    /// </summary>
    public long WorkplaceStatusId { get; set; }

    /// <summary>
    /// خالص پرداختی به حقوق بگیر
    /// </summary>
    public long PurePaymentAmount { get; set; }

    /// <summary>
    /// استثنائات موضوع قانون بودجه 1404 - جدول 10
    /// </summary>
    public long ExceptionsSubjectToTheBudgetLawOf1404 { get; set; }

    /// <summary>
    /// نوع ارز - جدول 13
    /// </summary>
    public long CurrencyCode { get; set; }

    /// <summary>
    /// نرخ تسعیر ارز
    /// </summary>
    public long ExchangeRateOfCurrency { get; set; }

    [Comment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی")]
    public long GrossContinuousCashCurrentMonth { get; set; }

    [Comment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی")]
    public long ContinuousCashArearsNoTax { get; set; }

    public int HouseStatusId { get; set; }

    [Comment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی")]
    public long EmployeeHousingDeductionCurrentMonth { get; set; }

    public int CarStatusId { get; set; }

    [Comment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی")]
    public long EmployeeCarDeductionCurrentMonth { get; set; }

    [Comment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی")]
    public long ContinuousNonCashOtherBenefitsCost { get; set; }

    [Comment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public long ContinuousNonCashArearsNoTax { get; set; }

    [Comment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف")]
    public long ConsultingFeesAndSimilar { get; set; }

    [Comment("مبلغ قراردادهای پژوهشی- ریالی")]
    public long ResearchContracts { get; set; }

    [Comment("اضافه کاری- ریالی")]
    public long Overtime { get; set; }

    [Comment("هزینه سفر- ریالی")]
    public long TravelExpense { get; set; }

    [Comment("فوق العاده مسافرت (ماموریت) - ریالی")]
    public long MissionAllowance { get; set; }

    [Comment("کارانه- ریالی")]
    public long Karaneh { get; set; }

    [Comment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی")]
    public long BonusExceptYearEndServiceEndProductivity { get; set; }

    [Comment("پاداش آخر سال- ریالی")]
    public long YearEndBonus { get; set; }

    [Comment("عیدی سالانه- ریالی")]
    public long AnnualEydi { get; set; }

    [Comment("پاداش پایان خدمت- ریالی")]
    public long EndOfServiceBonus { get; set; }

    [Comment("خسارت اخراج- ریالی")]
    public long DismissalCompensation { get; set; }

    [Comment("بازخرید خدمت- ریالی")]
    public long ServiceBuyback { get; set; }

    [Comment("حق سنوات- ریالی")]
    public long SeverancePay { get; set; }

    [Comment("حقوق ایام مرخصی استفاده نشده- ریالی")]
    public long UnusedLeavePay { get; set; }

    [Comment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی")]
    public long NonContinuousCashCurrentMonth { get; set; }

    [Comment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public long NonContinuousCashArearsNoTax { get; set; }

    [Comment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی")]
    public long NonContinuousNonCashCostCurrentMonth { get; set; }

    [Comment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public long NonContinuousNonCashArearsNoTax { get; set; }

    [Comment("حق بیمه های درمان موضوع ماده  137ق.م.م")]
    public long MedicalInsuranceArticle137 { get; set; }

    [Comment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م")]
    public long LifeInsuranceArticle137 { get; set; }

    [Comment("حق التدریس/حق التحقیق/ حق پژوهش")]
    public long TeachingResearchFees { get; set; }

    [Comment("حق کشیک")]
    public long OnCallPay { get; set; }

    [Comment("رفاهی و انگیزشی و بهره وری")]
    public long WelfareMotivationProductivity { get; set; }

    [Comment("حق السعی ( به استثنای مزد، حقوق، پاداش )")]
    public long WorkEffortExcludingWageSalaryBonus { get; set; }
}


