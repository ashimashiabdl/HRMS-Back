using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeCoefficientDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public long CoefficientId { get; set; }
        public string? Coefficient { get; set; }
        public long EnterTypeId { get; set; }
        public string? EnterTypeTitle { get; set; }
        public long? CheckingTimeId { get; set; }
        public long? LastInterdictId { get; set; }
        public Nullable<long> OrganisationFormulaId { get; set; }
        public string? Formula { get; set; }
        public string? CheckFormula { get; set; }
        public Nullable<long> OrganisationCheckFormulaId { get; set; }
        public Nullable<int> FixValue { get; set; }
        public Nullable<int> Min { get; set; }
        public Nullable<int> Max { get; set; }
        public long? HelperTableId { get; set; }
        public bool IsEditable { get; set; }
        public bool HideInOrderPrint { get; set; }
        public bool IsDefault { get; set; }
        public bool IsBatch { get; set; }
        public bool? BuildTreeTrace { get; set; }
        public double? OutPutFactValue { get; set; }
        public double? NewOutPutFactValue { get; set; }
        public bool IsRowSuccess { get; set; }
        public string? formularowmessage { get; set; }
        public string? FormulaFriendlyText { get; set; }
        public string? FormulaText { get; set; }
        public string? FormulaHelpDesc { get; set; }
        public string? CheckFormulaHelpDesc { get; set; }
        public string? CheckFormulaText { get; set; }
        public string? CheckFormulaFriendlyText { get; set; }
        public Dictionary<string, string?>? VariableFriendlyList { get; set; }
        public Dictionary<string, string?>? CheckFormulaVariableFriendlyList { get; set; }
        public int SuccessRunTimeInmilliseconds { get; set; }
        public string? FormulaErrorMessage { get; set; }
        public string? CheckFormulaErrorMessage { get; set; }
        public string? Description { get; set; }

        public int? Priority { get; set; }
        public FormulaExecutionTree? FormulaTreeParser { get; set; }
        public FormulaExecutionTree? CheckFormulaTreeParser { get; set; }
    }
}
