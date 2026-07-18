using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class CarouselService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<Carousel, BaseInfoContext, CarouselDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<Carousel>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        IQueryable<Carousel> dataSource = All(IgnoreExpired);

        if (CustomDataSource != null)
        {
            dataSource = CustomDataSource;
        }

        if (string.IsNullOrEmpty(activeSortColumn))
        {
            dataSource = dataSource.OrderBy(x => x.Priority).ThenBy(x => x.Id);
        }

        return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, dataSource, IgnoreDefaultOrganId);
    }

    public OperationResult GetActiveCarousels()
    {
        try
        {
            var items = All(true)
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Id)
                .ToList();

            var dtos = _mapper.Map<List<CarouselDTO>>(items);
            return OperationResult.Succeeded(payload: dtos);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }
}
