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
    public class UserPayLocationDTO : BaseDTO
    {
        public long PayLocationId { get; set; }
        public string? PayLocation { get; set; }
        public long? UserId { get; set; }
        public string? User { get; set; }
        public List<long>? PayLocationIds { get; set; }
    }
}
