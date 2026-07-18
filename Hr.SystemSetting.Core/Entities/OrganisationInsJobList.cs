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
    [Table("Organisation_InsJobList", Schema = "Setting")]
    public class OrganisationInsJobList : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [StringLength(50)]
        public string Code { get; set; } = null!;
        [StringLength(50)]
        public string? Name { get; set; }   
        [StringLength(256)]
        public string? Description { get; set; }
        public bool? Active { get; set; }
    }
}
