using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Settlement_Cause", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Settlement_Cause_title", IsUnique = true)]
public partial class SettlementCause
{
    [Key]
    public long Id { get; set; }

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

    [InverseProperty("SettlementCause")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlements { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("SettlementCause")]
    public virtual ICollection<OrganisationSettlementCause> OrganisationSettlementCauses { get; set; } = new List<OrganisationSettlementCause>();
}
