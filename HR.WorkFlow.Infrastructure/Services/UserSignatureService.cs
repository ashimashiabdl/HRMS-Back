using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Services;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Infrastructure.Services;

public class UserSignatureService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, TempGlobalFileService tempGlobalFileService, UserResolverService userService
) : BaseService<HR.WorkFlow.Core.Data.UserSignature, WorkFlowContext, UserSignatureDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{

    private TempGlobalFileService _tempGlobalFileService = tempGlobalFileService;
    
    public OperationResult GetSignatureImageFile(long fileId)
    {
        var file = _tempGlobalFileService.GetMainFile(fileId);
        if (file != null)
        {
            return OperationResult.Succeeded(payload: file);
        }
        return OperationResult.NotFound();
    }
    public new OperationResult Get(long id)
    {
        var Properties = typeof(HR.WorkFlow.Core.Data.UserSignature).GetProperties();
        var all = All(false);
        foreach (var Propertiy in Properties)
        {
            if (Propertiy.PropertyType.BaseType == typeof(Core.Data.UserSignature).BaseType)
            {
                all = all.Include(Propertiy.Name);
            }
        }
        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<UserSignatureDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (record.SignatureImageId > 0)
            {
                var file = _tempGlobalFileService.GetMainFile(record.SignatureImageId);
                record.SignBase64Image = "data:image/" + file.Extension + ";base64," + Convert.ToBase64String(file.Content);
            }

            return OperationResult.Succeeded(payload: record);
        }

    }
    public new async Task<OperationResult> UpdateForAsync(UserSignatureDTO entityToUpdate)
    {
        var mappedTodo = _mapper.Map<Core.Data.UserSignature>(entityToUpdate);
        
        // اگر فایل جدید آپلود شده باشد
        if (entityToUpdate.tempFileId > 0)
        {
            var tempFile = await _tempGlobalFileService.GetIdAsync(entityToUpdate.tempFileId);
            BaseInfo.Core.Entities.File singFile = new()
            {
                Content = tempFile.Content,
                CreateDate = DateTime.Now,
                Extension = tempFile.Extension,
                IPAddress = tempFile.IPAddress,
                MimeType = tempFile.MimeType,
                Size = tempFile.Size,
                UniqueId = tempFile.UniqueId,
                title = tempFile.title
            };
            _tempGlobalFileService._unitOfWork.Context.Files.Add(singFile);
            _tempGlobalFileService._unitOfWork.Context.SaveChanges();
            mappedTodo.SignatureImageId = singFile.Id;
            if (singFile.Id > 0)
            {

            }
            else
            {
                return OperationResult.Failed("خطا در ذخیر فایل نقش امضا");
            }
        }
        // در حالت ویرایش، اگر فایل جدید آپلود نشده، از فایل موجود استفاده می‌کنیم
        else
        {
            // اگر SignatureImageId در DTO موجود باشد، از آن استفاده می‌کنیم
            if (entityToUpdate.SignatureImageId > 0)
            {
                mappedTodo.SignatureImageId = entityToUpdate.SignatureImageId;
            }
            // در غیر این صورت، از entity موجود در دیتابیس SignatureImageId را می‌گیریم
            else
            {
                var existingEntity = All(false).SingleOrDefault(x => x.Id == entityToUpdate.Id);
                if (existingEntity != null && existingEntity.SignatureImageId > 0)
                {
                    mappedTodo.SignatureImageId = existingEntity.SignatureImageId;
                }
                else
                {
                    return OperationResult.Failed("فایل نقش امضا مشخص نشده است");
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

    public new async Task<OperationResult> CreateForAsync(UserSignatureDTO entityToCreate)
    {
        try
        {
            var mappedTodo = _mapper.Map<Core.Data.UserSignature>(entityToCreate);

            if (entityToCreate.tempFileId > 0)
            {
                var tempFile = await _tempGlobalFileService.GetIdAsync(entityToCreate.tempFileId);

                BaseInfo.Core.Entities.File singFile = new()
                {
                    Content = tempFile.Content,
                    CreateDate = DateTime.Now,
                    Extension = tempFile.Extension,
                    IPAddress = tempFile.IPAddress,
                    MimeType = tempFile.MimeType,
                    Size = tempFile.Size,
                    UniqueId = tempFile.UniqueId,
                    title = tempFile.title

                };

                _tempGlobalFileService._unitOfWork.Context.Files.Add(singFile);
                _tempGlobalFileService._unitOfWork.Context.SaveChanges();

                if (typeof(Core.Data.UserSignature).GetInterfaces().Contains(typeof(IOrganisationChartId)))
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

                if (CheckDateRangeNoOverLap(mappedTodo) || typeof(HR.WorkFlow.Core.Data.UserSignature).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
                {
                    mappedTodo.SignatureImageId = singFile.Id;

                    if (singFile.Id > 0)
                    {

                    }
                    else
                    {
                        return OperationResult.Failed("خطا در ذخیر فایل نقش امضا");
                    }
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
            else
            {
                return OperationResult.Failed("فایل نقش امضا مشخص نشده است");
            }


        }
        catch (Exception ex)
        {
            return OperationResult.Failed();
        }

    }
    public OperationResult GetUserSignaturesForCurrentUser(long userId, long organisationChartId)
    {
        try
        {
            var userSignatures = All(false)
                .Where(us => us.AspNetUsersId == userId && us.OrganisationChartId == organisationChartId && us.Enabled)
                .ToList();

            var userSignatureDTOs = _mapper.Map<List<UserSignatureDTO>>(userSignatures);
            
            // اضافه کردن Base64 برای هر امضا
            foreach (var dto in userSignatureDTOs)
            {
                if (dto.SignatureImageId > 0)
                {
                    try
                    {
                        var file = _tempGlobalFileService.GetMainFile(dto.SignatureImageId);
                        dto.SignBase64Image = "data:image/" + file.Extension + ";base64," + Convert.ToBase64String(file.Content);
                    }
                    catch
                    {
                        // اگر فایل یافت نشد، ادامه بده
                    }
                }
            }

            return OperationResult.Succeeded(payload: userSignatureDTOs);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطا در دریافت امضاهای کاربر: " + ex.Message);
        }
    }

    public bool Validate(Core.Data.UserSignature entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
