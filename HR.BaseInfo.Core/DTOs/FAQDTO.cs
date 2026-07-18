using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class FAQDTO : BaseDTO
{
    [StringLength(500)]
    [Required(ErrorMessage = "سوال الزامی است")]
    public string Question { get; set; } = string.Empty;

    [StringLength(2000)]
    [Required(ErrorMessage = "پاسخ الزامی است")]
    public string Answer { get; set; } = string.Empty;

    [Required]
    public int Priority { get; set; } = 0; // الویت نمایش (عدد کمتر = الویت بالاتر)

    public bool IsActive { get; set; } = true; // فعال/غیرفعال
}

