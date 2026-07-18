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
    [Table("Payment_Period", Schema = "Payroll")]
    public class PaymentPeriod : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        public int ShamsiYear { get; set; }
        public int ShamsiMonth { get; set; }
        public int PeriodDays { get; set; }
        public bool IsClosed { get; set; }
        public bool UpdatedOnSite { get; set; }
    }
}
