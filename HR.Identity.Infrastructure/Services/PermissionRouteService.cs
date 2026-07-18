using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Identity.infrastructure.Services;

public class PermissionRouteService : BaseService<PermissionRoute, IdentityContext, PermissionRouteDTO>, IScopedServices
{
    public PermissionRouteService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
    }
}
