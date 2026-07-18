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
    [Table("OrganizationJob_Required_Softwares_Qualification", Schema = "Org")]
    public class OrganizationJobRequiredSoftwaresQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SoftwareId { get; set; }
        public virtual BaseTableValue? Software { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SoftwareTypeId { get; set; }
        public virtual BaseTableValue? SoftwareType { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long MasteryLevelTypeId { get; set; }
        public virtual BaseTableValue? MasteryLevelType { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
