using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Employee_Login_History", Schema = "emp")]
    public class EmployeeLoginHistory : HR.SharedKernel.Data.BaseEntity
    {
            public EmployeeLoginHistory()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("Employee")]
        public long? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public bool IsSuccess { get; set; } = false;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
