using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;
[Table("Field_Operator", Schema = "rpt")]
public class FieldOperator : BaseEntity , IignoreDateRangeValidation
{
    public long FieldDataTypeId { get; set; }
    public string? Operator { get; set; }
    public string? FriendlyName { get; set; }

    [ForeignKey(nameof(FieldDataTypeId))]
    public virtual FieldDataType FieldDataType { get; set; }
}
