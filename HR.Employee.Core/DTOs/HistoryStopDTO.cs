using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class HistoryStopDTO : BaseDTO
    {
        public long? OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public bool? IsComputable { get; set; }
        public int? HistoryStopDays { get; set; }
        public long? HistoryStopTypeId { get; set; }
        public string? HistoryStopType { get; set; }
    }
}
