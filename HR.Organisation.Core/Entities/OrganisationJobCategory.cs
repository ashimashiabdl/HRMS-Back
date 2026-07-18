using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    [Table("Organisation_Job_Category", Schema = "Org")]
    public class OrganisationJobCategory : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("JobCategory")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? JobCategoryId { get; set; }
        public virtual JobCategory? JobCategory { get; set; }
        [StringLength(10)]
        public string? Code { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
