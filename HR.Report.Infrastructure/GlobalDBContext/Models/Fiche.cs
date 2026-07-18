using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Fiche", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Fiche_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Fiche_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Fiche_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("FicheStatusId", Name = "IX_Fiche_FicheStatusId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Fiche_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Fiche_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Fiche_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", "IsDeleted", Name = "IX_Fiche_PaymentPeriodId_IsDeleted")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionId", Name = "IX_Fiche_PersonnelFunctionId")]
[Microsoft.EntityFrameworkCore.Index("PeymanRowId", Name = "IX_Fiche_PeymanRowId")]
public partial class Fiche
{
    [Key]
    public long Id { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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

    public long OrganisationChartId { get; set; }

    public long PersonnelFunctionId { get; set; }

    public long InterdictOrderId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long FicheStatusId { get; set; }

    public long EmployeeId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long CostCenterId { get; set; }

    public long? PeymanRowId { get; set; }

    public long DeductedAmount { get; set; }

    public long TotalAmount { get; set; }

    public long InsuranceTotal_DSW { get; set; }

    public long PurePaymentAmount { get; set; }

    public long PaymentTax { get; set; }

    public long PaymentInsuranceCovered { get; set; }

    public long PaymentRetiredCovered { get; set; }

    public long? DailyFunctionAmount { get; set; }

    public long? UnEmploymentAmount { get; set; }

    public bool? IsActiveInsurance { get; set; }

    public long MonthJobBenefit { get; set; }

    public long? BillBazkharidSanavatAmount { get; set; }

    public long? BillEydiOpadashAmount { get; set; }

    public long? BillSumItemsAmount { get; set; }

    public long? BillItemsWage { get; set; }

    public long? SumCashTaxCoveredAndCountinious { get; set; }

    public long? SumNonCashTaxCoveredAndCountinious { get; set; }

    public long? SumNonCashTaxCoveredAndNotCountinious { get; set; }

    public long? SumCashTaxCoveredAndNotCountinious { get; set; }

    public long NotNetCurrentMonthOverTimePayment { get; set; }

    public long SumOfDelayedCountiniousPaymentInCurrentMonth { get; set; }

    public long HouseAmount { get; set; }

    public long CarAmount { get; set; }

    [StringLength(128)]
    public string? BankAccountNo { get; set; }

    [StringLength(64)]
    public string? InsuranceNo { get; set; }

    public long IncAmount { get; set; }

    public long SpouseAmount { get; set; }

    [StringLength(1500)]
    public string? Description { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("Fiche")]
    public virtual ICollection<BankDisketteItem> BankDisketteItems { get; set; } = new List<BankDisketteItem>();

    [ForeignKey("CostCenterId")]
    [InverseProperty("FicheCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("Fiches")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("Fiche")]
    public virtual ICollection<EmployeeDeductionPayment> EmployeeDeductionPayments { get; set; } = new List<EmployeeDeductionPayment>();

    [InverseProperty("Fiche")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlements { get; set; } = new List<EmployeeSettlement>();

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("Fiches")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [InverseProperty("Fiche")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [InverseProperty("Fiche")]
    public virtual ICollection<FicheLeaveItem> FicheLeaveItems { get; set; } = new List<FicheLeaveItem>();

    [ForeignKey("FicheStatusId")]
    [InverseProperty("Fiches")]
    public virtual FicheStatus FicheStatus { get; set; } = null!;

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("Fiches")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("FicheOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("Fiches")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [ForeignKey("PersonnelFunctionId")]
    [InverseProperty("Fiches")]
    public virtual PersonnelFunction PersonnelFunction { get; set; } = null!;

    [InverseProperty("Fiche")]
    public virtual ICollection<PersonnelPayment> PersonnelPayments { get; set; } = new List<PersonnelPayment>();

    [ForeignKey("PeymanRowId")]
    [InverseProperty("Fiches")]
    public virtual OrganisationPeymanRow? PeymanRow { get; set; }

    [InverseProperty("Fiche")]
    public virtual ICollection<TaxDisketteWh> TaxDisketteWhs { get; set; } = new List<TaxDisketteWh>();

    [InverseProperty("Fiche")]
    public virtual ICollection<TaxDisketteWp> TaxDisketteWps { get; set; } = new List<TaxDisketteWp>();
}
