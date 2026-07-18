using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class GetinterdictWorkFlowDetails_Result
    {
        public Nullable<long> Id { get; set; }
        public Nullable<long> WorkFlowId { get; set; }
        public Nullable<long> InterdictOrderId { get; set; }
        public Nullable<long> LastActivityId { get; set; }
        public long ActivityId { get; set; }
        public string CreateBy { get; set; }
        public Nullable<long> FromNodeId { get; set; }
        public string FromNode { get; set; }
        public Nullable<long> ToNodeId { get; set; }
        public string ToNode { get; set; }
        public string Name { get; set; }
        public bool Pending { get; set; }
        public Nullable<long> RelatedUserId { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public string PositionName { get; set; }
        public Nullable<int> Priority { get; set; }
        public int Index { get; set; }
        public DateTime? DoDate { get; set; }
        public long? ActionId { get; set; }
        public string? ActionDesc { get; set; }
        public string? Comment { get; set; }
        public long? UserSignatureId { get; set; }
        public string? SignTitle { get; set; }
        public string? ActorName { get; set; }
        public string? AssignedUserNames { get; set; }
        public string? AssignedRoleNames { get; set; }
        public string? AssignedRoleMemberNames { get; set; }
        public bool IsFinalTransition { get; set; }
    }
}
