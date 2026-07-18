using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Attendance.Core.DTOs;

public class AttendanceLocationDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    [StringLength(32)]
    public string? Code { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Radius { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public long? RelatedOrganisationChartId { get; set; }
    public string? RelatedOrganisationChart { get; set; }
}
