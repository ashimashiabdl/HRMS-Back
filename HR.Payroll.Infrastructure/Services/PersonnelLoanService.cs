using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace HR.Payroll.Infrastructure.Services
{
    public class PersonnelLoanService : BaseService<PersonnelLoan, PayrollContext, PersonnelLoanDTO>, IScopedServices
    {
        public PersonnelLoanService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public async Task<OperationResult> CreateForAsync(PersonnelLoanDTO entityToCreate)
        {
            try
            {
                var mappedTodo = _mapper.Map<PersonnelLoan>(entityToCreate);
                if (typeof(PersonnelLoan).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                {
                    if (_currentUserDefaultOrganId > 0)
                    {
                        PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                        propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        throw new Exception("سازمان پیش فرض مشخض نشده است");
                    }

                }
                var loanTypeSetting = _unitOfWork.Context.LoanTypes.Find(mappedTodo.LoanTypeId);

                if (loanTypeSetting == null)
                {
                    return OperationResult.Failed("نوع وام ارسالی یافت نشد");
                }
                if (loanTypeSetting.IsActive != true)
                {
                    return OperationResult.Failed("نوع وام انتخابی غیرفعال است");
                }
                else
                {
                    if (loanTypeSetting.MaxAmount >= mappedTodo.AllAmount)
                    {

                    }
                    else
                    {
                        if (loanTypeSetting.MaxAmount.HasValue)
                        {
                            return OperationResult.Failed("مبلغ وارد شده از حداکثر مجاز ثبت شده برای این وام بیشتر می باشد" + " " + $" حداکثر مبلغ مجاز این وام : {loanTypeSetting.MaxAmount.Value.ToString("N")} ریال می باشد");
                        }
                        else
                        {
                            return OperationResult.Failed("حداکثر مبلغ این وام مشخص نشده است");

                        }
                    }
                }
                if (CheckDateRangeNoOverLap(mappedTodo) || typeof(PersonnelLoan).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
                {
                    Add(mappedTodo);
                    if (await _unitOfWork.Save() > 0)
                    {
                        return OperationResult.Succeeded(payload: mappedTodo.Id);
                    }
                    return OperationResult.Failed();
                }
                else
                {
                    return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                }
            }
            catch (Exception ex)
            {
                return OperationResult.Failed();
            }

        }
        public new async Task<OperationResult> UpdateForAsync(PersonnelLoanDTO entityToUpdate)
        {
            try
            {
                var mappedTodo = _mapper.Map<PersonnelLoan>(entityToUpdate);
                var existingPayment = _unitOfWork.Context.PersonnelLoanPayments.Where(i => i.PersonnelLoanId == entityToUpdate.Id);
                if (existingPayment == null)
                {

                }
                else
                {
                    if (existingPayment.Any())
                    {
                        if (entityToUpdate.RemainingCrumbsAtFirst == _db.Entry(mappedTodo).GetDatabaseValues().GetValue<bool>("RemainingCrumbsAtFirst"))
                        {

                        }
                        else
                        {
                            return OperationResult.Failed("برای وام انتخابی اقساط پرداخت شده وجود دارد، امکان تغییر تنظیمات خرده مانده وجود ندارد");
                        }
                    }
                }

                var loanTypeSetting = _unitOfWork.Context.LoanTypes.Find(mappedTodo.LoanTypeId);

                if (loanTypeSetting == null)
                {
                    return OperationResult.Failed("نوع وام ارسالی یافت نشد");
                }
                if (loanTypeSetting.IsActive != true && mappedTodo.IsActive)
                {
                    return OperationResult.Failed("نوع وام انتخابی غیرفعال است و امکان فعال‌سازی وام وجود ندارد");
                }
                else
                {
                    if (loanTypeSetting.MaxAmount >= mappedTodo.AllAmount)
                    {

                    }
                    else
                    {
                        if (loanTypeSetting.MaxAmount.HasValue)
                        {
                            var nfi = new NumberFormatInfo()
                            {
                                NumberDecimalDigits = 0,
                                NumberGroupSeparator = "."
                            };
                            return OperationResult.Failed("مبلغ وارد شده از حداکثر مجاز ثبت شده برای این وام بیشتر می باشد" + " " + $" حداکثر مبلغ مجاز این وام : {loanTypeSetting.MaxAmount.Value.ToString("N", nfi)} ریال می باشد");
                        }
                        else
                        {
                            return OperationResult.Failed("حداکثر مبلغ این وام مشخص نشده است");

                        }
                    }
                }

                Update(mappedTodo);
                if (CheckDateRangeNoOverLap(mappedTodo))
                {
                    if (await _unitOfWork.Save() > 0)
                    {
                        return OperationResult.Succeeded(payload: 1);
                    }
                    else { return OperationResult.Failed(); }
                }
                else
                {
                    return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                }
            }
            catch (Exception ex)
            {
                return OperationResult.Failed();
            }

        }
        public bool Validate(PersonnelLoan entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
