using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("MessageAttachment", Schema = "Identity")]
public class MessageAttachment : BaseEntity, IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public long MessageId { get; set; }
    public virtual Message? Message { get; set; }

    [StringLength(512)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Required(ErrorMessage = "نام فایل الزامی می باشد")]
    public string FileName { get; set; }


    [StringLength(100)]
    public string? Extension { get; set; }

    [StringLength(512)]
    public string? MimeType { get; set; }

    public long Size { get; set; }

    // محتوای فایل به صورت byte array ذخیره می‌شود
    [Required]
    public byte[] Content { get; set; } = null!;

    public Guid? UniqueId { get; set; }
}
