using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_OrganizationUnit", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_User_OrganizationUnit_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationUnitId", Name = "IX_User_OrganizationUnit_OrganizationUnitId")]
[Microsoft.EntityFrameworkCore.Index("UserId", Name = "IX_User_OrganizationUnit_UserId")]
public partial class UserOrganizationUnit
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long OrganizationUnitId { get; set; }

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
    [InverseProperty("UserOrganizationUnitOrganisationCharts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganizationUnitId")]
    [InverseProperty("UserOrganizationUnitOrganizationUnits")]
    public virtual OrganisationChart OrganizationUnit { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserOrganizationUnits")]
    public virtual AspNetUser User { get; set; } = null!;
}
