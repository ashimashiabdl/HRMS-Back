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
    public class UserOrganizationUnitService : BaseService<UserOrganizationUnit, CustomIdentityContext, UserOrganizationUnitDTO>, IScopedServices
    {

        CustomIdentityContext _CustomIdentityContext;
        public UserOrganizationUnitService(CustomIdentityContext Context, IMapper mapper, IOptions<Identitysetting> config, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

            _mapper = mapper;
            _CustomIdentityContext = Context;


        }
        public OperationResult GetAsKeyValuePair(long UserId)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.OrganizationUnit).Where(i => i.UserId == UserId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.OrganizationUnit.Id,
                value = i.OrganizationUnit.title
            }));
        }
        public new async Task<OperationResult> UpdateForAsync(UserOrganizationUnitDTO entityToUpdate)
        {
            try
            {
                var mappedTodo = _mapper.Map<UserOrganizationUnit>(entityToUpdate);
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
        public bool Validate(UserOrganizationUnit entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
