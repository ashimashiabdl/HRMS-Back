using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.Build.Tasks;


using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationMRTService : BaseService<OrganisationMRT, SystemSettingContext, OrganisationMRTDTO>, IScopedServices
    {
        public OrganisationMRTService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.Id + " - " + i.title
            }));
        }

        public OperationResult downloadMRT(long id)
        {
            var MRT = GetIdAsync(id).Result;
            string temp_inBase64 = Convert.ToBase64String(MRT.Content);
            return OperationResult.Succeeded(payload: temp_inBase64, msg: MRT.title);
        }
        public bool Validate(OrganisationMRT entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
