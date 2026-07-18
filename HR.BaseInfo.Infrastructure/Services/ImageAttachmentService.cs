using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HR.BaseInfo.infrastructure.Services;

public class ImageAttachmentService(
    IMapper mapper,
    IUnitOfWork<BaseInfoContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    IOptions<FileUploadValidationOptions> uploadOptions)
    : BaseService<ImageAttachment, BaseInfoContext, ImageAttachmentDTO>(
        unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private static readonly HashSet<string> AllowedImageMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp",
        "image/bmp",
        "image/svg+xml"
    };

    public async Task<OperationResult> CreateFromFileAsync(IFormFile file, string? title)
    {
        if (file == null || file.Length == 0)
            return OperationResult.Failed("فایل تصویر انتخاب نشده است.");

        var (validationError, mimeType, extension) = ValidateImageFile(file);
        if (validationError != null)
            return OperationResult.Failed(validationError);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var content = ms.ToArray();

        var entity = new ImageAttachment
        {
            title = string.IsNullOrWhiteSpace(title) ? file.FileName : title.Trim(),
            Content = content,
            MimeType = mimeType,
            Extension = extension,
            Size = file.Length
        };

        Add(entity);
        if (await _unitOfWork.Save() > 0)
            return OperationResult.Succeeded(payload: entity.Id);
        return OperationResult.Failed();
    }

    public async Task<OperationResult> UpdateFromFileAsync(long id, IFormFile? file, string? title)
    {
        var existing = _unitOfWork.Context.Set<ImageAttachment>().FirstOrDefault(e => e.Id == id);
        if (existing == null)
            return OperationResult.NotFound();

        if (!string.IsNullOrWhiteSpace(title))
            existing.title = title.Trim();

        if (file != null && file.Length > 0)
        {
            var (validationError, mimeType, extension) = ValidateImageFile(file);
            if (validationError != null)
                return OperationResult.Failed(validationError);

            await using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            existing.Content = ms.ToArray();
            existing.MimeType = mimeType;
            existing.Extension = extension;
            existing.Size = file.Length;
        }

        Update(existing);
        if (await _unitOfWork.Save() > 0)
            return OperationResult.Succeeded(payload: 1);
        return OperationResult.Failed();
    }

    private (string? Error, string? MimeType, string? Extension) ValidateImageFile(IFormFile file)
    {
        var contentType = file.ContentType ?? "";
        if (!AllowedImageMimeTypes.Contains(contentType))
            return ("فرمت فایل باید یکی از تصاویر متداول باشد (JPEG, PNG, GIF, WebP, BMP, SVG).", null, null);

        var ext = Path.GetExtension(file.FileName);
        var allowedExt = uploadOptions.Value.AllowedExtensions ?? new List<string>();
        var allowedSet = new HashSet<string>(allowedExt, StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(ext) || !allowedSet.Contains(ext))
            return ("پسوند فایل نامعتبر است. پسوندهای مجاز طبق تنظیمات سیستم است.", null, null);

        return (null, contentType, ext);
    }
}
