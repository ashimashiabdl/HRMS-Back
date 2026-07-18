using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Deducted_Arears_Detail", Schema = "Payroll")]
    public class DeductedArearsDetail : BaseEntity
    {
        [ForeignKey("DeductedArears")]
        public long DeductedArearsId { get; set; }
        public virtual DeductedArears? DeductedArears { get; set; }
        public long? PaymentAmount { get; set; }
        [ForeignKey("PaymentPeriod")]
        public long PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }

        [ForeignKey("ArearFiche")]
        public long ArearFicheId { get; set; }
        public virtual ArearFiche? ArearFiche { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PaymentDate { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
