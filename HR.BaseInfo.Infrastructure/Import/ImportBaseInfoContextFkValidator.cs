using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.BaseInfo.infrastructure.Import;

/// <summary>
/// Context FK validation for BaseInfo reference entities (FundType, LeaveType, BaseTableValue).
/// </summary>
public class ImportBaseInfoContextFkValidator(IUnitOfWork<BaseInfoContext> baseInfoUnitOfWork)
    : IImportOrganScopedFkValidator
{
    private static readonly HashSet<string> SupportedEntities = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(FundType),
        "FundType",
        nameof(LeaveType),
        "LeaveType",
        nameof(BaseTableValue),
        "BaseTableValue",
        nameof(HistoryType),
        "HistoryType",
        nameof(Places),
        "Places",
        nameof(SkillLevel),
        "SkillLevel",
        nameof(ConfidentialityLevel),
        "ConfidentialityLevel",
        nameof(TaminInsuranceJobList),
        "TaminInsuranceJobList",
        nameof(ManagementAndStewardshipJob),
        "ManagementAndStewardshipJob",
        nameof(TaxExemptionType),
        "TaxExemptionType",
        nameof(EducationGrade),
        "EducationGrade",
        nameof(EducationField),
        "EducationField",
        nameof(EducationOrientation),
        "EducationOrientation",
        nameof(University),
        "University",
        nameof(EmployeeType),
        "EmployeeType",
        nameof(EmployeeStatus),
        "EmployeeStatus",
        nameof(OrderType),
        "OrderType",
        nameof(OrderStatus),
        "OrderStatus",
        nameof(WageItem),
        "WageItem",
        nameof(Coefficient),
        "Coefficient",
        nameof(Project),
        "Project",
    };

    public bool CanValidate(string? fkReferenceEntity, string? fkReferenceSchema = null) =>
        !string.IsNullOrWhiteSpace(fkReferenceEntity) && SupportedEntities.Contains(fkReferenceEntity.Trim());

    public async Task<string?> ValidateAsync(
        string fkReferenceEntity,
        long id,
        long organisationChartId,
        string? fkReferenceSchema = null)
    {
        if (id <= 0)
            return "مقدار انتخاب‌شده معتبر نیست.";

        var ctx = baseInfoUnitOfWork.Context;
        var entity = fkReferenceEntity.Trim();

        if (entity.Equals(nameof(FundType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("FundType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.FundTypes.AsNoTracking()
                .AnyAsync(f => f.Id == id && !f.IsDeleted);
            return exists ? null : "نوع صندوق معتبر نیست.";
        }

        if (entity.Equals(nameof(LeaveType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("LeaveType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.LeaveTypes.AsNoTracking()
                .AnyAsync(l => l.Id == id && !l.IsDeleted);
            return exists ? null : "نوع مرخصی معتبر نیست.";
        }

        if (entity.Equals(nameof(BaseTableValue), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("BaseTableValue", StringComparison.OrdinalIgnoreCase))
        {
            var query = ctx.BaseTableValues.AsNoTracking().Where(v => v.Id == id && !v.IsDeleted);
            if (!string.IsNullOrWhiteSpace(fkReferenceSchema)
                && long.TryParse(fkReferenceSchema.Trim(), out var baseTableId)
                && baseTableId > 0)
            {
                query = query.Where(v => v.BaseTableId == baseTableId);
            }

            var exists = await query.AnyAsync();
            return exists ? null : "مقدار جدول پایه معتبر نیست.";
        }

        if (entity.Equals(nameof(HistoryType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("HistoryType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.HistoryTypes.AsNoTracking()
                .AnyAsync(h => h.Id == id && !h.IsDeleted);
            return exists ? null : "نوع سابقه معتبر نیست.";
        }

        if (entity.Equals(nameof(ConfidentialityLevel), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("ConfidentialityLevel", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.ConfidentialityLevels.AsNoTracking()
                .AnyAsync(c => c.Id == id && !c.IsDeleted);
            return exists ? null : "سطح محرمانگی معتبر نیست.";
        }

        if (entity.Equals(nameof(TaminInsuranceJobList), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("TaminInsuranceJobList", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.TaminInsuranceJobLists.AsNoTracking()
                .AnyAsync(t => t.Id == id && !t.IsDeleted);
            return exists ? null : "شغل بیمه تأمین اجتماعی معتبر نیست.";
        }

        if (entity.Equals(nameof(ManagementAndStewardshipJob), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("ManagementAndStewardshipJob", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.ManagementAndStewardshipJobs.AsNoTracking()
                .AnyAsync(m => m.Id == id && !m.IsDeleted);
            return exists ? null : "شغل مدیریتی/نظارتی معتبر نیست.";
        }

        if (entity.Equals(nameof(TaxExemptionType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("TaxExemptionType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.TaxExemptionTypes.AsNoTracking()
                .AnyAsync(t => t.Id == id && !t.IsDeleted);
            return exists ? null : "نوع معافیت مالیاتی معتبر نیست.";
        }

        if (entity.Equals(nameof(Places), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("Places", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.Places.AsNoTracking()
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
            return exists ? null : "محل (شهر/استان) معتبر نیست.";
        }

        if (entity.Equals(nameof(SkillLevel), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("SkillLevel", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.SkillLevels.AsNoTracking()
                .AnyAsync(s => s.Id == id && !s.IsDeleted);
            return exists ? null : "سطح مهارت معتبر نیست.";
        }

        if (entity.Equals(nameof(EducationGrade), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("EducationGrade", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.EducationGrades.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "مقطع تحصیلی معتبر نیست.";
        }

        if (entity.Equals(nameof(EducationField), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("EducationField", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.EducationFields.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "رشته تحصیلی معتبر نیست.";
        }

        if (entity.Equals(nameof(EducationOrientation), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("EducationOrientation", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.EducationOrientations.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "گرایش تحصیلی معتبر نیست.";
        }

        if (entity.Equals(nameof(University), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("University", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.Universities.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "دانشگاه معتبر نیست.";
        }

        if (entity.Equals(nameof(EmployeeType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("EmployeeType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.EmployeeTypes.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "نوع استخدام معتبر نیست.";
        }

        if (entity.Equals(nameof(EmployeeStatus), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("EmployeeStatus", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.EmployeeStatuses.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "وضعیت استخدامی معتبر نیست.";
        }

        if (entity.Equals(nameof(OrderType), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("OrderType", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.OrderTypes.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "نوع حکم معتبر نیست.";
        }

        if (entity.Equals(nameof(OrderStatus), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("OrderStatus", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.OrderStatuses.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "وضعیت حکم معتبر نیست.";
        }

        if (entity.Equals(nameof(WageItem), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("WageItem", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.WageItems.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "عامل حقوقی معتبر نیست.";
        }

        if (entity.Equals(nameof(Coefficient), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("Coefficient", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.Coefficients.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "ضریب معتبر نیست.";
        }

        if (entity.Equals(nameof(Project), StringComparison.OrdinalIgnoreCase)
            || entity.Equals("Project", StringComparison.OrdinalIgnoreCase))
        {
            var exists = await ctx.Projects.AsNoTracking()
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
            return exists ? null : "پروژه معتبر نیست.";
        }

        return null;
    }
}
