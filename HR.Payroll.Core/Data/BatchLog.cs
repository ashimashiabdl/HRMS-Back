using HR.Order.Core.Data;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Batch_Log", Schema = "Payroll")]
    public class BatchLog : BaseEntity , IignoreDateRangeValidation
    {
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? LogDescription { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? ServiceName { get; set; }
        public int LogTypeId { get; set; }
        [ForeignKey("InterdictOrder")]
        public long? InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }
        [ForeignKey("PersonnelFunction")]
        public long? PersonnelFunctionId { get; set; }
        public virtual PersonnelFunction? PersonnelFunction { get; set; }
        [ForeignKey("Employee")]
        public long? EmployeeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        [ForeignKey("PaymentPeriod")]
        public long? PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }
    }
}
