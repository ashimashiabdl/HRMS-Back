using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Report.Infrastructure.Services;

public class ReportableFieldService(IMapper mapper, IUnitOfWork<ReportContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<ReportableField, ReportContext, ReportableFieldDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    /// <summary>
    /// دریافت داده‌های صفحه‌بندی شده با فیلتر بر اساس موجودیت
    /// </summary>
    public OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? entityId = null)
    {
        IQueryable<ReportableField> all = All(IgnoreExpired)
            .Include(f => f.ReportableEntity)
            .Include(f => f.FieldDataType);

        // فیلتر بر اساس موجودیت
        if (entityId.HasValue && entityId.Value > 0)
        {
            all = all.Where(f => f.ReportableEntityId == entityId.Value);
        }

        return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource: all);
    }

    /// <summary>
    /// حذف فیزیکی فیلد قابل گزارش
    /// </summary>
    public new OperationResult DeleteRecord(long id)
    {
        var entity = _unitOfWork.Context.Set<ReportableField>()
                  .FirstOrDefault(e => e.Id == id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        // حذف فیزیکی فیلد
        _unitOfWork.Context.Set<ReportableField>().Remove(entity);

        _unitOfWork.Context.SaveChanges();

        return OperationResult.Succeeded("رکورد با موفقیت حذف شد");
    }
}

