using HR.Order.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    internal class BatchRequestFileDTO
    {
        public long BatchRequestId { get; set; }
        public string? BatchRequest { get; set; }
        public byte[] AggragateZipFile { get; set; } = null!;
        public byte[] RelatedExcel { get; set; } = null!;
        [StringLength(2048)]
        public string? Description { get; set; }
    }
}
