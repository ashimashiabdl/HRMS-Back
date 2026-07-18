using Hr.SystemSetting.Core.Entities;
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
using System.Data.Entity.Infrastructure.Annotations;


namespace HR.Payroll.Core.Data
{
    [Table("Calclulation_Setting", Schema = "Payroll")]
    public class CalclulationSetting : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("RewardFormula")]
        public long? RewardFormulaId { get; set; }
        public virtual OrganisationFormula? RewardFormula { get; set; }

        [ForeignKey("SanavatFormula")]
        public long? SanavatFormulaId { get; set; }
        public virtual OrganisationFormula? SanavatFormula { get; set; }
        [ForeignKey("RewardAndSanavatStoreType")]
        public long? RewardAndSanavatStoreTypeId { get; set; }
        public virtual BaseTableValue? RewardAndSanavatStoreType { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
