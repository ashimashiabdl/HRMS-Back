using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Share
{
    public sealed class Convertor
    {
        public static string ToIranianDate(System.DateTime? dateTime)
        {
            try
            {
                var persianCalendar = new System.Globalization.PersianCalendar();
                return (dateTime != null ? string.Format("{0}/{1}/{2}", persianCalendar.GetYear(dateTime.Value).ToString(), persianCalendar.GetMonth(dateTime.Value).ToString("00"), persianCalendar.GetDayOfMonth(dateTime.Value).ToString("00")) : string.Empty);

                //return (dateTime != null ? string.Format("{0}/{1}/{2}-{3}:{4}:{5}", persianCalendar.GetYear(dateTime.Value).ToString(), persianCalendar.GetMonth(dateTime.Value).ToString("00"), persianCalendar.GetDayOfMonth(dateTime.Value).ToString("00"),persianCalendar.GetHour(dateTime.Value).ToString("00"),persianCalendar.GetMinute(dateTime.Value).ToString("00"),persianCalendar.GetSecond(dateTime.Value).ToString("00") ) : string.Empty);


            }
            catch
            {
                //return "موجود نمی باشد";
                return string.Empty;
            }
        }
    }
}
