using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class UserLoginHistoryDTO : BaseDTO
    {
        public bool IsSuccess { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalNo { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? FailReason { get; set; }
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
