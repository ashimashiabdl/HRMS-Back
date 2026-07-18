using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.Data
{
    [Table("Node", Schema = "wf")]
    public class Node : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("WorkFlow")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WorkFlowId { get; set; }
        public virtual WorkFlow? WorkFlow { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
        public int Priority { get; set; }
    }
}
