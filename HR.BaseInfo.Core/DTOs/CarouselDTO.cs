using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class CarouselDTO : BaseDTO
{
    [StringLength(500)]
    public string? Subtitle { get; set; }

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
