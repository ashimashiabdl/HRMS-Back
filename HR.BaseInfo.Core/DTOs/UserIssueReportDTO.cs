using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class UserIssueReportDTO : BaseDTO
{
    [StringLength(256)]
    public string title { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string? RelatedPersonNationalCode { get; set; }

    public long? FileId { get; set; }

    // کاربر ثبت کننده خرابی
    public long? CreatedByUserId { get; set; }

    // نام کاربر ثبت کننده خرابی
    public string? CreatedByUserFullName { get; set; }

    // کاربر پاسخ دهنده خرابی
    public long? ResponseByUserId { get; set; }

    // نام کاربر پاسخ دهنده خرابی
    public string? ResponseByUserFullName { get; set; }

    // پاسخ
    [StringLength(2000)]
    public string? Response { get; set; }

    // ثبت شده است؟
    public bool IsSubmitted { get; set; }

    // زمان پاسخ
    public DateTime? ResponseDate { get; set; }
}


