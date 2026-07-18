using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class JobScoreAbundanceComplexityDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long? JobScoringFactorId { get; set; }
        public string? JobScoringFactor { get; set; }
        public string? jobScoringFactorGroup { get; set; }
        public short AbundanceLevel { get; set; }
        public long? AbundanceId { get; set; }
        public string? Abundance { get; set; }
        public long? ComplexityId { get; set; }
        public string? Complexity { get; set; }
        public short? ComplexityLevel { get; set; }
        public int? Score { get; set; }
        public bool? Selected { get; set; }
        public bool? SelectedFromQuestion { get; set; }

        public bool? IsForQuestion { get; set; }
    }
}
