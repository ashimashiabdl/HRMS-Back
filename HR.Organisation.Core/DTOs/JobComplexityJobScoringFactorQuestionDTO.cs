using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class JobComplexityJobScoringFactorQuestionDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? JobScoringFactorId { get; set; }
        public string? JobScoringFactor { get; set; }
        public string? JobScoringFactorGroup { get; set; }
        public long? ComplexityId { get; set; }
        public string? Complexity { get; set; }
        public string? Question { get; set; }
    }
}
