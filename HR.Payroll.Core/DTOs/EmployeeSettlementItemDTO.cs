using System.ComponentModel.DataAnnotations;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs;

public class EmployeeSettlementItemDTO : BaseDTO
{
    public long SettlementItemId { get; set; }
    public string? SettlementItemTitle { get; set; }
    /// <summary>
    /// مبلغ دستی/نهایی؛ در صورت خالی بودن از محاسبات فرمولی/سیستمی استفاده می‌شود.
    /// </summary>
    public long? Amount { get; set; }
    /// <summary>
    /// مبلغ محاسبه‌شده توسط سیستم؛ برای حسابرسی و مقایسه با مبلغ نهایی کاربر ذخیره می‌شود.
    /// </summary>
    public long? SystemCalculatedAmount { get; set; }
    public string? Duration { get; set; }
    [MaxLength(256)]
    public string? Description { get; set; }
}
