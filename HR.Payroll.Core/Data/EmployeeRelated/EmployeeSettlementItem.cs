using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HR.Payroll.Core.Data.EmployeeRelated;

[Table("Employee_Settlement_Item", Schema = "Payroll")]
public class EmployeeSettlementItem : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("EmployeeSettlement")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeSettlementId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual EmployeeSettlement? EmployeeSettlement { get; set; }

    [ForeignKey("SettlementItem")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SettlementItemId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual SettlementItem? SettlementItem { get; set; }

    /// <summary>
    /// بازه زمانی تسویه حساب
    /// </summary>
    [StringLength(6)]
    public string? Duration { get; set; }
    /// <summary>
    /// مبلغ نهایی آیتم (پس از اعمال مبلغ دستی کاربر در صورت ویرایش).
    /// </summary>
    public long Amount { get; set; }
    /// <summary>
    /// مبلغی که سیستم (فرمول/مقدار ثابت/مانده وام یا کسور) محاسبه کرده؛ حتی در صورت تغییر دستی مبلغ نهایی حفظ می‌شود.
    /// </summary>
    public long SystemCalculatedAmount { get; set; }
    [MaxLength(256)]
    public string? Description { get; set; }
}
