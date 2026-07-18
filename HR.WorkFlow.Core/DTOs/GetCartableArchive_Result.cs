using System;

namespace HR.WorkFlow.Core.DTOs
{
    public class GetCartableArchive_Result
    {
        public long Id { get; set; }
        public long Index { get; set; }
        public long WorkFlowId { get; set; }
        public long? WorkFlowTypeId { get; set; }
        public string? WorkFlowType { get; set; }
        public long? InterdictOrderId { get; set; }
        public long? EmployeeSettlementId { get; set; }
        public string? CreateBy { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long EntityEmployeeId { get; set; }
        public string? NationalNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long? OrderTypeId { get; set; }
        public string? OrderType { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime? MyLastActionDate { get; set; }
        public string? MyLastActionDesc { get; set; }
        public string? LatestStatus { get; set; }
        public string? CurrentNodeTitle { get; set; }
    }
}
