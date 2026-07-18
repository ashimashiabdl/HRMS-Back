using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.DTOs
{
    public class EducationGradeDTO : BaseDTO
    {
        /// <summary>
        /// کد متناظر در جدول پایه مالیاتی
        /// </summary>
        [StringLength(1)]
        public string? TaxCode { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
