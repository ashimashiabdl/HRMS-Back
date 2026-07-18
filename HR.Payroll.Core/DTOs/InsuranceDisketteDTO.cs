using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class InsuranceDisketteDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        public long InsuranceBranchId { get; set; }
        public string? InsuranceBranch { get; set; }
        public long InsuranceDisketteStatusId { get; set; }
        public string? InsuranceDisketteStatus { get; set; }
        public long? BatchPayRollRequestId { get; set; }
        /// <summary>
        /// فهرست مراکز هزینه
        /// </summary>
        public List<long>? CostCenterIdList { get; set; }
        public string? BatchPayRollRequest { get; set; }
        public long ReportTypeId { get; set; }
        public string? ReportType { get; set; }
        public long? PeymanRowId { get; set; }
        public string? PeymanRow { get; set; }
    
        /// <summary>
        /// تعداد کارکنان
        /// </summary>
        public int DSK_NUM { get; set; }
        /// <summary>
        /// TBIME_DSK -- مجموع حق بیمه سهم بیمه شده
        /// </summary>
        public long DSK_TBIME { get; set; }
        /// <summary>
        /// مجموع بیمه شده سهم کارفرما
        /// </summary>
        public long DSK_TKOSO { get; set; }
        /// <summary>
        /// تعداد روز های کارکرد - سرجمع
        /// </summary>
        public long DSK_TDD { get; set; }
        /// <summary>
        /// کد کارگاه
        /// </summary>
        [StringLength(10)]
        public string? DSK_ID { get; set; }
        /// <summary>
        /// نام کارگاه
        /// </summary>
        [StringLength(100)]
        public string? DSK_NAME { get; set; }
        /// <summary>
        /// نام کارفرما
        /// </summary>
        [StringLength(100)]
        public string? DSK_FARM { get; set; }
        /// <summary>
        /// آدرس کارگاه
        /// </summary>
        [StringLength(100)]
        public string? DSK_ADRS { get; set; }
        /// <summary>
        /// شماره فهرست
        /// </summary>
        [StringLength(12)]
        public string? DSK_LISTNO { get; set; }
        /// <summary>
        /// شرح فهرست
        /// </summary>
        [StringLength(100)]
        public string? DSK_DISC { get; set; }
        /// <summary>
        /// ردیف پیمان
        /// </summary>
        [StringLength(3)]
        public string? MON_PYM { get; set; }
        /// <summary>
        /// نوع فهرست - 0 رد می شود
        /// </summary>
        public int DSK_KIND { get; set; }
        /// <summary>
        /// سال عملکرد - 2 رقمی
        /// </summary>
        public int DSK_YY { get; set; }
        /// <summary>
        /// ماه عملکرد
        /// </summary>
        public int DSK_MM { get; set; }
        /// <summary>
        /// مجموع دستمزد روزانه
        /// </summary>
        public long DSK_TROOZ { get; set; }
        /// <summary>
        /// مجموع دستمزد ماهانه
        /// </summary>
        public long DSK_TMAH { get; set; }
        /// <summary>
        /// مجموع مزایای ماهانه مشمول
        /// </summary>
        public long DSK_TMAZ { get; set; }
        /// <summary>
        /// مجموع دستمزد و مزایای ماهانه مشمول
        /// </summary>
        public long DSK_TMASH { get; set; }
        /// <summary>
        /// مجموع کل دستمزد و مزایای ماهانه ( مشمول و غیر مشمول 
        /// </summary>
        public long DSK_TTOTL { get; set; }
        /// <summary>
        /// مجموع حق بیمه بیکاری
        /// </summary>
        public long DSK_BIC { get; set; }
        /// <summary>
        /// نرخ حق بیمه
        /// </summary>
        public int DSK_RATE { get; set; }
        /// <summary>
        /// نرخ پورسانتاژ
        /// </summary>
        public long DSK_PRATE { get; set; }
        /// <summary>
        /// نرخ مشاغل سخت و زیان آور
        /// </summary>
        public long DSK_BIMH { get; set; }
        /// <summary>
        /// پایه سنوات
        /// </summary>
        public long DSK_INC { get; set; }
        /// <summary>
        /// حق تاهل در فیش
        /// </summary>
        public long DSK_SPOUSE { get; set; }
    }
}
