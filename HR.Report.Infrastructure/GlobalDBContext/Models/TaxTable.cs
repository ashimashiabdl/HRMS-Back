using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_Table", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Tax_Table_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("TaxId", Name = "IX_Tax_Table_TaxId")]
public partial class TaxTable
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

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

    public long FromValue { get; set; }

    public int TaxPercent { get; set; }

    public long ToValue { get; set; }

    public bool BasedOnFunctionality { get; set; }

    public long TaxId { get; set; }

    public int RelevantValue { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TaxTables")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("TaxId")]
    [InverseProperty("TaxTables")]
    public virtual Tax Tax { get; set; } = null!;
}
