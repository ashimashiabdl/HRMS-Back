using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Tax_Table", Schema = "Payroll")]
    public class TaxTable : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long FromValue { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long ToValue { get; set; }
        [ForeignKey("Tax")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public int TaxPercent { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public int RelevantValue { get; set; }
        public bool BasedOnFunctionality { get; set; }
        [NotMapped]
        public long FromValueYearly
        {
            get
            {
                return FromValue * 12;
            }
        }

        [NotMapped]
        public long ToValueYearly
        {
            get
            {
                return ToValue * 12;
            }
        }

        [NotMapped]
        private new string title { get; set; }
    }
}
