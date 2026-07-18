using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;
[Table("Absence_Record", Schema = "emp")]
public class AbsenceRecord : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
{
        public AbsenceRecord()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
public long? OrganisationChartId { get; set ; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; } 
    
    [ForeignKey("AbsenceTypeValue")]
    public long AbsenceTypeValueId { get; set; }
    public virtual AbsenceTypeValue? AbsenceTypeValue { get; set; }
    public bool FirstApprove { get; set; } = false;
    public bool SecondApprove { get; set; } = false;
}
