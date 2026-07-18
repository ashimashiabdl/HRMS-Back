using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Employee.Core.DTOs;

public class ExperienceDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChartTitle { get; set; }
    public long EmployeeId { get; set; }
    public string? EmployeeTitle { get; set; }
    public long? HistoryTypeId { get; set; }
    public string? HistoryTypeTitle { get; set; }
    // „œ 
    [StringLength(6)]
    public string? Duration { get; set; }

    public bool IsInternal { get; set; }
    public bool IsAcceptable { get; set; }
    public int? AcceptablePercent { get; set; }
    public string? CompanyTitle { get; set; }
}


