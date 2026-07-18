using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services
{
    public class InsuranceDetailService : BaseService<HR.Employee.Core.Entities.InsuranceDetail, EmployeeContext, InsuranceDetailDTO>, IScopedServices
    {
        public InsuranceDetailService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(HR.Employee.Core.Entities.InsuranceDetail entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
