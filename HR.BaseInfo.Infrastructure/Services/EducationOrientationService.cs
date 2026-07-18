using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Service;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class EducationOrientationService : BaseService<EducationOrientation, BaseInfoContext, EducationOrientationDTO>, IScopedServices
{
    private const int LazySearchMinFilterLength = 2;
    private const int LazySearchMaxResults = 30;

    public EducationOrientationService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    /// <summary>
    /// جستجوی تنبل گرایش تحصیلی — بدون فیلتر خالی برنمی‌گرداند تا بار اولیه سنگین نشود.
    /// </summary>
    public OperationResult GetAsKeyValuePairLazy(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Trim().Length < LazySearchMinFilterLength)
        {
            return OperationResult.Succeeded(payload: new List<HR.SharedKernel.Data.KeyValuePair>());
        }

        var term = filter.Trim();
        var results = All()
            .Where(i => i.title != null && i.title.Contains(term))
            .OrderBy(i => i.title)
            .Take(LazySearchMaxResults)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.title
            })
            .ToList();

        return OperationResult.Succeeded(payload: results);
    }
}
