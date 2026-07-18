using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationSettingDTO : BaseDTO
    {
        public long SettingId { get; set; }
        public string? SettingTitle { get; set; }
        public bool IsActive { get; set; }
        public string? Value { get; set; }
    }
}
