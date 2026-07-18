using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.BaseInfo.infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class AgentOfPunishmentEncourageService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IConfiguration configuration, UserResolverService userService
) : BaseService<AgentOfPunishmentEncourage, BaseInfoContext, AgentOfPunishmentEncourageDTO>(unitOfWork.Context, mapper, unitOfWork, null, configuration, userService), IScopedServices
{
    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title + " ( " + (i.IsPunishment == true ? " تنبیه " : " تشویق") + " ) "
        }));
    }


}
