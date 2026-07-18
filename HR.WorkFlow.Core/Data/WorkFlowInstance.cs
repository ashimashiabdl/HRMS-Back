using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Core.Data;

[Table("WorkFlow_Instance", Schema = "wf")]
public class WorkFlowInstance : BaseEntity
{
    [ForeignKey("WorkFlow")]
    public long WorkFlowId { get; set; }
    public virtual WorkFlow? WorkFlow { get; set; }
    [ForeignKey("InterdictOrder")]
    public long? InterdictOrderId { get; set; }
    public virtual InterdictOrder? InterdictOrder { get; set; }

    public long? EmployeeSettlementId { get; set; }
    public long LastActivityId { get; set; }
    [StringLength(64)]
    public string? CreateBy { get; set; } = null!;
    public string? FormulaData { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
