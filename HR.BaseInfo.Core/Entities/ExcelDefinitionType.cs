using HR.SharedKernel.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Excel_Definition_Type", Schema = "bas")]
public class ExcelDefinitionType : HR.SharedKernel.Data.BaseEntity
{
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
}


