using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("Permission_Route", Schema = "Identity")]
public class PermissionRoute : BaseEntity
{
    [MaxLength(1024)]
    public string? Claim { get; set; }

    [MaxLength(1024)]
    public string? Route { get; set; }

    [MaxLength(1024)]
    public string? Icon { get; set; }
    [MaxLength(1024)]
    public string? Tooltip { get; set; }

    [MaxLength(1024)]
    public string? Description { get; set; }
    public int Priority { get; set; }
    
    public bool IsEmployeeSpecific { get; set; }

    [MaxLength(1024)]
    public string? ParentMenuKey { get; set; }

    public bool IsSpecial { get; set; }

    [MaxLength(1024)]
    public string? PreferColor { get; set; }

}
