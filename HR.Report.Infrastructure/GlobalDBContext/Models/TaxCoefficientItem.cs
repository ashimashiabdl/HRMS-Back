using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_Coefficient_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Tax_Coefficient_Item_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("TaxId", Name = "IX_Tax_Coefficient_Item_TaxId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Tax_Coefficient_Item_WageItemId")]
public partial class TaxCoefficientItem
{
    [Key]
    public long Id { get; set; }

    public long WageItemId { get; set; }

    public double? CoefficientTax { get; set; }

    public long TaxId { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TaxCoefficientItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("TaxId")]
    [InverseProperty("TaxCoefficientItems")]
    public virtual Tax Tax { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("TaxCoefficientItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
