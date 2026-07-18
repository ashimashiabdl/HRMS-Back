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
    public class OrganizationJobAbilityQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public  string? OrganizationJob { get; set; }
        
        public long? AbilityTypeId { get; set; }
        public string? AbilityType { get; set; }

        public long LevelTypeId { get; set; }
        public string? LevelType { get; set; }
    }
}
