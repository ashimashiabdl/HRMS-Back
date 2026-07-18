using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Insurance_Type", Schema = "Payroll")]
public class InsuranceType : SharedKernel.Data.BaseEntity
{
    [StringLength(256)]
    public string? InsuranceCode { get; set; }
    public bool? IsActive { get; set; }
}
