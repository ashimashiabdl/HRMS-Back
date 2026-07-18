using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("Role_Claim", Schema = "Identity")]
public class RoleClaim : IdentityRoleClaim<long>
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the role associated with this claim.
    /// </summary>
    public AspNetRoles Role { get; set; }

    #endregion Public Properties
}
