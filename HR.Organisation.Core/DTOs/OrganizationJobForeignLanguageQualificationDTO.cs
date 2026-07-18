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
    public class OrganizationJobForeignLanguageQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }

        public long LanguageTypeId { get; set; }
        public string? LanguageType { get; set; }
        
        public long LanguageLevelTypeId { get; set; }
        public string? LanguageLevelType { get; set; }
        public long LanguageSkillTypeId { get; set; }
        public string? LanguageSkillType { get; set; }
    }
}
