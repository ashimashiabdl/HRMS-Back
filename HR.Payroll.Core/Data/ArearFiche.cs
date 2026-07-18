using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    /// <summary>
    /// فیش معوقه — تمام فیلدهای مشترک از <see cref="Fiche"/> ارث می‌برد.
    /// هر تغییر در Fiche به‌صورت خودکار در این موجودیت لحاظ می‌شود.
    /// </summary>
    [Table("Arear_Fiche", Schema = "Payroll")]
    public class ArearFiche : Fiche, IignoreDateRangeValidation
    {
        /// <summary>
        /// معوقه والد (در صورت وجود)
        /// </summary>
        [ForeignKey(nameof(Arear))]
        [Comment("معوقه والد")]
        public long? ArearId { get; set; }
        public virtual Arear? Arear { get; set; }

        /// <summary>
        /// دوره ای که قصد پرداخت این معوقه در آن را داریم
        /// </summary>
        [Comment("دوره ای که قصد پرداخت این معوقه در آن را داریم")]
        [ForeignKey(nameof(PaymentPeriodIntendToPay))]
        public long PaymentPeriodIntendToPayId { get; set; }
        public virtual PaymentPeriod? PaymentPeriodIntendToPay { get; set; }

        public virtual ICollection<ArearFicheItem>? ArearFicheItems { get; set; }
        public virtual ICollection<ArearsChangedFicheItem>? ArearsChangedFicheItems { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
