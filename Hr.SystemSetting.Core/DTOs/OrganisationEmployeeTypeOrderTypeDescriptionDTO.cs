using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeDescriptionDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public bool IsDefault { get; set; }
        public string? Description { get; set; }
    }
}
