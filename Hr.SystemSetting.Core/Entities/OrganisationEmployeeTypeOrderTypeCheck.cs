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
    [Table("Organisation_EmployeeType_OrderType_Checks", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderTypeCheck : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
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
        [ForeignKey("OrderType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }

        public long CheckTypeId { get; set; }
        public virtual BaseTableValue? CheckType { get; set; }
        [ForeignKey("OrganisationFormula")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationFormulaId { get; set; }
        public virtual OrganisationFormula? OrganisationFormula { get; set; }
        [StringLength(256)]
        public string? FailMessage { get; set; }
        [StringLength(256)]
        public string? Description { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
