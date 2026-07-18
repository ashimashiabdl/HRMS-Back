using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs;

public class UserMenuDTO : BaseDTO
{
    public long UserId { get; set; }
    public string User { get; set; }

    [MaxLength(1024)]
    public string? Claim { get; set; }
}
