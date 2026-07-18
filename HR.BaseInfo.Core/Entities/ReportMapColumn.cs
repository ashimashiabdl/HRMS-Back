using HR.SharedKernel.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("ReportMapColumn", Schema = "bas")]
public class ReportMapColumn : HR.SharedKernel.Data.BaseEntity
{
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? PersianName { get; set; }
}
