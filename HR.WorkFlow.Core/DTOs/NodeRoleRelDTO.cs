using HR.SharedKernel.Data;

namespace HR.WorkFlow.Core.DTOs
{
    public class NodeRoleRelDTO : BaseDTO
    {
        public long WorkFlowId { get; set; }
        public string? WorkFlow { get; set; }
        public long RoleId { get; set; }
        public string? Role { get; set; }
        public long NodeId { get; set; }
        public string? Node { get; set; }
    }
}
