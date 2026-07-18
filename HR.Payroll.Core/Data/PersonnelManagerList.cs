using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Personnel_ManagerList", Schema = "Payroll")]
    public class PersonnelManagerList : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        public long MaxAmount { get; set; }
    }
}
