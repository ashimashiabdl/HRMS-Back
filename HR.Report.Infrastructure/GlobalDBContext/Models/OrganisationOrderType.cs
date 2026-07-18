using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_OrderType", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrderDirectionTypeId", Name = "IX_Organisation_OrderType_OrderDirectionTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeGroupId", Name = "IX_Organisation_OrderType_OrderTypeGroupId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_OrderType_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_OrderType_OrganisationChartId")]
public partial class OrganisationOrderType
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long OrderTypeGroupId { get; set; }

    public long OrderTypeId { get; set; }

    public long? OrderDirectionTypeId { get; set; }

    public bool IsBatch { get; set; }

    public bool IsPrintable { get; set; }

    [StringLength(128)]
    public string? Code { get; set; }

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

    public bool ShowInHistory { get; set; }

    public int? ExperienceCoefficient { get; set; }

    public int? RetiredCoefficient { get; set; }

    public int? YearCoefficient { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationOrderTypes")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrderTypeGroupId")]
    [InverseProperty("OrganisationOrderTypes")]
    public virtual OrderTypeGroup OrderTypeGroup { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationOrderTypes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
