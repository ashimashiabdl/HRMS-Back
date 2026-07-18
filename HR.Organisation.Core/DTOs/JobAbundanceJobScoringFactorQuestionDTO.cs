using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.DTOs
{
    public class JobAbundanceJobScoringFactorQuestionDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? JobScoringFactorId { get; set; }
        public string? JobScoringFactor { get; set; }
        public long? AbundanceId { get; set; }
        public string? Abundance { get; set; }
        [StringLength(2048)]
        public string? Question { get; set; }
    }
}
