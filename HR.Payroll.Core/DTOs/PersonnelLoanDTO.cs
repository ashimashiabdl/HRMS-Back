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
    public class PersonnelLoanDTO : BaseDTO
    {
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        public long LoanTypeId { get; set; }
        public string? LoanType { get; set; }
        public long? BankBranchId { get; set; }
        public string? BankBranch { get; set; }
        public long StartDeductPaymentPeriodId { get; set; }
        public string? StartDeductPaymentPeriod { get; set; }

        public DateTime PaymentDate { get; set; }
        public long RemainAmount { get; set; }
        public string RemainAmountSep
        {
            get
            {
                return RemainAmount.ToString("#,##0") + " ريال ";
            }
        }
        public bool? IsActive { get; set; }
        [StringLength(128)]
        public string? LoanPaymentDocNo { get; set; }
        [StringLength(128)]
        public string? LoanPaymentDocDesc { get; set; }
        public long? AllAmount { get; set; }
        public string AllAmountSep
        {
            get
            {
                if (AllAmount == null)
                {
                    return "0";
                }
                return AllAmount.Value.ToString("#,##0") + " ريال ";
            }
        }
        public long? InstallmentAmount { get; set; }
        public string InstallmentAmountSep
        {
            get
            {
                if (InstallmentAmount == null)
                {
                    return "0";
                }
                return InstallmentAmount.Value.ToString("#,##0") + " ريال ";
            }
        }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(128)]
        public string? AccountNumber { get; set; }
        [StringLength(128)]
        public string? ReciverDesc { get; set; }
        public bool? AutoReceive { get; set; }
        [StringLength(128)]
        public string? ShebaNo { get; set; }
        /// <summary>
        /// خرده باقیمانده وام در اولین قسط کم شود
        /// </summary>
        public bool RemainingCrumbsAtFirst { get; set; }
    }
}
