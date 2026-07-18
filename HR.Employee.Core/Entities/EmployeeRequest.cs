using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

[Table("Employee_Request", Schema = "emp")]
public class EmployeeRequest : BaseEntity, IEmployeeHistoryOrganisationChartId, IignoreDateRangeValidation
{
        public EmployeeRequest()
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

    [ForeignKey("RequestDocumentRequirement")]
    public long RequestDocumentRequirementId { get; set; }
    public virtual RequestDocumentRequirement? RequestDocumentRequirement { get; set; }

    [ForeignKey("EmployeeRequestStatus")]
    public long EmployeeRequestStatusId { get; set; } = 1;
    public virtual EmployeeRequestStatus? EmployeeRequestStatus { get; set; }

    [StringLength(1000)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; } = string.Empty;

    public virtual ICollection<EmployeeRequestDetail> Details { get; set; } = new List<EmployeeRequestDetail>();
}
