using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HR.Payroll.Core.DTOs;

public class OrganisationEmployeeTypeFicheItemDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long EmployeeTypeId { get; set; }
    public string? EmployeeType { get; set; }
    public long WageItemId { get; set; }
    public string? WageItem { get; set; }
    public long? EnterTypeId { get; set; }
    public string? EnterType { get; set; }
    public long? PersonnelPaymentId { get; set; }
    public long? PaymentTypeId { get; set; }
    public string? PaymentType { get; set; }
    public long? OrganisationCheckFormulaId { get; set; }
    public string? OrganisationCheckFormula { get; set; }
    public long? OrganisationFormulaId { get; set; }
    public string? OrganisationFormula { get; set; }

    public bool IsArear { get; set; }
    //public long? TypeId { get; set; }
    //public string? Type { get; set; }
    /// <summary>
    /// قلم مستمر
    /// </summary>
    public bool? Continuous { get; set; }
    public bool? ShowZeroInFiche { get; set; }
    /// <summary>
    /// قلم محاسبه ای
    /// </summary>
    public bool IsVirtual { get; set; }
    
    public bool? IsInsuranceCovered { get; set; }
    /// <summary>
    /// مشمول مالیات
    /// </summary>
    public bool? IsTaxCovered { get; set; }
    public bool? RetiredCovered { get; set; }
    public bool? DailyCovered { get; set; }
    public bool? IsDaily { get; set; }
    public bool? UseDefaultOrganSetting { get; set; }
    public int? Priority { get; set; }
    public int? FixValue { get; set; }
    [StringLength(512)]
    public string? Description { get; set; }
    public double Amount { get; set; }
    public string? FormulaFriendlyText { get; set; }
    public Dictionary<string, string?>? VariableFriendlyList { get; set; }
    public string? FormulaText { get; set; }
    public FormulaExecutionTree? FormulaTreeParser { get; set; }
    public string? FormulaHelpDesc { get; set; }
    public bool IsRowSuccess { get; set; }
    public long? RemainLoanAmount { get; set; }
    public long? PersonnelLoanId { get; set; }
    public  string? PersonnelLoan { get; set; }

    public long? RemainDeductionAmount { get; set; }
    public long? EmployeeDeductionId { get; set; }
    public  string? EmployeeDeduction { get; set; }
    public long PersonnelFicheItemId { get; set; }

    
    public int SuccessRunTimeInmilliseconds { get; set; }
    public string? formularowmessage { get; set; }
    public int Index { get; set; }
    public bool? IsFixed { get; set; }
    public bool OnceInFiche { get; set; }
    public bool IsTaminInsurance { get; set; }
    //public int DurationMonth { get; set; }
    //public bool DailyFunctionAmountCovered { get; set; }
    //public bool ManagerCovered { get; set; }
    public bool ArearsCovered { get; set; }
    public bool? IsEmployerItem { get; set; }
    public bool? RetiredCover { get; set; }
    /// <summary>
    /// به عنوان حق عائله برای بیمه ارسال شود
    /// </summary>
    public bool IsSpouse { get; set; }
    /// <summary>
    /// قلم حق اولاد است و نباید در جمع دستمزد و مزایای ماهانه مشمول برای بیمه جمع زده شود
    /// </summary>
    public bool IsChildItem { get; set; }
    public bool IsForOtherSources { get; set; }
    public string? Origin { get; set; }
    public int OriginId { get; set; }
    public long? ArearPaymentPeriodId { get; set; }

    // Tax reporting/category flags (mirror of entity)
    public bool IsGrossContinuousCashCurrentMonth { get; set; }
    public bool IsContinuousCashArearsNoTax { get; set; }
    public bool IsEmployeeHousingDeductionCurrentMonth { get; set; }
    public bool IsEmployeeCarDeductionCurrentMonth { get; set; }
    public bool IsContinuousNonCashOtherBenefitsCost { get; set; }
    public bool IsContinuousNonCashArearsNoTax { get; set; }
    public bool IsConsultingFeesAndSimilar { get; set; }
    public bool IsResearchContracts { get; set; }
    public bool IsOvertime { get; set; }
    public bool IsTravelExpense { get; set; }
    public bool IsMissionAllowance { get; set; }
    public bool IsKaraneh { get; set; }
    public bool IsBonusExceptYearEndServiceEndProductivity { get; set; }
    public bool IsYearEndBonus { get; set; }
    public bool IsAnnualEydi { get; set; }
    public bool IsEndOfServiceBonus { get; set; }
    public bool IsDismissalCompensation { get; set; }
    public bool IsServiceBuyback { get; set; }
    public bool IsSeverancePay { get; set; }
    public bool IsUnusedLeavePay { get; set; }
    public bool IsNonContinuousCashCurrentMonth { get; set; }
    public bool IsNonContinuousCashArearsNoTax { get; set; }
    public bool IsNonContinuousNonCashCostCurrentMonth { get; set; }
    public bool IsNonContinuousNonCashArearsNoTax { get; set; }
    public bool IsMedicalInsuranceArticle137 { get; set; }
    public bool IsLifeInsuranceArticle137 { get; set; }
    public bool IsTeachingResearchFees { get; set; }
    public bool IsOnCallPay { get; set; }
    public bool IsWelfareMotivationProductivity { get; set; }
    public bool IsWorkEffortExcludingWageSalaryBonus { get; set; }
    public bool IsMainTaxItem { get; set; }
    /// <summary>مالیات پذیر مستمر نقدی</summary>
    public bool IsTaxableContinuousCash { get; set; }
    /// <summary>مالیات پذیر غیر مستمر نقدی</summary>
    public bool IsTaxableNonContinuousCash { get; set; }
    /// <summary>مالیات پذیر مستمر غیر نقدی</summary>
    public bool IsTaxableContinuousNonCash { get; set; }
    /// <summary>مالیات پذیر غیر مستمر غیر نقدی</summary>
    public bool IsTaxableNonContinuousNonCash { get; set; }
    /// <summary>صفر شدن معوقه منفی</summary>
    public bool ZeroNegativeArears { get; set; }
    /// <summary>مالیات خاص</summary>
    public bool IsSpecialTax { get; set; }
    /// <summary>تخفیف مالیاتی</summary>
    public bool IsTaxDiscount { get; set; }
    /// <summary>معوقه پذیر سال جاری</summary>
    public bool CurrentYearArearsCovered { get; set; }
    /// <summary>
    /// قلم فرعی می باشد
    /// </summary>
    public bool IsSubItem { get; set; }

    /// <summary>
    /// قلم وام غیرفعال است و مبلغ آن عمداً صفر شده است.
    /// </summary>
    public bool IsInactiveLoanSuppressed { get; set; }

}
