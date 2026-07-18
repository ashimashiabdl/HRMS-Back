using System.ComponentModel.DataAnnotations;

namespace HR.Identity.Core.DTOs;

public class MessageAttachmentUploadDTO
{
    [Required]
    public string FileName { get; set; }

    public string? Extension { get; set; }

    public string? MimeType { get; set; }

    [Required]
    public string ContentBase64 { get; set; }
}
