using Hr.SystemSetting.Core.Entities;
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

namespace HR.Payroll.Core.Data
{
    [Table("CostCenter_FicheItem", Schema = "Payroll")]
    public class CostCenterFicheItem : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("CostCenter")]
        public long CostCenterId { get; set; }
        public virtual OrganisationChart? CostCenter { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        [StringLength(256)]
        public string? Description { get; set; }
        public int? PriorityNo { get; set; }
        public bool IsFixed { get; set; }
        public bool OnceInFiche { get; set; }
        public long? Amount { get; set; }
        [NotMapped]
        private new string title { get; set; }

    }
}
