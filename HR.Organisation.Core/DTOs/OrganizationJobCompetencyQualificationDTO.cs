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
    public class OrganizationJobCompetencyQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }

        public long? CompetencyTypeId { get; set; }
        public string? CompetencyType { get; set; }

        public long? CompetencyLevelId { get; set; }
        public string? CompetencyLevel { get; set; }
    }
}
