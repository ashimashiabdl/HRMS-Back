using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.DTOs
{
    public class FormulaExecutionTree
    {
        public string? FormulaFriendlyText { get; set; }
        public string? FormulaText { get; set; }
        public string? FormulaName { get; set; }
        public string? FormulaHelpDesc { get; set; }
        public double Result { get; set; }
        public int SuccessRunTimeInmilliseconds { get; set; }
        public List<FormulaExecutionTree>? ChildList { get; set; }
        public string? DebugLog { get; set; }
    }
}
