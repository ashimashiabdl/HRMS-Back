using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

[Table("Attendance_Shift", Schema = "Attendance")]
public class Shift : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("کد شیفت")]
    public string Code { get; set; } = string.Empty;

    [StringLength(7)]
    [Comment("رنگ نمایشی (hex)")]
    public string Color { get; set; } = "#2563eb";

    [Comment("فعال")]
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ShiftDetail> Details { get; set; } = new List<ShiftDetail>();
}
