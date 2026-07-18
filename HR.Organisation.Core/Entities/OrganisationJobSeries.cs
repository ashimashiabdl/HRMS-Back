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
    [Table("Organisation_Job_Series", Schema = "Org")]
    public class OrganisationJobSeries : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("JobSeries")]

        public long? JobSeriesId { get; set; }
        public virtual JobSeries? JobSeries { get; set; }

        [ForeignKey("OrganisationJobCategory")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationJobCategoryId { get; set; }
        public virtual OrganisationJobCategory? OrganisationJobCategory { get; set; }

        [ForeignKey("OrganisationJobGroup")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationJobGroupId { get; set; }
        public virtual OrganisationJobGroup? OrganisationJobGroup { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
