using System.ComponentModel;

namespace HR.Attendance.Core.Enums;

/// <summary>
/// کد نوع عدم حضور — مطابق جدول [Attendance].[Attendance_Absence_Type]
/// </summary>
public enum AbsenceTypeCode
{
    [Description("مرخصی")]
    Leave = 1,

    [Description("ماموریت")]
    Mission = 2,

    [Description("غیبت")]
    Absence = 3,

    [Description("تأخیر")]
    Delay = 4,

    [Description("تعجیل در خروج")]
    EarlyDeparture = 5,
}
