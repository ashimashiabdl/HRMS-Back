using System.ComponentModel.DataAnnotations;
using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class MessageAttachmentDTO : BaseDTO
{
    public long MessageId { get; set; }

    [StringLength(512)]
    [Required(ErrorMessage = "نام فایل الزامی می باشد")]
    public string FileName { get; set; }

    [StringLength(100)]
    public string? Extension { get; set; }

    [StringLength(512)]
    public string? MimeType { get; set; }

    public long Size { get; set; }

    // برای دانلود - محتوای base64
    public string? ContentBase64 { get; set; }

    public Guid? UniqueId { get; set; }
}
