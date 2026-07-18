using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxTableDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long FromValue { get; set; }
        public string FromValueSep
        {
            get
            {

                return FromValue.ToString("#,##0");


            }
        }
        public string FromValueYearSep
        {
            get
            {

                return (FromValue * 12).ToString("#,##0");


            }
        }
        public long ToValue { get; set; }
        public string ToValueSep
        {
            get
            {
                return ToValue.ToString("#,##0");
            }
        }
        public string ToValueYearSep
        {
            get
            {
                return (ToValue * 12).ToString("#,##0");
            }
        }
        public int TaxPercent { get; set; }
        public int RelevantValue { get; set; }
        public string RelevantValuesep
        {
            get
            {
                return (RelevantValue).ToString("#,##0") + " ريال";
            }
        }
        public long TaxId { get; set; }
        public string? Tax { get; set; }
    }
}
