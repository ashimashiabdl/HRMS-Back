using HR.Payroll.Core.Data;
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
    public class BankDisketteItemDTO : BaseDTO
    {
        public long BankDisketteId { get; set; }
        public  string? BankDiskette { get; set; }
        public long EmployeeId { get; set; }
        public  string? Employee { get; set; }
        public  string? NationalNo { get; set; }
        public long FicheId { get; set; }
        public long? PersonnelPaymentId { get; set; }
        public  string? PersonnelPayment { get; set; }
        public long? OrganCodesId { get; set; }
        public  string? OrganCodes { get; set; }
        public long? CostCenterId { get; set; }
        public  string? CostCenter { get; set; }
        public string? AccountNo { get; set; }
        public long Amount { get; set; }
    }
}
