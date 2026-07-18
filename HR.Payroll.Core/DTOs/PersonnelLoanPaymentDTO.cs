using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelLoanPaymentDTO : BaseDTO
    {
        public long FicheId { get; set; }
        public string? Fiche { get; set; }
        public long PersonnelLoanId { get; set; }
        public virtual string? PersonnelLoan { get; set; }
        public bool IsPaid { get; set; }
        public long PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public long PaymentTypeId { get; set; }
        public string? PaymentType { get; set; }
        public long? RemainLoanAmount { get; set; }

    }
}
