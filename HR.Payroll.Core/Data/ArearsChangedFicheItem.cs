using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Arears_Changed_FicheItem", Schema = "Payroll")]
    public class ArearsChangedFicheItem : BaseEntity
    {
        [ForeignKey("ArearFiche")]
        public long ArearFicheId { get; set; }
        public virtual ArearFiche? ArearFiche { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        //[ForeignKey("PaymentPeriod")]
        //[Comment("دوره محاسبه")]
        //public long PaymentPeriodId { get; set; }
        //public virtual PaymentPeriod? PaymentPeriod { get; set; }
        /// <summary>
        /// مبلغ جدید
        /// </summary>
        [Comment("مبلغ جدید")]
        public long CurrentAmount { get; set; }
        /// <summary>
        /// مبلغ در فیش قبلی
        /// </summary>
        [Comment("مبلغ فیش قبلی")]
        public long LastAmount { get; set; }
        ///// <summary>
        ///// مبلغ در فیش قبلی
        ///// </summary>
        //[Comment("مبلغ قابل پرداخت")]
        //public long PayableAmount { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
