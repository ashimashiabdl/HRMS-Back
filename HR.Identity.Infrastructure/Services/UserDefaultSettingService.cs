using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HR.Identity.infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HR.Identity.infrastructure.Services
{
    public class UserDefaultSettingService : BaseService<UserDefaultSetting, CustomIdentityContext, UserDefaultSettingDTO>, IScopedServices
    {

        CustomIdentityContext _CustomIdentityContext;
        UserResolverService _userService;
        public UserDefaultSettingService(CustomIdentityContext Context, IMapper mapper, IOptions<Identitysetting> config, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _mapper = mapper;
            _CustomIdentityContext = Context;
            _userService = userService;
        }
        public new async Task<OperationResult> UpdateForAsync(UserDefaultSettingDTO entityToUpdate)
        {
            var currentUserId = _userService.GetUserId();

            var context = _unitOfWork.Context;

            var existing = await context.Set<UserDefaultSetting>()
                .FirstOrDefaultAsync(x => x.UserId == currentUserId);

            if (existing == null)
            {
                var newEntity = new UserDefaultSetting()
                {
                    UserId = currentUserId,
                    DefaultOrganId = entityToUpdate.DefaultOrganId,
                    DefaultWorkPlaceId = entityToUpdate.DefaultWorkPlaceId,
                    DefaultCostCenterId = entityToUpdate.DefaultCostCenterId,
                    DefaultOrganizationUnitId = entityToUpdate.DefaultOrganizationUnitId,
                    DefaultPaymentPeriodId = entityToUpdate.DefaultPaymentPeriodId,
                    IPAddress = _userService.GetIP(),
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                };

                context.Set<UserDefaultSetting>().Add(newEntity);
                try
                {
                    await _unitOfWork.Save();
                    return OperationResult.Succeeded(payload: 1);
                }
                catch (Exception)
                {
                    var already = await context.Set<UserDefaultSetting>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.UserId == currentUserId);
                    if (already != null)
                        return OperationResult.Succeeded(payload: 1);
                    return OperationResult.Failed();
                }
            }
            else
            {
                existing.DefaultOrganId = entityToUpdate.DefaultOrganId;
                existing.DefaultWorkPlaceId = entityToUpdate.DefaultWorkPlaceId;
                existing.DefaultCostCenterId = entityToUpdate.DefaultCostCenterId;
                existing.DefaultOrganizationUnitId = entityToUpdate.DefaultOrganizationUnitId;
                existing.DefaultPaymentPeriodId = entityToUpdate.DefaultPaymentPeriodId;
                existing.IPAddress = _userService.GetIP();

                try
                {
                    await _unitOfWork.Save();
                    return OperationResult.Succeeded(payload: 1);
                }
                catch (Exception)
                {
                    return OperationResult.Failed();
                }
            }
        }
        public async Task<OperationResult> UpsertForUserAsync(UserDefaultSettingDTO entityToUpdate)
        {
            if (entityToUpdate == null || entityToUpdate.UserId <= 0)
            {
                return OperationResult.Failed("شناسه کاربر نامعتبر است");
            }

            var context = _unitOfWork.Context;

            var existing = await context.Set<UserDefaultSetting>()
                .FirstOrDefaultAsync(x => x.UserId == entityToUpdate.UserId);

            if (existing == null)
            {
                var newEntity = new UserDefaultSetting()
                {
                    UserId = entityToUpdate.UserId,
                    DefaultOrganId = entityToUpdate.DefaultOrganId,
                    DefaultWorkPlaceId = entityToUpdate.DefaultWorkPlaceId,
                    DefaultCostCenterId = entityToUpdate.DefaultCostCenterId,
                    DefaultOrganizationUnitId = entityToUpdate.DefaultOrganizationUnitId,
                    DefaultPaymentPeriodId = entityToUpdate.DefaultPaymentPeriodId,
                    IPAddress = _userService.GetIP(),
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                };

                context.Set<UserDefaultSetting>().Add(newEntity);
                try
                {
                    await _unitOfWork.Save();
                    return OperationResult.Succeeded(payload: 1);
                }
                catch (Exception)
                {
                    // Handle race: if another request inserted it meanwhile, treat as success
                    var already = await context.Set<UserDefaultSetting>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.UserId == entityToUpdate.UserId);
                    if (already != null)
                        return OperationResult.Succeeded(payload: 1);
                    return OperationResult.Failed();
                }
            }
            else
            {
                existing.DefaultOrganId = entityToUpdate.DefaultOrganId;
                existing.DefaultWorkPlaceId = entityToUpdate.DefaultWorkPlaceId;
                existing.DefaultCostCenterId = entityToUpdate.DefaultCostCenterId;
                existing.DefaultOrganizationUnitId = entityToUpdate.DefaultOrganizationUnitId;
                existing.DefaultPaymentPeriodId = entityToUpdate.DefaultPaymentPeriodId;
                existing.IPAddress = _userService.GetIP();

                try
                {
                    await _unitOfWork.Save();
                    return OperationResult.Succeeded(payload: 1);
                }
                catch (Exception)
                {
                    return OperationResult.Failed();
                }
            }
        }
        public async Task<OperationResult> GetByUserId(long userId)
        {
            if (userId <= 0)
            {
                return OperationResult.Failed("شناسه کاربر نامعتبر است");
            }

            var context = _unitOfWork.Context;
            var currentSetting = await context.Set<UserDefaultSetting>()
                .AsNoTracking()
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.UserId == userId);

            if (currentSetting == null)
            {
                return OperationResult.NotFound();
            }

            var dto = _mapper.Map<UserDefaultSettingDTO>(currentSetting);
            return OperationResult.Succeeded(payload: dto);
        }
        public async Task<OperationResult> GetCurrentUserDefultSettingAndInsertIfNotExist(long CurrentUserId)
        {
            var context = _unitOfWork.Context;

            var currentSetting = await context.Set<UserDefaultSetting>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.UserId == CurrentUserId);

            if (currentSetting != null)
            {
                return OperationResult.Succeeded(payload: currentSetting);
            }

            var newSetting = new UserDefaultSetting()
            {
                UserId = CurrentUserId,
                CreateDate = DateTime.Now,
                IPAddress = "",
                IsDeleted = false,
            };

            context.Set<UserDefaultSetting>().Add(newSetting);
            try
            {
                await _unitOfWork.Save();
                return OperationResult.Succeeded(payload: newSetting);
            }
            catch (Exception)
            {
                var existing = await context.Set<UserDefaultSetting>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.UserId == CurrentUserId);
                if (existing != null)
                {
                    return OperationResult.Succeeded(payload: existing);
                }
                return OperationResult.Failed();
            }
        }
        public bool Validate(UserDefaultSetting entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
