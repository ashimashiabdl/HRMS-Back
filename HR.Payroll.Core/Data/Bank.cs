using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bank", Schema = "Payroll")]
    public class Bank : BaseEntity
    {
        public bool? IsValid { get; set; }
        public int? BankCode { get; set; }
    }
}
