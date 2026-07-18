using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Peyman_Row", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Peyman_Row_OrganisationChartId")]
public partial class OrganisationPeymanRow
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    /// <summary>
    /// کد ردیف پیمان
    /// </summary>
    [StringLength(32)]
    public string? Code { get; set; }

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

    [InverseProperty("PeymanRow")]
    public virtual ICollection<Fiche> Fiches { get; set; } = new List<Fiche>();

    [InverseProperty("PeymanRow")]
    public virtual ICollection<InsuranceDiskette> InsuranceDiskettes { get; set; } = new List<InsuranceDiskette>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationPeymanRows")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("PeymanRow")]
    public virtual ICollection<OrganisationCostCenter> OrganisationCostCenters { get; set; } = new List<OrganisationCostCenter>();
}
