using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Payment_Period", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Payment_Period_OrganisationChartId")]
public partial class PaymentPeriod
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public int ShamsiYear { get; set; }

    public int ShamsiMonth { get; set; }

    public int PeriodDays { get; set; }

    public bool IsClosed { get; set; }

    public bool UpdatedOnSite { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<BankDiskette> BankDiskettes { get; set; } = new List<BankDiskette>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<BatchLog> BatchLogs { get; set; } = new List<BatchLog>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<BillInstance> BillInstances { get; set; } = new List<BillInstance>();

    [InverseProperty("StartDeductedPaymentPeriod")]
    public virtual ICollection<DeductedArear> DeductedArears { get; set; } = new List<DeductedArear>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<DeductedArearsDetail> DeductedArearsDetails { get; set; } = new List<DeductedArearsDetail>();

    [InverseProperty("StartDeductPaymentPeriod")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [InverseProperty("StartDeductPaymentPeriod")]
    public virtual ICollection<EmployeeFund> EmployeeFunds { get; set; } = new List<EmployeeFund>();

    [InverseProperty("ArearPaymentPeriod")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<FichePdfArchive> FichePdfArchives { get; set; } = new List<FichePdfArchive>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<FicheReportArchive> FicheReportArchives { get; set; } = new List<FicheReportArchive>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<Fiche> Fiches { get; set; } = new List<Fiche>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<InsuranceDiskette> InsuranceDiskettes { get; set; } = new List<InsuranceDiskette>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PaymentPeriods")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<PaymentPeriodEmployeeBonu> PaymentPeriodEmployeeBonus { get; set; } = new List<PaymentPeriodEmployeeBonu>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; } = new List<PersonnelFunctionExcelFile>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<PersonnelLeave> PersonnelLeaves { get; set; } = new List<PersonnelLeave>();

    [InverseProperty("StartDeductPaymentPeriod")]
    public virtual ICollection<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<StatusList> StatusLists { get; set; } = new List<StatusList>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<TaxDiskette> TaxDiskettes { get; set; } = new List<TaxDiskette>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<TaxNonCashPayment> TaxNonCashPayments { get; set; } = new List<TaxNonCashPayment>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<TaxableIncome> TaxableIncomes { get; set; } = new List<TaxableIncome>();

    [InverseProperty("StartDeductPaymentPeriod")]
    public virtual ICollection<TempEmployeeDeduction> TempEmployeeDeductions { get; set; } = new List<TempEmployeeDeduction>();

    [InverseProperty("PaymentPeriod")]
    public virtual ICollection<TempPersonnelLeave> TempPersonnelLeaves { get; set; } = new List<TempPersonnelLeave>();
}
