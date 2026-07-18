using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Core.DTOs
{
    public class FormulaCalculateResponseDTO
    {
        public FormulaExecutionTree? FormulaTreeParser { get; set; }
        public bool Succees { get; set; }
        public string? ResponseMessage { get; set; }
        public string? FormulaText { get; set; }
        public string? FormulaHelpDesc { get; set; }
        public string? FormulaFriendlyText { get; set; }
        public double Result { get; set; }
        public int SuccessRunTimeInmilliseconds { get; set; }
        public Dictionary<string, string?>? VariableFriendlyList { get; set; }
        public string? DebugLog { get; set; }
        
    }



}
