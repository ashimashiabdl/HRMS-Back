using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_DisketteWH", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Tax_DisketteWH_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Tax_DisketteWH_FicheId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Tax_DisketteWH_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Tax_DisketteWH_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteId", Name = "IX_Tax_DisketteWH_TaxDisketteId")]
public partial class TaxDisketteWh
{
    [Key]
    public long Id { get; set; }

    public long TaxDisketteId { get; set; }

    /// <summary>
    /// شناسه کارمند
    /// </summary>
    public long EmployeeId { get; set; }

    public long InterdictOrderId { get; set; }

    public long FicheId { get; set; }

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

    public long PaymentTypeId { get; set; }

    /// <summary>
    /// عیدی سالانه- ریالی
    /// </summary>
    public long AnnualEydi { get; set; }

    /// <summary>
    /// پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی
    /// </summary>
    public long BonusExceptYearEndServiceEndProductivity { get; set; }

    public int CarStatusId { get; set; }

    /// <summary>
    /// مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف
    /// </summary>
    public long ConsultingFeesAndSimilar { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی
    /// </summary>
    public long ContinuousCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public long ContinuousNonCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی
    /// </summary>
    public long ContinuousNonCashOtherBenefitsCost { get; set; }

    public long CurrencyCode { get; set; }

    /// <summary>
    /// خسارت اخراج- ریالی
    /// </summary>
    public long DismissalCompensation { get; set; }

    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی
    /// </summary>
    public long EmployeeCarDeductionCurrentMonth { get; set; }

    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی
    /// </summary>
    public long EmployeeHousingDeductionCurrentMonth { get; set; }

    /// <summary>
    /// پاداش پایان خدمت- ریالی
    /// </summary>
    public long EndOfServiceBonus { get; set; }

    public long ExceptionsSubjectToTheBudgetLawOf1404 { get; set; }

    public long ExchangeRateOfCurrency { get; set; }

    /// <summary>
    /// مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی
    /// </summary>
    public long GrossContinuousCashCurrentMonth { get; set; }

    public int HouseStatusId { get; set; }

    /// <summary>
    /// کارانه- ریالی
    /// </summary>
    public long Karaneh { get; set; }

    /// <summary>
    /// حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م
    /// </summary>
    public long LifeInsuranceArticle137 { get; set; }

    /// <summary>
    /// حق بیمه های درمان موضوع ماده  137ق.م.م
    /// </summary>
    public long MedicalInsuranceArticle137 { get; set; }

    /// <summary>
    /// فوق العاده مسافرت (ماموریت) - ریالی
    /// </summary>
    public long MissionAllowance { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public long NonContinuousCashArearsNoTax { get; set; }

    /// <summary>
    /// سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی
    /// </summary>
    public long NonContinuousCashCurrentMonth { get; set; }

    /// <summary>
    /// مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public long NonContinuousNonCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی
    /// </summary>
    public long NonContinuousNonCashCostCurrentMonth { get; set; }

    /// <summary>
    /// حق کشیک
    /// </summary>
    public long OnCallPay { get; set; }

    /// <summary>
    /// اضافه کاری- ریالی
    /// </summary>
    public long Overtime { get; set; }

    /// <summary>
    /// مبلغ قراردادهای پژوهشی- ریالی
    /// </summary>
    public long ResearchContracts { get; set; }

    /// <summary>
    /// بازخرید خدمت- ریالی
    /// </summary>
    public long ServiceBuyback { get; set; }

    /// <summary>
    /// حق سنوات- ریالی
    /// </summary>
    public long SeverancePay { get; set; }

    /// <summary>
    /// حق التدریس/حق التحقیق/ حق پژوهش
    /// </summary>
    public long TeachingResearchFees { get; set; }

    /// <summary>
    /// هزینه سفر- ریالی
    /// </summary>
    public long TravelExpense { get; set; }

    /// <summary>
    /// حقوق ایام مرخصی استفاده نشده- ریالی
    /// </summary>
    public long UnusedLeavePay { get; set; }

    /// <summary>
    /// رفاهی و انگیزشی و بهره وری
    /// </summary>
    public long WelfareMotivationProductivity { get; set; }

    /// <summary>
    /// حق السعی ( به استثنای مزد، حقوق، پاداش )
    /// </summary>
    public long WorkEffortExcludingWageSalaryBonus { get; set; }

    public long WorkplaceStatusId { get; set; }

    /// <summary>
    /// پاداش آخر سال- ریالی
    /// </summary>
    public long YearEndBonus { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    public long PurePaymentAmount { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TaxDisketteWhs")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("TaxDisketteWhs")]
    public virtual Fiche Fiche { get; set; } = null!;

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("TaxDisketteWhs")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("TaxDisketteWhs")]
    public virtual PaymentType PaymentType { get; set; } = null!;

    [ForeignKey("TaxDisketteId")]
    [InverseProperty("TaxDisketteWhs")]
    public virtual TaxDiskette TaxDiskette { get; set; } = null!;
}
