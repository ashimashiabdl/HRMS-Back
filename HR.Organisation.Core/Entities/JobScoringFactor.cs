using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    [Table("Job_Scoring_Factor", Schema = "Org")]
    public class JobScoringFactor : HR.SharedKernel.Data.BaseEntity
    {
        public virtual BaseTableValue? Group { get; set; }
        public int Percent { get; set; }
        public int MaximumScore { get; set; }
        public int Priority { get; set; }
        public long GroupId { get; set; }
    }
}
