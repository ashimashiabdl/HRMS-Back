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
    [Table("Personnel_Function_Detail", Schema = "Payroll")]
    public class PersonnelFunctionDetail : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }

        [ForeignKey("PersonnelFunction")]
        public long PersonnelFunctionId { get; set; }
        public virtual PersonnelFunction? PersonnelFunction { get; set; }

        [ForeignKey("OrganisationPosition")]
        public long? OrganisationPositionId { get; set; }
        public virtual OrganisationPosition? OrganisationPosition { get; set; }

    }
}
