using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class UserRoleDTO : BaseDTO
    {

        public long? UserId { get; set; }
        public string? User { get; set; }
        public long? RoleId { get; set; }
        public string? Role { get; set; }
        public List<long>? RoleIds { get; set; }
    }
}
