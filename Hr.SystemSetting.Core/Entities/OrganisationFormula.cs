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
    [Table("Organisation_Formula", Schema = "Setting")]
    public class OrganisationFormula : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Formula")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long FormulaId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual Formula? Formula { get; set; }

        [ForeignKey("FormulaUsageLocation")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long FormulaUsageLocationId { get; set; }
        public virtual FormulaUsageLocation? FormulaUsageLocation { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
