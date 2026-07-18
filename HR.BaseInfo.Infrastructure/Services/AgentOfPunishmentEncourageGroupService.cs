using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.BaseInfo.infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class AgentOfPunishmentEncourageGroupService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IConfiguration configuration, UserResolverService userService
) : BaseService<AgentOfPunishmentEncourageGroup, BaseInfoContext, AgentOfPunishmentEncourageGroupDTO>(unitOfWork.Context, mapper, unitOfWork, null, configuration, userService), IScopedServices
{
   
}
