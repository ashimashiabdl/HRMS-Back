using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Settlement_Item", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Settlement_Item_title", IsUnique = true)]
public partial class SettlementItem
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

    [InverseProperty("SettlementItem")]
    public virtual ICollection<DeductionType> DeductionTypes { get; set; } = new List<DeductionType>();

    [InverseProperty("SettlementItem")]
    public virtual ICollection<EmployeeSettlementItem> EmployeeSettlementItems { get; set; } = new List<EmployeeSettlementItem>();

    [InverseProperty("SettlementItem")]
    public virtual ICollection<LoanType> LoanTypes { get; set; } = new List<LoanType>();

    [InverseProperty("SettlementItem")]
    public virtual ICollection<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; } = new List<OrganisationEmployeeTypeSettlementItem>();

    [InverseProperty("SettlementItem")]
    public virtual ICollection<OrganisationSettlementItem> OrganisationSettlementItems { get; set; } = new List<OrganisationSettlementItem>();
}
