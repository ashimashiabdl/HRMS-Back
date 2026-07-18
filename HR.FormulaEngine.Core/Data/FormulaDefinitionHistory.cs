using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.FormulaEngine.Core.Data
{
    [Table("Formula_Definition_History", Schema = "For")]
    public class FormulaDefinitionHistory : BaseEntity, IignoreDateRangeValidation
    {
        [ForeignKey("FormulaDefinition")]
        public long FormulaDefinitionId { get; set; }
        public virtual FormulaDefinition? FormulaDefinition { get; set; }

        public string? PreviousFormulaText { get; set; }
        public string? IPAddress { get; set; }
        public long? UserId { get; set; }
        public string? UserFullName { get; set; }
        public DateTime ChangeDateTime { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}


