using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Employee_Request_Status", Schema = "bas")]
public class EmployeeRequestStatus : HR.SharedKernel.Data.BaseEntity
{
    public int StatusCode { get; set; }
}
