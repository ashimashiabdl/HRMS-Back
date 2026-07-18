using AutoMapper;
using HR.BaseInfo.infrastructure.Services;
using HR.Report.Core.DTOs;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Infrastructure.Security;
using HR.Identity.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text.Json;
using System;

namespace HRMS.API.Controllers.Report;

/// <summary>
/// کنترلر گزارش ساز پویا
/// </summary>
[Route("api/ReportBuilder")]
[ControllerGroup("Report", "گزارش گیری")]
[DisplayName("گزارش ساز پویا")]
public class ReportBuilderController : AppBaseController
{
    private readonly ReportBuilderService _service;
    private readonly EntityScannerService _scannerService;
    private readonly TempGlobalFileService _tempGlobalFileService;
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly CaptchaService _captchaService;

    public ReportBuilderController(
        ReportBuilderService service,
        EntityScannerService scannerService,
        TempGlobalFileService tempGlobalFileService,
        UserManager<AspNetUsers> userManager,
        CaptchaService captchaService,
        ILogger<ReportBuilderController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _service = service;
        _scannerService = scannerService;
        _tempGlobalFileService = tempGlobalFileService;
        _userManager = userManager;
        _captchaService = captchaService;
        _service._currentUserDefaultOrganId = currentUserDefaultOrganId;
    }

    /// <summary>
    /// دریافت متادیتای گزارش ساز (موجودیت‌ها، فیلدها، انواع داده و عملگرها)
    /// </summary>
    /// <param name="entityId">شناسه موجودیت (اختیاری)</param>
    [HttpGet("GetMetadata")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetMetadata([FromQuery] long? entityId = null)
    {
        return this.AppOk(_service.GetMetadata(entityId));
    }

    /// <summary>
    /// دریافت محدودیت تعداد رکورد گزارش
    /// </summary>
    [HttpGet("GetReportLimit")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetReportLimit()
    {
        return this.AppOk(_service.GetReportLimit());
    }

    /// <summary>
    /// اجرای گزارش و دریافت نتایج
    /// </summary>
    /// <param name="request">درخواست گزارش</param>
    [HttpPost("ExecuteReport")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> ExecuteReport([FromBody] ReportBuilderRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        try
        {
            // لاگ JSON درخواست گزارش
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions 
            { 
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            _logger.LogInformation("Report Execute Request - UserId: {UserId}, Request: {RequestJson}", 
                currentUserId, requestJson);

            // اعتبارسنجی کلمه عبور کاربر جاری
            if (string.IsNullOrWhiteSpace(request.CurrentUserPassword))
            {
                _logger.LogWarning("REPORT BUILDER - ExecuteReport - Missing current user password - UserId: {UserId}, IP: {IP}", 
                    currentUserId, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return this.AppBadRequest("کلمه عبور فعلی برای تأیید هویت الزامی است");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser == null)
            {
                _logger.LogWarning("REPORT BUILDER - ExecuteReport - Current user not found - UserId: {UserId}, IP: {IP}", 
                    currentUserId, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return this.AppBadRequest("کاربر جاری یافت نشد");
            }

            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Decrypt password if encrypted (one-time use encryption)
            var decryptedPassword = request.CurrentUserPassword;
            if (!string.IsNullOrWhiteSpace(request.CurrentUserPassword) && request.CurrentUserPassword.StartsWith("enc::", StringComparison.Ordinal))
            {
                var rsaKeyService = HttpContext.RequestServices.GetService<RsaKeyService>();
                if (rsaKeyService != null)
                {
                    var parts = request.CurrentUserPassword.Split(new[] { "::" }, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        var keyId = parts[1];
                        var cipher = parts[2];
                        if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                        {
                            decryptedPassword = plain;
                        }
                        else
                        {
                            _logger.LogWarning("REPORT BUILDER - ExecuteReport - Failed to decrypt password - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                                currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                            return this.AppBadRequest("خطا در رمزگشایی کلمه عبور");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("REPORT BUILDER - ExecuteReport - Invalid encrypted password format - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                        return this.AppBadRequest("فرمت رمزنگاری کلمه عبور معتبر نیست");
                    }
                }
                else
                {
                    _logger.LogError("REPORT BUILDER - ExecuteReport - RsaKeyService not available - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                        currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                    return this.AppBadRequest("خطا در سیستم رمزنگاری");
                }
            }

            // Mask password for logging (show first 2 chars, rest as *)
            var maskedPassword = MaskPassword(decryptedPassword);
            
            // لاگ تلاش برای اعتبارسنجی password
            _logger.LogInformation("REPORT BUILDER - ExecuteReport - Password validation attempt - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                currentUserId, currentUser.UserName, maskedPassword, ipAddress, userAgent, DateTime.UtcNow);

            var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, decryptedPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning(
                    "REPORT BUILDER - ExecuteReport - PASSWORD VALIDATION FAILED - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
                    currentUserId,
                    currentUser.UserName,
                    ipAddress,
                    userAgent,
                    DateTime.UtcNow);
                return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
            }

            // لاگ password موفق
            _logger.LogInformation("REPORT BUILDER - ExecuteReport - PASSWORD VALIDATION SUCCESS - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                currentUserId, 
                currentUser.UserName, 
                maskedPassword, 
                ipAddress, 
                userAgent,
                DateTime.UtcNow);

            var result = _service.ExecuteReport(request);
            
            _logger.LogInformation("REPORT BUILDER - ExecuteReport - Report execution completed successfully - UserId: {UserId}, UserName: {UserName}, EntityId: {EntityId}, IP: {IP}", 
                currentUserId, currentUser.UserName, request.EntityId, ipAddress);
            
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در اجرای گزارش - UserId: {UserId}", currentUserId);
            return this.AppBadRequest("خطا در اجرای گزارش");
        }
    }

    /// <summary>
    /// دریافت داده‌های گزارش برای خروجی اکسل
    /// </summary>
    /// <param name="request">درخواست گزارش</param>
    [HttpPost("ExecuteReportForExport")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> ExecuteReportForExport([FromBody] ReportBuilderRequestDTO request)
    {
        var passwordValidation = await ValidateCurrentUserPasswordAsync(request.CurrentUserPassword, "ExecuteReportForExport");
        if (passwordValidation != null)
            return passwordValidation;

        return this.AppOk(_service.ExecuteReportForExport(request));
    }

    /// <summary>
    /// دانلود فایل اکسل گزارش
    /// </summary>
    /// <param name="request">درخواست گزارش</param>
    [HttpPost("DownloadExcel")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> DownloadExcel([FromBody] ReportBuilderRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        try
        {
            // لاگ JSON درخواست گزارش
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions 
            { 
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            _logger.LogInformation("Report Download Request - UserId: {UserId}, Request: {RequestJson}", 
                currentUserId, requestJson);

            // اعتبارسنجی کپچا
            if (string.IsNullOrWhiteSpace(request.CaptchaId) || string.IsNullOrWhiteSpace(request.Captcha))
            {
                _logger.LogWarning("Report Download - Missing captcha - UserId: {UserId}", currentUserId);
                return this.AppBadRequest("کد امنیتی الزامی است");
            }

            var isCaptchaValid = _captchaService.Validate(request.CaptchaId, request.Captcha);
            if (!isCaptchaValid)
            {
                _logger.LogWarning("Report Download - Invalid captcha - UserId: {UserId}", currentUserId);
                return this.AppBadRequest("کد امنیتی صحیح نیست");
            }

            // اعتبارسنجی کلمه عبور کاربر جاری
            if (string.IsNullOrWhiteSpace(request.CurrentUserPassword))
            {
                _logger.LogWarning("REPORT BUILDER - DownloadExcel - Missing current user password - UserId: {UserId}, IP: {IP}", 
                    currentUserId, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return this.AppBadRequest("کلمه عبور فعلی برای تأیید هویت الزامی است");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser == null)
            {
                _logger.LogWarning("REPORT BUILDER - DownloadExcel - Current user not found - UserId: {UserId}, IP: {IP}", 
                    currentUserId, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return this.AppBadRequest("کاربر جاری یافت نشد");
            }

            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Decrypt current user password if encrypted (one-time use encryption)
            var decryptedCurrentPassword = request.CurrentUserPassword;
            if (!string.IsNullOrWhiteSpace(request.CurrentUserPassword) && request.CurrentUserPassword.StartsWith("enc::", StringComparison.Ordinal))
            {
                var rsaKeyService = HttpContext.RequestServices.GetService<RsaKeyService>();
                if (rsaKeyService != null)
                {
                    var parts = request.CurrentUserPassword.Split(new[] { "::" }, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        var keyId = parts[1];
                        var cipher = parts[2];
                        if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                        {
                            decryptedCurrentPassword = plain;
                        }
                        else
                        {
                            _logger.LogWarning("REPORT BUILDER - DownloadExcel - Failed to decrypt current password - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                                currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                            return this.AppBadRequest("خطا در رمزگشایی کلمه عبور فعلی");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("REPORT BUILDER - DownloadExcel - Invalid encrypted password format - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                        return this.AppBadRequest("فرمت رمزنگاری کلمه عبور معتبر نیست");
                    }
                }
                else
                {
                    _logger.LogError("REPORT BUILDER - DownloadExcel - RsaKeyService not available - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                        currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                    return this.AppBadRequest("خطا در سیستم رمزنگاری");
                }
            }

            // Mask password for logging (show first 2 chars, rest as *)
            var maskedPassword = MaskPassword(decryptedCurrentPassword);
            
            // لاگ تلاش برای اعتبارسنجی password
            _logger.LogInformation("REPORT BUILDER - DownloadExcel - Password validation attempt - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                currentUserId, currentUser.UserName, maskedPassword, ipAddress, userAgent, DateTime.UtcNow);

            var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, decryptedCurrentPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning(
                    "REPORT BUILDER - DownloadExcel - PASSWORD VALIDATION FAILED - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
                    currentUserId,
                    currentUser.UserName,
                    ipAddress,
                    userAgent,
                    DateTime.UtcNow);
                return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
            }

            // لاگ password موفق
            _logger.LogInformation("REPORT BUILDER - DownloadExcel - PASSWORD VALIDATION SUCCESS - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                currentUserId, 
                currentUser.UserName, 
                maskedPassword, 
                ipAddress, 
                userAgent,
                DateTime.UtcNow);

            // Decrypt file encryption password if encrypted (one-time use encryption)
            // بررسی رمز عبور برای رمزنگاری فایل
            if (string.IsNullOrWhiteSpace(request.FileEncryptionPassword))
            {
                _logger.LogWarning("Report Download - Missing file encryption password - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                    currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                return this.AppBadRequest("رمز عبور برای رمزنگاری فایل الزامی است");
            }

            var decryptedFilePassword = request.FileEncryptionPassword;
            if (request.FileEncryptionPassword.StartsWith("enc::", StringComparison.Ordinal))
            {
                var rsaKeyService = HttpContext.RequestServices.GetService<RsaKeyService>();
                if (rsaKeyService != null)
                {
                    var parts = request.FileEncryptionPassword.Split(new[] { "::" }, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        var keyId = parts[1];
                        var cipher = parts[2];
                        if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                        {
                            decryptedFilePassword = plain;
                        }
                        else
                        {
                            _logger.LogWarning("REPORT BUILDER - DownloadExcel - Failed to decrypt file encryption password - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                                currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                            return this.AppBadRequest("خطا در رمزگشایی رمز عبور فایل");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("REPORT BUILDER - DownloadExcel - Invalid encrypted file password format - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                        return this.AppBadRequest("فرمت رمزنگاری رمز عبور فایل معتبر نیست");
                    }
                }
                else
                {
                    _logger.LogError("REPORT BUILDER - DownloadExcel - RsaKeyService not available for file password - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                        currentUserId, currentUser.UserName, ipAddress, userAgent, DateTime.UtcNow);
                    return this.AppBadRequest("خطا در سیستم رمزنگاری");
                }
            }

            // دریافت نام موجودیت برای نام فایل
            var entityName = "گزارش";
            if (request.EntityId > 0)
            {
                var entity = _service.GetMetadata(request.EntityId);
                if (entity.Success && entity.Payload != null)
                {
                    var metadata = entity.Payload as ReportBuilderMetadataDTO;
                    if (metadata?.Entities != null)
                    {
                        var selectedEntity = metadata.Entities.FirstOrDefault(e => e.Id == request.EntityId);
                        if (selectedEntity != null)
                        {
                            entityName = selectedEntity.FriendlyName ?? entityName;
                        }
                    }
                }
            }

            // ساخت اکسل و ذخیره در TempGlobalFile (با رمزنگاری)
            var result = _service.ExportToExcel(request, entityName, decryptedFilePassword);
            
            if (!result.Success)
            {
                return this.AppBadRequest(result.Message);
            }

            // دریافت فایل از TempGlobalFile
            var fileId = (long)result.Payload;
            var file = _tempGlobalFileService.GetIdAsync(fileId).GetAwaiter().GetResult();
            
            if (file == null)
            {
                return this.AppBadRequest("فایل یافت نشد");
            }

            file.temp_inBase64 = Convert.ToBase64String(file.Content);
            file.Content = null;
            file.title = file.title?.Replace(" ", "_") ?? "گزارش";

            _logger.LogInformation("REPORT BUILDER - DownloadExcel - Report download completed successfully - UserId: {UserId}, UserName: {UserName}, FileId: {FileId}, EntityName: {EntityName}, EntityId: {EntityId}, IP: {IP}", 
                currentUserId, currentUser.UserName, fileId, entityName, request.EntityId, ipAddress);

            return Ok(HR.SharedKernel.DTOs.OperationResult.Succeeded(payload: file));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دانلود فایل اکسل - UserId: {UserId}", currentUserId);
            return this.AppBadRequest("خطا در دانلود فایل اکسل");
        }
    }

    /// <summary>
    /// Mask password for logging (show first 2 characters, rest as *)
    /// </summary>
    private static string MaskPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return "***";
        
        if (password.Length <= 2)
            return new string('*', password.Length);
        
        return password.Substring(0, 2) + new string('*', password.Length - 2);
    }

    private async Task<IActionResult?> ValidateCurrentUserPasswordAsync(string? encryptedOrPlainPassword, string actionName)
    {
        if (string.IsNullOrWhiteSpace(encryptedOrPlainPassword))
        {
            _logger.LogWarning(
                "REPORT BUILDER - {ActionName} - Missing current user password - UserId: {UserId}, IP: {IP}",
                actionName,
                currentUserId,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
            return this.AppBadRequest("کلمه عبور فعلی برای تأیید هویت الزامی است");
        }

        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        if (currentUser == null)
        {
            _logger.LogWarning(
                "REPORT BUILDER - {ActionName} - Current user not found - UserId: {UserId}, IP: {IP}",
                actionName,
                currentUserId,
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
            return this.AppBadRequest("کاربر جاری یافت نشد");
        }

        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = Request.Headers["User-Agent"].ToString();
        var decryptedPassword = encryptedOrPlainPassword;

        if (encryptedOrPlainPassword.StartsWith("enc::", StringComparison.Ordinal))
        {
            var rsaKeyService = HttpContext.RequestServices.GetService<RsaKeyService>();
            if (rsaKeyService == null)
            {
                _logger.LogError(
                    "REPORT BUILDER - {ActionName} - RsaKeyService not available - UserId: {UserId}, UserName: {UserName}, IP: {IP}",
                    actionName,
                    currentUserId,
                    currentUser.UserName,
                    ipAddress);
                return this.AppBadRequest("خطا در سیستم رمزنگاری");
            }

            var parts = encryptedOrPlainPassword.Split(new[] { "::" }, StringSplitOptions.None);
            if (parts.Length != 3)
            {
                return this.AppBadRequest("فرمت رمزنگاری کلمه عبور معتبر نیست");
            }

            if (!rsaKeyService.TryDecrypt(parts[1], parts[2], out var plain) || string.IsNullOrEmpty(plain))
            {
                _logger.LogWarning(
                    "REPORT BUILDER - {ActionName} - Failed to decrypt password - UserId: {UserId}, UserName: {UserName}, IP: {IP}",
                    actionName,
                    currentUserId,
                    currentUser.UserName,
                    ipAddress);
                return this.AppBadRequest("خطا در رمزگشایی کلمه عبور");
            }

            decryptedPassword = plain;
        }

        var maskedPassword = MaskPassword(decryptedPassword);
        _logger.LogInformation(
            "REPORT BUILDER - {ActionName} - Password validation attempt - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
            actionName,
            currentUserId,
            currentUser.UserName,
            maskedPassword,
            ipAddress,
            userAgent,
            DateTime.UtcNow);

        var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, decryptedPassword);
        if (!isPasswordValid)
        {
            _logger.LogWarning(
                "REPORT BUILDER - {ActionName} - PASSWORD VALIDATION FAILED - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
                actionName,
                currentUserId,
                currentUser.UserName,
                ipAddress,
                userAgent,
                DateTime.UtcNow);
            return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
        }

        _logger.LogInformation(
            "REPORT BUILDER - {ActionName} - PASSWORD VALIDATION SUCCESS - UserId: {UserId}, UserName: {UserName}, MaskedPassword: {MaskedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
            actionName,
            currentUserId,
            currentUser.UserName,
            maskedPassword,
            ipAddress,
            userAgent,
            DateTime.UtcNow);

        return null;
    }

    /// <summary>
    /// اسکن و ثبت خودکار تمام Entity های سیستم
    /// </summary>
    [HttpPost("ScanAndPopulateEntities")]
    [CustomAccessKey(AccessKey: "ScanAndPopulateEntities")]
    public IActionResult ScanAndPopulateEntities()
    {
        _logger.LogInformation("شروع اسکن Entity های سیستم");
        var result = _scannerService.ScanAndPopulateEntities();

        if (result.Success)
        {
            _logger.LogInformation($"اسکن با موفقیت انجام شد: {result.Message}");
        }
        else
        {
            _logger.LogError($"خطا در اسکن: {result.Message}");
        }

        return this.AppOk(result);
    }

    /// <summary>
    /// دریافت لیست Schema های دیتابیس
    /// </summary>
    [HttpGet("GetDatabaseSchemas")]
    [CustomAccessKey(AccessKey: "ScanAndPopulateEntities")]
    public IActionResult GetDatabaseSchemas()
    {
        try
        {
            var result = _scannerService.GetDatabaseSchemas();
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت Schema ها");
            return this.AppBadRequest("خطا در دریافت Schema ها");
        }
    }

    /// <summary>
    /// دریافت لیست جداول یک Schema
    /// </summary>
    [HttpGet("GetTablesBySchema/{schemaName}")]
    [CustomAccessKey(AccessKey: "ScanAndPopulateEntities")]
    public IActionResult GetTablesBySchema(string schemaName)
    {
        try
        {
            var result = _scannerService.GetTablesBySchema(schemaName);
            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطا در دریافت جداول Schema {schemaName}");
            return this.AppBadRequest($"خطا در دریافت جداول: {ex.Message}");
        }
    }

    /// <summary>
    /// اسکن و ثبت یک جدول خاص
    /// </summary>
    [HttpPost("ScanAndPopulateSingleTable")]
    [CustomAccessKey(AccessKey: "ScanAndPopulateEntities")]
    public IActionResult ScanAndPopulateSingleTable([FromBody] ScanTableRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation($"شروع اسکن جدول {request.SchemaName}.{request.TableName}");
            var result = _scannerService.ScanAndPopulateSingleTable(request.SchemaName, request.TableName);

            if (result.Success)
            {
                _logger.LogInformation($"اسکن جدول با موفقیت انجام شد: {result.Message}");
            }
            else
            {
                _logger.LogError($"خطا در اسکن جدول: {result.Message}");
            }

            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"خطا در اسکن جدول {request.SchemaName}.{request.TableName}");
            return this.AppBadRequest($"خطا در اسکن جدول: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO برای درخواست اسکن یک جدول
/// </summary>
public class ScanTableRequestDTO
{
    public string SchemaName { get; set; } = "";
    public string TableName { get; set; } = "";
}

