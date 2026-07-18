using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class RoleReportDTO : BaseDTO
    {
        public long RoleId { get; set; }
        public string? Role { get; set; }
        public long DynamicReportId { get; set; }
        public string? DynamicReport { get; set; }
    }
}

