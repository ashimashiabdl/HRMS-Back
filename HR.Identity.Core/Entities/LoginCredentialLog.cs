using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

/// <summary>
/// لاگ ترکیب نام کاربری و رمز عبور رمزنگاری شده برای تشخیص استفاده مکرر از همان اعتبارنامه
/// این جدول برای شناسایی حمله‌های credential reuse استفاده می‌شود
/// </summary>
[Table("Login_Credential_Log", Schema = "Identity")]
public class LoginCredentialLog : HR.SharedKernel.Data.BaseEntity, HR.SharedKernel.Data.IignoreDateRangeValidation
{
    /// <summary>
    /// نام کاربری رمزنگاری شده (به همان شکلی که از کلاینت دریافت شده است)
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string EncryptedUsername { get; set; }

    /// <summary>
    /// رمز عبور رمزنگاری شده (به همان شکلی که از کلاینت دریافت شده است)
    /// همیشه لاگ می‌شود (چه موفق چه ناموفق) - به صورت encrypted (نه بعد از decrypt)
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string EncryptedPassword { get; set; }

    /// <summary>
    /// User Agent برای ردیابی اضافی
    /// </summary>
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// شناسه کاربر در صورت موفقیت آمیز بودن احراز هویت (null اگر احراز هویت ناموفق بود)
    /// </summary>
    [ForeignKey("AspNetUser")]
    public long? AspNetUserId { get; set; }
    public virtual AspNetUsers? AspNetUser { get; set; }

    /// <summary>
    /// آیا احراز هویت موفقیت‌آمیز بود یا خیر
    /// </summary>
    public bool IsSuccess { get; set; }
}
