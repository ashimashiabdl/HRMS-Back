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
    public class UserWorkPlaceService : BaseService<UserWorkPlace, CustomIdentityContext, UserWorkPlaceDTO>, IScopedServices
    {
        public UserWorkPlaceService(CustomIdentityContext Context, IMapper mapper, IOptions<Identitysetting> config, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _mapper = mapper;
        }
        public OperationResult GetAsKeyValuePair(long UserId)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.WorkPlace).Where(i => i.UserId == UserId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.WorkPlace.Id,
                value = i.WorkPlace.title
            }));
        }
        public new async Task<OperationResult> UpdateForAsync(UserWorkPlaceDTO entityToUpdate)
        {
            try
            {
                var mappedTodo = _mapper.Map<UserWorkPlace>(entityToUpdate);

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
        public bool Validate(UserWorkPlace entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
