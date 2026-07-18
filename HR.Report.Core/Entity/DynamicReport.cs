using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;

[Table("Dynamic_Report", Schema = "rpt")]
public class DynamicReport : BaseEntity, IOrganisationChartId , IignoreDateRangeValidation
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]

    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("FuctionType")]
    public long FuctionTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? FuctionType { get; set; }
    [Comment("base table value Id : 40286 (excel or pdf)")]
    [ForeignKey("ExportType")]
    public long ExportTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? ExportType { get; set; }
    [ForeignKey("OrganisationMRT")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long? OrganisationMRTId { get; set; }
    public virtual OrganisationMRT? OrganisationMRT { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? SqlQuery { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    [StringLength(256)]
    public string? EnglishName { get; set; }
    [StringLength(32)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? Schema { get; set; }
    [StringLength(255)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? FunctionName { get; set; }
    [StringLength(1024)]
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? Help { get; set; }
    public bool IsActive { get; set; }
}
