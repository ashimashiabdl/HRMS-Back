using AutoMapper;
using Castle.Core.Resource;
using Dapper;
using Hr.Employee.infrastructure.Data;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class BankAccountService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<BankAccount, EmployeeContext, BankAccountDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<HR.Employee.Core.Entities.BankAccount>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, CustomDataSource, IgnoreDefaultOrganId);

        if (result.Success && result.Payload is List<BankAccountDTO> list)
        {
            var bankIds = list.Where(x => x.BankId.HasValue)
                              .Select(x => x.BankId!.Value)
                              .Distinct()
                              .ToList();

            if (bankIds.Any())
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Fetch bank titles for the referenced BankIds
                    var sql = @"select b.Id, b.Title from [Payroll].[Bank] b where b.Id in @Ids";
                    var banks = connection.Query<(long Id, string Title)>(sql, new { Ids = bankIds });
                    var dict = banks.ToDictionary(k => k.Id, v => v.Title);

                    foreach (var dto in list)
                    {
                        if (dto.BankId.HasValue && dict.TryGetValue(dto.BankId.Value, out var title))
                        {
                            dto.FncBank = title;
                        }
                    }
                }
            }
        }

        return result;
    }
    public OperationResult GetAsKeyValuePair(long EmployeeId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var sql = $"select acc.id , acc.id as [key]  ,bank.title +' - '+ acc.AccountNumber as value from [emp].Bank_Account acc\r\ninner join [Payroll].[Bank] bank\r\non bank.Id = acc.BankId\r\n\r\nwhere  Status = 1 and acc.IsDeleted <> 1 and EmployeeId =  '{EmployeeId}' order by Priority desc";
            var res = connection.Query<HR.SharedKernel.Data.KeyValuePair>(sql);
            return OperationResult.Succeeded(payload: res);
        }
    }

    public OperationResult GetDefaultAccountNumber(long employeeId)
    {
        if (employeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        var accountNumber = _unitOfWork.Context.BankAccounts
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.IsDeleted != true && x.Status)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.Id)
            .Select(x => x.AccountNumber)
            .FirstOrDefault();

        return OperationResult.Succeeded(payload: accountNumber);
    }
    public new async Task<OperationResult> CreateForAsync(BankAccountDTO entityToCreate)
    {
        try
        {
            var mapped = _mapper.Map<BankAccount>(entityToCreate);
            if (string.IsNullOrEmpty(mapped.title))
            {
                mapped.title = "";
            }
            if (typeof(BankAccount).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                if (_currentUserDefaultOrganId > 0)
                {
                    var propertyInfo = mapped.GetType().GetProperty("OrganisationChartId");
                    propertyInfo.SetValue(mapped, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                }
                else
                {
                    throw new Exception("سازمان پیش فرض مشخض نشده است");
                }
            }

            if (!CheckDateRangeNoOverLap(mapped) && !typeof(BankAccount).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
            {
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }

            _unitOfWork.CreateTransaction();
            try
            {
                // If this record is marked as default (Status = true), set others to false for the same employee
                if (mapped.Status)
                {
                    var others = _unitOfWork.Context.BankAccounts
                        .Where(x => x.EmployeeId == mapped.EmployeeId && x.IsDeleted != true && x.Status == true);
                    foreach (var item in others)
                    {
                        item.Status = false;
                        _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                    }
                }

                Add(mapped);
                await _unitOfWork.Save();
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: mapped.Id);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed();
            }
        }
        catch (Exception)
        {
            return OperationResult.Failed();
        }
    }

    public new async Task<OperationResult> UpdateForAsync(BankAccountDTO entityToUpdate)
    {
        try
        {
            var mapped = _mapper.Map<BankAccount>(entityToUpdate);

            if (!CheckDateRangeNoOverLap(mapped))
            {
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }

            _unitOfWork.CreateTransaction();
            try
            {
                // If this record is marked as default (Status = true), set others to false for the same employee
                if (mapped.Status)
                {
                    var others = _unitOfWork.Context.BankAccounts
                        .Where(x => x.EmployeeId == mapped.EmployeeId && x.Id != mapped.Id && x.IsDeleted != true && x.Status == true);
                    foreach (var item in others)
                    {
                        item.Status = false;
                        _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                    }
                }

                Update(mapped);
                await _unitOfWork.Save();
                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: 1);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed();
            }
        }
        catch (Exception)
        {
            return OperationResult.Failed();
        }
    }
  
}
