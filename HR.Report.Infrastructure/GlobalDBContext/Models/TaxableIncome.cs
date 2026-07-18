using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Taxable_Income", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Taxable_Income_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Taxable_Income_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Taxable_Income_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Taxable_Income_WageItemId")]
public partial class TaxableIncome
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long WageItemId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long Amount { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

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

    [ForeignKey("EmployeeId")]
    [InverseProperty("TaxableIncomes")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TaxableIncomes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("TaxableIncomes")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("TaxableIncomes")]
    public virtual WageItem WageItem { get; set; } = null!;
}
