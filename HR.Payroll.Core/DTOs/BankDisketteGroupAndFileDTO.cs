using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BankDisketteGroupAndFileDTO : BaseDTO
    {
        public long BankDisketteId { get; set; }
        public long BankDisketteTemplateId { get; set; }
        public string? BankDisketteTemplate { get; set; }
        [StringLength(64)]
        public string? Extension { get; set; }
        [StringLength(64)]
        public string? FileName { get; set; }
        /// <summary>
        /// تعداد افراد این بانک خاص
        /// </summary>
        public int PersonCount { get; set; }
        /// <summary>
        /// جمع مبلغ این بانک خاص
        /// </summary>
        public long SumAmount { get; set; }
    }
}
