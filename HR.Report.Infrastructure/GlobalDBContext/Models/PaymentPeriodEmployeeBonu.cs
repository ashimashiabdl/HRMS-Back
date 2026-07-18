using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("PaymentPeriod_Employee_Bonus", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CoefficientId", Name = "IX_PaymentPeriod_Employee_Bonus_CoefficientId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_PaymentPeriod_Employee_Bonus_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_PaymentPeriod_Employee_Bonus_PaymentPeriodId")]
public partial class PaymentPeriodEmployeeBonu
{
    [Key]
    public long Id { get; set; }

    public long PaymentPeriodId { get; set; }

    public long EmployeeId { get; set; }

    public long CoefficientId { get; set; }

    public double Value { get; set; }

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

    [ForeignKey("CoefficientId")]
    [InverseProperty("PaymentPeriodEmployeeBonus")]
    public virtual Coefficient Coefficient { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("PaymentPeriodEmployeeBonus")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("PaymentPeriodEmployeeBonus")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;
}
