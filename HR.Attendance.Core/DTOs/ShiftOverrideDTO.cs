using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class ShiftOverrideDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long ShiftId { get; set; }
    public string? Shift { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public List<ShiftOverrideDetailDTO>? Details { get; set; }

    // Summary fields for list display
    public bool IsFlexible { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int RequiredWorkSeconds { get; set; }
    public bool NightShift { get; set; }
    public bool CrossDay { get; set; }
}
