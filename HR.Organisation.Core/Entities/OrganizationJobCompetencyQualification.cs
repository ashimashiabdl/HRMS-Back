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
    /// جدول شماره 16
    /// </summary>
    [Table("OrganizationJob_Competency_Qualification", Schema = "Org")]
    public class OrganizationJobCompetencyQualification : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CompetencyTypeId { get; set; }
        public virtual BaseTableValue? CompetencyType { get; set; }


        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? CompetencyLevelId { get; set; }
        public virtual BaseTableValue? CompetencyLevel { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
