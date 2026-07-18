using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_OrderType_HistoryExclusion", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_OrderType_HistoryExclusion_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_OrderType_HistoryExclusion_OrganisationChartId")]
public partial class OrganisationOrderTypeHistoryExclusion
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long OrderTypeId { get; set; }

    [Column("TerminatingOrderTypeID")]
    public int TerminatingOrderTypeId { get; set; }

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
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationOrderTypeHistoryExclusions")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationOrderTypeHistoryExclusions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
