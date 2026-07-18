using HR.Payroll.Core.Data;
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
    public class BankDisketteTemplateRowDTO : BaseDTO
    {
        public long BankDisketteTemplateId { get; set; }
        public string? BankDisketteTemplate { get; set; }
        public long DisketteItemTypeId { get; set; }
        public string? DisketteItemType { get; set; }
        public int Length { get; set; }
        public string? PadLeftCharacter { get; set; }
        public string? StaticText { get; set; }
        public int Priority { get; set; }
    }
}
