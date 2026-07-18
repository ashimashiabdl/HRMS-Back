using System;
using System.ComponentModel.DataAnnotations;
using HR.SharedKernel.DTOs;

namespace HR.Identity.Core.DTOs;

public class PermissionsDTO : SharedKernel.Data.BaseDTO
{
    [Required(ErrorMessage = "نام دسترسی اجباری می باشد")]
    [MaxLength(50, ErrorMessage = "طول نام دسترسی نباید بیشتر از 50 کاراکتر باشد")]
    public string Name { get; set; }

    [MaxLength(150, ErrorMessage = "طول توضیحات نباید بیشتر از 150 کاراکتر باشد")]
    public string Description { get; set; }

    public string AccessKey { get; set; }

    public bool IsActive { get; set; } = true;
}