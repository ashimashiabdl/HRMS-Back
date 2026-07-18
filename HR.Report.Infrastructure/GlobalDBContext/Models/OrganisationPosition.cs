using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Position", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Position_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("InsurancePositionId", Name = "IX_Organisation_Position_InsurancePositionId")]
[Microsoft.EntityFrameworkCore.Index("PositionId", Name = "IX_Organisation_Position_PositionId")]
[Microsoft.EntityFrameworkCore.Index("PositionTypeId", Name = "IX_Organisation_Position_PositionTypeId")]
[Microsoft.EntityFrameworkCore.Index("RelatedNodeId", Name = "IX_Organisation_Position_RelatedNodeId")]
public partial class OrganisationPosition
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long PositionId { get; set; }

    public long PositionTypeId { get; set; }

    public long InsurancePositionId { get; set; }

    [StringLength(25)]
    public string? PositionCode { get; set; }

    public long RelatedNodeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LockStartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LockEndDate { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDedicated { get; set; }

    public bool? IsState { get; set; }

    public bool? IsStarable { get; set; }

    public bool? IsFreez { get; set; }

    public bool? IsManager { get; set; }

    public bool? IsSubstitute { get; set; }

    public bool? IsExpert { get; set; }

    public int? Capacity { get; set; }

    public int? MaxTenureYears { get; set; }

    [StringLength(256)]
    public string? Other1 { get; set; }

    [StringLength(256)]
    public string? Other2 { get; set; }

    [StringLength(128)]
    public string? UniqueIdentifier { get; set; }

    [StringLength(128)]
    public string? ShortName { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationPositions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganisationPosition")]
    public virtual ICollection<OrganisationPositionJob> OrganisationPositionJobs { get; set; } = new List<OrganisationPositionJob>();

    [InverseProperty("OrganisationPosition")]
    public virtual ICollection<OrganisationPositionSuggested> OrganisationPositionSuggesteds { get; set; } = new List<OrganisationPositionSuggested>();

    [InverseProperty("OrganisationPosition")]
    public virtual ICollection<OrganisationPositionSupervisor> OrganisationPositionSupervisors { get; set; } = new List<OrganisationPositionSupervisor>();

    [InverseProperty("OrganisationPosition")]
    public virtual ICollection<RecruitOrder> RecruitOrders { get; set; } = new List<RecruitOrder>();

    [ForeignKey("InsurancePositionId")]
    [InverseProperty("OrganisationPositions")]
    public virtual InsurancePosition InsurancePosition { get; set; } = null!;

    [ForeignKey("PositionId")]
    [InverseProperty("OrganisationPositions")]
    public virtual Position Position { get; set; } = null!;

    [ForeignKey("PositionTypeId")]
    [InverseProperty("OrganisationPositions")]
    public virtual PositionType PositionType { get; set; } = null!;

    [ForeignKey("RelatedNodeId")]
    [InverseProperty("RelatedNodeOrganisationPositions")]
    public virtual OrganisationChart RelatedNode { get; set; } = null!;
}
