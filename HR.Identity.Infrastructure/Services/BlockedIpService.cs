using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Identity.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.Identity.infrastructure.Services;

public class BlockedIpService : BaseService<BlockedIp, IdentityContext, BlockedIpDTO>, IScopedServices
{
    public BlockedIpService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) 
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
    }

    public bool Validate(BlockedIp entity, object etc = null)
    {
        return true;
    }
}

