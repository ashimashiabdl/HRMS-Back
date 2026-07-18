using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_Log", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Batch_Log_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Batch_Log_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Batch_Log_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionId", Name = "IX_Batch_Log_PersonnelFunctionId")]
public partial class BatchLog
{
    [Key]
    public long Id { get; set; }

    public string? LogDescription { get; set; }

    public string? ServiceName { get; set; }

    public int LogTypeId { get; set; }

    public long? InterdictOrderId { get; set; }

    public long? PersonnelFunctionId { get; set; }

    public long? EmployeeId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long? PaymentPeriodId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("BatchLogs")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("BatchLogs")]
    public virtual InterdictOrder? InterdictOrder { get; set; }

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("BatchLogs")]
    public virtual PaymentPeriod? PaymentPeriod { get; set; }
}
