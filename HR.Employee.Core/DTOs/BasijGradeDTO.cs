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
    public class BasijGradeDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string?  OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public int Grade { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }
    }
}
