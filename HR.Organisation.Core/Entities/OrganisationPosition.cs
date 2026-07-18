using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Organisation.Core.Entities;

[Table("Organisation_Position", Schema = "Org")]
public class OrganisationPosition : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Position")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long PositionId { get; set; }
    public virtual Position? Position { get; set; }
    [ForeignKey("PositionType")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long PositionTypeId { get; set; }
    public virtual PositionType? PositionType { get; set; }
    [ForeignKey("InsurancePosition")]
    public long InsurancePositionId { get; set; }
    public virtual InsurancePosition? InsurancePosition { get; set; }

    [ForeignKey("Rank")]
    public long RankId { get; set; }
    public virtual Rank? Rank { get; set; }

    [ForeignKey("PositionManagementLevel")]
    public long PositionManagementLevelId { get; set; }
    public virtual PositionManagementLevel? PositionManagementLevel { get; set; }

    [ForeignKey("PositionState")]
    public long PositionStateId { get; set; }
    public virtual PositionState? PositionState { get; set; }

    [StringLength(25)]
    public string? PositionCode { get; set; }

    [ForeignKey("RelatedNode")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long RelatedNodeId { get; set; }
    public virtual OrganisationChart? RelatedNode { get; set; }

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

    /// <summary>سقف مدت تصدی (سال)</summary>
    public int? MaxTenureYears { get; set; }

    /// <summary>سایر ۱</summary>
    [StringLength(256)]
    public string? Other1 { get; set; }

    /// <summary>سایر ۲</summary>
    [StringLength(256)]
    public string? Other2 { get; set; }

    /// <summary>شناسه یکتا</summary>
    [StringLength(128)]
    public string? UniqueIdentifier { get; set; }

    /// <summary>نام اختصار</summary>
    [StringLength(128)]
    public string? ShortName { get; set; }
}
