using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_CostCenter", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Organisation_CostCenter_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_CostCenter_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PeymanRowId", Name = "IX_Organisation_CostCenter_PeymanRowId")]
public partial class OrganisationCostCenter
{
    [Key]
    public long Id { get; set; }

    public long CostCenterId { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(256)]
    public string? OverrideCostCenterTitle { get; set; }

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

    /// <summary>
    /// در صد حق بالاسری مرکز هزینه
    /// </summary>
    public int CostCenterPercent { get; set; }

    /// <summary>
    /// ردیف پیمان متناظر
    /// </summary>
    public long? PeymanRowId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// کد مالی مربوط به مرکز هزینه
    /// </summary>
    [StringLength(50)]
    public string? CostCenterFinancialCode { get; set; }

    [ForeignKey("CostCenterId")]
    [InverseProperty("OrganisationCostCenterCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationCostCenterOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PeymanRowId")]
    [InverseProperty("OrganisationCostCenters")]
    public virtual OrganisationPeymanRow? PeymanRow { get; set; }
}
