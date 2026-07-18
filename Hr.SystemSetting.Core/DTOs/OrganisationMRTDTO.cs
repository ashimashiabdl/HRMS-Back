using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationMRTDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public long TempFileId { get; set; }
  
        [StringLength(1024)]
        public string? Description { get; set; }

        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[]? Content { get; set; } = null!;
        [StringLength(512)]
        public string? MimeType { get; set; }
    }
}
