using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BankDisketteTemplateDTO : BaseDTO
    {
        [StringLength(1024)]
        public string? FileName { get; set; }
        [StringLength(64)]
        public string? FileExtension { get; set; }
        [StringLength(8096)]
        public string? FileHeader { get; set; }
        [StringLength(8096)]
        public string? FileEnd { get; set; }
        public bool HasLineStartCharacter { get; set; }
        public string? LineStartCharacter { get; set; }
        public bool HasLineEndCharacter { get; set; }
        public string? LineEndCharacter { get; set; }
        public bool HasLineDelimiterCharacter { get; set; }
        public string? LineDelimiterCharacter { get; set; }
        public long BankId { get; set; }
        public string? Bank { get; set; }
    }
}
