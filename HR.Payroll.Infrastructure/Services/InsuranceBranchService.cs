using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class InsuranceBranchService : BaseService<InsuranceBranch, PayrollContext, InsuranceBranchDTO>, IScopedServices
    {
        public InsuranceBranchService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public OperationResult GetAsKeyValuePairId(long id)
        {
            return OperationResult.Succeeded(payload: All().Where(i => i.InsuranceTypeId == id).Include(i => i.InsuranceType).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.InsuranceType.title + " - ( " + i.WorkshopName + " - " + i.WorkshopCode + " ) "
            }));
        }
        public bool Validate(InsuranceBranch entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
