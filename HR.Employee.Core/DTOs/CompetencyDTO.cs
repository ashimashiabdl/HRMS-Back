using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class CompetencyDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long? CompetencyLevelId { get; set; }
        public string? CompetencyLevel { get; set; }
        public long? CompetencyTypeId { get; set; }
        public string? CompetencyType { get; set; }
        public string? Description { get; set; }
        public bool? Acceptable { get; set; }
    }
}
