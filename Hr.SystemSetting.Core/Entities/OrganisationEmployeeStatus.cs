using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_EmployeeStatus", Schema = "Setting")]
    public class OrganisationEmployeeStatus : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("EmployeeStatus")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeStatusId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeStatus? EmployeeStatus { get; set; }
        [ForeignKey("EmployeeStatusGroup")]
        public long EmployeeStatusGroupId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeStatusGroup? EmployeeStatusGroup { get; set; }
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        public bool? IsEmployed { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
