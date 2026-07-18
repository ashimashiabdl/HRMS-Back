using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("PaymentPeriod_Employee_Bonus", Schema = "Payroll")]
public class PaymentPeriodEmployeeBonus : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("PaymentPeriod")]
    public long PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("Coefficient")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long CoefficientId { get; set; }
    public virtual Coefficient? Coefficient { get; set; }
    public double Value { get; set; }
}
