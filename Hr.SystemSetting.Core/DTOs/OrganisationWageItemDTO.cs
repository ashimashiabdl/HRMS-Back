using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationWageItemDTO : BaseDTO
    {
        public long WageItemId { get; set; }
        public string? WageItemTitle { get; set; }
        public long? MappedExcelColumnId { get; set; }
        public string? MappedExcelColumnTitle { get; set; }
    }
}
