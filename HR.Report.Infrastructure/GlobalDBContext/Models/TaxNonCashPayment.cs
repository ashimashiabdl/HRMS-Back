using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_NonCash_Payment", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Tax_NonCash_Payment_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("ItemTypeId", Name = "IX_Tax_NonCash_Payment_ItemTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Tax_NonCash_Payment_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Tax_NonCash_Payment_PaymentPeriodId")]
public partial class TaxNonCashPayment
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public double Value { get; set; }

    public long ItemTypeId { get; set; }

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

    public bool Continuous { get; set; }

    public long PaymentPeriodId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("TaxNonCashPayments")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TaxNonCashPayments")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("TaxNonCashPayments")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;
}
