using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.Entities
{
    [Table("Job_Abundance_JobScoringFactor_Question", Schema = "Org")]
    public class JobAbundanceJobScoringFactorQuestion : BaseEntity
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
        [StringLength(2048)]
        public string? Question { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
