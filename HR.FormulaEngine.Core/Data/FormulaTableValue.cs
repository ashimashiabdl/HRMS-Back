using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.Data
{
    [Table("Formula_Table_Value", Schema = "For")]
    public class FormulaTableValue : BaseEntity
    {
        [ForeignKey("FormulaTable")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long FormulaTableId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual FormulaTable? FormulaTable { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? FromValue1 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? ToValue1 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? FromValue2 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? ToValue2 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? FromValue3 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? ToValue3 { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public decimal? DiscreteValue { get; set; }
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public decimal? Resultvalue { get; set; }
        [StringLength(512)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Description { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public int? Year { get; set; }
        [NotMapped()]
        private new string title { get; set; }
    }
}