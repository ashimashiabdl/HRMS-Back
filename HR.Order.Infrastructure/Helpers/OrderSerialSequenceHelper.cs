using HR.Order.Core.Data;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;

namespace HR.Order.Infrastructure.Helpers;

/// <summary>
/// اعتبارسنجی ترتیب تایید/رد احکام بر اساس سریال برای هر فرد.
/// تایید: از سریال کوچک به بزرگ — رد: از سریال بزرگ به کوچک.
/// </summary>
public static class OrderSerialSequenceHelper
{
    public const string ApproveOutOfOrderMessage = "ابتدا احکام کوچکتر را تایید و سپس این حکم را تایید بکنید";
    public const string RejectOutOfOrderMessage = "ابتدا احکام بزرگتر را رد و سپس این حکم را رد بکنید";

    public static bool HasSmallerPendingOrder(long employeeId, short currentSerial, long currentOrderId, DbContext db)
    {
        return db.Set<InterdictOrder>()
            .Include(i => i.RecruitOrder)
            .Any(i => !i.IsDeleted
                      && i.Id != currentOrderId
                      && i.StatusId == (long)Enums.OrderStatus.Pending
                      && i.RecruitOrder != null
                      && i.RecruitOrder.EmployeeId == employeeId
                      && (i.Serial ?? 0) < currentSerial);
    }

    public static bool HasLargerPendingOrder(long employeeId, short currentSerial, long currentOrderId, DbContext db)
    {
        return db.Set<InterdictOrder>()
            .Include(i => i.RecruitOrder)
            .Any(i => !i.IsDeleted
                      && i.Id != currentOrderId
                      && i.StatusId == (long)Enums.OrderStatus.Pending
                      && i.RecruitOrder != null
                      && i.RecruitOrder.EmployeeId == employeeId
                      && (i.Serial ?? 0) > currentSerial);
    }
}
