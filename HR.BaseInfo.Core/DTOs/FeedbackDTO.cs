using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class FeedbackDTO : BaseDTO
{
    [StringLength(2000)]
    [Required(ErrorMessage = "متن انتقاد یا پیشنهاد الزامی است")]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FeedbackType { get; set; } // "انتقاد" یا "پیشنهاد"

    public long? SubmittedByUserId { get; set; }
    
    public string? SubmittedByUserName { get; set; } // برای نمایش در فهرست

    [StringLength(50)]
    public string? Status { get; set; } // "جدید", "در حال بررسی", "بررسی شده"

    [StringLength(2000)]
    public string? Response { get; set; } // پاسخ مدیر

    public DateTime? ResponseDate { get; set; }

    public long? RespondedByUserId { get; set; }
    
    public string? RespondedByUserName { get; set; } // برای نمایش در فهرست
}

