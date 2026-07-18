using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class UserDefaultSettingDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string? User { get; set; }
        public long? DefaultOrganId { get; set; }
        public string? DefaultOrganTitle { get; set; }
        public long? DefaultWorkPlaceId { get; set; }
        public string? DefaultWorkPlaceTitle { get; set; }
        public long? DefaultCostCenterId { get; set; }
        public string? DefaultCostCenterTitle { get; set; }
        public long? DefaultOrganizationUnitId { get; set; }
        public string? DefaultOrganizationUnitTitle { get; set; }
        public long? DefaultPaymentPeriodId { get; set; }
        public string? DefaultPaymentPeriodTitle { get; set; }
    }
}
