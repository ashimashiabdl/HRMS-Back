using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Employee.Core.DTOs
{
    public class EmployeeFileDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? FileId { get; set; }
        public long? TempFileId { get; set; }
        public string? File { get; set; }
        [StringLength(500)]
        public string? Name { get; set; }
        public long EmployeeId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsImage { get; set; }
        public string? Extension { get; set; }
        public string? MimeType { get; set; }
        public string? temp_inBase64 { get; set; }
        public long FileGroupId { get; set; }
        public string? FileGroup { get; set; }
        [StringLength(200)]
        public string? OtherFileGroupName { get; set; }
        public long Size { get; set; }
    }
}
