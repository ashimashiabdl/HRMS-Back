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
    [Table("Tax", Schema = "Payroll")]
    public class Tax : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        public virtual EmployeeType EmployeeType { get; set; }
        public double? CoefficientTax { get; set; }
        public bool? IsAdjustment { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
