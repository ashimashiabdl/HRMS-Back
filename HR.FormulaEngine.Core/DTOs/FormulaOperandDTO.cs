using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaOperandDTO : BaseDTO
    {
        [StringLength(256)]
        public string? EnglishName { get; set; }
        [StringLength(256)]
        public string? Datatype { get; set; }
    }
}
