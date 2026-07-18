using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class EmployeeTypeOrderTypeChecks_Result
    {
        public long Id { get; set; }
        public string FormulaTitle { get; set; }
        public long OrganisationChartId { get; set; }
        public long EmployeeTypeId { get; set; }
        public long OrderTypeId { get; set; }
        public long CheckTypeId { get; set; }
        public long OrganisationFormulaId { get; set; }
        public string FailMessage { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
