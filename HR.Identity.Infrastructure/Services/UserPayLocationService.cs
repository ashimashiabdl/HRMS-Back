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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HR.Identity.infrastructure.Data;

namespace HR.Identity.infrastructure.Services
{
    public class UserPayLocationService : BaseService<UserPayLocation, CustomIdentityContext, UserPayLocationDTO>, IScopedServices
    {

        CustomIdentityContext _CustomIdentityContext;
        private readonly ILogger<UserPayLocationService> _logger;
        public UserPayLocationService(CustomIdentityContext Context, IMapper mapper, IOptions<Identitysetting> config, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<UserPayLocationService> logger) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

            _mapper = mapper;
            _CustomIdentityContext = Context;
            _logger = logger;


        }
        public  OperationResult GetAsKeyValuePair(long UserId)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.PayLocation).Where(i => i.UserId == UserId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.PayLocation.Id,
                value = i.PayLocation.title
            }).ToList());
        }
        public async Task<OperationResult> AssignMultipleAsync(UserPayLocationDTO dto)
        {
            if (dto.UserId == null || dto.PayLocationIds == null || dto.PayLocationIds.Count == 0)
            {
                return OperationResult.Failed("ورودی نامعتبر است");
            }
            try
            {
                _unitOfWork.CreateTransaction();
                var existingForUser = All(false).Where(i => i.UserId == dto.UserId.Value).ToList();
                _logger.LogInformation("UserPayLocation AssignMultiple replace start. UserId={UserId}, existingCount={ExistingCount}, startDate={StartDate}, endDate={EndDate}", dto.UserId, existingForUser.Count, dto.StartDate, dto.EndDate);
                if (existingForUser.Count > 0)
                {
                    _logger.LogInformation("Hard deleting existing UserPayLocation rows. Ids={Ids}", string.Join(",", existingForUser.Select(x => x.Id)));
                    _db.Set<UserPayLocation>().RemoveRange(existingForUser);
                    await _unitOfWork.Save();
                    _logger.LogInformation("Deleted {Count} existing rows for UserId={UserId}", existingForUser.Count, dto.UserId);
                }
                foreach (var plId in dto.PayLocationIds.Distinct())
                {
                    var entity = new UserPayLocation
                    {
                        UserId = dto.UserId.Value,
                        PayLocationId = plId,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                        title = string.Empty
                    };
                    // After prior removals, overlap check should pass; keep it as a safety
                    if (!CheckDateRangeNoOverLap(entity))
                    {
                        _logger.LogWarning("Overlap detected after cleanup for UserId={UserId}, PayLocationId={PayLocationId}, startDate={StartDate}, endDate={EndDate}", dto.UserId, plId, dto.StartDate, dto.EndDate);
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                    }
                    Add(entity);
                }
                _logger.LogInformation("Inserting new UserPayLocation rows. UserId={UserId}, payLocationIds={PayLocationIds}", dto.UserId, string.Join(",", dto.PayLocationIds.Distinct()));
                var saved = await _unitOfWork.Save();
                if (saved > 0)
                {
                    _unitOfWork.Commit();
                    _logger.LogInformation("AssignMultiple completed successfully. UserId={UserId}, insertedCount={InsertedCount}", dto.UserId, dto.PayLocationIds.Distinct().Count());
                    return OperationResult.Succeeded(payload: saved);
                }
                _unitOfWork.Rollback();
                return OperationResult.Failed();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex, "AssignMultiple failed. UserId={UserId}", dto.UserId);
                throw;
            }
        }
        public new async Task<OperationResult> UpdateForAsync(UserPayLocationDTO entityToUpdate)
        {
            try
            {
                var mappedTodo = _mapper.Map<UserPayLocation>(entityToUpdate);

                mappedTodo.UserId = _db.Entry(mappedTodo).GetDatabaseValues().GetValue<long>("UserId");
                Update(mappedTodo);
                if (CheckDateRangeNoOverLap(mappedTodo))
                {
                    if (await _unitOfWork.Save() > 0)
                    {
                        return OperationResult.Succeeded(payload: 1);
                    }
                    else { return OperationResult.Failed(); }
                }
                else
                {
                    return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public bool Validate(UserPayLocation entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
