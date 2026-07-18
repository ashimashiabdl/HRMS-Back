using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Fiche_Item", Schema = "Payroll")]
public class FicheItem : SharedKernel.Data.BaseEntity
{
    [ForeignKey("Fiche")]
    public long FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }
    [ForeignKey("WageItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long WageItemId { get; set; }
    public virtual WageItem? WageItem { get; set; }
    public long PaymentTypeId { get; set; }
    public double Value { get; set; }
    [StringLength(512)]
    public string? Comment { get; set; }
    public long? RemainLoanAmount { get; set; }
    [ForeignKey("PersonnelLoan")]
    public long? PersonnelLoanId { get; set; }
    public virtual PersonnelLoan? PersonnelLoan { get; set; }

    [ForeignKey("EmployeeFund")]
    public long? EmployeeFundId { get; set; }
    public virtual EmployeeFund? EmployeeFund { get; set; }
    /// <summary>
    /// مجموع پرداختی ها به این صنندوق
    /// </summary>
    public long? FundSumAmount { get; set; }

    [ForeignKey("EmployeeDeduction")]
    public long? EmployeeDeductionId { get; set; }
    public virtual EmployeeDeduction? EmployeeDeduction { get; set; }

    [ForeignKey("PersonnelPayment")]
    public long? PersonnelPaymentId { get; set; }
    public virtual PersonnelPayment? PersonnelPayment { get; set; }

    public long? RemainDeductionAmount { get; set; }

    /// <summary>
    /// آیا این قلم منشا معوقه دارد ؟
    /// </summary>
    [Comment(" آیا این قلم منشا معوقه دارد ؟")]
    public bool IsArear { get; set; }

    [ForeignKey("ArearPaymentPeriod")]
    public long? ArearPaymentPeriodId { get; set; }
    public virtual PaymentPeriod? ArearPaymentPeriod { get; set; }

    public bool? IsEmployerItem { get; set; }

    /// <summary>
    /// قلم فرعی می باشد
    /// </summary>
    [Comment("قلم فرعی می باشد")]
    public bool IsSubItem { get; set; }

    [NotMapped]
    private new string title { get; set; }

}
