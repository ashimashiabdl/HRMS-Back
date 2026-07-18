using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Fiche_Leave_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Fiche_Leave_Item_FicheId")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Fiche_Leave_Item_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelLeaveId", Name = "IX_Fiche_Leave_Item_PersonnelLeaveId")]
public partial class FicheLeaveItem
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public long LeaveTypeId { get; set; }

    public long PersonnelLeaveId { get; set; }

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

    public long LeaveAmountTicks { get; set; }

    public long LeaveBalanceTicks { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("FicheId")]
    [InverseProperty("FicheLeaveItems")]
    public virtual Fiche Fiche { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("FicheLeaveItems")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [ForeignKey("PersonnelLeaveId")]
    [InverseProperty("FicheLeaveItems")]
    public virtual PersonnelLeave PersonnelLeave { get; set; } = null!;
}
