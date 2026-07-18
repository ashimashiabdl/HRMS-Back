using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Position_Suggested", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Position_Suggested_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationPositionId", Name = "IX_Organisation_Position_Suggested_OrganisationPositionId")]
public partial class OrganisationPositionSuggested
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [Column("EmployeeID")]
    public long EmployeeId { get; set; }

    public long OrganisationPositionId { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationPositionSuggesteds")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationPositionId")]
    [InverseProperty("OrganisationPositionSuggesteds")]
    public virtual OrganisationPosition OrganisationPosition { get; set; } = null!;
}
