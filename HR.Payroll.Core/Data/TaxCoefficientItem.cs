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
    [Table("Tax_Coefficient_Item", Schema = "Payroll")]
    public class TaxCoefficientItem : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("WageItem")]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        public double? CoefficientTax { get; set; }
        [ForeignKey("Tax")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
