using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;

[Table("User_Role", Schema = "Identity")]
public class UserRole : IdentityUserRole<long>
{
    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
