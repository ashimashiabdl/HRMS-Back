using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class GetCurrentUserAccess_Result
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RelatedTreeNodeId { get; set; }
    }
}
