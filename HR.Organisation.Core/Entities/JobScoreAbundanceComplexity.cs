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
    [Table("Job_Score_Abundance_Complexity", Schema = "Org")]
    public class JobScoreAbundanceComplexity : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [ForeignKey("JobScoringFactor")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? JobScoringFactorId { get; set; }
        public virtual JobScoringFactor? JobScoringFactor { get; set; }

        [ForeignKey("Abundance")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? AbundanceId { get; set; }
        public virtual Abundance? Abundance { get; set; }

        [ForeignKey("Complexity")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? ComplexityId { get; set; }
        public virtual Complexity? Complexity { get; set; }

        public int Score { get; set; }
        public bool Selected { get; set; }
        public bool SelectedFromQuestion { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
