using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class ImageDTO : BaseDTO
    {
        
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        
        public long EmployeeId { get; set; }
        
        public bool IsDefault { get; set; }

        public long? ImageTypeId { get; set; }
        public string?  ImageTypeTitle { get; set; }

        public string? ImageDataBase64 { get; set; } = null!;
    }
}
