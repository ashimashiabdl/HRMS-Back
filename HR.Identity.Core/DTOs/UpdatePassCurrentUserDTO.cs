using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs;

public class UpdatePassCurrentUserDTO
{
    [Required]
    public string? oldpass { get; set; }
    [Required]
    public string? newpass { get; set; }
    [Required]
    public string? newpassconfirm { get; set; }
}
