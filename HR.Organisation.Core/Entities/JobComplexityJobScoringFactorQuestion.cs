using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    [Table("Job_Complexity_JobScoringFactor_Question", Schema = "Org")]
    public class JobComplexityJobScoringFactorQuestion : SharedKernel.Data.BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }
        [ForeignKey("JobScoringFactor")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? JobScoringFactorId { get; set; }
        public virtual JobScoringFactor? JobScoringFactor { get; set; }
        [ForeignKey("Complexity")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? ComplexityId { get; set; }
        public virtual Complexity? Complexity { get; set; }
        [StringLength(2048)]
        public string? Question { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
