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
    public class OrganizationJobRequiredSoftwaresQualificationService : BaseService<OrganizationJobRequiredSoftwaresQualification, OrganisationContext, OrganizationJobRequiredSoftwaresQualificationDTO>, IScopedServices
    {
        public OrganizationJobRequiredSoftwaresQualificationService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(OrganizationJobRequiredSoftwaresQualification entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
