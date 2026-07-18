using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using System.Linq;

using System.Data.Entity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.BaseInfo.Core.DTOs;
using System;
using HR.SharedKernel.Interaces;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class JobCategoryService : BaseService<JobCategory, BaseInfoContext, JobCategoryDTO>, IScopedServices
    {
        public JobCategoryService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(JobCategory entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
