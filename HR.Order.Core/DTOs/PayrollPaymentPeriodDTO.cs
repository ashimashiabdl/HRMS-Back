using HR.SharedKernel.Data;

namespace HR.Order.Core.DTOs
{
    public class PayrollPaymentPeriodDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        //public string? OrganisationChart { get; set; }
        public int ShamsiYear { get; set; }
        public int ShamsiMonth { get; set; }
        public int PeriodDays { get; set; }
        public bool IsClosed { get; set; }
        public bool UpdatedOnSite { get; set; }
    }
}