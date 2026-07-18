using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("CommonPassword", Schema = "Identity")]
public class CommonPassword : HR.SharedKernel.Data.BaseEntity , IignoreDateRangeValidation
{
    [StringLength(256)]
    [Required(ErrorMessage = "کلمه عبور الزامی می باشد")]
    public string Password { get; set; } = string.Empty;
}

