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
    [Table("Related_OrganizationJob_Description", Schema = "Org")]
    public class RelatedOrganizationJobDescription : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [ForeignKey("OrganizationRelatedJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationRelatedJobId { get; set; }
        public virtual OrganizationJob? OrganizationRelatedJob { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
