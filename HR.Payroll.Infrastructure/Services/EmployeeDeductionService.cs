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

public class EmployeeDeductionService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<EmployeeDeduction, PayrollContext, EmployeeDeductionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{

    /// <summary>
    /// حذف فیزیکی رکورد کسور. در صورت استفاده در فیش حقوق، حذف انجام نمی‌شود.
    /// </summary>
    public OperationResult PhysicalDelete(long id)
    {
        var ctx = _unitOfWork.Context;
        if (ctx.Set<FicheItem>().Any(x => x.EmployeeDeductionId == id))
            return OperationResult.Failed("این کسور در فیش حقوق استفاده شده و حذف فیزیکی امکان‌پذیر نیست.");

        var entity = ctx.Set<EmployeeDeduction>().Find(id);
        if (entity == null)
            return OperationResult.Failed("رکورد یافت نشد.");

        try
        {
            _unitOfWork.CreateTransaction();

            var payments = ctx.Set<EmployeeDeductionPayment>().Where(x => x.EmployeeDeductionId == id).ToList();
            foreach (var p in payments)
                ctx.Remove(p);
            ctx.Remove(entity);

            var saveResult = _unitOfWork.Save().Result;
            if (saveResult >= 0)
            {
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: 1);
            }

            _unitOfWork.Rollback();
            return OperationResult.Failed();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}


