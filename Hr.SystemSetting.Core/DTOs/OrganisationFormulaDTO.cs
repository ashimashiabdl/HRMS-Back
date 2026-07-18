using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationFormulaDTO : BaseDTO
    {
        public long FormulaId { get; set; }
        public string? FormulaTitle { get; set; }

        [Required]
        public long FormulaUsageLocationId { get; set; }
        public string? FormulaUsageLocationTitle { get; set; }

        public bool HasFormulaDefinition { get; set; }

        public int? FormulaDefinitionVersion { get; set; }

        public string? FormulaDefinitionLastEditor { get; set; }
    }
}
