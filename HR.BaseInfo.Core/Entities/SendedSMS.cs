using HR.SharedKernel.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// لاگ پیامک های ارسال شده
/// </summary>
[DisplayName("پیامک های ارسال شده")]
[Table("SendedSMS", Schema = "Log")]
public class SendedSMS : SharedKernel.Data.BaseEntity , IignoreDateRangeValidation
{
    /// <summary>
    /// شناسه کاربر در صورتی که موجود باشد
    /// </summary>
    [DisplayName("شناسه کاربر در صورتی که موجود باشد")]
    public long? UserId { get; set; }
    //public virtual User? User { get; set; }
    /// <summary>
    /// شماره موبایل گیرنده پیامک
    /// </summary>
    [StringLength(11)]
    [DisplayName("شماره موبایل گیرنده پیامک")]
    public string? MobileNumber { get; set; }
    /// <summary>
    /// متن پیامک
    /// </summary>
    [StringLength(512)]
    [DisplayName("متن پیامک")]
    public string? SMSBody { get; set; }
    /// <summary>
    /// کد پاسخ فراهم کننده سرویس پیامک
    /// </summary>
    //[StringLength(2048)] به خاطر لاگ استک و خطا ها مکس گرفته شد
    [DisplayName("کد پاسخ فراهم کننده سرویس پیامک اول")]
    public string? Provider1Response { get; set; }
    [DisplayName("کد پاسخ فراهم کننده سرویس پیامک دوم")]
    public string? Provider2Response { get; set; }
    /// <summary>
    /// نام فراهم کننده سرویس پیامک
    /// </summary>
    [StringLength(512)]
    public string Provider1Name { get; set; }
    [StringLength(512)]
    public string Provider2Name { get; set; }
    public bool IsSuccess { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime Provider1SendDateTime { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime Provider1ResponseDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Provider2SendDateTime { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime Provider2ResponseDateTime { get; set; }
    [StringLength(32)]
    public string? ValidationCode { get; set; }
}
