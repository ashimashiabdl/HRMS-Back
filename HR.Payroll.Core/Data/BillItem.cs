using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bill_Item", Schema = "Payroll")]
    public class BillItem :BaseEntity
    {
        [ForeignKey("Bill")]
        public long BillId { get; set; }
        public virtual Bill? Bill { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
    }
}
