using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Settlement_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeSettlementId", Name = "IX_Employee_Settlement_Item_EmployeeSettlementId")]
[Microsoft.EntityFrameworkCore.Index("SettlementItemId", Name = "IX_Employee_Settlement_Item_SettlementItemId")]
public partial class EmployeeSettlementItem
{
    [Key]
    public long Id { get; set; }

    public long EmployeeSettlementId { get; set; }

    [StringLength(6)]
    public string? Duration { get; set; }

    public long Amount { get; set; }

    public long SystemCalculatedAmount { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

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

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    public long SettlementItemId { get; set; }

    [ForeignKey("EmployeeSettlementId")]
    [InverseProperty("EmployeeSettlementItems")]
    public virtual EmployeeSettlement EmployeeSettlement { get; set; } = null!;

    [ForeignKey("SettlementItemId")]
    [InverseProperty("EmployeeSettlementItems")]
    public virtual SettlementItem SettlementItem { get; set; } = null!;
}
