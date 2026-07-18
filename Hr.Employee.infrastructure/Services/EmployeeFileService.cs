using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using System.Reflection;
using HR.Employee.Core.Entities;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services
{
    public class EmployeeFileService : BaseService<HR.Employee.Core.Entities.EmployeeFile, EmployeeContext, EmployeeFileDTO>, IScopedServices
    {
        public EmployeeFileService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        private static OperationResult? ValidateOtherFileGroup(EmployeeFileDTO entity)
        {
            if (entity.FileGroupId == HR.Employee.Core.Entities.EmployeeFile.OtherFileGroupId)
            {
                if (string.IsNullOrWhiteSpace(entity.OtherFileGroupName))
                {
                    return OperationResult.Failed("عنوان نوع فایل الزامی است");
                }

                entity.OtherFileGroupName = entity.OtherFileGroupName.Trim();
            }
            else
            {
                entity.OtherFileGroupName = null;
            }

            return null;
        }

        private static void NormalizeOtherFileGroup(HR.Employee.Core.Entities.EmployeeFile entity)
        {
            if (entity.FileGroupId == HR.Employee.Core.Entities.EmployeeFile.OtherFileGroupId)
            {
                entity.OtherFileGroupName = entity.OtherFileGroupName?.Trim();
            }
            else
            {
                entity.OtherFileGroupName = null;
            }
        }
        public OperationResult downloadFile(long id)
        {
            var EmployeeFile = GetIdAsync(id).Result;
            var File = _db.Set<HR.Employee.Core.Entities.File>().Find(EmployeeFile.FileId);
            string temp_inBase64 = Convert.ToBase64String(File.Content);
            EmployeeFileDTO ret = new EmployeeFileDTO()
            {
                temp_inBase64 = temp_inBase64,
                MimeType = File.MimeType,
                Name = File.title,
                Extension = File.Extension,
            };
            return OperationResult.Succeeded(payload: ret);
        }
        public new async Task<OperationResult> CreateForAsync(EmployeeFileDTO entityToCreate)
        {
            var validationResult = ValidateOtherFileGroup(entityToCreate);
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var mappedTodo = _mapper.Map<HR.Employee.Core.Entities.EmployeeFile>(entityToCreate);
                NormalizeOtherFileGroup(mappedTodo);
                if (typeof(HR.Employee.Core.Entities.EmployeeFile).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                {
                    PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                    propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                }

                if (CheckDateRangeNoOverLap(mappedTodo) || typeof(HR.Employee.Core.Entities.EmployeeFile).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
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

                throw;
            }

        }

        public new async Task<OperationResult> UpdateForAsync(EmployeeFileDTO entityToUpdate)
        {
            var validationResult = ValidateOtherFileGroup(entityToUpdate);
            if (validationResult != null)
            {
                return validationResult;
            }

            var mappedTodo = _mapper.Map<HR.Employee.Core.Entities.EmployeeFile>(entityToUpdate);
            NormalizeOtherFileGroup(mappedTodo);

            if (typeof(HR.Employee.Core.Entities.EmployeeFile).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
            }

            if (!CheckDateRangeNoOverLap(mappedTodo)
                && !typeof(HR.Employee.Core.Entities.EmployeeFile).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
            {
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }

            Update(mappedTodo);
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: mappedTodo.Id);
            }

            return OperationResult.Failed();
        }
    
    }
}
