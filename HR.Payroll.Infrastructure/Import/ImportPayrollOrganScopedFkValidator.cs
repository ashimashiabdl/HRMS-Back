using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// Organ-scoped FK validation for Import context metadata (Payroll entities only).
/// </summary>
public class ImportPayrollOrganScopedFkValidator(IUnitOfWork<PayrollContext> payrollUnitOfWork)
    : IImportOrganScopedFkValidator
{
    private static readonly HashSet<string> SupportedEntities = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(HR.Payroll.Core.Data.DeductionType),
        "DeductionType",
        nameof(HR.Payroll.Core.Data.PaymentPeriod),
        "PaymentPeriod",
        nameof(HR.Payroll.Core.Data.Bank),
        "Bank",
    };

    private const string PayrollSchema = "Payroll";

    public bool CanValidate(string? fkReferenceEntity, string? fkReferenceSchema = null)
    {
        if (string.IsNullOrWhiteSpace(fkReferenceEntity) || !SupportedEntities.Contains(fkReferenceEntity.Trim()))
            return false;

        if (string.IsNullOrWhiteSpace(fkReferenceSchema))
            return true;

        return string.Equals(fkReferenceSchema.Trim(), PayrollSchema, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string?> ValidateAsync(
        string fkReferenceEntity,
        long id,
        long organisationChartId,
        string? fkReferenceSchema = null)
    {
        if (!CanValidate(fkReferenceEntity, fkReferenceSchema))
            return "مرجع FK برای اعتبارسنجی سازمانی پشتیبانی نمی‌شود.";

        if (id <= 0 || organisationChartId <= 0)
            return "مقدار یا سازمان معتبر نیست.";

        var ctx = payrollUnitOfWork.Context;
        var entity = fkReferenceEntity.Trim();

        if (entity.Equals(nameof(HR.Payroll.Core.Data.DeductionType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("DeductionType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.DeductionTypes.AsNoTracking()
                .AnyAsync(d => d.Id == id && d.OrganisationChartId == organisationChartId);
            return exists ? null : "نوع کسور برای سازمان پیش‌فرض شما معتبر نیست.";
        }

        if (entity.Equals(nameof(HR.Payroll.Core.Data.PaymentPeriod), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("PaymentPeriod", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.PaymentPeriods.AsNoTracking()
                .AnyAsync(p => p.Id == id && p.OrganisationChartId == organisationChartId);
            return exists ? null : "دوره پرداخت برای سازمان پیش‌فرض شما معتبر نیست.";
        }

        if (entity.Equals(nameof(HR.Payroll.Core.Data.Bank), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("Bank", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.Banks.AsNoTracking()
                .AnyAsync(b => b.Id == id && !b.IsDeleted);
            return exists ? null : "بانک انتخاب‌شده معتبر نیست.";
        }

        return null;
    }
}
