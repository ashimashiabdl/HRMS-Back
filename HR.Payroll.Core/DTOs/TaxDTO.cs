using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public double? CoefficientTax { get; set; }
  
        public bool? IsAdjustment { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public string? chequeNo { get; set; }
        public long PaymentTypeId { get; set; }
        public DateTime? ChequeDate { get; set; }
        public long? BankBranchId { get; set; }

    }
}
