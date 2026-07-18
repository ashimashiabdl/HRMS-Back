using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    /// <summary>
    /// قلم فیش معوقه — تمام فیلدهای مشترک از <see cref="FicheItem"/> ارث می‌برد.
    /// هر تغییر در FicheItem به‌صورت خودکار در این موجودیت لحاظ می‌شود.
    /// کلید والد به‌جای FicheId از ArearFicheId استفاده می‌کند.
    /// </summary>
    [Table("Arear_FicheItem", Schema = "Payroll")]
    public class ArearFicheItem : FicheItem
    {
        [ForeignKey(nameof(ArearFiche))]
        public long ArearFicheId { get; set; }
        public virtual ArearFiche? ArearFiche { get; set; }
    }
}
