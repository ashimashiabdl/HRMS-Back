using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs
{
    public class ArearDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalNo { get; set; }
        public string? PersonelCode { get; set; }
        public long? PersonnelFunctionId { get; set; }
        public string? PersonnelFunction { get; set; }
        public long? InterdictOrderId { get; set; }
        public string? InterdictOrder { get; set; }
        public short? Serial { get; set; }
        public long ArearsStatusId { get; set; }
        public string? ArearsStatus { get; set; }
        public long? ApproveTimePaymentPeriodId { get; set; }
        public string? ApproveTimePaymentPeriod { get; set; }
        public long? PaymentPeriodIntendToPayId { get; set; }
        public string? PaymentPeriodIntendToPay { get; set; }
        public long TotalDifferenceAmount { get; set; }
        public long PayableDifferenceAmount { get; set; }
        public int ArearFicheCount { get; set; }
        public int ChangedItemCount { get; set; }
        public DateTime? CalculatedDate { get; set; }
        [StringLength(2000)]
        public string? LastErrorMessage { get; set; }
        [StringLength(1500)]
        public string? Description { get; set; }
    }
}
