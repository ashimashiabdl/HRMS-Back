using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using HR.SharedKernel.DTOs;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeImageService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.Image, EmployeeContext, ImageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public async Task<OperationResult> UploadUserImage(ImageDTO dto)
    {
        var image = _mapper.Map<HR.Employee.Core.Entities.Image>(dto);
        if (string.IsNullOrWhiteSpace(dto.ImageDataBase64))
        {
            return OperationResult.Failed("فایل تصویر ارسال نشده است");
        }

        // پشتیبانی از انواع تصویر رایج در data URI
        var prefixes = new[] {
            "data:image/jpeg;base64,",
            "data:image/jpg;base64,",
            "data:image/png;base64,",
            "data:image/gif;base64,",
            "data:image/bmp;base64,",
            "data:image/webp;base64,"
        };
        var cleaned = dto.ImageDataBase64;
        var matchedPrefix = prefixes.FirstOrDefault(p => cleaned.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        if (matchedPrefix == null)
        {
            return OperationResult.Failed("فرمت تصویر پشتیبانی نمی‌شود");
        }
        cleaned = cleaned.Replace(matchedPrefix, "");
        try
        {
            image.ImageData = Convert.FromBase64String(cleaned);
        }
        catch
        {
            return OperationResult.Failed("داده تصویر نامعتبر است");
        }

        long maxlength = 0;
        var maxSizeSetting = GetSettingById(10008);

        if (string.IsNullOrEmpty(maxSizeSetting))
        {
            maxlength = HR.SharedKernel.Share.Constants.defaultMaxUploadSize;
        }
        else
        {
            maxlength = Convert.ToInt64(maxSizeSetting);
        }
        if (maxlength > (image.ImageData.Length / 1000))
        {

        }
        else
        {
            return OperationResult.Failed("حجم فایل ارسالی بیش از مقدار مجاز تعیین شده می باشد " + maxlength + " KB ");
        }
        image.IsDefault = true;
        image.OrganisationChartId = _currentUserDefaultOrganId;

        var existingImage = All().Where(i => i.EmployeeId == image.EmployeeId);
        if (existingImage != null)
        {
            if (existingImage.Any())
            {
                image.Id = existingImage.ToList().Single().Id;
                Update(image);
                // Set Shadow Property for ImageTypeId
                _unitOfWork.Context.Entry(image).Property("ImageTypeId").CurrentValue = (long)1166;
                if (await _unitOfWork.Save() > 0)
                {
                    return OperationResult.Succeeded(payload: image.Id);
                }
            }
        }
        Add(image);
        // Set Shadow Property for ImageTypeId
        _unitOfWork.Context.Entry(image).Property("ImageTypeId").CurrentValue = (long)1166;
        if (await _unitOfWork.Save() > 0)
        {
            return OperationResult.Succeeded(payload: image.Id);
        }
        return OperationResult.Failed();
    }
    public byte[] UserImage(long EmployeeId)
    {
        // Single, optimized no-tracking query that projects only the ImageData column
        // Avoids multiple queries and navigation includes from All()
        return _unitOfWork.Context.Set<HR.Employee.Core.Entities.Image>()
            .AsNoTracking()
            .Where(i => i.EmployeeId == EmployeeId)
            .OrderByDescending(i => i.Id)
            .Select(i => i.ImageData)
            .FirstOrDefault();

    }
}
