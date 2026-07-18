using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HR.SharedKernel
{
    public static class Utilities
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string SetFarsiNumber(string Input)
        {
            if (Input != null)
            {
                for (int i = 0; i < Input.Length; i++)
                {
                    if (Input[i] >= '0' && Input[i] <= '9')
                    {
                        char temp = Convert.ToChar(Input[i] + 1776 - '0');
                        Input = Input.Replace(Input[i], temp);
                    }
                }
            }
            return Input.Replace('٫', '.');
        }
        public static string PersianNumberToEnglish(string Data)
        {
            string result = "";
            for (int i = 0; i < Data.Count(); i++)
            {
                if (((int)Data[i]) >= 1776 && ((int)Data[i]) <= 1785)
                {
                    result += ((char)(1728 - (int)Data[i]));
                }
                else
                {
                    result += Data[i];
                }
            }
            return result;
        }

        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }
        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static Boolean IsValidNationalCode(this String nationalCode)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (String.IsNullOrEmpty(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (nationalCode.Length != 10)
                throw new Exception("طول کد ملی باید ده کاراکتر باشد");

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }


        /// <summary>
        /// گرفتن اولین روز ماه جاری
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentMonthFirstDay()
        {
            DateTime today = DateTime.UtcNow;
            PersianCalendar persianCalendar = new PersianCalendar();
            int yearPersian = persianCalendar.GetYear(today);
            int monthPersian = persianCalendar.GetMonth(today);
            DateTime firstDayOfMonth = persianCalendar.ToDateTime(yearPersian, monthPersian, 1, 0, 0, 0, 0);
            return firstDayOfMonth;
        }



        public static int ConvertDateToNumber(DateTime dt)
        {
            var pc = new PersianCalendar();
            int NumberDate = Convert.ToInt32(pc.GetYear(dt).ToString() + pc.GetMonth(dt).ToString().PadLeft(2,'0') + pc.GetDayOfMonth(dt).ToString().PadLeft(2, '0'));
            return NumberDate;
        }

        public static int ConvertDateToNumberMiladi(DateTime dt)
        {
            
            int NumberDate = Convert.ToInt32(dt.Year.ToString() + dt.Month.ToString().PadLeft(2, '0') + dt.Month.ToString().PadLeft(2, '0'));
            return NumberDate;
        }
    }
}
