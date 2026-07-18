using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs;

public class FicheLeaveItemDTO
{
    public long FicheId { get; set; }
    public long LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }

    public long PersonnelLeaveId { get; set; }
    public string? PersonnelLeave { get; set; }

    /// <summary>
    /// مانده مرخصی به صورت Ticks
    /// </summary>
    [Column(TypeName = "bigint")]
    public long LeaveBalanceTicks { get; set; }

    /// <summary>
    /// میزان مرخصی استفاده شده در این دوره به صورت Ticks
    /// </summary>
    [Column(TypeName = "bigint")]
    public long LeaveAmountTicks { get; set; }

    /// <summary>
    /// مانده مرخصی
    /// </summary>
    [JsonIgnore]
    public TimeSpan LeaveBalance
    {
        get => TimeSpan.FromTicks(LeaveBalanceTicks);
        set => LeaveBalanceTicks = value.Ticks;
    }

    /// <summary>
    /// میزان مرخصی استفاده شده در این دوره
    /// </summary>
    [JsonIgnore]
    public TimeSpan LeaveAmount
    {
        get => TimeSpan.FromTicks(LeaveAmountTicks);
        set => LeaveAmountTicks = value.Ticks;
    }
}
