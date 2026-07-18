using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Image_Attachment", Schema = "bas")]
public class ImageAttachment : BaseEntity, HR.SharedKernel.Data.IignoreDateRangeValidation
{
    [Required]
    public byte[] Content { get; set; } = null!;

    [StringLength(256)]
    public string? MimeType { get; set; }

    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Extension { get; set; }

    public long Size { get; set; }
}
