using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Branch", Schema = "Payroll")]
    public class BankBranch : BaseEntity
    {
        [ForeignKey("Bank")]
        public long BankId { get; set; }
        public virtual Bank? Bank { get; set; }
        [StringLength(512)]
        public string? Code { get; set; }
        [StringLength(64)]
        public string? Phone { get; set; }
        [StringLength(1024)]
        public string? Address { get; set; }
        public bool? IsValid { get; set; }
    }
}
