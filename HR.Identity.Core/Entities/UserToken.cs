using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_Token", Schema = "Identity")]
public class UserToken : IdentityUserToken<long>
{
    #region Public Properties
    [Key]
    public long Id { get; set; }
    /// <summary>
    /// Gets or sets the user associated with this token.
    /// </summary>
    public AspNetUsers User { get; set; }

    #endregion Public Properties
}
