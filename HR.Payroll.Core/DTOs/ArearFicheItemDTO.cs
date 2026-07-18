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
    public class ArearFicheItemDTO : BaseDTO
    {
        public long ArearFicheId { get; set; }
        public long WageItemId { get; set; }
        public string? WageItem { get; set; }
        public long PaymentTypeId { get; set; }
        public double Value { get; set; }
        [StringLength(512)]
        public string? Comment { get; set; }
        public long FicheId { get; set; }
        public string? PaymentType { get; set; }
        public long? RemainLoanAmount { get; set; }
        public long? PersonnelLoanId { get; set; }
        public string? PersonnelLoan { get; set; }
        public string? PaymentPeriod { get; set; }
        public long? EmployeeFundId { get; set; }
        public long? FundSumAmount { get; set; }
        public long? EmployeeDeductionId { get; set; }
        public long? PersonnelPaymentId { get; set; }
        public long? RemainDeductionAmount { get; set; }
        public bool IsArear { get; set; }
        public long? ArearPaymentPeriodId { get; set; }
        public string? ArearPaymentPeriod { get; set; }
        public bool? IsEmployerItem { get; set; }
        public bool IsSubItem { get; set; }
    }
}
