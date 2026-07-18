using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class GetLightOrderListForInsuranceDiskette_Result
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderType { get; set; }
        public Nullable<long> OrderDirectionTypeId { get; set; }
        public string? OrderDirectionType { get; set; }
        public long EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public Nullable<bool> IsEmployed { get; set; }
        public long OrderStatusId { get; set; }
        public Nullable<Int16> OrderSerial { get; set; }
        public string? OrderStatus { get; set; }
        public bool IsMartyrs { get; set; }
        public bool? IsIsar { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> PayRollRealExecuteDate { get; set; }

    }
}
