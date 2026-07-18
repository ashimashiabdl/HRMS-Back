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
    public class OrganisationJobSeriesService : BaseService<OrganisationJobSeries, OrganisationContext, OrganisationJobSeriesDTO>, IScopedServices
    {
        public OrganisationJobSeriesService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        /// <summary>
        /// فهرست گروه ( طبقه ) های رشته انتخابی
        /// </summary>
        /// <param name="JobGroupId"></param>
        /// <returns></returns>
        public OperationResult GetAsKeyValuePair(long JobGroupId)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.OrganisationJobCategory).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId && i.OrganisationJobGroupId == JobGroupId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.JobSeries == null ? "نا مشخص" : i.JobSeries.title
            }));
        }
        public bool Validate(OrganisationJobSeries entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
