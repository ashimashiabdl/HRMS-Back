using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class EmployeeLoginHistoryDTO : BaseDTO
    {
        public long? EmployeeId { get; set; }
        public string? ActiveName { get; set; }
        public string? IPAddress { get; set; }
        public string FullTimeStamp
        {
            get
            {
                if (CreateDate == null)
                {
                    return "";
                }
                else
                {
                    PersianCalendar pc = new PersianCalendar();
                    return string.Format("{0}/{1}/{2}", pc.GetYear(CreateDate.Value), pc.GetMonth(CreateDate.Value).ToString().PadLeft(2, '0'), pc.GetDayOfMonth(CreateDate.Value).ToString().PadLeft(2, '0')) + " - " + CreateDate.Value.Hour.ToString().PadLeft(2, '0') + ":" + CreateDate.Value.Minute.ToString().PadLeft(2, '0') + ":" + CreateDate.Value.Second.ToString().PadLeft(2, '0');
                }

            }
        }
    }
}
