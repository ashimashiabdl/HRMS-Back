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
    [Table("Organisation_EmployeeType_OrderType_WageItem", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderTypeWageItem : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
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
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual WageItem? WageItem { get; set; }

        [ForeignKey("OrganisationFormula")]
        public long? OrganisationFormulaId { get; set; }
        public virtual OrganisationFormula? OrganisationFormula { get; set; }
        [ForeignKey("OrganisationCheckFormula")]
        public long? OrganisationCheckFormulaId { get; set; }
        public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }

        
        public long EnterTypeId { get; set; }
        
        
        public long? CheckingTimeId { get; set; }
        

        public Nullable<int> FixValue { get; set; }
        public Nullable<int> Min { get; set; }
        public Nullable<int> Max { get; set; }
        public bool IsEditable { get; set; }
        public bool HideInOrderPrint { get; set; }
        public bool IsDefault { get; set; }
        public bool IsBatch { get; set; }
        [StringLength(256)]
        public string? Description { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
