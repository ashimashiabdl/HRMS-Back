using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("ConfidentialityLevel", Schema = "bas")]
public class ConfidentialityLevel : BaseEntity
{
    [StringLength(256)]
    public string? Description { get; set; }
}


