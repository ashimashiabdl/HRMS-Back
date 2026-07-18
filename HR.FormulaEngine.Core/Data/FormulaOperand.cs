using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.FormulaEngine.Core.Data
{
    [Table("Formula_Operand", Schema = "For")]
    public class FormulaOperand : SharedKernel.Data.BaseEntity 
    {
        [StringLength(256)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? EnglishName { get; set; }    
        [StringLength(256)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? Datatype { get; set; }


    }
}
