using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class GetCurrentOrganPayrollPendigFunctions_Result
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Nationalno { get; set; }
        public string Personelcode { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Month { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public Nullable<long> Recrank { get; set; }
        public Nullable<int> Totalcount { get; set; }
    }
}
