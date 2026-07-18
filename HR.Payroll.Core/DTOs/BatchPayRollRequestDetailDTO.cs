using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BatchPayRollRequestDetailDTO : BaseDTO
    {
        public long EmployeeId { get; set; }
        public string? ActiveName { get; set; }
        public string? NationalNo { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonelCode { get; set; }
        public string? IdentityNo { get; set; }
        public long BatchPayRollRequestId { get; set; }
        public string? BatchPayRollRequest { get; set; }
        public long? FicheId { get; set; }
        public string? Fiche { get; set; }
        public string? FinalMessage { get; set; }
        public DateTime? DoDatetime { get; set; }
        public DateTime? LastTryDateTime { get; set; }
        public int RunTimeinMilliseconds { get; set; }
    }
}
