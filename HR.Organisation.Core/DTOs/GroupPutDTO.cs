using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class GroupPutDTO
    {

        public List<JobScoreAbundanceComplexityDTO>? DetailList { get; set; }
        public long? PrimaryComplexityId { get; set; }
        public long? PrimaryAbundanceId { get; set; }

    }
}
