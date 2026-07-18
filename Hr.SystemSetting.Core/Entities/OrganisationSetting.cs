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
    [Table("Organisation_Setting", Schema = "Setting")]
    public class OrganisationSetting : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Setting")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long SettingId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual Setting? Setting { get; set; }
        [ForeignKey("EmployeeType")]
        public long? EmployeeTypeId { get; set; }
        public virtual EmployeeType? EmployeeType { get; set; }
        public bool IsActive { get; set; }
        public int Code { get; set; }
        [StringLength(256)]
        public string? Hourparameters { get; set; }
        [StringLength(128)]
        public string? Value { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
