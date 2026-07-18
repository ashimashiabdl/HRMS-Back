using AutoMapper;
using HR.BaseInfo.Core.Entities;

using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.BaseInfo.Core.DTOs;
using System;
using HR.SharedKernel.Interaces;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class JobSeriesService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<JobSeries, BaseInfoContext, JobSeriesDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(JobSeries entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
