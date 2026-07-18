using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using global::HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.Identity.Core.Entities;

[Table("User_Login_History", Schema = "Identity")]
public class UserLoginHistory : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("AspNetUser")]
    public long? AspNetUserId { get; set; }
    public virtual AspNetUsers? AspNetUser { get; set; }

    /// <summary>
    /// شناسه کارمند — برای ثبت تاریخچه ورود از پورتال UserProfile (بدون وابستگی به AspNetUsers)
    /// </summary>
    public long? EmployeeId { get; set; }

    public bool IsSuccess { get; set; }

    
    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(2048)]
    public string? FailReason { get; set; }
}
