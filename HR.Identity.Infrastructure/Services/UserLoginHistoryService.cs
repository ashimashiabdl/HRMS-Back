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
    public class UserLoginHistoryService : BaseService<UserLoginHistory, CustomIdentityContext, UserLoginHistoryDTO>, IScopedServices
    {

        CustomIdentityContext _CustomIdentityContext;
        public UserLoginHistoryService(CustomIdentityContext Context, IMapper mapper, IOptions<Identitysetting> config, IUnitOfWork<CustomIdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _mapper = mapper;
            _CustomIdentityContext = Context;
        }

        public bool Validate(UserLoginHistory entity, object etc = null)
        {
            // اگر ورود ناموفق است، FailReason باید پر باشد
            if (!entity.IsSuccess && string.IsNullOrWhiteSpace(entity.FailReason))
            {
                return false;
            }
            return true;
        }
    }
}
