using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.DTOs
{
    public class G20ScoreDomainJobDegreeDTO : BaseDTO
    {
        public short JobDegree { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
        public float AmountOfRankIncrease { get; set; }
    }
}
