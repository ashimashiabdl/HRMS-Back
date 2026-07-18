using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("User_Issue_Report", Schema = "bas")]
public class UserIssueReport : BaseEntity , IignoreDateRangeValidation
{
    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(2000)]
    public string? Description { get; set; }

    // Optional reference to a person by national code when relevant
    [StringLength(20)]
    public string? RelatedPersonNationalCode { get; set; }

    // Optional attachment stored in BaseInfo File table
    public long? FileId { get; set; }

    [ForeignKey(nameof(FileId))]
    public File? Attachment { get; set; }

    // کاربر ثبت کننده خرابی
    public long? CreatedByUserId { get; set; }

    // کاربر پاسخ دهنده خرابی
    public long? ResponseByUserId { get; set; }

    // پاسخ
    [StringLength(2000)]
    public string? Response { get; set; }

    // ثبت شده است؟
    public bool IsSubmitted { get; set; }

    // زمان پاسخ (بدون Column attribute تا migration نزند)
    public DateTime? ResponseDate { get; set; }
}


