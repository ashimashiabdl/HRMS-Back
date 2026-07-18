using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class JobScoringFactorDTO : BaseDTO
    {
        public long GroupId { get; set; }
        public string? Group { get; set; }
        public int Percent { get; set; }
        public int MaximumScore { get; set; }
        public int Priority { get; set; }
    }
}
