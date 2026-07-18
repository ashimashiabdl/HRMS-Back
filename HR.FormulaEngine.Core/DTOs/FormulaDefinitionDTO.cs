using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaDefinitionDTO : BaseDTO
    {
        public string? Description { get; set; }
        public string? OrganisationFormula { get; set; }
        [Required]
        public string? FormulaText { get; set; }
        public int? Version { get; set; }
        //public int? Priority { get; set; }
        [StringLength(256, ErrorMessage = "طول فیلد علت تغییر فرمول می تواند حد اکثر 256 کاراکتر باشد")]
        public string? LastChangeReason { get; set; }
        //[StringLength(256)]
        //public string? Code { get; set; }
        [StringLength(256)]
        [Required]
        public string? ErrorMessage { get; set; }
    }
}
