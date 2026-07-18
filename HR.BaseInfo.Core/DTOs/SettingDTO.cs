using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs;

public class SettingDTO : BaseDTO
{
    [StringLength(128)]
    public string? Value { get; set; }
}
