using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Organisation_EmployeeType_Leave", Schema = "Payroll")]

public class OrganisationEmployeeTypeLeave : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("LeaveType")]
    public long LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }

    [ForeignKey("EmployeeType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long EmployeeTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public virtual EmployeeType? EmployeeType { get; set; }

    /// <summary>
    /// آیا مرخصی با حقوق است؟
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// سهمیه سالانه پیش‌فرض (در صورت عدم تعریف سیاست خاص)
    /// </summary>
    public decimal? DefaultAnnualQuota { get; set; }

    /// <summary>
    /// سهمیه سالانه مرخصی (بر حسب روز)
    /// </summary>
    public decimal? AnnualQuotaDays { get; set; }

    /// <summary>
    /// حداکثر روزهای قابل انتقال به سال بعد
    /// </summary>
    public decimal? CarryForwardLimit { get; set; }

    /// <summary>
    /// آیا مرخصی قابل تبدیل به وجه نقد است؟
    /// </summary>
    public bool Encashable { get; set; } = false;

    /// <summary>
    /// آیا این سیاست فعال است؟
    /// </summary>
    public bool IsActive { get; set; } = true;
    /// <summary>
    /// روزانه است یا ساعت دقیقه ؟
    /// </summary>
    public bool IsDailyOrHourMinute { get; set; } = false;

}
