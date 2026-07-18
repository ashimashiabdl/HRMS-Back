using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class BlockedAccountDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        public long EmployeeId { get; set; }
        public string? Employee { get; set; }
        [StringLength(64)]
        public string? AccountNo { get; set; }
        [StringLength(512)]
        public string? Comment { get; set; }
    }
}
