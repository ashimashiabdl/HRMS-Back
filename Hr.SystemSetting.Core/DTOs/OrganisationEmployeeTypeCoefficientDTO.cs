using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeCoefficientDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long CoefficientId { get; set; }
        public string? CoefficientTitle { get; set; }
        public int? Priority { get; set; }
    }
}
