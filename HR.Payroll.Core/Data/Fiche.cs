
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hr.SystemSetting.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Order.Core.Data;

namespace HR.Payroll.Core.Data;

[Table("Fiche", Schema = "Payroll")]
public class Fiche : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("PersonnelFunction")]
    public long PersonnelFunctionId { get; set; }
    public virtual PersonnelFunction? PersonnelFunction { get; set; }

    [ForeignKey("InterdictOrder")]
    public long InterdictOrderId { get; set; }
    public virtual InterdictOrder? InterdictOrder { get; set; }

    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }

    [ForeignKey("FicheStatus")]
    public long FicheStatusId { get; set; }
    public virtual FicheStatus? FicheStatus { get; set; }

    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeType? EmployeeType { get; set; }

    [ForeignKey("CostCenter")]
    public long CostCenterId { get; set; }
    public virtual OrganisationChart? CostCenter { get; set; }

    /// <summary>
    /// توضیحات فیش
    /// </summary>
    [StringLength(1500)]
    public string? Description { get; set; }

    /// <summary>
    /// ردیف پیمان متناظر
    /// </summary>
    [ForeignKey("PeymanRow")]
    [Comment("ردیف پیمان متناظر")]
    public long? PeymanRowId { get; set; }
    public virtual OrganisationPeymanRow? PeymanRow { get; set; }
    /// <summary>
    /// جمع کسورات با احتساب قلم‌های فرعی
    /// </summary>
    public long DeductedAmount { get; set; }
    /// <summary>
    /// جمع کل پرداختی با احتساب قلم‌های فرعی
    /// </summary>
    public long TotalAmount { get; set; }
    /// <summary>
    /// جمع کل دستمزد و مزایای ماهانه برای دیسکت بیمه (بدون اقلام حق اولاد)
    /// </summary>
    [Comment("جمع کل دستمزد و مزایای ماهانه برای دیسکت بیمه (بدون اقلام حق اولاد)")]
    public long InsuranceTotal_DSW { get; set; }
    /// <summary>
    /// خالص پرداختی به حقوق بگیر با احتساب قلم‌های فرعی
    /// </summary>
    public long PurePaymentAmount { get; set; }
    /// <summary>
    /// مبلغ مالیات
    /// </summary>
    public long PaymentTax { get; set; }
    public long PaymentInsuranceCovered { get; set; }
    public long PaymentRetiredCovered { get; set; }
    /// <summary>
    /// مبلغ کارکرد روزانه
    /// </summary>
    public long? DailyFunctionAmount { get; set; }
    /// <summary>
    /// مبلغ بیمه بیکاری
    /// </summary>
    public long? UnEmploymentAmount { get; set; }
    public bool? IsActiveInsurance { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long MonthJobBenefit { get; set; }
    /// <summary>
    /// حق تاهل در فهرست بیمه
    /// </summary>
    public long SpouseAmount { get; set; }
    /// <summary>
    /// سنوات در فهرست بیمه
    /// </summary>
    public long IncAmount { get; set; }


    public long? BillBazkharidSanavatAmount { get; set; }
    public long? BillEydiOpadashAmount { get; set; }
    public long? BillSumItemsAmount { get; set; }
    public long? BillItemsWage { get; set; }
    /// <summary>
    /// جمع مزایای مستمر نقدی و مشمول مالیات
    /// </summary>
    public long? SumCashTaxCoveredAndCountinious { get; set; }
    /// <summary>
    /// جمع مزایای مستمر غیر نقدی و مشمول مالیات
    /// </summary>
    public long? SumNonCashTaxCoveredAndCountinious { get; set; }
    /// <summary>
    /// جمع مزایای غیر مستمر غیر نقدی و مشمول مالیات
    /// </summary>
    public long? SumNonCashTaxCoveredAndNotCountinious { get; set; }
    /// <summary>
    /// جمع مزایای غیر مستمر نقدی و مشمول مالیات
    /// </summary>
    public long? SumCashTaxCoveredAndNotCountinious { get; set; }
    /// <summary>
    /// نا خالص اضافه کاری ماه جاری - ریالی
    /// </summary>
    [Comment("نا خالص اضافه کاری ماه جاری - ریالی")]
    public long NotNetCurrentMonthOverTimePayment { get; set; }
    /// <summary>
    /// پرداخت های مستمر معوق که مالیاتی برای آن ها محاسبه نشده است - ریالی
    /// </summary>
    [Comment("پرداخت های مستمر معوق که مالیاتی برای آن ها محاسبه نشده است - ریالی")]
    public long SumOfDelayedCountiniousPaymentInCurrentMonth { get; set; }
    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری
    /// </summary>
    [Comment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری")]
    public long HouseAmount { get; set; }
    /// <summary>
    /// مبلغ کسر شده از حقوق کارمند بابت وسیله نقلیه ماه جاری
    /// </summary>
    [Comment("مبلغ کسر شده از حقوق کارمند بابت وسیله نقلیه ماه جاری")]
    public long CarAmount { get; set; }
    [StringLength(128)]
    public string? BankAccountNo { get; set; }      
    [StringLength(64)]
    public string? InsuranceNo { get; set; }

   
    
}
