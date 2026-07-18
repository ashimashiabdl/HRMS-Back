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
    public class OrganisationJobCategoryService : BaseService<OrganisationJobCategory, OrganisationContext, OrganisationJobCategoryDTO>, IScopedServices
    {
        public OrganisationJobCategoryService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        /// <summary>
        /// فهرست رسته های سازمان جاری
        /// </summary>
        /// <returns></returns>
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i=>i.JobCategory).Where(i=>i.OrganisationChartId == _currentUserDefaultOrganId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.JobCategory == null ? "نا مشخص" : i.JobCategory.title
            }));
        }
        public bool Validate(OrganisationJobCategory entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
