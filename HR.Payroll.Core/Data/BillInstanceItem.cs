using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
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
    [Table("Bill_Instance_Item", Schema = "Payroll")]
    public class BillInstanceItem :BaseEntity
    {
        [ForeignKey("BillInstance")]
        public long BillInstanceId { get; set; }
        public virtual BillInstance? BillInstance { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        public long Amount { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
    }
}
