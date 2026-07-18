using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Deduction_Payment", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeDeductionId", Name = "IX_Employee_Deduction_Payment_EmployeeDeductionId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Employee_Deduction_Payment_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentTypeId", Name = "IX_Employee_Deduction_Payment_PaymentTypeId")]
public partial class EmployeeDeductionPayment
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public long EmployeeDeductionId { get; set; }

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

    [ForeignKey("EmployeeDeductionId")]
    [InverseProperty("EmployeeDeductionPayments")]
    public virtual EmployeeDeduction EmployeeDeduction { get; set; } = null!;

    [ForeignKey("FicheId")]
    [InverseProperty("EmployeeDeductionPayments")]
    public virtual Fiche Fiche { get; set; } = null!;

    [ForeignKey("PaymentTypeId")]
    [InverseProperty("EmployeeDeductionPayments")]
    public virtual PaymentType PaymentType { get; set; } = null!;
}
