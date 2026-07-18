using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    /// <summary>
    /// ÃœÊ· ‘„«—Â ‘‘ ò «»
    /// </summary>
    [Table("OrganizationJob_Risk_And_Fault_Description", Schema = "Org")]
    public class OrganizationJobRiskAndFaultDescription : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }
        public bool Has { get; set; }public virtual BaseTableValue? RiskOrFaultType { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
