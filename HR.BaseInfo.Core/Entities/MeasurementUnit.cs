using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// واحد سنجش
/// </summary>
[Table("Measurement_Unit", Schema = "bas")]
public class MeasurementUnit : BaseEntity
{
    [StringLength(450)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; }
}
