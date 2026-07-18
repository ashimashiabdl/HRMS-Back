using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_CostCenter", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_User_CostCenter_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_User_CostCenter_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_CostCenter_UserId")]
public partial class UserCostCenter
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long CostCenterId { get; set; }

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

    [ForeignKey("CostCenterId")]
    [InverseProperty("UserCostCenterCostCenters")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("UserCostCenterOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserCostCenters")]
    public virtual AspNetUser User { get; set; } = null!;
}
