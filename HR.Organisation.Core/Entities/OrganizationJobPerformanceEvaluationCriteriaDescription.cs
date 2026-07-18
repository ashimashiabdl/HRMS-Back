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
    /// جدول شماره 5
    /// </summary>
    [Table("OrganizationJob_Performance_Evaluation_Criteria_Description", Schema = "Org")]
    public class OrganizationJobPerformanceEvaluationCriteriaDescription : BaseEntity
    {
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }

        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CriteriaTypeId { get; set; }
        public virtual BaseTableValue? CriteriaType { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
