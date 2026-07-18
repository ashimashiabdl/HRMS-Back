using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_FicheItem", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EnterTypeId", Name = "IX_OFI_EnterTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_OFI_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationCheckFormulaId", Name = "IX_OFI_OrganisationCheckFormulaId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_OFI_OrganisationFormulaId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_OFI_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_OFI_WageItemId")]
[Microsoft.EntityFrameworkCore.Index("EnterTypeId", Name = "IX_Organisation_FicheItem_EnterTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_FicheItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationCheckFormulaId", Name = "IX_Organisation_FicheItem_OrganisationCheckFormulaId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_Organisation_FicheItem_OrganisationFormulaId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Organisation_FicheItem_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Organisation_FicheItem_WageItemId")]
public partial class OrganisationFicheItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long WageItemId { get; set; }

    public long EnterTypeId { get; set; }

    public long PaymentTypeId { get; set; }

    public long? OrganisationCheckFormulaId { get; set; }

    public long? OrganisationFormulaId { get; set; }

    public bool Continuous { get; set; }

    public bool ShowZeroInFiche { get; set; }

    public bool IsInsuranceCovered { get; set; }

    public bool IsTaxCovered { get; set; }

    public bool RetiredCovered { get; set; }

    public bool DailyCovered { get; set; }

    public bool IsDaily { get; set; }

    public int Priority { get; set; }

    public int FixValue { get; set; }

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

    [StringLength(512)]
    public string? Description { get; set; }

    public bool? IsFixed { get; set; }

    public bool IsTaminInsurance { get; set; }

    public bool OnceInFiche { get; set; }

    public bool ArearsCovered { get; set; }

    public bool? IsEmployerItem { get; set; }

    public bool? RetiredCover { get; set; }

    public bool IsVirtual { get; set; }

    /// <summary>
    /// �� ��� ��� ���� ������ ��� ��� ��� ���� ����� ������� ������� �� ���
    /// </summary>
    public bool IsCarWageItemForTax { get; set; }

    /// <summary>
    /// ����� ��� ����� ��� ���� - �����
    /// </summary>
    public bool IsCaseBonusWageItemForTax { get; set; }

    /// <summary>
    /// �� ��� ��� ���� ������ ��� ��� ��� ���� ���� ������� ������� �� ���
    /// </summary>
    public bool IsHouseWageItemForTax { get; set; }

    /// <summary>
    /// �� ���� ������� ����� ���� 137 ����� ������ ��� ������
    /// </summary>
    public bool IsInsuranceWageItemForTax { get; set; }

    /// <summary>
    /// ����� ��� ������ ����� ���� 137 ����� ������ ��� ������
    /// </summary>
    public bool IsMedicalExpensesArticle137WageItemForTax { get; set; }

    public bool IsSpouse { get; set; }

    public bool IsChildItem { get; set; }

    /// <summary>
    /// عیدی سالانه- ریالی
    /// </summary>
    public bool IsAnnualEydi { get; set; }

    /// <summary>
    /// پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی
    /// </summary>
    public bool IsBonusExceptYearEndServiceEndProductivity { get; set; }

    /// <summary>
    /// مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف
    /// </summary>
    public bool IsConsultingFeesAndSimilar { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی
    /// </summary>
    public bool IsContinuousCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public bool IsContinuousNonCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی
    /// </summary>
    public bool IsContinuousNonCashOtherBenefitsCost { get; set; }

    /// <summary>
    /// خسارت اخراج- ریالی
    /// </summary>
    public bool IsDismissalCompensation { get; set; }

    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی
    /// </summary>
    public bool IsEmployeeCarDeductionCurrentMonth { get; set; }

    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی
    /// </summary>
    public bool IsEmployeeHousingDeductionCurrentMonth { get; set; }

    /// <summary>
    /// پاداش پایان خدمت- ریالی
    /// </summary>
    public bool IsEndOfServiceBonus { get; set; }

    /// <summary>
    /// مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی
    /// </summary>
    public bool IsGrossContinuousCashCurrentMonth { get; set; }

    /// <summary>
    /// کارانه- ریالی
    /// </summary>
    public bool IsKaraneh { get; set; }

    /// <summary>
    /// حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م
    /// </summary>
    public bool IsLifeInsuranceArticle137 { get; set; }

    /// <summary>
    /// حق بیمه های درمان موضوع ماده  137ق.م.م
    /// </summary>
    public bool IsMedicalInsuranceArticle137 { get; set; }

    /// <summary>
    /// فوق العاده مسافرت (ماموریت) - ریالی
    /// </summary>
    public bool IsMissionAllowance { get; set; }

    /// <summary>
    /// مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public bool IsNonContinuousCashArearsNoTax { get; set; }

    /// <summary>
    /// سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی
    /// </summary>
    public bool IsNonContinuousCashCurrentMonth { get; set; }

    /// <summary>
    /// مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی
    /// </summary>
    public bool IsNonContinuousNonCashArearsNoTax { get; set; }

    /// <summary>
    /// مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی
    /// </summary>
    public bool IsNonContinuousNonCashCostCurrentMonth { get; set; }

    /// <summary>
    /// حق کشیک
    /// </summary>
    public bool IsOnCallPay { get; set; }

    /// <summary>
    /// اضافه کاری- ریالی
    /// </summary>
    public bool IsOvertime { get; set; }

    /// <summary>
    /// مبلغ قراردادهای پژوهشی- ریالی
    /// </summary>
    public bool IsResearchContracts { get; set; }

    /// <summary>
    /// بازخرید خدمت- ریالی
    /// </summary>
    public bool IsServiceBuyback { get; set; }

    /// <summary>
    /// حق سنوات- ریالی
    /// </summary>
    public bool IsSeverancePay { get; set; }

    /// <summary>
    /// حق التدریس/حق التحقیق/ حق پژوهش
    /// </summary>
    public bool IsTeachingResearchFees { get; set; }

    /// <summary>
    /// هزینه سفر- ریالی
    /// </summary>
    public bool IsTravelExpense { get; set; }

    /// <summary>
    /// حقوق ایام مرخصی استفاده نشده- ریالی
    /// </summary>
    public bool IsUnusedLeavePay { get; set; }

    /// <summary>
    /// رفاهی و انگیزشی و بهره وری
    /// </summary>
    public bool IsWelfareMotivationProductivity { get; set; }

    /// <summary>
    /// حق السعی ( به استثنای مزد، حقوق، پاداش )
    /// </summary>
    public bool IsWorkEffortExcludingWageSalaryBonus { get; set; }

    /// <summary>
    /// پاداش آخر سال- ریالی
    /// </summary>
    public bool IsYearEndBonus { get; set; }

    /// <summary>
    /// قلم اصلی مالیات
    /// </summary>
    public bool IsMainTaxItem { get; set; }

    /// <summary>
    /// مالیات پذیر مستمر نقدی
    /// </summary>
    public bool IsTaxableContinuousCash { get; set; }

    /// <summary>
    /// مالیات پذیر غیر مستمر نقدی
    /// </summary>
    public bool IsTaxableNonContinuousCash { get; set; }

    /// <summary>
    /// مالیات پذیر مستمر غیر نقدی
    /// </summary>
    public bool IsTaxableContinuousNonCash { get; set; }

    /// <summary>
    /// مالیات پذیر غیر مستمر غیر نقدی
    /// </summary>
    public bool IsTaxableNonContinuousNonCash { get; set; }

    /// <summary>
    /// صفر شدن معوقه منفی
    /// </summary>
    public bool ZeroNegativeArears { get; set; }

    /// <summary>
    /// مالیات خاص
    /// </summary>
    public bool IsSpecialTax { get; set; }

    /// <summary>
    /// تخفیف مالیاتی
    /// </summary>
    public bool IsTaxDiscount { get; set; }

    /// <summary>
    /// معوقه پذیر سال جاری
    /// </summary>
    public bool CurrentYearArearsCovered { get; set; }

    /// <summary>
    /// قلم فرعی می باشد
    /// </summary>
    public bool IsSubItem { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EnterTypeId")]
    [InverseProperty("OrganisationFicheItemEnterTypes")]
    public virtual BaseTableValue EnterType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationFicheItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationCheckFormulaId")]
    [InverseProperty("OrganisationFicheItemOrganisationCheckFormulas")]
    public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }

    [ForeignKey("OrganisationFormulaId")]
    [InverseProperty("OrganisationFicheItemOrganisationFormulas")]
    public virtual OrganisationFormula? OrganisationFormula { get; set; }

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("OrganisationFicheItemPaymentTypes")]
    public virtual BaseTableValue PaymentType { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("OrganisationFicheItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
