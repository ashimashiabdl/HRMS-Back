namespace HR.BaseInfo.Core.Enums;

/// <summary>
/// نوع مرخصی (FK به bas.LeaveType)
/// </summary>
public enum LeaveTypeId : long
{
    /// <summary>
    /// مرخصی استحقاقی
    /// </summary>
    EarnedLeave = 1,

    /// <summary>
    /// مرخصی نوبت‌کار
    /// </summary>
    ShiftWorkLeave = 2,

    /// <summary>
    /// مرخصی استعلاجی
    /// </summary>
    SickLeave = 3,

    /// <summary>
    /// مرخصی زایمان
    /// </summary>
    MaternityLeave = 4,

    /// <summary>
    /// مرخصی پدران
    /// </summary>
    PaternityLeave = 5,

    /// <summary>
    /// مرخصی ازدواج دائم
    /// </summary>
    PermanentMarriageLeave = 6,

    /// <summary>
    /// مرخصی فوت بستگان
    /// </summary>
    BereavementLeave = 7,

    /// <summary>
    /// مرخصی بدون حقوق
    /// </summary>
    UnpaidLeave = 8,

    /// <summary>
    /// مرخصی حج واجب
    /// </summary>
    HajjLeave = 9,

    /// <summary>
    /// مرخصی تحصیلی
    /// </summary>
    StudyLeave = 10,

    /// <summary>
    /// مرخصی خدمت نظام وظیفه
    /// </summary>
    MilitaryServiceLeave = 11,

    /// <summary>
    /// مرخصی مأموریت
    /// </summary>
    MissionLeave = 12,

    /// <summary>
    /// مرخصی وظایف اجتماعی
    /// </summary>
    SocialDutiesLeave = 13,

    /// <summary>
    /// تعطیلات رسمی
    /// </summary>
    PublicHoliday = 14,

    /// <summary>
    /// مرخصی ساعتی (ساعت)
    /// </summary>
    HourlyLeaveByHour = 15,

    /// <summary>
    /// مرخصی ساعتی (دقیقه)
    /// </summary>
    HourlyLeaveByMinute = 16,
}
