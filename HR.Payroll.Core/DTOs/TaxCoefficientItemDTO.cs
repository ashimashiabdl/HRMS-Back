using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs
{
    public class TaxCoefficientItemDTO : BaseDTO
    {
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public double? CoefficientTax { get; set; }
        public string? CoefficientTaxVw { get {

                return "  7 / " + CoefficientTax ;

            } }
        public long TaxId { get; set; }
        public string? Tax { get; set; }
    }
}
