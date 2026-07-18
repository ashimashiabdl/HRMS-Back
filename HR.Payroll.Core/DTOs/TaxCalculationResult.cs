using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class TaxCalculationResult : BaseResult
    {
        public double TaxCoveredSum { get; set; }
        public double CurrentTax { get; set; }
        public long RelatedTaxWageItemId { get; set; }

        /// <summary>
        /// جمع مزایای مستمر نقدی و مشمول مالیات
        /// </summary>
        public long? SumCashTaxCoveredAndCountinious { get; set; }
        /// <summary>
        /// جمع مزایای مستمر غیر نقدی و مشمول مالیات
        /// </summary>
        public long? SumNonCashTaxCoveredAndCountinious { get; set; }
        /// <summary>
        /// جمع مزایای غیر مستمر غیر نقدی و مشمول مالیات
        /// </summary>
        public long? SumNonCashTaxCoveredAndNotCountinious { get; set; }
        /// <summary>
        /// جمع مزایای غیر مستمر نقدی و مشمول مالیات
        /// </summary>
        public long? SumCashTaxCoveredAndNotCountinious { get; set; }
    }
}
