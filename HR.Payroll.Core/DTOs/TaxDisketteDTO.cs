using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.DTOs
{
    public class TaxDisketteDTO : BaseDTO
    {
        /// <summary>
        /// محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود
        /// </summary>
        [Comment("محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود")]
        public bool CalculateAllFichesInCurrentPeriod { get; set; }
        /// <summary>
        /// فهرست مراکز هزینه
        /// </summary>
        public List<long>? CostCenterIdList { get; set; }
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long? PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        public int? CodeList { get; set; }
        public long TaxDisketteStatusId { get; set; }
        public string? TaxDisketteStatus { get; set; }
        public long? BatchPayRollRequestId { get; set; }
        public string? BatchPayRollRequest { get; set; }
        #region TaxDisketteWk
        /// <summary>
        /// 3- بدهی مالیاتی ماه جاری
        /// </summary>
        public long CurrentMonthDebtAmount { get; set; }
        /// <summary>
        /// بدهی مالیاتی ماه گذشته
        /// </summary>
        public long LastMonthDebtAmount { get; set; }

        /// <summary>
        ///  تاریخ ثبت در دفتر روزنامه ( تخصیص / پرداخت )
        /// </summary>

        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// شماره حساب
        /// </summary>
        [StringLength(29)]
        public string? BankAccountNo { get; set; }

        public long? PaymentTypeId { get; set; }
        public string? PaymentType { get; set; }
        /// <summary>
        /// شماره سریال چک
        /// </summary>
        [StringLength(6)]
        public string? ChequeNo { get; set; }
        /// <summary>
        /// تاریخ چک
        /// </summary>

        public DateTime? ChequeDate { get; set; }

        /// <summary>
        /// شعبه بانک
        /// </summary>
   
        public long? BankBranchId { get; set; }
        public string? BankBranch { get; set; }
        public string? BankBranchCode { get; set; }

        /// <summary>
        /// مبلغ پرداختی یا مبلغ چک
        /// </summary>
        public long ChequeAmount { get; set; }

        /// <summary>
        /// تعداد افراد موجود در دیسکت
        /// </summary>
        public int PersonnelCount { get; set; }

        /// <summary>
        /// تعداد اتباع خارجی
        /// </summary>
        public int ForeignerPersonnelCount { get; set; }

        /// <summary>
        /// مبلغ پرداختی خزانه
        /// </summary>
        public long TreasuryPaymentAmount { get; set; }
        /// <summary>
        /// تاریخ پرداخت خزانه
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? TreasuryPaymentDate { get; set; }
        #endregion TaxDisketteWk





    }
}