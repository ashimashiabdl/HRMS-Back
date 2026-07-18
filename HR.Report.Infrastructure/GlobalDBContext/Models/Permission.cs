using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Permission", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("ParentId", Name = "IX_Permission_ParentId")]
public partial class Permission
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool IsCustomPermission { get; set; }

    public Guid? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Permission> InverseParent { get; set; } = new List<Permission>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Permission? Parent { get; set; }
}
