using System.ComponentModel.DataAnnotations;
using HR.SharedKernel.Data;

namespace HR.Identity.Core.DTOs;

public class MessageDTO : BaseDTO
{
    public long SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderFullName { get; set; }

    public long ReceiverId { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverFullName { get; set; }

    // نام فرستنده آخرین پیام در thread
    public string? LatestSenderFullName { get; set; }

    [StringLength(500)]
    [Required(ErrorMessage = "عنوان پیام الزامی می باشد")]
    public string Subject { get; set; }

    [StringLength(4000)]
    [Required(ErrorMessage = "متن پیام الزامی می باشد")]
    public string Body { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    public long? ParentMessageId { get; set; }

    public long? ThreadRootMessageId { get; set; }

    public List<MessageAttachmentDTO> Attachments { get; set; } = new List<MessageAttachmentDTO>();

    // برای نمایش thread - فهرست پاسخ‌ها
    public List<MessageDTO> Replies { get; set; } = new List<MessageDTO>();

    // برای inbox - تعداد پیام‌های خوانده نشده در thread
    public int UnreadCount { get; set; }

    // آیا این پیام اصلی thread است؟
    public bool IsThreadRoot { get; set; }
}
