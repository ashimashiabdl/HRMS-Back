using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypes;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using static HR.SharedKernel.Share.Enums;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationEmployeeTypeSettlementItemService : BaseService<OrganisationEmployeeTypeSettlementItem, SystemSettingContext, OrganisationEmployeeTypeSettlementItemDTO>, IScopedServices
{
    public OrganisationEmployeeTypeSettlementItemService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<OrganisationEmployeeTypeSettlementItem>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        return base.GetPagedData(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            SelectedEmployeeTypeId,
            EmployeeId,
            CustomDataSource ?? BuildQuery(IgnoreExpired),
            IgnoreDefaultOrganId);
    }

    public new OperationResult Get(long id)
    {
        var row = BuildQuery(false).SingleOrDefault(i => i.Id == id);
        if (row == null)
        {
            return OperationResult.NotFound();
        }

        var dto = _mapper.Map<OrganisationEmployeeTypeSettlementItemDTO>(row);
        NormalizeSettlementItem(dto);
        return OperationResult.Succeeded(payload: dto);
    }

    public new async Task<OperationResult> CreateForAsync(OrganisationEmployeeTypeSettlementItemDTO entityToCreate)
    {
        var validation = ValidateEnterType(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        NormalizeSettlementItem(entityToCreate);
        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(OrganisationEmployeeTypeSettlementItemDTO entityToUpdate)
    {
        var validation = ValidateEnterType(entityToUpdate);
        if (!validation.Success)
        {
            return validation;
        }

        NormalizeSettlementItem(entityToUpdate);
        return await base.UpdateForAsync(entityToUpdate);
    }

    private static OperationResult ValidateEnterType(OrganisationEmployeeTypeSettlementItemDTO dto)
    {
        if (!(dto.EnterTypeId > 0))
        {
            return OperationResult.Failed("انتخاب کردن نحوه محاسبه الزامی می باشد");
        }

        if (dto.EnterTypeId == (long)EnterTypeId.UseFormula)
        {
            if (!(dto.OrganisationFormulaId is > 0))
            {
                return OperationResult.Failed("فرمول محاسبه انتخاب نشده است");
            }
        }

        return OperationResult.Succeeded();
    }

    private static void NormalizeSettlementItem(OrganisationEmployeeTypeSettlementItemDTO dto)
    {
        if (dto.EnterTypeId == (long)EnterTypeId.UseFormula)
        {
            // keep OrganisationFormulaId
        }
        else
        {
            dto.OrganisationFormulaId = null;
        }

        if (dto.EnterTypeId != (long)EnterTypeId.fixValue)
        {
            dto.FixValue = null;
        }

        var usesFormula = dto.EnterTypeId == (long)EnterTypeId.UseFormula && dto.OrganisationFormulaId is > 0;
        var usesFixValue = dto.EnterTypeId == (long)EnterTypeId.fixValue;

        dto.IsEditAble = usesFormula || usesFixValue
            ? dto.IsEditAble == true
            : true;
    }

    private IQueryable<OrganisationEmployeeTypeSettlementItem> BuildQuery(bool ignoreExpired)
    {
        return All(ignoreExpired)
            .Include(i => i.EmployeeType)
            .Include(i => i.SettlementItem)
            .Include(i => i.PaymentType)
            .Include(i => i.EnterType)
            .Include(i => i.MeasurementUnit)
            .Include(i => i.OrganisationFormula)
                .ThenInclude(f => f!.Formula);
    }

    public bool Validate(OrganisationEmployeeTypeSettlementItem entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
