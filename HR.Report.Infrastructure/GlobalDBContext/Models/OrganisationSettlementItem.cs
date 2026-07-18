using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Settlement_Item", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Settlement_Item_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettlementItemId", Name = "IX_Organisation_Settlement_Item_SettlementItemId")]
public partial class OrganisationSettlementItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long SettlementItemId { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationSettlementItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SettlementItemId")]
    [InverseProperty("OrganisationSettlementItems")]
    public virtual SettlementItem SettlementItem { get; set; } = null!;
}
