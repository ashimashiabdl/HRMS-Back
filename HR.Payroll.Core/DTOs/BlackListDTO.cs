using HR.Payroll.Core.Data;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.DTOs
{
    public class BlackListDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long BlackListEnumerationId { get; set; }
        public string? BlackListEnumeration { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        [StringLength(128)]
        public string? Comment { get; set; }
        public bool WillBeCalculated { get; set; }
        [StringLength(512)]
        public string? Description { get; set; }
    }
}
