using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class ConsequenceRespone
    {
        public List<OrganisationEmployeeTypeOrderTypeWageItemDTO>? OrderWageSettingList { set; get; }
        public List<CheckFormula>? CheckFormulas { set; get; }
        public List<OrganisationEmployeeTypeOrderTypeCoefficientDTO>? OrderCoefficientSettingList { set; get; }
        public List<OrganisationEmployeeTypeOrderTypeDescriptionDTO>? OrganisationEmployeeTypeOrderTypeDescriptionList { set; get; }
        public OrganisationEmployeeTypeOrderTypeCanChange? CanChangeDTO { set; get; }
        /// <summary>
        /// True: فیلتر پست بر اساس واحد سازمانی (RelatedNodeId)؛
        /// False: همه رکوردهای جدول OrganisationPosition.
        /// در هر دو حالت منبع داده جدول پست است.
        /// </summary>
        public bool SelectPostFromChart { set; get; } = false;
        public VwInterdict_Order? VwInterdict_Order { set; get; }
        public InterdictOrderFlatDTO? InterdictOrderDTO { set; get; }
        public bool Permission { get; set; }
        public bool CalculateSummary { get; set; }
        public bool IsOutDate { get; set; }
        public long? OrderLevelId { get; set; }
        public long? lastorderId { get; set; }
        public long? latorderSerial { get; set; }
        public bool SettingHasEqulToInput { get; set; }
        public List<OrderComparitivePropertiesDTO>? OrderComparitiveProperties { get; set; }
        // When BuildTreeTrace is true on the request, backend fills this with step-by-step progress logs
        public bool TraceEnabled { get; set; }
        public List<string>? ProgressLogs { get; set; }
    }

    /// <summary>
    /// نتیجه اجرای فرمول چک
    /// </summary>
    public class CheckFormula
    {
        public bool IsWarning { get; set; }
        public string? FormulaName { get; set; }
        public string? RelatedErrorMessage { get; set; }
        public string? FormulaFriendlyText { get; set; }
        public string? FormulaText { get; set; }
        public string? FormulaHelpDesc { get; set; }
        public Dictionary<string, string?>? VariableFriendlyList { get; set; }
        public bool Result { get; set; }
        public int ExecutionTimeInMs { get; set; }
        public FormulaExecutionTree? FormulaTreeParser { get; set; }
        public int SuccessRunTimeInmilliseconds { get; set; }
    }
}
