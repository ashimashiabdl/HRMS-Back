using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class ShiftDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    [StringLength(32)]
    public string Code { get; set; } = string.Empty;

    [StringLength(7)]
    public string Color { get; set; } = "#2563eb";

    public bool IsActive { get; set; } = true;

    public List<ShiftDetailDTO>? Details { get; set; }

    // Summary fields for list display (from Saturday detail or first available)
    public bool IsFlexible { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int RequiredWorkSeconds { get; set; }
    public bool NightShift { get; set; }
    public bool CrossDay { get; set; }
}
