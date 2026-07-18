using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;

[Table("Reportable_Field", Schema = "rpt")]
public class ReportableField : BaseEntity , IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public long ReportableEntityId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? TechnicalName { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? FriendlyName { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public long FieldDataTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public string? NavigationPath { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public long? BaseTableId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]

    public bool IsFilterable { get; set; }

    public bool IsSelectable { get; set; }
    public bool IsSortable { get; set; }
    public bool IsActive { get; set; }

    public int Priority { get; set; }

    [ForeignKey(nameof(ReportableEntityId))]
    public virtual ReportableEntity ReportableEntity { get; set; }
    [ForeignKey(nameof(FieldDataTypeId))]
    public virtual FieldDataType FieldDataType { get; set; }
}
