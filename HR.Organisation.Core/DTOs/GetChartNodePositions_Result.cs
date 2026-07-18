namespace HR.Organisation.Core.DTOs;

public class GetChartNodePositions_Result
{
    public long Id { get; set; }
    public long PositionId { get; set; }
    public string? PositionName { get; set; }
    public bool? IsStarable { get; set; }
    public string? PositionCode { get; set; }
    public long OrganisationChartId { get; set; }
    public long? Organisation_ParentNode_Id { get; set; }
    public long Organisation_Related_Node { get; set; }
    public long OrganisationPositionId { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsDedicated { get; set; }
    public bool? IsState { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsFreez { get; set; }
    public long InsurancePositionId { get; set; }
    public string? InsPositionCode { get; set; }
    public string? InsPositionName { get; set; }
    public bool? IsManager { get; set; }
    public bool IsDeleted { get; set; }
}
