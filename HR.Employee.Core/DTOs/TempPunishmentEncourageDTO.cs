using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class TempPunishmentEncourageDTO : BaseDTO
    {
        public long? EmployeeId { get; set; }
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(40)]
        public string? FatherName { get; set; }
        [StringLength(50)]
        public string? PersonelCode { get; set; }
        [StringLength(10)]
        public string? NationalNo { get; set; }
        /// <summary>
        /// مقدار واحد تنبیه یا تشویق
        /// </summary>
        public int UnitValue { get; set; }
        public int? Value { get; set; }
        public long? TempFileId { get; set; }
    }
}
