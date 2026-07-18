using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.WorkFlow.Core.Data
{
    [Table("Node_Role_Rel", Schema = "wf")]
    public class NodeRoleRel : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("WorkFlow")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WorkFlowId { get; set; }
        public virtual WorkFlow? WorkFlow { get; set; }

        [ForeignKey("Role")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long RoleId { get; set; }
        public virtual AspNetRoles? Role { get; set; }

        [ForeignKey("Node")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long NodeId { get; set; }
        public virtual Node? Node { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
