using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.Organisation.Infrastructure.Services;

public class OrganisationPositionJobService(
    IMapper mapper,
    IUnitOfWork<OrganisationContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService
) : BaseService<OrganisationPositionJob, OrganisationContext, OrganisationPositionJobDTO>(
    unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetCurrentPositionJobs(long organisationPositionId)
    {
        return OperationResult.Succeeded(payload: All()
            .Where(i => i.OrganisationPositionId == organisationPositionId)
            .OrderByDescending(i => i.Id)
            .Include(i => i.OrganizationJob)
            .Include(i => i.OrganizationJob!.Job)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.OrganizationJob!.Id,
                value = i.OrganizationJob.Job!.title
                    + (string.IsNullOrEmpty(i.OrganizationJob.Code) ? "" : i.OrganizationJob.Code)
                    + " - گروه " + i.OrganizationJob.JobDegree.ToString()
            }));
    }

    public bool Validate(OrganisationPositionJob entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
