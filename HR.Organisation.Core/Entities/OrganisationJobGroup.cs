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
    [Table("Organisation_Job_Group", Schema = "Org")]
    public class OrganisationJobGroup : SharedKernel.Data.BaseEntity , IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }public virtual BaseTableValue? State { get; set; }

        [ForeignKey("JobGroup")]
        public long? JobGroupId { get; set; }
        public virtual JobGroup? JobGroup { get; set; } 
        
        [ForeignKey("OrganisationJobCategory")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationJobCategoryId { get; set; }
        public virtual OrganisationJobCategory? OrganisationJobCategory { get; set; }

        [StringLength(50)]
        public string? Code { get; set; } 
        [StringLength(500)]
        public string? Description { get; set; }


        public int Order { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
