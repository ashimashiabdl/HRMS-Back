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
    [Table("Organisation_Project", Schema = "Org")]
    public class OrganisationProject : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Project")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? RefId { get; set; }
        public int? RefType { get; set; }
        [StringLength(512)]
        public string? ProjectDescription { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
