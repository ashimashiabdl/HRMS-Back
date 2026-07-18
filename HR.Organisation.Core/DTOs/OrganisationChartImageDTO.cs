using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationChartImageDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        [StringLength(2048)]
        public string? MimeType { get; set; }
        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; } = null!;
    }
}
