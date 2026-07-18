using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Fiche_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("ArearPaymentPeriodId", Name = "IX_Fiche_Item_ArearPaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeDeductionId", Name = "IX_Fiche_Item_EmployeeDeductionId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeFundId", Name = "IX_Fiche_Item_EmployeeFundId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Fiche_Item_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Fiche_Item_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelLoanId", Name = "IX_Fiche_Item_PersonnelLoanId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelPaymentId", Name = "IX_Fiche_Item_PersonnelPaymentId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Fiche_Item_WageItemId")]
public partial class FicheItem
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public long WageItemId { get; set; }

    public long PaymentTypeId { get; set; }

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

    public double Value { get; set; }

    [StringLength(512)]
    public string? Comment { get; set; }

    public long? PersonnelLoanId { get; set; }

    public long? RemainLoanAmount { get; set; }

    /// <summary>
    ///  آیا این قلم منشا معوقه دارد ؟
    /// </summary>
    public bool IsArear { get; set; }

    public long? ArearPaymentPeriodId { get; set; }

    public long? EmployeeDeductionId { get; set; }

    public long? RemainDeductionAmount { get; set; }

    public long? PersonnelPaymentId { get; set; }

    public long? EmployeeFundId { get; set; }

    public long? FundSumAmount { get; set; }

    public bool? IsEmployerItem { get; set; }

    public bool IsSubItem { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("ArearPaymentPeriodId")]
    [InverseProperty("FicheItems")]
    public virtual PaymentPeriod? ArearPaymentPeriod { get; set; }

    [ForeignKey("EmployeeDeductionId")]
    [InverseProperty("FicheItems")]
    public virtual EmployeeDeduction? EmployeeDeduction { get; set; }

    [ForeignKey("EmployeeFundId")]
    [InverseProperty("FicheItems")]
    public virtual EmployeeFund? EmployeeFund { get; set; }

    [ForeignKey("FicheId")]
    [InverseProperty("FicheItems")]
    public virtual Fiche Fiche { get; set; } = null!;

    [ForeignKey("PersonnelLoanId")]
    [InverseProperty("FicheItems")]
    public virtual PersonnelLoan? PersonnelLoan { get; set; }

    [ForeignKey("PersonnelPaymentId")]
    [InverseProperty("FicheItems")]
    public virtual PersonnelPayment? PersonnelPayment { get; set; }

    [ForeignKey("WageItemId")]
    [InverseProperty("FicheItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
