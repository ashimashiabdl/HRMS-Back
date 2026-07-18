using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.DTOs
{
    public class GetCartable_Result
    {
        public long Id { get; set; }
        public long Index { get; set; }
        public long WorkFlowId { get; set; }
        public Nullable<long> WorkFlowTypeId { get; set; }
        public string? WorkFlowType { get; set; }
        public long? InterdictOrderId { get; set; }
        public long? EmployeeSettlementId { get; set; }
        public string? CreateBy { get; set; }
        public string? Name { get; set; }
        public bool Pending { get; set; }
        public long RelatedUserId { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public Nullable<System.DateTime> DoDate { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long EntityEmployeeId { get; set; }
        public string? NationalNo { get; set; }
        public long? OrderTypeId { get; set; }
        public string? OrderType { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
