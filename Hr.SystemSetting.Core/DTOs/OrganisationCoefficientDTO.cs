using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationCoefficientDTO : BaseDTO
    {
        public long CoefficientId { get; set; }
        public string? CoefficientTitle { get; set; }
        public long? MappedExcelColumnId { get; set; }
        public string? MappedExcelColumnTitle { get; set; }
    }
}
