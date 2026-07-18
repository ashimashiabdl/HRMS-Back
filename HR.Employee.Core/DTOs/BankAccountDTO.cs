using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class BankAccountDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChartTitle { get; set; }
        public long EmployeeId { get; set; }
        public string? EmployeeTitle { get; set; }
        public string? AccountNumber { get; set; }
        public long? AccountTypeId { get; set; }
        public string? AccountTypeTitle { get; set; }
        public int? Priority { get; set; }
        public int? FromPrice { get; set; }
        public int? ToPrice { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
        public long? BankId { get; set; }
        public int? BankBranchId { get; set; }
        public int? OldId { get; set; }
        public long? PayrollStatusId { get; set; }
        public string? PayrollStatusTitle { get; set; }
        public string? BonCardNumber { get; set; }
        public string? CardNumber { get; set; }
        public string? ShabaNumber { get; set; }
        public string? FncBank { get; set; }

    }
}
