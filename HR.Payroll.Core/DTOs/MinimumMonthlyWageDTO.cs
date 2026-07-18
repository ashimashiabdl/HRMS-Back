using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class MinimumMonthlyWageDTO : BaseDTO
    {
        public long MinimumWage { get; set; }
        public string? MinimumWageSep
        {
            get
            {

                return MinimumWage.ToString("#,##0");


            }
        }
        public string? MinimumWageDailySep
        {
            get
            {

                return (MinimumWage / 30).ToString("#,##0");


            }
        }
    }
}
