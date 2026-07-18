using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Experience", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Experience_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("HistoryTypeId", Name = "IX_Experience_HistoryTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Experience_OrganisationChartId")]
public partial class Experience
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public bool IsInternal { get; set; }

    public bool IsAcceptable { get; set; }

    public int? AcceptablePercent { get; set; }

    [StringLength(250)]
    public string? CompanyTitle { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public long? HistoryTypeId { get; set; }

    [StringLength(6)]
    public string? Duration { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Experiences")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("HistoryTypeId")]
    [InverseProperty("Experiences")]
    public virtual HistoryType? HistoryType { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("Experiences")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
