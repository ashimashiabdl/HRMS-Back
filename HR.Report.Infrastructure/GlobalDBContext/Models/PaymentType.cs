using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Payment_Type", Schema = "Payroll")]
public partial class PaymentType
{
    [Key]
    public long Id { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeducted { get; set; }

    [StringLength(50)]
    public string? EnglishName { get; set; }

    public bool IsReward { get; set; }

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

    [StringLength(2)]
    public string? TaxCode { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("PaymentType")]
    public virtual ICollection<EmployeeDeductionPayment> EmployeeDeductionPayments { get; set; } = new List<EmployeeDeductionPayment>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<PersonnelLoanPayment> PersonnelLoanPayments { get; set; } = new List<PersonnelLoanPayment>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<PersonnelPayment> PersonnelPayments { get; set; } = new List<PersonnelPayment>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<TaxDisketteWh> TaxDisketteWhs { get; set; } = new List<TaxDisketteWh>();

    [InverseProperty("PaymentType")]
    public virtual ICollection<TaxDisketteWk> TaxDisketteWks { get; set; } = new List<TaxDisketteWk>();
}
