using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Carousel", Schema = "bas")]
public class Carousel : BaseEntity, IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(500)]
    public string? Subtitle { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(256)]
    [Required(ErrorMessage = "متن جایگزین الزامی است")]
    public string Alt { get; set; } = string.Empty;

    [StringLength(32)]
    [Required(ErrorMessage = "رنگ اصلی الزامی است")]
    public string Accent { get; set; } = "#2563eb";

    [StringLength(32)]
    [Required(ErrorMessage = "رنگ شروع گرادیان الزامی است")]
    public string GradientStart { get; set; } = "#0c44c9";

    public int Priority { get; set; }

    public bool IsActive { get; set; } = true;
}
