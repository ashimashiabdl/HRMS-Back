using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

[Table("Attendance", Schema = "emp")]
public class Attendance : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
{
        public Attendance()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
public long? OrganisationChartId { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
    public int InOutType { get; set; } = 0;
    [Column(TypeName = "datetime")]
    public DateTime? DateTime { get; set; }
    [StringLength(50)]
    public string? DeviceName { get; set; } = string.Empty;
    [StringLength(20)]
    public string? InOutCard { get; set; } = string.Empty;
}
