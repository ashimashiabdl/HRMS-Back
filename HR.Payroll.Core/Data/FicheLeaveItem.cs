using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Fiche_Leave_Item", Schema = "Payroll")]

public class FicheLeaveItem : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("Fiche")]
    public long FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }

    [ForeignKey("LeaveType")]
    public long LeaveTypeId { get; set; }
    public virtual LeaveType? LeaveType { get; set; }

    [ForeignKey("PersonnelLeave")]
    public long PersonnelLeaveId { get; set; }
    public virtual PersonnelLeave? PersonnelLeave { get; set; }

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
    [NotMapped]
    public TimeSpan LeaveBalance
    {
        get => TimeSpan.FromTicks(LeaveBalanceTicks);
        set => LeaveBalanceTicks = value.Ticks;
    }

    /// <summary>
    /// میزان مرخصی استفاده شده در این دوره
    /// </summary>
    [NotMapped]
    public TimeSpan LeaveAmount
    {
        get => TimeSpan.FromTicks(LeaveAmountTicks);
        set => LeaveAmountTicks = value.Ticks;
    }
}
