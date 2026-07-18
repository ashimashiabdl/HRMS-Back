using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs
{
    public class EmployeeDeductionDTO : BaseDTO
    {
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }

        public long DeductionTypeId { get; set; }
        public string? DeductionType { get; set; }

        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }

        public long StartDeductPaymentPeriodId { get; set; }
        public string? StartDeductPaymentPeriod { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(128)]
        public string? LoanPaymentDocDesc { get; set; }

        public long? AllAmount { get; set; }
        public long? InstallmentAmount { get; set; }

        public bool RemainingCrumbsAtFirst { get; set; }

        public bool IsActive { get; set; }
    }
}


