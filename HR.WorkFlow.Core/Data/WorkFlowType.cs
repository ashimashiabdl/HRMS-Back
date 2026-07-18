using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.Data
{
    [Table("WorkFlowType", Schema = "wf")]
    public class WorkFlowType : BaseEntity
    {
    }
}
