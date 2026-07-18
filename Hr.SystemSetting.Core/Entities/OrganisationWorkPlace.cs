using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_WorkPlace", Schema = "Setting")]
    public class OrganisationWorkPlace : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }   
        
        [ForeignKey("WorkPlace")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WorkPlaceId { get; set; }
        public virtual OrganisationChart? WorkPlace { get; set; } 
        
        
        [ForeignKey("BaseWorkPlace")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long BaseWorkPlaceId { get; set; }
        public virtual BaseWorkPlace? BaseWorkPlace { get; set; }

        [StringLength(255)]
        public string? OrgChartWorkPlaceName { get; set; }
        [StringLength(255)]
        public string? OrgChartWorkPlaceCode { get; set; }
    }
}
