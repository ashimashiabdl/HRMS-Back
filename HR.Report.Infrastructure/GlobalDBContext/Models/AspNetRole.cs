using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("AspNetRoles", Schema = "Identity")]
public partial class AspNetRole
{
    [Key]
    public long Id { get; set; }

    [StringLength(256)]
    public string PersianName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<NodeRoleRel> NodeRoleRels { get; set; } = new List<NodeRoleRel>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleReportableEntity> RoleReportableEntities { get; set; } = new List<RoleReportableEntity>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleReport> RoleReports { get; set; } = new List<RoleReport>();
}
