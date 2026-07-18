using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Feedback", Schema = "bas")]
public class Feedback : BaseEntity
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(2000)]
    [Required(ErrorMessage = "متن انتقاد یا پیشنهاد الزامی است")]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FeedbackType { get; set; } // "انتقاد" یا "پیشنهاد"

    [ForeignKey("SubmittedByUser")]
    public long? SubmittedByUserId { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } // "جدید", "در حال بررسی", "بررسی شده"

    [StringLength(2000)]
    public string? Response { get; set; } // پاسخ مدیر

    [Column(TypeName = "datetime")]
    public DateTime? ResponseDate { get; set; }

    [ForeignKey("RespondedByUser")]
    public long? RespondedByUserId { get; set; } // کاربری که پاسخ داده است
}

