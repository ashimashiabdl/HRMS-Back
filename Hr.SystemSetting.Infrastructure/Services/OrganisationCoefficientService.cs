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
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationCoefficientService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<OrganisationCoefficient, SystemSettingContext, OrganisationCoefficientDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload:

                   All()
        .Include(i => i.Coefficient)
        .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)

            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.CoefficientId,
                value = i.Coefficient.title
            }));
    }
    public bool Validate(OrganisationCoefficient entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
