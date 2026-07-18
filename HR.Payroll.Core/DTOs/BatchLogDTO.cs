using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class BatchLogDTO : BaseDTO
    {
        public string? LogDescription { get; set; }
        public string? ServiceName { get; set; }
        public int LogTypeId { get; set; }
        public string? LogTypeTitle { get; set; }
        public long? InterdictOrderId { get; set; }
        public long? PersonnelFunctionId { get; set; }
        public long? EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? PersonelCode { get; set; }
        public long? PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
    }
}
