using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.DTOs
{
    public class VersionChangeLogDTO : BaseDTO
    {
        public long VersionId { get; set; }
        public string ChangeType { get; set; }
        public string Description { get; set; }
    }
}
