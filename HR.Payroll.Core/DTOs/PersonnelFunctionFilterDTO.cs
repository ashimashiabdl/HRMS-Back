namespace HR.Payroll.Core.DTOs
{
    public class PersonnelFunctionFilterDTO
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string Filter { get; set; } = string.Empty;
        public string ActiveSortColumn { get; set; } = string.Empty;
        public string Sortdirection { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public long? EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalNo { get; set; } = string.Empty;
        public string PersonnelCode { get; set; } = string.Empty;
        public long? PaymentPeriodId { get; set; }
        public long? CostCenterId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public string InterdictOrderFirstName { get; set; } = string.Empty;
        public string InterdictOrderNationalNo { get; set; } = string.Empty;
        public string InterdictOrderName { get; set; } = string.Empty;
    }
}

