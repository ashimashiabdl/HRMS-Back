using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("Permission", Schema = "Identity")]
public class Permission
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the permission.
    /// </summary>
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the Default display name of the permission.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the permission is a custom permission.
    /// </summary>
    public bool IsCustomPermission { get; set; }
    [NotMapped]
    public bool Checked { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent permission.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent permission.
    /// </summary>
    [ForeignKey(name: "ParentId")]
    public Permission Parent { get; set; }

    /// <summary>
    /// Gets or sets the list of child permissions.
    /// </summary>
    public IList<Permission> Permissions { get; set; } = new List<Permission>();
}
