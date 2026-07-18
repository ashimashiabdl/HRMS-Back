using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class CoefficientDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public long? CoefficientTypeId { get; set; }
        public string?  CoefficientTypeTitle { get; set; }
        public decimal? Value { get; set; }
    }
}
