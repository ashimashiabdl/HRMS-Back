using HR.SharedKernel.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Report.Core.Entity;

[Table("Field_DataType", Schema = "rpt")]
public class FieldDataType : BaseEntity , IignoreDateRangeValidation
{
    public string? TypeName { get; set; }
    public string? FriendlyName { get; set; }

    public virtual ICollection<ReportableField> ReportableFields { get; set; } = new List<ReportableField>();
    public virtual ICollection<FieldOperator> FieldOperators { get; set; } = new List<FieldOperator>();
}
