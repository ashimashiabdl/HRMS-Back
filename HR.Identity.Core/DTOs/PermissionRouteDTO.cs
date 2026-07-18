using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs;

public class PermissionRouteDTO : BaseDTO
{
    [MaxLength(1024)]
    public string? Claim { get; set; }
    public string? title { get; set; }

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
