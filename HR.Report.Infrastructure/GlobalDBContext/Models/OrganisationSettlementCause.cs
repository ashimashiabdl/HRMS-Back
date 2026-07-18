using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Settlement_Cause", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Settlement_Cause_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettlementCauseId", Name = "IX_Organisation_Settlement_Cause_SettlementCauseId")]
public partial class OrganisationSettlementCause
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long SettlementCauseId { get; set; }

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
    [InverseProperty("OrganisationSettlementCauses")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SettlementCauseId")]
    [InverseProperty("OrganisationSettlementCauses")]
    public virtual SettlementCause SettlementCause { get; set; } = null!;
}
