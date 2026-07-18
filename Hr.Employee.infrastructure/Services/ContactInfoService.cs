using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class ContactInfoService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.ContactInfo, EmployeeContext, ContactInfoDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new async Task<OperationResult> CreateForAsync(ContactInfoDTO entityToCreate)
    {
        try
        {
            var mapped = _mapper.Map<HR.Employee.Core.Entities.ContactInfo>(entityToCreate);
            if (string.IsNullOrEmpty(mapped.title))
            {
                mapped.title = "";
            }
            if (typeof(HR.Employee.Core.Entities.ContactInfo).GetInterfaces().Contains(typeof(IOrganisationChartId)))
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

            // ContactInfo implements IignoreDateRangeValidation, so we skip date overlap failure
            _unitOfWork.CreateTransaction();
            try
            {
                if (mapped.IsLast)
                {
                    var others = _unitOfWork.Context.ContactInfos
                        .Where(x => x.EmployeeId == mapped.EmployeeId && x.IsDeleted != true && x.IsLast == true);
                    foreach (var item in others)
                    {
                        item.IsLast = false;
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

    public new async Task<OperationResult> UpdateForAsync(ContactInfoDTO entityToUpdate)
    {
        try
        {
            var mapped = _mapper.Map<HR.Employee.Core.Entities.ContactInfo>(entityToUpdate);

            _unitOfWork.CreateTransaction();
            try
            {
                if (mapped.IsLast)
                {
                    var others = _unitOfWork.Context.ContactInfos
                        .Where(x => x.EmployeeId == mapped.EmployeeId && x.Id != mapped.Id && x.IsDeleted != true && x.IsLast == true);
                    foreach (var item in others)
                    {
                        item.IsLast = false;
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
