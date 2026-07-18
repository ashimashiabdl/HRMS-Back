using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeWageItemDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long WageItemId { get; set; }
        public string? WageItemTitle { get; set; }
        public int? Priority { get; set; }
        public bool? HideInOrder { get; set; }
        public bool? IsDaily { get; set; }
        public bool? IsDailyAndWage { get; set; }
        public bool? IsSanavatINC { get; set; }
        public bool? IsSpouse { get; set; }
    }
}
