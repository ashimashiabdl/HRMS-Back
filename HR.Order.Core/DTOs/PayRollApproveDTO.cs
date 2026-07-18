using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Core.DTOs;

public class PayRollApproveDTO
{
    public long InterdictId { set; get; }
    [Required]
    public DateTime RealExecuteDate { get; set; }

    public List<long>? InterdictIdList { get; set; }
}
