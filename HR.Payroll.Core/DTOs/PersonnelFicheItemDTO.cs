using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelFicheItemDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public long? PersonnelPaymentId { get; set; }

        public long PaymentIntervalId { get; set; }
        public string? PaymentInterval { get; set; }
        public Nullable<int> Value { get; set; }
        public string ValueSep
        {
            get
            {

                if (Value == null)
                {
                    return "0";
                }
                else
                {
                    var nfi = new NumberFormatInfo()
                    {
                        NumberDecimalDigits = 0,
                        NumberGroupSeparator = "."
                    };
                    return Value.Value.ToString("N", nfi) + " ريال ";
                }

            }
        }
        public long? OrganisationCheckFormulaId { get; set; }
        public string? OrganisationCheckFormula { get; set; }
        public long? OrganisationFormulaId { get; set; }
        public string? OrganisationFormula { get; set; }
        public bool DeductAtOnce { get; set; }
        public long EnterTypeId { get; set; }
        public string? EnterType { get; set; }
    }
}
