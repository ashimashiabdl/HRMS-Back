using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.Payroll.Core.Data.EmployeeRelated;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using HR.SharedKernel.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace HR.Payroll.Infrastructure.Services;

public class EmployeeSettlementAttachmentService(
    IMapper mapper,
    IUnitOfWork<PayrollContext> unitOfWork,
    BaseInfoContext baseInfoContext,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    IOptions<FileUploadValidationOptions> uploadOptions) : IScopedServices
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork<PayrollContext> _unitOfWork = unitOfWork;
    private readonly BaseInfoContext _baseInfoContext = baseInfoContext;
    private readonly UserResolverService _userService = userService;
    private readonly FileUploadValidationOptions _uploadOptions = uploadOptions.Value;

    public long _currentUserDefaultOrganId { get; set; }

    public OperationResult GetBySettlementId(long settlementId)
    {
        var accessError = ValidateSettlementAccess(settlementId, requireInitialStatus: false);
        if (accessError != null)
        {
            return accessError;
        }

        var items = _unitOfWork.Context.EmployeeSettlementAttachments
            .AsNoTracking()
            .Include(a => a.SettlementDocumentAttachmentType)
            .Include(a => a.File)
            .Where(a => a.EmployeeSettlementId == settlementId && !a.IsDeleted)
            .OrderByDescending(a => a.Id)
            .ToList();

        var dtos = items.Select(MapToDto).ToList();
        return OperationResult.Succeeded(payload: dtos);
    }

    public async Task<OperationResult> UploadAsync(
        long settlementId,
        long settlementDocumentAttachmentTypeId,
        IFormFile file,
        string? description)
    {
        if (file == null || file.Length == 0)
        {
            return OperationResult.Failed("فایل انتخاب نشده است");
        }

        if (file.Length > _uploadOptions.MaxFileSizeBytes)
        {
            return OperationResult.Failed(
                $"حداکثر حجم فایل {_uploadOptions.MaxFileSizeBytes / (1024 * 1024)} مگابایت است.");
        }

        var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
        var allowed = _uploadOptions.AllowedExtensions ?? [];
        if (allowed.Count == 0)
        {
            return OperationResult.Failed("تنظیمات پسوندهای مجاز فایل در دسترس نیست");
        }

        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            return OperationResult.Failed($"نوع فایل ({ext}) مجاز نیست");
        }

        var accessError = ValidateSettlementAccess(settlementId, requireInitialStatus: true);
        if (accessError != null)
        {
            return accessError;
        }

        if (settlementDocumentAttachmentTypeId <= 0)
        {
            return OperationResult.Failed("انتخاب نوع پیوست سند الزامی است");
        }

        var attachmentType = _baseInfoContext.SettlementDocumentAttachmentTypes
            .AsNoTracking()
            .Where(DateValidityExtension<SettlementDocumentAttachmentType>.GetDateValidationPredicate(true))
            .FirstOrDefault(t => t.Id == settlementDocumentAttachmentTypeId && !t.IsDeleted);

        if (attachmentType == null)
        {
            return OperationResult.Failed("نوع پیوست سند تسویه حساب معتبر یافت نشد");
        }

        byte[] content;
        await using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            content = ms.ToArray();
        }

        var now = DateTime.Now;
        var sanitizedName = Helper.SanitizeFileName(file.FileName);
        var fileEntity = new HR.BaseInfo.Core.Entities.File
        {
            CreateDate = now,
            IPAddress = _userService.GetIP(),
            title = sanitizedName,
            IsDeleted = false,
            UniqueId = Guid.NewGuid(),
            Extension = ext,
            Size = file.Length,
            MimeType = Helper.GetMimeType(ext),
            Content = content,
            CreatedBy = _userService.GetUserId().ToString(),
        };

        _baseInfoContext.Files.Add(fileEntity);
        await _baseInfoContext.SaveChangesAsync();

        var attachment = new EmployeeSettlementAttachment
        {
            EmployeeSettlementId = settlementId,
            SettlementDocumentAttachmentTypeId = settlementDocumentAttachmentTypeId,
            FileId = fileEntity.Id,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CreateDate = now,
            StartDate = now,
            IPAddress = _userService.GetIP(),
            IsDeleted = false,
            CreatedBy = _userService.GetUserId().ToString(),
        };

        _unitOfWork.Context.EmployeeSettlementAttachments.Add(attachment);
        await _unitOfWork.Save();

        attachment.SettlementDocumentAttachmentType = attachmentType;
        attachment.File = fileEntity;

        return OperationResult.Succeeded("پیوست با موفقیت ثبت شد", payload: MapToDto(attachment));
    }

    public OperationResult DeleteRecord(long id)
    {
        var attachment = _unitOfWork.Context.EmployeeSettlementAttachments
            .FirstOrDefault(a => a.Id == id && !a.IsDeleted);

        if (attachment == null)
        {
            return OperationResult.NotFound("پیوست یافت نشد");
        }

        var accessError = ValidateSettlementAccess(attachment.EmployeeSettlementId, requireInitialStatus: true);
        if (accessError != null)
        {
            return accessError;
        }

        _unitOfWork.Context.LogicalRemove<EmployeeSettlementAttachment>(attachment.Id);
        _unitOfWork.Context.SaveChanges();

        return OperationResult.Succeeded("پیوست حذف شد");
    }

    public async Task<(byte[]? Content, string? MimeType, string? FileName, OperationResult? Error)> GetFileForDownloadAsync(long attachmentId)
    {
        var attachment = await _unitOfWork.Context.EmployeeSettlementAttachments
            .AsNoTracking()
            .Include(a => a.File)
            .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

        if (attachment == null)
        {
            return (null, null, null, OperationResult.NotFound("پیوست یافت نشد"));
        }

        var accessError = ValidateSettlementAccess(attachment.EmployeeSettlementId, requireInitialStatus: false);
        if (accessError != null)
        {
            return (null, null, null, accessError);
        }

        if (attachment.File == null || attachment.File.IsDeleted)
        {
            return (null, null, null, OperationResult.NotFound("فایل پیوست یافت نشد"));
        }

        if (attachment.File.Content == null || attachment.File.Content.Length == 0)
        {
            var fileFromDb = await _baseInfoContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == attachment.FileId && !f.IsDeleted);

            if (fileFromDb?.Content == null || fileFromDb.Content.Length == 0)
            {
                return (null, null, null, OperationResult.NotFound("محتوای فایل یافت نشد"));
            }

            attachment.File = fileFromDb;
        }

        var fileName = string.IsNullOrWhiteSpace(attachment.File.title)
            ? $"attachment_{attachment.Id}{attachment.File.Extension}"
            : attachment.File.title;

        return (
            attachment.File.Content,
            attachment.File.MimeType ?? Helper.GetMimeType(attachment.File.Extension ?? string.Empty),
            fileName,
            null);
    }

    private OperationResult? ValidateSettlementAccess(long settlementId, bool requireInitialStatus)
    {
        if (settlementId <= 0)
        {
            return OperationResult.Failed("ابتدا تسویه حساب را ذخیره کنید؛ آپلود پیوست فقط پس از دریافت شناسه تسویه امکان‌پذیر است");
        }

        if (_currentUserDefaultOrganId <= 0)
        {
            return OperationResult.Failed("واحد سازمانی جاری مشخص نیست");
        }

        var settlement = _unitOfWork.Context.EmployeeSettlements
            .AsNoTracking()
            .FirstOrDefault(s => s.Id == settlementId && !s.IsDeleted);

        if (settlement == null)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        if (settlement.OrganisationChartId != _currentUserDefaultOrganId)
        {
            return OperationResult.Failed("دسترسی به تسویه حساب این واحد سازمانی مجاز نیست");
        }

        if (requireInitialStatus
            && settlement.SettlementStatusId.HasValue
            && settlement.SettlementStatusId.Value != (long)Enums.SettlementStatus.Initial)
        {
            return OperationResult.Failed("فقط در وضعیت «ایجاد اولیه» امکان مدیریت پیوست‌ها وجود دارد");
        }

        return null;
    }

    private EmployeeSettlementAttachmentDTO MapToDto(EmployeeSettlementAttachment entity)
    {
        var dto = _mapper.Map<EmployeeSettlementAttachmentDTO>(entity);
        dto.SettlementDocumentAttachmentTypeTitle = entity.SettlementDocumentAttachmentType?.title;
        dto.FileTitle = entity.File?.title;
        dto.FileExtension = entity.File?.Extension;
        dto.FileSize = entity.File?.Size ?? 0;
        dto.MimeType = entity.File?.MimeType;
        return dto;
    }
}
