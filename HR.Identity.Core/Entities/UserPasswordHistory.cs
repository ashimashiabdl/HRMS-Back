using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Identity.Core.Entities;

[Table("User_Password_History", Schema = "Identity")]
public class UserPasswordHistory : HR.SharedKernel.Data.BaseEntity, HR.SharedKernel.Data.IignoreDateRangeValidation
{
    [ForeignKey("AspNetUser")]
    public long AspNetUserId { get; set; }
    public virtual AspNetUsers? AspNetUser { get; set; }

    [StringLength(512)]
    public string PasswordHash { get; set; }

    [MaxLength(64)]
    public byte[]? Salt { get; set; }

    [NotMapped]
    private new string title { get; set; }
}


