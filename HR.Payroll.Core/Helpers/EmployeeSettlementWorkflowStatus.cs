using HR.Payroll.Core.Data.EmployeeRelated;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;

namespace HR.Payroll.Core.Helpers;

/// <summary>
/// تغییر وضعیت تسویه حساب در گردش کار (بدون مدیریت تراکنش).
/// </summary>
public static class EmployeeSettlementWorkflowStatus
{
    public static OperationResult ApplyFinalApprove(EmployeeSettlement? settlement)
    {
        if (settlement == null)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        if (settlement.SettlementStatusId != (long)Enums.SettlementStatus.PendingReview)
        {
            return OperationResult.Failed("وضعیت تسویه حساب برای تایید نهایی معتبر نیست");
        }

        settlement.SettlementStatusId = (long)Enums.SettlementStatus.ApprovedAndPaid;
        settlement.LastModifiedDate = DateTime.Now;
        return OperationResult.Succeeded();
    }

    public static OperationResult ApplyReject(EmployeeSettlement? settlement)
    {
        if (settlement == null)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        if (settlement.SettlementStatusId != (long)Enums.SettlementStatus.PendingReview)
        {
            return OperationResult.Failed("وضعیت تسویه حساب برای رد معتبر نیست");
        }

        settlement.SettlementStatusId = (long)Enums.SettlementStatus.NotApproved;
        settlement.LastModifiedDate = DateTime.Now;
        return OperationResult.Succeeded();
    }
}
