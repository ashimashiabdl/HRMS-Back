using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelPaymentDTO : BaseDTO
    {
        public long PaymentTypeId { get; set; }
        public string? PaymentType { get; set; }
        public long? BankBranchId { get; set; }
        public string? BankBranch { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long? PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        public int SendType { get; set; }
        public long Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        [StringLength(256)]
        public string? Description { get; set; }
        public string? FicheStatus { get; set; }
        public bool IsClosed { get; set; }
    }
}
