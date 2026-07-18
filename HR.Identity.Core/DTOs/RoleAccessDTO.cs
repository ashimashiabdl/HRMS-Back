using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs;

public class RoleAccessDTO 
{
    public long RoleId { get; set; }
    public string? Role { get; set; }
    public string claimType { get; set; }
    public bool claimValue { get; set; }

}
