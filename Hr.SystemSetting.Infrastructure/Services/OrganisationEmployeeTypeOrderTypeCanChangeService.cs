using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;

using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;


using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationEmployeeTypeOrderTypeCanChangeService : BaseService<OrganisationEmployeeTypeOrderTypeCanChange, SystemSettingContext, OrganisationEmployeeTypeOrderTypeCanChangeDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeOrderTypeCanChangeService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(OrganisationEmployeeTypeOrderTypeCanChange entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
