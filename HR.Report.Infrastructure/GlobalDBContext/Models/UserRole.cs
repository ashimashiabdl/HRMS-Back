using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[PrimaryKey("UserId", "RoleId")]
[Table("User_Role", Schema = "Identity")]
public partial class UserRole
{
    [Key]
    public long UserId { get; set; }

    [Key]
    public long RoleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }
}
