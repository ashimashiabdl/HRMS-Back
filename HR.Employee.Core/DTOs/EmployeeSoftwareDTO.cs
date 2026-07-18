using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class EmployeeSoftwareDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long SoftwareId { get; set; }
        public string? Software { get; set; }
        public long SoftwareTypeId { get; set; }
        public string? SoftwareType { get; set; }
        public long MasteryLevelTypeId { get; set; }
        public string? MasteryLevelType { get; set; }
    }
}
