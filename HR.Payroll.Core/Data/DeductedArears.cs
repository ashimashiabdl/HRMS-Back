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
    /// <summary>
    /// کسورات معوقه
    /// </summary>
    [Table("Deducted_Arears", Schema = "Payroll")]
    public class DeductedArears : BaseEntity
    {
        [ForeignKey("Arear")]
        public long ArearId { get; set; }
        public virtual Arear? Arear { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        public long? AllAmount { get; set; }
        public long? RemainAmount { get; set; }
        public long? InstalmentAmount { get; set; }
        public int? InstalmentCount { get; set; }
        [ForeignKey("StartDeductedPaymentPeriod")]
        public long StartDeductedPaymentPeriodId { get; set; }
        public bool? IsActive { get; set; }
        public virtual PaymentPeriod? StartDeductedPaymentPeriod { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
