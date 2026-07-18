using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class GetBatchRequestFiles_Result
    {
        public long Id { get; set; }
        public long BatchRequestId { get; set; }
        public string? Description { get; set; }
        public string? title { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string? IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string? Extension { get; set; }
        public long FileTypeId { get; set; }
        public string? FileType { get; set; }
        public string? MimeType { get; set; }
        public long Size { get; set; }
        public Nullable<System.Guid> UniqueId { get; set; }
    }
}
