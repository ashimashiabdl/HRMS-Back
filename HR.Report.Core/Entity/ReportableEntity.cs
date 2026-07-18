using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;
[Table("Reportable_Entity", Schema = "rpt")]
public class ReportableEntity : BaseEntity , IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string TechnicalName { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string FriendlyName { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Schema { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? TableName { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<ReportableField> Fields { get; set; } = new List<ReportableField>();
}
