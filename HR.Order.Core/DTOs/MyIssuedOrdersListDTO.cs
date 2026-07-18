using System;

namespace HR.Order.Core.DTOs
{
    public class MyIssuedOrdersListDTO
    {
        public long Id { get; set; }
        
        // From Employee
        public long? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalNo { get; set; }
        
        // From RecruitOrder
        public string? PayLocation { get; set; }
        public string? CostCenter { get; set; }
        public string? EmployeeStatus { get; set; }
        
        // From InterdictOrder
        public string? OrderType { get; set; }
        public long? StatusId { get; set; }
        public string? Status { get; set; }
        public string? IssueType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
