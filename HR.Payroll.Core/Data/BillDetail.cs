using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bill_Detail", Schema = "Payroll")]
    public class BillDetail : BaseEntity
    {
        [ForeignKey("Bill")]
        public long BillId { get; set; }
        public virtual Bill? Bill { get; set; }public virtual BaseTableValue? BillType { get; set; }

        public int Value { get; set; }

    }
}
