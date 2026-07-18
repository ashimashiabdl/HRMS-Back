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

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class TaxService : BaseService<Tax, PayrollContext, TaxDTO>, IScopedServices
{
    public TaxService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {

    }
    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All(IgnoreExpired: false).Include(i => i.EmployeeType).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.EmployeeType.title + " ( " + i.CoefficientTax + " ) "
        }));
    }

    //public new async Task<OperationResult> CreateForAsync(TaxDTO entityToCreate)
    //{
    //    try
    //    {
    //        var mappedTodo = _mapper.Map<Tax>(entityToCreate);
    //        if (typeof(Tax).GetInterfaces().Contains(typeof(IOrganisationChartId)))
    //        {
    //            if (_currentUserDefaultOrganId > 0)
    //            {
    //                PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
    //                propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
    //            }
    //            else
    //            {
    //                throw new Exception("سازمان پیش فرض مشخض نشده است");
    //            }

    //        }

    //        if (CheckDateRangeNoOverLap(mappedTodo) || typeof(Tax).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
    //        {
    //            _unitOfWork.CreateTransaction();
    //            try
    //            {
    //                Add(mappedTodo, _IP);
    //                if (await _unitOfWork.Save(_UserName) > 0)
    //                {
    //                    _unitOfWork.Context.Add(new TaxDisketteWk()
    //                    {
    //                        TaxDisketteId = mappedTodo.Id,
    //                        CreateDate = DateTime.Now,
    //                        IPAddress = _IP,
    //                        PaymentTypeId = entityToCreate.PaymentTypeId,
    //                        BankBranchId = entityToCreate.BankBranchId,
    //                        ChequeDate = entityToCreate.ChequeDate,
    //                        ChequeNo = entityToCreate.chequeNo,

    //                    });
    //                    _unitOfWork.Context.SaveChanges();
    //                    _unitOfWork.Commit();
    //                    return OperationResult.Succeeded(payload: mappedTodo.Id);
    //                }
    //                return OperationResult.Failed();
    //            }
    //            catch (Exception ex)
    //            {
    //                _unitOfWork.Rollback();
    //            }

    //            return OperationResult.Failed();

    //        }
    //        else
    //        {
    //            return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, ex.Message);
    //        return OperationResult.Failed();
    //    }

    //}
    public bool Validate(Tax entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
