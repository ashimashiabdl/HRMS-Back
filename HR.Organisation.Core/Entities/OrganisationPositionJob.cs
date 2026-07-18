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
    [Table("Organisation_Position_Job", Schema = "Org")]
    public class OrganisationPositionJob : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("OrganizationJob")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganizationJobId { get; set; }
        public virtual OrganizationJob? OrganizationJob { get; set; }  
        
        [ForeignKey("OrganisationPosition")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationPositionId { get; set; }
        public virtual OrganisationPosition? OrganisationPosition { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}