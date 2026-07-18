using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class SkillLevelDTO : BaseDTO
{
    [StringLength(450)]

    public string? Description { get; set; }
}


