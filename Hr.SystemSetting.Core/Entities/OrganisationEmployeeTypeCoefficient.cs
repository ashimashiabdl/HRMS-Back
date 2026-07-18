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
    [Table("Organisation_EmployeeType_Coefficient", Schema = "Setting")]
    public class OrganisationEmployeeTypeCoefficient : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
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
        [ForeignKey("Coefficient")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long CoefficientId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual Coefficient? Coefficient { get; set; }
        public int? Priority { get; set; }
        [NotMapped()]
        private new string title { get; set; }
    }
}
