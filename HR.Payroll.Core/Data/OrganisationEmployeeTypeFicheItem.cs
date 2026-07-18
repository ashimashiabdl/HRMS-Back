using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.Payroll.Core.Interface;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Organisation_EmployeeType_FicheItem", Schema = "Payroll")]
public class OrganisationEmployeeTypeFicheItem : BaseEntity, IOrganisationChartId , IWageItemAdvanceSetting
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }
    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual WageItem? WageItem { get; set; }
    public long? EnterTypeId { get; set; }
    public virtual BaseTableValue? EnterType { get; set; }
    public long? PaymentTypeId { get; set; }
    public virtual BaseTableValue? PaymentType { get; set; }
    [ForeignKey("OrganisationCheckFormula")]
    public long? OrganisationCheckFormulaId { get; set; }
    public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }
    [ForeignKey("OrganisationFormula")]
    public long? OrganisationFormulaId { get; set; }
    public virtual OrganisationFormula? OrganisationFormula { get; set; }
    //[ForeignKey("Type")]
    //public long? TypeId { get; set; }
    //public virtual BaseTableValue? Type { get; set; }
    public bool Continuous { get; set; }
    public bool ShowZeroInFiche { get; set; }
    public bool IsVirtual { get; set; }

    //public bool IsDelayable { get; set; }
    /// <summary>
    /// به عنوان حق عائله برای بیمه ارسال شود
    /// </summary>
    public bool IsSpouse { get; set; }
    /// <summary>
    /// قلم حق اولاد است و نباید در جمع دستمزد و مزایای ماهانه مشمول برای بیمه جمع زده شود
    /// </summary>
    [Comment("قلم حق اولاد است و نباید در جمع دستمزد و مزایای ماهانه مشمول برای بیمه جمع زده شود")]
    public bool IsChildItem { get; set; }
    public bool IsInsuranceCovered { get; set; }
    public bool IsTaxCovered { get; set; }
    public bool RetiredCovered { get; set; }
    public bool DailyCovered { get; set; }
    public bool IsDaily { get; set; }
    public int Priority { get; set; }
    public int FixValue { get; set; }
    [StringLength(512)]
    public string? Description { get; set; }
    public bool? IsFixed { get; set; }
    public bool OnceInFiche { get; set; }
    public bool IsTaminInsurance { get; set; }
    //public int DurationMonth { get; set; }
    //public bool DailyFunctionAmountCovered { get; set; }
    //public bool ManagerCovered { get; set; }
    public bool ArearsCovered { get; set; }
    public bool? IsEmployerItem { get; set; }
    public bool? RetiredCover { get; set; }
    public bool? UseDefaultOrganSetting { get; set; }
    /// <summary>
    /// به معني اين است که منشا قلم نوع عضويت نباشد
    /// </summary>
    public bool IsForOtherSources { get; set; }

    /// <summary>
    /// از اين قلم براي محاسبه رقم کسر شده بابت منزل سازماني استفاده مي شود
    /// </summary>
    [Comment("از اين قلم براي محاسبه رقم کسر شده بابت منزل سازماني استفاده مي شود")]
    public bool IsHouseWageItemForTax { get; set; }
    /// <summary>
    /// از اين قلم براي محاسبه رقم کسر شده بابت خودرو سازماني استفاده مي شود
    /// </summary>
    [Comment("از اين قلم براي محاسبه رقم کسر شده بابت خودرو سازماني استفاده مي شود")]
    public bool IsCarWageItemForTax { get; set; }
    /// <summary>
    /// حق بيمه پرداختي موضوع ماده 137 قانون ماليات هاي مستقيم
    /// </summary>
    [Comment("حق بيمه پرداختي موضوع ماده 137 قانون ماليات هاي مستقيم")]
    public bool IsInsuranceWageItemForTax { get; set; }
    /// <summary>
    /// هزينه هاي درماني موضوع ماده 137 قانون ماليات هاي مستقيم
    /// </summary>
    [Comment("هزينه هاي درماني موضوع ماده 137 قانون ماليات هاي مستقيم")]
    public bool IsMedicalExpensesArticle137WageItemForTax { get; set; }
    /// <summary>
    /// پاداش هاي موردي ماه جاري - ريالي
    /// </summary>
    [Comment("پاداش هاي موردي ماه جاري - ريالي")]
    public bool IsCaseBonusWageItemForTax { get; set; }

    // Tax reporting/category flags (same as OrganisationFicheItem)
    [Comment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی")]
    public bool IsGrossContinuousCashCurrentMonth { get; set; }
    [Comment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی")]
    public bool IsContinuousCashArearsNoTax { get; set; }
    [Comment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی")]
    public bool IsEmployeeHousingDeductionCurrentMonth { get; set; }
    [Comment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی")]
    public bool IsEmployeeCarDeductionCurrentMonth { get; set; }
    [Comment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی")]
    public bool IsContinuousNonCashOtherBenefitsCost { get; set; }
    [Comment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public bool IsContinuousNonCashArearsNoTax { get; set; }
    [Comment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف")]
    public bool IsConsultingFeesAndSimilar { get; set; }
    [Comment("مبلغ قراردادهای پژوهشی- ریالی")]
    public bool IsResearchContracts { get; set; }
    [Comment("اضافه کاری- ریالی")]
    public bool IsOvertime { get; set; }
    [Comment("هزینه سفر- ریالی")]
    public bool IsTravelExpense { get; set; }
    [Comment("فوق العاده مسافرت (ماموریت) - ریالی")]
    public bool IsMissionAllowance { get; set; }
    [Comment("کارانه- ریالی")]
    public bool IsKaraneh { get; set; }
    [Comment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی")]
    public bool IsBonusExceptYearEndServiceEndProductivity { get; set; }
    [Comment("پاداش آخر سال- ریالی")]
    public bool IsYearEndBonus { get; set; }
    [Comment("عیدی سالانه- ریالی")]
    public bool IsAnnualEydi { get; set; }
    [Comment("پاداش پایان خدمت- ریالی")]
    public bool IsEndOfServiceBonus { get; set; }
    [Comment("خسارت اخراج- ریالی")]
    public bool IsDismissalCompensation { get; set; }
    [Comment("بازخرید خدمت- ریالی")]
    public bool IsServiceBuyback { get; set; }
    [Comment("حق سنوات- ریالی")]
    public bool IsSeverancePay { get; set; }
    [Comment("حقوق ایام مرخصی استفاده نشده- ریالی")]
    public bool IsUnusedLeavePay { get; set; }
    [Comment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی")]
    public bool IsNonContinuousCashCurrentMonth { get; set; }
    [Comment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public bool IsNonContinuousCashArearsNoTax { get; set; }
    [Comment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی")]
    public bool IsNonContinuousNonCashCostCurrentMonth { get; set; }
    [Comment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی")]
    public bool IsNonContinuousNonCashArearsNoTax { get; set; }
    [Comment("حق بیمه های درمان موضوع ماده  137ق.م.م")]
    public bool IsMedicalInsuranceArticle137 { get; set; }
    [Comment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م")]
    public bool IsLifeInsuranceArticle137 { get; set; }
    [Comment("حق التدریس/حق التحقیق/ حق پژوهش")]
    public bool IsTeachingResearchFees { get; set; }
    [Comment("حق کشیک")]
    public bool IsOnCallPay { get; set; }
    [Comment("رفاهی و انگیزشی و بهره وری")]
    public bool IsWelfareMotivationProductivity { get; set; }
    [Comment("حق السعی ( به استثنای مزد، حقوق، پاداش )")]
    public bool IsWorkEffortExcludingWageSalaryBonus { get; set; }
    [Comment("قلم اصلی مالیات")]
    public bool IsMainTaxItem { get; set; }
    [Comment("مالیات پذیر مستمر نقدی")]
    public bool IsTaxableContinuousCash { get; set; }
    [Comment("مالیات پذیر غیر مستمر نقدی")]
    public bool IsTaxableNonContinuousCash { get; set; }
    [Comment("مالیات پذیر مستمر غیر نقدی")]
    public bool IsTaxableContinuousNonCash { get; set; }
    [Comment("مالیات پذیر غیر مستمر غیر نقدی")]
    public bool IsTaxableNonContinuousNonCash { get; set; }
    [Comment("صفر شدن معوقه منفی")]
    public bool ZeroNegativeArears { get; set; }
    [Comment("مالیات خاص")]
    public bool IsSpecialTax { get; set; }
    [Comment("تخفیف مالیاتی")]
    public bool IsTaxDiscount { get; set; }
    [Comment("معوقه پذیر سال جاری")]
    public bool CurrentYearArearsCovered { get; set; }
    /// <summary>
    /// قلم فرعی می باشد
    /// </summary>
    [Comment("قلم فرعی می باشد")]
    public bool IsSubItem { get; set; }

    [NotMapped]
    private new string title { get; set; }
}
