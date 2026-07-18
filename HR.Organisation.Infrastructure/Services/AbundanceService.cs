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
    public class AbundanceService : BaseService<Abundance, OrganisationContext, AbundanceDTO>, IScopedServices
    {
        public AbundanceService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().OrderBy(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                id = i.Level,
                key = i.Id,
                value = i.title
            }));
        }

        public OperationResult GetAsKeyValuePairdesc()
        {
            return OperationResult.Succeeded(payload: All().OrderBy(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                id = i.Level,
                key = i.Id,
                value = i.title + " " + (i.Description == null ? "" : i.Description)
            }));
        }

       
    }
}
