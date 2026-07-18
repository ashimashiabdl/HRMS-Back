using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class ImageAttachmentDTO : BaseDTO
{
    [StringLength(256)]
    public string? MimeType { get; set; }

    [StringLength(32)]
    public string? Extension { get; set; }

    public long Size { get; set; }

    /// <summary>
    /// پر شده فقط هنگام دریافت یک رکورد (برای پیش‌نمایش)
    /// </summary>
    public byte[]? Content { get; set; }
}
