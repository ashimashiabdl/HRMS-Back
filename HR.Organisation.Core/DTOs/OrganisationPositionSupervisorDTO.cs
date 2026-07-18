using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Organisation.Core.DTOs;

public class OrganisationPositionSupervisorDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long EmployeeID { get; set; }

    public long OrganisationPositionId { get; set; }
    public string? OrganisationPosition { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    public bool IsMain { get; set; }
}
