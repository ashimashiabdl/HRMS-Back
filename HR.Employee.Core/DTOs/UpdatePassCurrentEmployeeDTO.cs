using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class UpdatePassCurrentEmployeeDTO
    {
        public long EmployeeId { get; set; }
        [Required]
        public string? newpass { get; set; }
        [Required]
        public string? newpassconfirm { get; set; }
    }
}
