using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Employee.Core.Entities;

[Table("Employee_Request_Detail", Schema = "emp")]
public class EmployeeRequestDetail : BaseEntity, IignoreDateRangeValidation
{
        public EmployeeRequestDetail()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("EmployeeRequest")]
    public long EmployeeRequestId { get; set; }
    public virtual EmployeeRequest? EmployeeRequest { get; set; }

    [ForeignKey("RequestDocumentRequirementDetail")]
    public long RequestDocumentRequirementDetailId { get; set; }
    public virtual RequestDocumentRequirementDetail? RequestDocumentRequirementDetail { get; set; }

    [ForeignKey("File")]
    public long? FileId { get; set; }
    public virtual File? File { get; set; }

    [StringLength(1000)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; } = string.Empty;
}
