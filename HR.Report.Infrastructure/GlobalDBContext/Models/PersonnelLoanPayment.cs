using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Personnel_Loan_Payment", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Personnel_Loan_Payment_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Personnel_Loan_Payment_PaymentTypeId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelLoanId", Name = "IX_Personnel_Loan_Payment_PersonnelLoanId")]
public partial class PersonnelLoanPayment
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public long PersonnelLoanId { get; set; }

    public bool IsPaid { get; set; }

    public long PaymentAmount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("PersonnelLoanPayments")]
    public virtual PaymentType PaymentType { get; set; } = null!;

    [ForeignKey("PersonnelLoanId")]
    [InverseProperty("PersonnelLoanPayments")]
    public virtual PersonnelLoan PersonnelLoan { get; set; } = null!;
}
