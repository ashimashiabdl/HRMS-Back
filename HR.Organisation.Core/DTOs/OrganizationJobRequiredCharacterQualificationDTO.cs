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
    public class OrganizationJobRequiredCharacterQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long CharacterTypeId { get; set; }
        public string? CharacterType { get; set; }
        public long RequiredLevelId { get; set; }
        public string? RequiredLevel { get; set; }
    }
}
