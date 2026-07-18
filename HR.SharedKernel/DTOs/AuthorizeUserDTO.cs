using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.DTOs
{
    public class AuthorizeUserDTO
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
