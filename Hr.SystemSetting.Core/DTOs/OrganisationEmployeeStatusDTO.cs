using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeStatusDTO : BaseDTO
    {
        public long EmployeeStatusId { get; set; }
        public long EmployeeStatusGroupId { get; set; }
        public string? EmployeeStatusTitle { get; set; }
        public string? EmployeeStatusGroupTitle { get; set; }
        public bool? IsEmployed { get; set; }

    }
}
