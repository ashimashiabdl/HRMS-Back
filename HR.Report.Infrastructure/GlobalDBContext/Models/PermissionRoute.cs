using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Permission_Route", Schema = "Identity")]
public partial class PermissionRoute
{
    [Key]
    public long Id { get; set; }

    [StringLength(1024)]
    public string? Claim { get; set; }

    [StringLength(1024)]
    public string? Route { get; set; }

    [StringLength(1024)]
    public string? Icon { get; set; }

    [StringLength(1024)]
    public string? Tooltip { get; set; }

    [StringLength(1024)]
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
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public int Priority { get; set; }

    public bool IsEmployeeSpecific { get; set; }

    [StringLength(1024)]
    public string? ParentMenuKey { get; set; }

    public bool IsSpecial { get; set; }

    [StringLength(1024)]
    public string? PreferColor { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }
}
