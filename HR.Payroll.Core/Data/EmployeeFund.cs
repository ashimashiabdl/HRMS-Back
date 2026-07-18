using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

/// <summary>
/// صندوق های کارمند
/// </summary>
[Table("Employee_Fund", Schema = "Payroll")]
public class EmployeeFund : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }


    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }

    [ForeignKey("FundType")]
    public long FundTypeId { get; set; }
    public virtual FundType? FundType { get; set; }
    [ForeignKey("StartDeductPaymentPeriod")]
    public long StartDeductPaymentPeriodId { get; set; }
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; }
    public bool  IsActive { get; set; }
}
