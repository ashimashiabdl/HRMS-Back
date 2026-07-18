using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeCheckDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public long CheckTypeId { get; set; }
        public string? CheckTypeTitle { get; set; }
        public Nullable<long> OrganisationFormulaId { get; set; }
        public string? OrganisationFormulaTitle { get; set; }
        public string? FailMessage { get; set; }
        public string? Description { get; set; }
    }
}
