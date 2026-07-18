using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

[Table("Attendance_Location", Schema = "Attendance")]
public class AttendanceLocation : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("کد محل حضور")]
    public string? Code { get; set; }

    [Column(TypeName = "decimal(10,7)")]
    [Comment("عرض جغرافیایی")]
    public decimal Latitude { get; set; }

    [Column(TypeName = "decimal(10,7)")]
    [Comment("طول جغرافیایی")]
    public decimal Longitude { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Comment("شعاع مجاز (متر)")]
    public decimal Radius { get; set; }

    [StringLength(500)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("آدرس")]
    public string? Address { get; set; }

    [ForeignKey("RelatedOrganisationChart")]
    [Comment("واحد سازمانی مرتبط")]
    public long? RelatedOrganisationChartId { get; set; }
    public virtual OrganisationChart? RelatedOrganisationChart { get; set; }
}
