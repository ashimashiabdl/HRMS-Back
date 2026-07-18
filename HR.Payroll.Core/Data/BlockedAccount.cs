using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Blocked_Account", Schema = "Payroll")]
    public class BlockedAccount : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        [StringLength(64)]
        public string? AccountNo { get; set; }     
        [StringLength(512)]
        public string? Comment { get; set; }
        [NotMapped]
        private new string title { get; set; }

    }
}
