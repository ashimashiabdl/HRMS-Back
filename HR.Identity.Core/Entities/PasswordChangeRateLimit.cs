using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

/// <summary>
/// لاگ فراخوانی‌های تغییر رمز عبور برای Rate Limiting
/// جلوگیری از فراخوانی بیش از حد UpdateCurrentUserPassword
/// </summary>
[Table("PasswordChangeRateLimit", Schema = "Identity")]
public class PasswordChangeRateLimit : HR.SharedKernel.Data.BaseEntity, HR.SharedKernel.Data.IignoreDateRangeValidation
{
    /// <summary>
    /// شناسه کاربر که اقدام به تغییر رمز عبور کرده است
    /// </summary>
    [Required]
    [ForeignKey("AspNetUser")]
    public long AspNetUserId { get; set; }
    public virtual AspNetUsers? AspNetUser { get; set; }

    /// <summary>
    /// آدرس IP که از آن درخواست ارسال شده است
    /// </summary>
    [Required]
    [StringLength(45)]
    [Column("RequestIPAddress")]
    public string RequestIPAddress { get; set; } = string.Empty;

    /// <summary>
    /// زمان فراخوانی
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// آیا این فراخوانی موفقیت‌آمیز بوده است یا خیر
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// پیام خطا در صورت ناموفق بودن
    /// </summary>
    [StringLength(500)]
    public string? ErrorMessage { get; set; }
}

