using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class BankBranchService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<BankBranch, PayrollContext, BankBranchDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetAsKeyValuePair(long id)
    {
        return OperationResult.Succeeded(payload: All().Where(i => i.BankId == id || id == 0).Include(i => i.Bank).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.Bank.title + " شعبه " + " ( " + i.title + (string.IsNullOrEmpty(i.Code) ? "" : i.Code) + " ) "
        }));
    }
}