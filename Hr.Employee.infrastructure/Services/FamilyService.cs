using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class FamilyService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.Family, EmployeeContext, FamilyDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(HR.Employee.Core.Entities.Family entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<HR.Employee.Core.Entities.Family>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, CustomDataSource, IgnoreDefaultOrganId);

        if (result.Success && result.Payload is List<FamilyDTO> list)
        {
            FillBaseTableTitles(list, x => x.DependentTypeId, (dto, title) => dto.DependentType = title, 4);
            FillBaseTableTitles(list, x => x.GenderTypeId, (dto, title) => dto.GenderType = title, 2);
            FillBaseTableTitles(list, x => x.SponsorshipTypeId, (dto, title) => dto.SponsorshipType = title, 10);
        }

        return result;
    }

    private void FillBaseTableTitles(
        List<FamilyDTO> list,
        Func<FamilyDTO, long?> idSelector,
        Action<FamilyDTO, string> titleSetter,
        int baseTableId)
    {
        var ids = list.Where(x => idSelector(x).HasValue)
                      .Select(x => idSelector(x)!.Value)
                      .Distinct()
                      .ToList();

        if (!ids.Any())
        {
            return;
        }

        var dict = _unitOfWork.Context.Set<BaseTableValue>()
            .Where(b => b.BaseTableId == baseTableId && ids.Contains(b.Id))
            .Select(b => new { b.Id, b.title })
            .ToDictionary(k => k.Id, v => v.title);

        foreach (var dto in list)
        {
            var id = idSelector(dto);
            if (id.HasValue && dict.TryGetValue(id.Value, out var title))
            {
                titleSetter(dto, title);
            }
        }
    }
}
