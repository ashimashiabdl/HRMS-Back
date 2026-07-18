using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Tax_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Tax_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Tax_WageItemId")]
public partial class Tax
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public double? CoefficientTax { get; set; }

    public bool? IsAdjustment { get; set; }

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

    public long WageItemId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("Taxes")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Taxes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("Tax")]
    public virtual ICollection<TaxCoefficientItem> TaxCoefficientItems { get; set; } = new List<TaxCoefficientItem>();

    [InverseProperty("Tax")]
    public virtual ICollection<TaxTable> TaxTables { get; set; } = new List<TaxTable>();

    [ForeignKey("WageItemId")]
    [InverseProperty("Taxes")]
    public virtual WageItem WageItem { get; set; } = null!;
}
