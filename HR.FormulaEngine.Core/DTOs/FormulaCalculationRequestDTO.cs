using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaCalculationRequestDTO
    {
        public bool? BuildTreeTrace { get; set; }
        public long OrganisationFormulaId { get; set; }
        public DateTime? StartDate { get; set; }
        public Dictionary<string,double>? VariableList { get; set; }
        public Dictionary<string, string?>? VariableFriendlyList { get; set; }
    }
}
