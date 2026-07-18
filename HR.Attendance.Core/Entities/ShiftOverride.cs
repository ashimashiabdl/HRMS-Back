using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

/// <summary>
/// بازتعریف موقت رفتار یک شیفت در بازه تاریخی مشخص.
/// </summary>
[Table("Attendance_Shift_Override", Schema = "Attendance")]
public class ShiftOverride : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("Shift")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long ShiftId { get; set; }
    public virtual Shift? Shift { get; set; }

    [StringLength(512)]
    [Comment("توضیحات")]
    public string? Description { get; set; }

    public virtual ICollection<ShiftOverrideDetail> Details { get; set; } = new List<ShiftOverrideDetail>();
}
