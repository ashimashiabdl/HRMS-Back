using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Attendance.Core.Entities;

[Table("Attendance_Device", Schema = "Attendance")]
public class AttendanceDevice : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual HR.Organisation.Core.Entities.OrganisationChart? OrganisationChart { get; set; }

    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("کد دستگاه")]
    public string? Code { get; set; }

    [StringLength(45)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("آدرس IP دستگاه")]
    public string? DeviceIP { get; set; }

    [Comment("پورت")]
    public int Port { get; set; }

    [StringLength(128)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("شماره سریال")]
    public string? SerialNumber { get; set; }

    [StringLength(128)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("نوع دستگاه")]
    public string? DeviceType { get; set; }

    [Comment("base table value Id : 40300 (برند دستگاه)")]
    public long? BrandId { get; set; }

    [StringLength(128)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [Comment("مدل دستگاه")]
    public string? Model { get; set; }

    [ForeignKey("AttendanceLocation")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long AttendanceLocationId { get; set; }
    public virtual AttendanceLocation? AttendanceLocation { get; set; }

    [StringLength(64)]
    [Comment("منطقه زمانی")]
    public string? TimeZone { get; set; }

    [Comment("فاصله همگام‌سازی (دقیقه)")]
    public int SyncInterval { get; set; }

    [Comment("آخرین تاریخ همگام‌سازی")]
    public DateTime? LastSyncDate { get; set; }

    [Comment("base table value Id : 40299 (وضعیت دستگاه)")]
    public long? StatusId { get; set; }
}
