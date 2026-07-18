using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.WorkFlow.Core.Data
{
    [Table("Node_User_Rel", Schema = "wf")]
    public class NodeUserRel : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("WorkFlow")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WorkFlowId { get; set; }
        public virtual WorkFlow? WorkFlow { get; set; }
        [ForeignKey("Employee")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeId { get; set; }
        public virtual AspNetUsers? Employee { get; set; }
        [ForeignKey("Node")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long NodeId { get; set; }
        public virtual Node? Node { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
