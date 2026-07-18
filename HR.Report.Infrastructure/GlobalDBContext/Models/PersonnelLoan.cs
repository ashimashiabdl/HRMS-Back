using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Personnel_Loan", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BankBranchId", Name = "IX_Personnel_Loan_BankBranchId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Personnel_Loan_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("LoanTypeId", Name = "IX_Personnel_Loan_LoanTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Personnel_Loan_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("StartDeductPaymentPeriodId", Name = "IX_Personnel_Loan_StartDeductPaymentPeriodId")]
public partial class PersonnelLoan
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long LoanTypeId { get; set; }

    public long? BankBranchId { get; set; }

    public long StartDeductPaymentPeriodId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    public bool IsActive { get; set; }

    [StringLength(128)]
    public string? LoanPaymentDocNo { get; set; }

    [StringLength(128)]
    public string? LoanPaymentDocDesc { get; set; }

    public long? AllAmount { get; set; }

    public long? InstallmentAmount { get; set; }

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(128)]
    public string? AccountNumber { get; set; }

    [StringLength(128)]
    public string? ReciverDesc { get; set; }

    public bool? AutoReceive { get; set; }

    [StringLength(128)]
    public string? ShebaNo { get; set; }

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

    public bool RemainingCrumbsAtFirst { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BankBranchId")]
    [InverseProperty("PersonnelLoans")]
    public virtual BankBranch? BankBranch { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PersonnelLoans")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("PersonnelLoan")]
    public virtual ICollection<FicheItem> FicheItems { get; set; } = new List<FicheItem>();

    [ForeignKey("LoanTypeId")]
    [InverseProperty("PersonnelLoans")]
    public virtual LoanType LoanType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("PersonnelLoans")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("PersonnelLoan")]
    public virtual ICollection<PersonnelLoanPayment> PersonnelLoanPayments { get; set; } = new List<PersonnelLoanPayment>();

    [ForeignKey("StartDeductPaymentPeriodId")]
    [InverseProperty("PersonnelLoans")]
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; } = null!;
}
