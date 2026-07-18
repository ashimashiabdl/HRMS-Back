using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Organisation.Infrastructure.Services
{
    public class OrganisationJobGroupService : BaseService<OrganisationJobGroup, OrganisationContext, OrganisationJobGroupDTO>, IScopedServices
    {
        public OrganisationJobGroupService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        /// <summary>
        /// فهرست رشته های رسته انتخابی
        /// </summary>
        /// <param name="JobCategoryId"></param>
        /// <returns></returns>
        public OperationResult GetAsKeyValuePair(long JobCategoryId)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.JobGroup).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId && i.OrganisationJobCategoryId == JobCategoryId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.JobGroup == null ? "نا مشخص" : i.JobGroup.title
            }));
        }
        public bool Validate(OrganisationJobGroup entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
