using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class EvaluationResultDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public decimal? Average { get; set; }
        public int Year { get; set; }
        public byte? EvaluationCoefficent { get; set; }
        public int? YearCoefficent { get; set; }
        public long? StateId { get; set; }
        public string?  StateTitle { get; set; }
        public long? EvaluationGroupTypeId { get; set; }
        public string? EvaluationGroupType { get; set; }
    }
}
