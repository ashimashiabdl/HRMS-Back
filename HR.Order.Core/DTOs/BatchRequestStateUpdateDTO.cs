using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs
{
    public class BatchRequestStateUpdateDTO
    {
        public long Id { get; set; }
        public long NewStateId { get; set; }
    }
}
