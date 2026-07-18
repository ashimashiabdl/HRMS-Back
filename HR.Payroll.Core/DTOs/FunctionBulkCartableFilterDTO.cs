namespace HR.Payroll.Core.DTOs
{
    public class FunctionBulkCartableFilterDTO
    {
        public long? EducationGradeId { get; set; }
        public long? JobNatureId { get; set; }
        public long? OrganizationJobId { get; set; }
        public long? GenderId { get; set; }
        public long? MaritalStatusId { get; set; }
        public long? CostCenterId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? WorkPlaceId { get; set; }
        public long? PayLocationId { get; set; }
        public long? EmployeeStatusId { get; set; }
        public long? EmployeeTypeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? IdentityNo { get; set; }
        public string? NationalNo { get; set; }
        public long? PaymentPeriodId { get; set; }
        public int Limit { get; set; } = 100;
        public string SortBy { get; set; } = "FirstName";
        public string SortDirection { get; set; } = "ASC";
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
