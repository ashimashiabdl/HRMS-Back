using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Entities;
[Table("User_Login", Schema = "Identity")]
public class UserLogin : IdentityUserLogin<long>
{

    public AspNetUsers User { get; set; }

  
}
