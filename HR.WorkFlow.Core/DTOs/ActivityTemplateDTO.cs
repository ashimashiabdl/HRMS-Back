using HR.SharedKernel.Data;
using System;

namespace HR.WorkFlow.Core.DTOs
{
    public class ActivityTemplateDTO : BaseDTO
    {
        public long WorkFlowInstanceId { get; set; }
        public string? WorkFlowInstance { get; set; }
        public string? WorkFlow { get; set; }
        public long? FromNodeId { get; set; }
        public string? FromNode { get; set; }
        public long? ToNodeId { get; set; }
        public string? ToNode { get; set; }
        public long ActionId { get; set; }
        public string? ActionDesc { get; set; }
        public long? UserSignatureId { get; set; }
        public string? UserSignature { get; set; }
        public DateTime? DoDate { get; set; }
        public bool Pending { get; set; }
        public bool IsFinalTransition { get; set; }
        public string? Comment { get; set; }
    }
}
