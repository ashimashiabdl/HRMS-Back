using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("FAQ", Schema = "bas")]
public class FAQ : BaseEntity , IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(500)]
    [Required(ErrorMessage = "سوال الزامی است")]
    public string Question { get; set; } = string.Empty;

    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(2000)]
    [Required(ErrorMessage = "پاسخ الزامی است")]
    public string Answer { get; set; } = string.Empty;

    [Required]
    public int Priority { get; set; } = 0; // الویت نمایش (عدد کمتر = الویت بالاتر)

    public bool IsActive { get; set; } = true; // فعال/غیرفعال
}

