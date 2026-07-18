using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("CostCenter_FicheItem", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_CostCenter_FicheItem_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_CostCenter_FicheItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_CostCenter_FicheItem_WageItemId")]
public partial class CostCenterFicheItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long CostCenterId { get; set; }

    public long WageItemId { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

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

    public bool IsFixed { get; set; }

    public bool OnceInFiche { get; set; }

    public int? PriorityNo { get; set; }

    public long? Amount { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("CostCenterId")]
    [InverseProperty("CostCenterFicheItemCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("CostCenterFicheItemOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("CostCenterFicheItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
