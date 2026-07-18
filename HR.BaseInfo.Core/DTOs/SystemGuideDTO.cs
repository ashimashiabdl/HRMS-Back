using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class SystemGuideDTO : BaseDTO
{


    [Required(ErrorMessage = "محتوا الزامی است")]
    public string Body { get; set; } = string.Empty;
}
