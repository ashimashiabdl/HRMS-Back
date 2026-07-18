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

namespace HR.Organisation.Infrastructure.Services;

public class OrganizationJobService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganizationJob, OrganisationContext, OrganizationJobDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    /// <summary>
    /// فهرست [Org].[Organisation_Job] به‌صورت KeyValuePair.
    /// key = Organisation_Job.Id (مطابق انتظار SP و فیلترها).
    /// </summary>
    public OperationResult GetAsKeyValuePair(long id = 0)
    {
        return GetAsKeyValuePairByOrganisationChartId(_currentUserDefaultOrganId);
    }

    public OperationResult GetAsKeyValuePairByOrganisationChartId(long organisationChartId)
    {
        return OperationResult.Succeeded(payload: All().Include(i => i.Job).Where(i => i.OrganisationChartId == organisationChartId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = (i.Job != null ? i.Job.title : "") + " ( " + (string.IsNullOrEmpty(i.Code) ? "فاقد کد شغل" : i.Code) + " ) " + " گروه " + i.JobDegree
        }));
    }

 
    public bool Validate(OrganizationJob entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
