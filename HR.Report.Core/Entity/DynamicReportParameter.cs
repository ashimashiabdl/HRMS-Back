using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;

[Table("Dynamic_Report_Parameter", Schema = "rpt")]
public class DynamicReportParameter : BaseEntity
{
    [ForeignKey("DynamicReport")]
    public long DynamicReportId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual DynamicReport? DynamicReport { get; set; }
    public bool Optional { get; set; }
    [StringLength(256)]
    public string? DefaultValue { get; set; }
    [ForeignKey("Parameter")]
    public long ParameterId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? Parameter { get; set; }
}