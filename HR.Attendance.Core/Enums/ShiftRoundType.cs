using System.ComponentModel;

namespace HR.Attendance.Core.Enums;

public enum ShiftRoundType
{
    [Description("بدون گرد کردن")]
    None = 0,

    [Description("گرد به بالا")]
    RoundUp = 1,

    [Description("گرد به پایین")]
    RoundDown = 2,

    [Description("گرد به نزدیک‌ترین")]
    RoundNearest = 3,
}
