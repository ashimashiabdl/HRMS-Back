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
    [Table("Organisation_EmployeeType_MRT", Schema = "Setting")]
    public class OrganisationEmployeeTypeMRT : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }
        
        [ForeignKey("OrganisationMRT")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationMRTId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrganisationMRT? OrganisationMRT { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public bool IsRaw { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public bool IsBatch { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public bool IsManager { get; set; }
        [ForeignKey("SettingType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? SettingTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BaseTableValue? SettingType { get; set; }
        [StringLength(1024)]
        public string? Description { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
