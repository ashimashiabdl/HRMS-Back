using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class TaxNonCashPaymentDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public  string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public  string? Employee { get; set; }
        public double Value { get; set; }
        public long ItemTypeId { get; set; }
        public  string? ItemType { get; set; }
        public long PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        /// <summary>
        /// مستمر / غیر مستمر
        /// </summary>
        public bool Continuous { get; set; }
    }
}
