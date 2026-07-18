using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class OrganisationEmployeeTypeCoefficientBonusWageItemDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }

        public long WageItemId { get; set; }
        public string? WageItem { get; set; }

        public long CoefficientId { get; set; }
        public string? Coefficient { get; set; }
    }
}


