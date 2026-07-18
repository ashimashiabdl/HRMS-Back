using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobEducationFieldQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }

        public long? EducationFieldId { get; set; }
        public string? EducationField { get; set; }
    }
}
