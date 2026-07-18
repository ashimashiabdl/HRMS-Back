using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class UniversityService : BaseService<University, BaseInfoContext, UniversityDTO>, IScopedServices
    {
        public UniversityService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(University entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
