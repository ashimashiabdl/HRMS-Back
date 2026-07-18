using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class AttendanceLogDTO : BaseDTO
{
    public long EmployeeId { get; set; }
    public string? Employee { get; set; }

    public long AttendanceDeviceId { get; set; }
    public string? AttendanceDevice { get; set; }

    [StringLength(128)]
    public string? DeviceUserId { get; set; }

    public DateTime LogDateTime { get; set; }

    [StringLength(32)]
    public string? Direction { get; set; }

    [StringLength(64)]
    public string? VerifyMode { get; set; }

    [StringLength(64)]
    public string? WorkCode { get; set; }

    public decimal? Temperature { get; set; }

    public bool? Mask { get; set; }

    public string? RawData { get; set; }

    public DateTime? ReceiveDate { get; set; }

    [StringLength(64)]
    public string? Status { get; set; }
}
