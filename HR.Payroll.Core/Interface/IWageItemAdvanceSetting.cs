using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Interface
{
    public interface IWageItemAdvanceSetting
    {
        public bool IsHouseWageItemForTax { get; set; }
        public bool IsCarWageItemForTax { get; set; }
        public bool IsInsuranceWageItemForTax { get; set; }
        public bool IsMedicalExpensesArticle137WageItemForTax { get; set; }
        public bool IsCaseBonusWageItemForTax { get; set; }

    }
}
