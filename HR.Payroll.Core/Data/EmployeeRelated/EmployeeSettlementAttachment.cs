using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data.EmployeeRelated;

/// <summary>
/// پیوست سند تسویه حساب کارمند
/// </summary>
[Table("Employee_Settlement_Attachment", Schema = "Payroll")]
public class EmployeeSettlementAttachment : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey(nameof(EmployeeSettlement))]
    public long EmployeeSettlementId { get; set; }
    public virtual EmployeeSettlement? EmployeeSettlement { get; set; }

    [ForeignKey(nameof(SettlementDocumentAttachmentType))]
    public long SettlementDocumentAttachmentTypeId { get; set; }
    public virtual SettlementDocumentAttachmentType? SettlementDocumentAttachmentType { get; set; }

    [ForeignKey(nameof(File))]
    public long FileId { get; set; }
    public virtual HR.BaseInfo.Core.Entities.File? File { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    [NotMapped]
    private new string title { get; set; } = string.Empty;
}
