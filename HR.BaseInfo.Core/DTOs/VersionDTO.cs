using HR.SharedKernel.Data;
using System;

namespace HR.BaseInfo.Core.DTOs
{
    public class VersionDTO : BaseDTO
    {
        public string VersionNumber { get; set; }
        public string? VersionName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ReleaseType { get; set; }
        public string Status { get; set; }
        public string Environment { get; set; }
        public bool IsBreakingChange { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VersionChangeLogDTO>? ChangeLogs { get; set; }
    }
}
