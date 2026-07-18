using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Status_List", Schema = "Payroll")]
public class StatusList : BaseEntity
{
    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }
    public int PersonnelCount { get; set; }
    public long SumPaymentAmount { get; set; }
    public long SumDeductAmount { get; set; }
    public long SumEmployerAmount { get; set; }
    public long? StatusListTypeId { get; set; }
    public virtual BaseTableValue? StatusListType { get; set; }
}
