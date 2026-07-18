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
    [Table("Organisation_WageItem", Schema = "Setting")]
    public class OrganisationWageItem : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual WageItem? WageItem { get; set; }
        public long? MappedExcelColumnId { get; set; }
        public virtual BaseTableValue? MappedExcelColumn { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
