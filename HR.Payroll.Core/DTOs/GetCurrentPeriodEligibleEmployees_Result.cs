using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class GetCurrentPeriodEligibleEmployees_Result
    {
        public long Id { get; set; }
        public long PaymentPeriodId { get; set; }
        public long EmployeeId { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string nationalno { get; set; }
        public string identityno { get; set; }
        public long InterdictOrderId { get; set; }
        public long PersonnelFunctionId { get; set; }
        public Nullable<short> OrderSerial { get; set; }
    }
}
