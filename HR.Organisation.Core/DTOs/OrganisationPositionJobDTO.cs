using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationPositionJobDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
        
        public long OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }

        public long OrganisationPositionId { get; set; }
        public string? OrganisationPosition { get; set; }
    }
}
