using HR.FormulaEngine.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaTableValueDTO : BaseDTO
    {
        public long FormulaTableId { get; set; }
        public string? FormulaTable { get; set; }
        public long TableTypeId { get; set; }
        public string? TableType { get; set; }
        public decimal? FromValue1 { get; set; }
        public decimal? ToValue1 { get; set; }
        public decimal? FromValue2 { get; set; }
        public decimal? ToValue2 { get; set; }
        public decimal? FromValue3 { get; set; }
        public decimal? ToValue3 { get; set; }
        public decimal? DiscreteValue { get; set; }
        public decimal? Resultvalue { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
        public int? Year { get; set; }
    }
}
