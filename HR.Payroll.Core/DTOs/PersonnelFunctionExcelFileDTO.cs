using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelFunctionExcelFileDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public string? Extension { get; set; }
        public Guid? UniqueId { get; set; }
        public long Size { get; set; }

        public string? AspNetUsers { get; set; }

        /// <summary>
        /// محتوای فایل - فقط برای آپلود و دانلود استفاده می‌شود، در فهرست نمایش داده نمی‌شود
        /// </summary>
        public byte[]? Content { get; set; }
        
        [StringLength(512)]
        public string? MimeType { get; set; }
        
        /// <summary>
        /// نام فایل
        /// </summary>
        [StringLength(500)]
        public string? FileName { get; set; }
        
        /// <summary>
        /// تاریخ آپلود
        /// </summary>
        public DateTime? UploadDate { get; set; }
        
        /// <summary>
        /// کاربر آپلود کننده
        /// </summary>
        public string? UploadedBy { get; set; }
        
        /// <summary>
        /// شناسه دوره پرداخت
        /// </summary>
        public long PaymentPeriodId { get; set; }
        
        /// <summary>
        /// عنوان دوره پرداخت
        /// </summary>
        public string? PaymentPeriod { get; set; }
        
        /// <summary>
        /// شناسه نوع استخدام
        /// </summary>
        public long EmployeeTypeId { get; set; }
        
        /// <summary>
        /// عنوان نوع استخدام
        /// </summary>
        public string? EmployeeType { get; set; }
    }
}

