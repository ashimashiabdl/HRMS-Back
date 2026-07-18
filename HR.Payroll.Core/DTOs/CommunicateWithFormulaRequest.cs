using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class CommunicateWithFormulaRequest
    {
        public bool? BuildTreeTrace { get; set; }
        public bool? DoFinalCalc { get; set; }
        public long? lastorderId { get; set; }
        public List<InterdictOrderCoefficientItem>? InterdictOrderCoefficientItems { get; set; }
        public List<InterdictOrderWageItem>? InterdictOrderWageItems { get; set; }
        public List<FicheItem>? FicheItems { get; set; }
        public List<SettlementRuntimeItem>? SettlementItems { get; set; }
        public InterdictOrder? InterdictOrder { get; set; }
        public RecruitOrder? RecruitOrder { get; set; }
        public PaymentPeriod? PaymentPeriod { get; set; }
        public PersonnelFunction? PersonnelFunction { get; set; }
        public bool MyProperty { get; set; }
        public Dictionary<string, double>? VariableList { get; set; }

        /// <summary>
        /// تاریخ اجرای فرمول برای اعتبارسنجی عملوندها/توابع — در تسویه از تاریخ بازه تسویه استفاده می‌شود، نه دوره فیش.
        /// </summary>
        public DateTime? FormulaImpleDate { get; set; }

    }
}
