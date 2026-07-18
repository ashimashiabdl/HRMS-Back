using System.ComponentModel.DataAnnotations.Schema;
using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// نوع پیوست سند تسویه حساب
/// </summary>
[Table("Settlement_Document_Attachment_Type", Schema = "bas")]
public class SettlementDocumentAttachmentType : BaseEntity
{
}
