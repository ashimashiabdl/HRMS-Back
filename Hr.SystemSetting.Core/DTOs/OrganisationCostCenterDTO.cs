using Hr.SystemSetting.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationCostCenterDTO : BaseDTO
    {
        public long CostCenterId { get; set; }
        public string? CostCenterTitle { get; set; }
        public long? PeymanRowId { get; set; }
        public string? PeymanRow { get; set; }
        public int CostCenterPercent { get; set; }
        /// <summary>
        /// کد تفضیل مرکز هزینه
        /// </summary>
        public string? CostCenterFinancialCode { get; set; }

    }
}
