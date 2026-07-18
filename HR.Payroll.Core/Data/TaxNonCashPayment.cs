using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Tax_NonCash_Payment", Schema = "Payroll")]
    public class TaxNonCashPayment : BaseEntity, IOrganisationChartId , IignoreDateRangeValidation
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        [ForeignKey("PaymentPeriod")]
        public long PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }

        public double Value { get; set; }
  public long ItemTypeId { get; set; }
  public virtual BaseTableValue? ItemType { get; set; }
       
        /// <summary>
        /// ����� / ��� �����
        /// </summary>
        public bool Continuous { get; set; }
    }
}
