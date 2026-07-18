using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("Message", Schema = "Identity")]
public class Message : BaseEntity, IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long SenderId { get; set; }
    public virtual AspNetUsers? Sender { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long ReceiverId { get; set; }
    public virtual AspNetUsers? Receiver { get; set; }

    [StringLength(500)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Required(ErrorMessage = "عنوان پیام الزامی می باشد")]
    public string Subject { get; set; }


    [StringLength(4000)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Required(AllowEmptyStrings = true, ErrorMessage = "متن پیام الزامی می باشد")]
    public string Body { get; set; }

    public bool IsRead { get; set; } = false; // همیشه false برای پیام جدید

    [Column(TypeName = "datetime")]
    public DateTime? ReadDate { get; set; }
    
    public Message()
    {
        // اطمینان حاصل کنید که IsRead همیشه false است
        IsRead = false;
        ReadDate = null;
    }

    // پیام والد برای نمایش thread (برای پاسخ‌ها)
    public long? ParentMessageId { get; set; }
    public virtual Message? ParentMessage { get; set; }

    // فهرست پاسخ‌ها به این پیام
    public virtual ICollection<Message> Replies { get; set; } = new List<Message>();

    // فهرست فایل‌های پیوست
    public virtual ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();

    // برای نمایش thread کامل - آخرین پیام در thread
    public long? ThreadRootMessageId { get; set; }
    public virtual Message? ThreadRootMessage { get; set; }
}
