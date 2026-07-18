using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class AttendanceDeviceDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }

    [StringLength(32)]
    public string? Code { get; set; }

    [StringLength(45)]
    public string? DeviceIP { get; set; }

    public int Port { get; set; }

    [StringLength(128)]
    public string? SerialNumber { get; set; }

    [StringLength(128)]
    public string? DeviceType { get; set; }

    public long? BrandId { get; set; }
    public string? Brand { get; set; }

    [StringLength(128)]
    public string? Model { get; set; }

    public long AttendanceLocationId { get; set; }
    public string? AttendanceLocation { get; set; }

    [StringLength(64)]
    public string? TimeZone { get; set; }

    public int SyncInterval { get; set; }

    public DateTime? LastSyncDate { get; set; }

    public long? StatusId { get; set; }
    public string? Status { get; set; }
}
