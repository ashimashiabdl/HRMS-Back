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
    public class UserOrganizationUnitDTO : BaseDTO
    {
        public long? UserId { get; set; }
        public string? User { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
    }
}
