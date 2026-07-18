using System.ComponentModel.DataAnnotations;

namespace HR.Identity.Core.DTOs;

public class SendMessageDTO
{
    [Required(ErrorMessage = "گیرنده پیام الزامی می باشد")]
    public long ReceiverId { get; set; }

    [StringLength(500, ErrorMessage = "طول عنوان پیام نمی‌تواند بیشتر از {1} کاراکتر باشد")]
    [Required(ErrorMessage = "عنوان پیام الزامی می باشد")]
    public string Subject { get; set; }

    [StringLength(4000, ErrorMessage = "طول متن پیام نمی‌تواند بیشتر از {1} کاراکتر باشد")]
    public string? Body { get; set; }

    public long? ParentMessageId { get; set; }

    // فهرست فایل‌های پیوست (base64)
    public List<MessageAttachmentUploadDTO>? Attachments { get; set; }
}
