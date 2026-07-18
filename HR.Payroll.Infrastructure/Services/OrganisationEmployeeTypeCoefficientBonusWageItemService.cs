using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class OrganisationEmployeeTypeCoefficientBonusWageItemService : BaseService<OrganisationEmployeeTypeCoefficientBonusWageItem, PayrollContext, OrganisationEmployeeTypeCoefficientBonusWageItemDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeCoefficientBonusWageItemService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
        }

        public bool Validate(OrganisationEmployeeTypeCoefficientBonusWageItem entity, object etc = null)
        {
            return true;
        }
    }
}


