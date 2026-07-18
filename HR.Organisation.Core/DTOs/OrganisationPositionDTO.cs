using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Organisation.Core.DTOs;

public class OrganisationPositionDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }

    public long PositionId { get; set; }
    public string? Position { get; set; }

    public long PositionTypeId { get; set; }
    public string? PositionType { get; set; }

    public long InsurancePositionId { get; set; }
    public string? InsurancePosition { get; set; }

    public long RankId { get; set; }
    public string? Rank { get; set; }

    public long PositionManagementLevelId { get; set; }
    public string? PositionManagementLevel { get; set; }

    public long PositionStateId { get; set; }
    public string? PositionState { get; set; }

    [StringLength(25)]
    public string? PositionCode { get; set; }

    public long RelatedNodeId { get; set; }
    public string? RelatedNode { get; set; }

    public DateTime? LockStartDate { get; set; }
    public DateTime? LockEndDate { get; set; }

    public bool? IsApproved { get; set; }
    public bool? IsDedicated { get; set; }
    public bool? IsState { get; set; }
    public bool? IsStarable { get; set; }
    public bool? IsFreez { get; set; }
    public bool? IsManager { get; set; }
    public bool? IsSubstitute { get; set; }
    public bool? IsExpert { get; set; }

    public int? Capacity { get; set; }

    public int? MaxTenureYears { get; set; }

    [StringLength(256)]
    public string? Other1 { get; set; }

    [StringLength(256)]
    public string? Other2 { get; set; }

    [StringLength(128)]
    public string? UniqueIdentifier { get; set; }

    [StringLength(128)]
    public string? ShortName { get; set; }
}
