using AutoMapper;
using DynamicExpressions.Mapping;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

using HR.Identity.infrastructure.Data;
using HR.Identity.Core.Entities;

namespace HR.Identity.infrastructure.Services;

public class UserCostCenterService : BaseService<UserCostCenter, CustomIdentityContext, UserCostCenterDTO>, IScopedServices
{
    private readonly ILogger<UserCostCenterService>? _logger;
    public UserCostCenterService(IMapper mapper, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<UserCostCenterService> logger) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
        _logger = logger;
    }
    public OperationResult GetAsKeyValuePair(long UserId)
    {
        // ابتدا UserPayLocation های کاربر را دریافت می‌کنیم
        var userPayLocationIds = _db.Set<UserPayLocation>()
            .Where(pl => pl.UserId == UserId)
            .Select(pl => pl.PayLocationId)
            .ToList();

        // فقط UserCostCenter هایی که OrganisationChartId آن‌ها در UserPayLocation های کاربر موجود است
        return OperationResult.Succeeded(payload: All().Include(i => i.CostCenter)
            .Where(i => i.UserId == UserId && userPayLocationIds.Contains(i.OrganisationChartId))
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.CostCenter.Id,
                value = i.CostCenter.title
            }).ToList());
    }
    public OperationResult GetAsKeyValuePairByPayLocationId(long UserId, long payLocationId)
    {
        // ابتدا UserPayLocation های کاربر را دریافت می‌کنیم
        var userPayLocationIds = _db.Set<UserPayLocation>()
            .Where(pl => pl.UserId == UserId)
            .Select(pl => pl.PayLocationId)
            .ToList();

        // بررسی می‌کنیم که payLocationId در UserPayLocation های کاربر موجود است
        if (!userPayLocationIds.Contains(payLocationId))
        {
            // اگر payLocationId در UserPayLocation های کاربر موجود نیست، فهرست خالی برمی‌گردانیم
            return OperationResult.Succeeded(payload: new List<HR.SharedKernel.Data.KeyValuePair>());
        }

        // فقط UserCostCenter هایی که OrganisationChartId آن‌ها در UserPayLocation های کاربر موجود است
        return OperationResult.Succeeded(payload: All().Include(i => i.CostCenter)
            .Where(i => i.UserId == UserId && i.OrganisationChartId == payLocationId && userPayLocationIds.Contains(i.OrganisationChartId))
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.CostCenter.Id,
                value = i.CostCenter.title
            }).ToList());
    }
    public async Task<OperationResult> AssignMultipleAsync(UserCostCenterDTO dto)
    {
        if (dto.UserId == null || dto.CostCenterIds == null || dto.CostCenterIds.Count == 0 || dto.PayLocationId == null)
        {
            return OperationResult.Failed("ورودی نامعتبر است");
        }
        try
        {
            _unitOfWork.CreateTransaction();
            var existingForUser = All(false).Where(i => i.UserId == dto.UserId.Value && i.OrganisationChartId == dto.PayLocationId.Value).ToList();
            _logger?.LogInformation("UserCostCenter AssignMultiple replace start. UserId={UserId}, PayLocationId={PayLocationId}, existingCount={ExistingCount}, startDate={StartDate}, endDate={EndDate}", dto.UserId, dto.PayLocationId, existingForUser.Count, dto.StartDate, dto.EndDate);
            if (existingForUser.Count > 0)
            {
                _logger?.LogInformation("Hard deleting existing UserCostCenter rows. Ids={Ids}", string.Join(",", existingForUser.Select(x => x.Id)));
                _db.Set<UserCostCenter>().RemoveRange(existingForUser);
                await _unitOfWork.Save();
                _logger?.LogInformation("Deleted {Count} existing rows for UserId={UserId}, PayLocationId={PayLocationId}", existingForUser.Count, dto.UserId, dto.PayLocationId);
            }
            foreach (var costCenterId in dto.CostCenterIds.Distinct())
            {
                var entity = new UserCostCenter
                {
                    UserId = dto.UserId.Value,
                    CostCenterId = costCenterId,
                    OrganisationChartId = dto.PayLocationId.Value,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    title = string.Empty
                };
                // After prior removals, overlap check should pass; keep it as a safety
                if (!CheckDateRangeNoOverLap(entity))
                {
                    _logger?.LogWarning("Overlap detected after cleanup for UserId={UserId}, CostCenterId={CostCenterId}, PayLocationId={PayLocationId}, startDate={StartDate}, endDate={EndDate}", dto.UserId, costCenterId, dto.PayLocationId, dto.StartDate, dto.EndDate);
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                }
                Add(entity);
            }
            _logger?.LogInformation("Inserting new UserCostCenter rows. UserId={UserId}, PayLocationId={PayLocationId}, costCenterIds={CostCenterIds}", dto.UserId, dto.PayLocationId, string.Join(",", dto.CostCenterIds.Distinct()));
            var saved = await _unitOfWork.Save();
            if (saved > 0)
            {
                _unitOfWork.Commit();
                _logger?.LogInformation("AssignMultiple completed successfully. UserId={UserId}, PayLocationId={PayLocationId}, insertedCount={InsertedCount}", dto.UserId, dto.PayLocationId, dto.CostCenterIds.Distinct().Count());
                return OperationResult.Succeeded(payload: saved);
            }
            _unitOfWork.Rollback();
            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            _logger?.LogError(ex, "AssignMultiple failed. UserId={UserId}, PayLocationId={PayLocationId}", dto.UserId, dto.PayLocationId);
            throw;
        }
    }
    public bool Validate(UserCostCenter entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
