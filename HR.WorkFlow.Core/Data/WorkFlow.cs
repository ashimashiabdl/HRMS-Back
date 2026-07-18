using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.WorkFlow.Core.Data;

[Table("WorkFlow", Schema = "wf")]
public class WorkFlow : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("WorkFlowType")]
    public long? WorkFlowTypeId { get; set; }
    public virtual WorkFlowType? WorkFlowType { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    [StringLength(512)]
    public string? Description { get; set; }
    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
}
