using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    /// <summary>
    /// جدول معوقه — واحد تجمیعی محاسبه معوقه برای یک حکم/کارمند
    /// </summary>
    [Table("Arear", Schema = "Payroll")]
    public class Arear : BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
    {
        [ForeignKey(nameof(OrganisationChart))]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey(nameof(Employee))]
        public long EmployeeId { get; set; }
        public virtual Employee.Core.Entities.Employee? Employee { get; set; }

        [ForeignKey(nameof(PersonnelFunction))]
        public long? PersonnelFunctionId { get; set; }
        public virtual PersonnelFunction? PersonnelFunction { get; set; }

        [ForeignKey(nameof(InterdictOrder))]
        public long? InterdictOrderId { get; set; }
        public virtual InterdictOrder? InterdictOrder { get; set; }

        /// <summary>
        /// وضعیت معوقه
        /// </summary>
        [ForeignKey(nameof(ArearsStatus))]
        [Comment("وضعیت معوقه")]
        public long ArearsStatusId { get; set; }
        public virtual ArearsStatus? ArearsStatus { get; set; }

        /// <summary>
        /// دوره حقوقی زمان تایید حکم (مبنا برای یافتن دوره‌های بسته قبلی)
        /// </summary>
        [ForeignKey(nameof(ApproveTimePaymentPeriod))]
        [Comment("دوره حقوقی زمان تایید حکم")]
        public long? ApproveTimePaymentPeriodId { get; set; }
        public virtual PaymentPeriod? ApproveTimePaymentPeriod { get; set; }

        /// <summary>
        /// دوره ای که قصد پرداخت معوقه در آن وجود دارد
        /// </summary>
        [ForeignKey(nameof(PaymentPeriodIntendToPay))]
        [Comment("دوره قصد پرداخت معوقه")]
        public long? PaymentPeriodIntendToPayId { get; set; }
        public virtual PaymentPeriod? PaymentPeriodIntendToPay { get; set; }

        /// <summary>
        /// جمع تفاوت اقلام (مبلغ جدید − مبلغ فیش عادی)
        /// </summary>
        [Comment("جمع تفاوت اقلام تغییر یافته")]
        public long TotalDifferenceAmount { get; set; }

        /// <summary>
        /// خالص تفاوت قابل پرداخت معوقه
        /// </summary>
        [Comment("خالص تفاوت قابل پرداخت")]
        public long PayableDifferenceAmount { get; set; }

        /// <summary>
        /// تعداد فیش معوقه تولید شده
        /// </summary>
        [Comment("تعداد فیش معوقه")]
        public int ArearFicheCount { get; set; }

        /// <summary>
        /// تعداد اقلام تغییر یافته
        /// </summary>
        [Comment("تعداد اقلام تغییر یافته")]
        public int ChangedItemCount { get; set; }

        /// <summary>
        /// زمان محاسبه موفق
        /// </summary>
        [Comment("زمان محاسبه")]
        [Column(TypeName = "datetime")]
        public DateTime? CalculatedDate { get; set; }

        /// <summary>
        /// آخرین پیام خطا / علت رد محاسبه (برای نمایش به کاربر)
        /// </summary>
        [StringLength(2000)]
        [Comment("آخرین پیام خطا یا علت رد")]
        public string? LastErrorMessage { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        [StringLength(1500)]
        public string? Description { get; set; }

        public virtual ICollection<ArearFiche>? ArearFiches { get; set; }
        public virtual ICollection<DeductedArears>? DeductedArears { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
