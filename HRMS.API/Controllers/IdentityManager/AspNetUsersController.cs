using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.Identity.Infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Controllers.IdentityManager.Model;
using HRMS.API.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/AspNetUsers")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("مدیریت کاربران")]
public partial class AspNetUsersController : AppBaseController
{
    private readonly HRAuthenticationService _hrAuthenticationService;
    private readonly IdentityContext _identityContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly CustomIdentityContext _customIdentityContext;

    public AspNetUsersController(
        IdentityContext identityContext,
        HRAuthenticationService hrAuthenticationService,
        ILogger<AspNetUsersController> logger,
        IMapper mapper,
        IHttpContextAccessor accessor,
        IDapper dapper,
        UserResolverService userResolverService,
        UserManager<AspNetUsers> userManager,
        CustomIdentityContext customIdentityContext)
        : base(userResolverService, logger, accessor, null, dapper)
    {
        _mapper = mapper;
        _identityContext = identityContext;
        _hrAuthenticationService = hrAuthenticationService;
        _hrAuthenticationService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _userManager = userManager;
        _customIdentityContext = customIdentityContext;
    }
    [HttpGet, Route("getCurrentUserMenuClaims")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> getCurrentUserMenuClaims()
    {
        if (currentUserId <= 0)
        {
            return this.AppOk(true);
        }

        var selectedUser = await _identityContext.Users.FindAsync(currentUserId);
        if (selectedUser == null || selectedUser.LockoutEnabled)
        {
            return this.AppOk(true);
        }

        var permissions = await _identityContext.Permissions
            .AsNoTracking()
            .OrderBy(i => i.DisplayName)
            .ToListAsync();

        var tree = TreeUtil.BuildTree(MapPermissionNodes(permissions));
        if (isAdmin)
        {
            return this.AppOk(tree);
        }

        var userClaimSet = await BuildCurrentUserClaimSetAsync();
        var root = tree.FirstOrDefault();
        var filteredRoot = root == null ? null : FilterPermissionTree(root, userClaimSet);
        return this.AppOk(new List<Node> { filteredRoot });
    }


    [HttpGet, Route("getCurrentUserLastLoginDate")]
    [CustomAccessKey(AccessKey: "getCurrentUserLastLoginDate")]
    public async Task<IActionResult> getCurrentUserLastLoginDate()
    {
        if (currentUserId <= 0)
        {
            return this.AppOk(UnknownLoginDateText);
        }

        var user = await _identityContext.AspNetUsers
            .AsNoTracking()
            .Where(u => u.Id == currentUserId)
            .Select(u => u.LastLoginDate)
            .FirstOrDefaultAsync();

        return this.AppOk(FormatLastLoginDate(user));
    }
    [HttpGet, Route("GetAsKeyValuePair")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetAsKeyValuePair()
    {
        return this.AppOk(OperationResult.Succeeded(payload: _identityContext.AspNetUsers
            .AsNoTracking()
            .Where(i => i.LockoutEnabled != true)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                id = i.Id,
                value = i.FirstName + " " + i.LastName + " ( " + i.NationalNo + " ) "
            })));
    }
    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> Get(long id)
    {
        const string methodName = nameof(Get);
        var ipAddress = GetClientIP();
        
        if (!IsUsersAddEditVerified())
        {
            _logger.LogWarning("[{MethodName}] Access denied - Users add/edit form verification required. UserId: {UserId}, IP: {IP}, TargetUserId: {TargetUserId}", 
                methodName, currentUserId, ipAddress, id);
            return this.AppOk(OperationResult.Failed("برای دسترسی به فرم ویرایش کاربر، ابتدا باید هویت خود را تأیید کنید."));
        }
        
        return this.AppOk(OperationResult.Succeeded(payload: _mapper.Map<AspNetUsersDTO>(await _identityContext.AspNetUsers.FindAsync(id))));
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
    {
        const string methodName = nameof(GetPagedData);
        var ipAddress = GetClientIP();
        
        if (!IsUsersListVerified())
        {
            _logger.LogWarning("[{MethodName}] Access denied - Users list verification required. UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppOk(OperationResult.Failed("برای دسترسی به فهرست کاربران، ابتدا باید هویت خود را تأیید کنید."));
        }
        
        var paged = _identityContext.AspNetUsers;
        int rowCount = 0;
        var list = _mapper.Map<List<AspNetUsersDTO>>(PagerUtility<AspNetUsers>.GetPagedData(paged, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection));
        return this.AppOk(OperationResult.Succeeded(payload: list, rowCount: rowCount));
    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] RegisterUserDTO body, [FromServices] CaptchaService captchaService, [FromServices] RsaKeyService rsaKeyService)
    {
        const string methodName = nameof(Post);
        var ipAddress = GetClientIP();
        
        if (!IsUsersAddEditVerified())
        {
            _logger.LogWarning("[{MethodName}] Access denied - Users add/edit form verification required. UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppBadRequest("برای دسترسی به فرم ایجاد کاربر، ابتدا باید هویت خود را تأیید کنید.");
        }

        if (!TryValidateCaptcha(captchaService, out var captchaError))
        {
            _logger.LogWarning("[{MethodName}] Captcha validation failed. UserId: {UserId}, IP: {IP}, Message: {Message}",
                methodName, currentUserId, ipAddress, captchaError);
            return this.AppBadRequest(captchaError!);
        }

        try
        {
            if (!TryDecryptRsaValue(body.CurrentPassword, rsaKeyService, out var currentPassword))
            {
                _logger.LogWarning("[{MethodName}] Failed to decrypt currentPassword. UserId: {UserId}", methodName, currentUserId);
                return this.AppBadRequest("خطا در رمزگشایی کلمه عبور فعلی");
            }

            body.CurrentPassword = currentPassword;

            if (!TryDecryptRsaValue(body.pasvord, rsaKeyService, out var password))
            {
                _logger.LogWarning("[{MethodName}] Failed to decrypt pasvord. UserId: {UserId}", methodName, currentUserId);
                return this.AppBadRequest("خطا در رمزگشایی کلمه عبور");
            }

            body.pasvord = password;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{MethodName}] Error decrypting passwords. UserId: {UserId}", methodName, currentUserId);
            return this.AppBadRequest("خطا در رمزگشایی رمزهای عبور");
        }

        // Step 3: Verify current user's password for sensitive operations
        if (string.IsNullOrWhiteSpace(body.CurrentPassword))
        {
            return this.AppBadRequest("کلمه عبور فعلی برای تأیید هویت الزامی است");
        }

        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        if (currentUser == null)
        {
            _logger.LogWarning("[{MethodName}] Current user not found. UserId: {UserId}", methodName, currentUserId);
            return this.AppBadRequest("کاربر جاری یافت نشد");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, body.CurrentPassword);
        if (!isPasswordValid)
        {
            _logger.LogWarning("[{MethodName}] Invalid current password provided. UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
        }

        try
        {
            if (!HR.SharedKernel.Utilities.IsValidNationalCode(body.NationalNo))
            {
                return this.AppBadRequest("کد ملی معتبر نمی باشد");
            }
        }
        catch (Exception ex)
        {
            return this.AppBadRequest(ex.Message);
        }

        if (await _identityContext.AspNetUsers.AnyAsync(i => i.NationalNo == body.NationalNo))
        {
            return this.AppBadRequest("کد ملی وارد شده در سیستم موجود می باشد");
        }

        var normalizedUserName = body.UserName.ToLower();
        if (await _identityContext.AspNetUsers.AnyAsync(i => i.UserName.ToLower() == normalizedUserName))
        {
            return this.AppBadRequest("نام کاربری وارد شده در سیستم موجود می باشد");
        }

        var result = await _hrAuthenticationService.CreateForAsync(new RegisterUserDTO()
        {
            Email = body.Email,
            FirstName = body.FirstName,
            LastName = body.LastName,
            PhoneNumber = body.PhoneNumber,
            UserName = body.UserName,
            Password = body.pasvord,
            NationalNo = body.NationalNo,
            Description = body.Description,
            title = body.title,
            StartDate = body.StartDate,
            EndDate = body.EndDate,
            Disabled = body.Disabled,
            CreateDate = DateTime.Now,
            PasswordExpirationDate = body.PasswordExpirationDate,
            ConfidentialityLevelId = body.ConfidentialityLevelId
        });
        return this.AppOk(result);
    }
    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] RegisterUserDTO body, [FromServices] CaptchaService captchaService, [FromServices] RsaKeyService rsaKeyService)
    {
        const string methodName = nameof(Put);
        string? userId = null;
        string? userName = null;
        var ipAddress = GetClientIP();
        
        if (!IsUsersAddEditVerified())
        {
            _logger.LogWarning("[{MethodName}] Access denied - Users add/edit form verification required. UserId: {UserId}, IP: {IP}, TargetUserId: {TargetUserId}", 
                methodName, currentUserId, ipAddress, body?.Id);
            return this.AppBadRequest("برای دسترسی به فرم ویرایش کاربر، ابتدا باید هویت خود را تأیید کنید.");
        }
        
        try
        {
            // Log method entry
            _logger.LogInformation("[{MethodName}] Starting user update request. CurrentUserId: {CurrentUserId}, IP: {IP}",
                methodName, currentUserId, ipAddress);

            // Validate ModelState
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("[{MethodName}] ModelState validation failed. Errors: {Errors}", methodName, modelErrors);
                return this.AppBadRequest(ModelState);
            }

            // Validate body is not null
            if (body == null)
            {
                _logger.LogError("[{MethodName}] Request body is null", methodName);
                return this.AppBadRequest("اطلاعات ارسالی معتبر نمی‌باشد");
            }

            if (!TryValidateCaptcha(captchaService, out var captchaError))
            {
                _logger.LogWarning("[{MethodName}] Captcha validation failed. UserId: {UserId}, IP: {IP}, Message: {Message}",
                    methodName, currentUserId, ipAddress, captchaError);
                return this.AppBadRequest(captchaError!);
            }

            try
            {
                if (!TryDecryptRsaValue(body.CurrentPassword, rsaKeyService, out var currentPassword))
                {
                    _logger.LogWarning("[{MethodName}] Failed to decrypt currentPassword. UserId: {UserId}", methodName, currentUserId);
                    return this.AppBadRequest("خطا در رمزگشایی کلمه عبور فعلی");
                }

                body.CurrentPassword = currentPassword;

                if (!TryDecryptRsaValue(body.pasvord, rsaKeyService, out var password))
                {
                    _logger.LogWarning("[{MethodName}] Failed to decrypt pasvord. UserId: {UserId}", methodName, currentUserId);
                    return this.AppBadRequest("خطا در رمزگشایی کلمه عبور");
                }

                body.pasvord = password;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Error decrypting passwords. UserId: {UserId}", methodName, currentUserId);
                return this.AppBadRequest("خطا در رمزگشایی رمزهای عبور");
            }

            // Step 3: Verify current user's password for sensitive operations
            if (string.IsNullOrWhiteSpace(body.CurrentPassword))
            {
                _logger.LogWarning("[{MethodName}] Current password not provided. UserId: {UserId}", 
                    methodName, currentUserId);
                return this.AppBadRequest("کلمه عبور فعلی برای تأیید هویت الزامی است");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser == null)
            {
                _logger.LogWarning("[{MethodName}] Current user not found. UserId: {UserId}", methodName, currentUserId);
                return this.AppBadRequest("کاربر جاری یافت نشد");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, body.CurrentPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning("[{MethodName}] Invalid current password provided. UserId: {UserId}, IP: {IP}", 
                    methodName, currentUserId, ipAddress);
                return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
            }

            userId = body.Id?.ToString() ?? "Unknown";
            userName = body.UserName ?? "Unknown";

            _logger.LogInformation("[{MethodName}] Processing update for UserId: {UserId}, UserName: {UserName}",
                methodName, userId, userName);

            // Validate National Code
            try
            {
                if (string.IsNullOrWhiteSpace(body.NationalNo))
                {
                    _logger.LogWarning("[{MethodName}] NationalNo is null or empty for UserId: {UserId}", methodName, userId);
                    return this.AppBadRequest("کد ملی الزامی است");
                }

                if (!HR.SharedKernel.Utilities.IsValidNationalCode(body.NationalNo))
                {
                    _logger.LogWarning("[{MethodName}] Invalid NationalCode: {NationalNo} for UserId: {UserId}",
                        methodName, body.NationalNo, userId);
                    return this.AppBadRequest("کد ملی معتبر نمی باشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Error validating NationalCode for UserId: {UserId}, NationalNo: {NationalNo}",
                    methodName, userId, body.NationalNo);
                return this.AppBadRequest($"خطا در بررسی کد ملی: {ex.Message}");
            }

            // Validate UserId
            if (body.Id <= 0)
            {
                _logger.LogWarning("[{MethodName}] Invalid UserId: {UserId}", methodName, body.Id);
                return this.AppBadRequest("شناسه کاربر معتبر نمی باشد");
            }

            // Check if user exists
            AspNetUsers? existingUser = null;
            try
            {
                existingUser = await _identityContext.AspNetUsers.FindAsync(body.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("[{MethodName}] User not found. UserId: {UserId}", methodName, body.Id);
                    return this.AppNotFound("کاربر یافت نشد");
                }
                _logger.LogDebug("[{MethodName}] User found. UserId: {UserId}, CurrentUserName: {CurrentUserName}",
                    methodName, body.Id, existingUser.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Database error while checking user existence. UserId: {UserId}",
                    methodName, body.Id);
                return StatusCode(500, OperationResult.Failed("خطا در ارتباط با پایگاه داده"));
            }

            // Check for duplicate NationalNo
            try
            {
                var hasDuplicateNationalNo = await _identityContext.AspNetUsers
                    .AnyAsync(i => i.NationalNo == body.NationalNo && i.Id != body.Id);
                
                if (hasDuplicateNationalNo)
                {
                    _logger.LogWarning("[{MethodName}] Duplicate NationalNo detected. UserId: {UserId}, NationalNo: {NationalNo}",
                        methodName, body.Id, body.NationalNo);
                    return this.AppBadRequest("کد ملی وارد شده در سیستم موجود می باشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Database error while checking duplicate NationalNo. UserId: {UserId}, NationalNo: {NationalNo}",
                    methodName, body.Id, body.NationalNo);
                return StatusCode(500, OperationResult.Failed("خطا در بررسی تکراری بودن کد ملی"));
            }

            // Check for duplicate UserName
            try
            {
                if (string.IsNullOrWhiteSpace(body.UserName))
                {
                    _logger.LogWarning("[{MethodName}] UserName is null or empty for UserId: {UserId}", methodName, body.Id);
                    return this.AppBadRequest("نام کاربری الزامی است");
                }

                var normalizedUserName = body.UserName.ToLower();
                var hasDuplicateUserName = await _identityContext.AspNetUsers
                    .AnyAsync(i => i.UserName.ToLower() == normalizedUserName && i.Id != body.Id);
                
                if (hasDuplicateUserName)
                {
                    _logger.LogWarning("[{MethodName}] Duplicate UserName detected. UserId: {UserId}, UserName: {UserName}",
                        methodName, body.Id, body.UserName);
                    return this.AppBadRequest("نام کاربری وارد شده در سیستم موجود می باشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Database error while checking duplicate UserName. UserId: {UserId}, UserName: {UserName}",
                    methodName, body.Id, body.UserName);
                return StatusCode(500, OperationResult.Failed("خطا در بررسی تکراری بودن نام کاربری"));
            }

            // Call update service
            _logger.LogInformation("[{MethodName}] Calling UpdateAsync for UserId: {UserId}", methodName, body.Id);
            var result = await _hrAuthenticationService.UpdateAsync(body);

            if (result.Success)
            {
                _logger.LogInformation("[{MethodName}] User updated successfully. UserId: {UserId}, UserName: {UserName}",
                    methodName, body.Id, body.UserName);
                return this.AppOk(result);
            }
            else
            {
                _logger.LogWarning("[{MethodName}] UpdateAsync returned failure. UserId: {UserId}, Message: {Message}",
                    methodName, body.Id, result.Message);
                return this.AppBadRequest(result);
            }
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "[{MethodName}] SQL Exception occurred. UserId: {UserId}, UserName: {UserName}, ErrorNumber: {ErrorNumber}, State: {State}",
                methodName, userId, userName, sqlEx.Number, sqlEx.State);
            
            string errorMessage = sqlEx.Number switch
            {
                2 => "خطا در ارتباط با پایگاه داده. لطفا دوباره تلاش کنید.",
                547 => "این رکورد در بخش دیگری از سیستم استفاده شده و قابل تغییر نیست.",
                2627 => "مقدار تکراری در پایگاه داده",
                2601 => "مقدار تکراری در پایگاه داده",
                _ => $"خطای پایگاه داده: {sqlEx.Message}"
            };
            
            return StatusCode(500, OperationResult.Failed(errorMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{MethodName}] Unexpected error occurred. UserId: {UserId}, UserName: {UserName}, ExceptionType: {ExceptionType}",
                methodName, userId, userName, ex.GetType().Name);
            
            return StatusCode(500, OperationResult.Failed($"خطای غیرمنتظره در به‌روزرسانی کاربر: {ex.Message}"));
        }
    }

    private string GetClientIP()
    {
        try
        {
            return HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    [HttpPut("UpdateCurrentUserPassword")]
    [CustomAccessKey(AccessKey: "UpdateCurrentUserPassword")]
    public async Task<IActionResult> UpdateCurrentUserPassword(
        [FromBody] UpdatePassCurrentUserDTO body, 
        [FromServices] CaptchaService captchaService, 
        [FromServices] RsaKeyService rsaKeyService,
        [FromServices] PasswordChangeRateLimitService rateLimitService)
    {
        const string methodName = nameof(UpdateCurrentUserPassword);
        var ipAddress = GetClientIP();

        // Step 0: Check Rate Limit BEFORE any processing
        var rateLimitCheck = await rateLimitService.CheckRateLimitAndRecordAsync(
            currentUserId, ipAddress, false, "Rate limit check");
        
        if (!rateLimitCheck.Success)
        {
            // Rate limit exceeded - user and IP are already blocked
            _logger.LogError("[{MethodName}] Rate limit exceeded - UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppBadRequest(rateLimitCheck.Message);
        }

        // Step 1: Validate captcha FIRST (always required, before any other processing)
        var captchaId = Request.Headers["X-Captcha-Id"].FirstOrDefault();
        var captchaCode = Request.Headers["X-Captcha-Code"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(captchaCode))
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Captcha not provided");
            _logger.LogWarning("[{MethodName}] Captcha not provided. UserId: {UserId}, IP: {IP}", methodName, currentUserId, ipAddress);
            return this.AppBadRequest("کد امنیتی الزامی است");
        }

        var isCaptchaValid = captchaService.Validate(captchaId, captchaCode);
        if (!isCaptchaValid)
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Invalid captcha");
            _logger.LogWarning("[{MethodName}] Invalid captcha. UserId: {UserId}, IP: {IP}, CaptchaId: {CaptchaId}", 
                methodName, currentUserId, ipAddress, captchaId);
            return this.AppBadRequest("کد امنیتی صحیح نیست");
        }

        // Step 1.5: Check one-time use encryption (prevent reuse of encrypted credentials)
        var encryptedCredentials = $"{body.oldpass}|{body.newpass}|{body.newpassconfirm}";
        var previousUsage = await _customIdentityContext.LoginCredentialLogs
            .Where(log => log.EncryptedPassword == encryptedCredentials 
                       && !log.IsDeleted
                       && log.CreateDate >= DateTime.UtcNow.AddMinutes(-60))
            .OrderByDescending(log => log.CreateDate)
            .FirstOrDefaultAsync();

        if (previousUsage != null)
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Encrypted credentials reuse detected");
            _logger.LogWarning("[{MethodName}] Encrypted credentials reuse detected - UserId: {UserId}, IP: {IP}, PreviousUsage: {PreviousUsage}", 
                methodName, currentUserId, ipAddress, previousUsage.CreateDate);
            return this.AppBadRequest("این اعتبارنامه قبلاً استفاده شده است و یک بار مصرف می‌باشد. لطفاً مجدداً تلاش کنید.");
        }

        // Step 2: Decrypt passwords AFTER captcha validation (one-time use encryption)
        string? decryptedOldPassword = null;
        string? decryptedNewPassword = null;
        string? decryptedNewPasswordConfirm = null;
        
        try
        {
            // Decrypt oldpass
            if (!string.IsNullOrEmpty(body.oldpass) && body.oldpass.StartsWith("enc::", StringComparison.Ordinal))
            {
                var parts = body.oldpass.Split(new[] { "::" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    var keyId = parts[1];
                    var cipher = parts[2];
                    if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                    {
                        decryptedOldPassword = plain;
                        body.oldpass = plain;
                    }
                    else
                    {
                        await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Failed to decrypt oldpass");
                        _logger.LogWarning("[{MethodName}] Failed to decrypt oldpass. UserId: {UserId}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            methodName, currentUserId, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
                        return this.AppBadRequest("خطا در رمزگشایی کلمه عبور فعلی");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(body.oldpass))
            {
                decryptedOldPassword = body.oldpass;
            }

            // Decrypt newpass
            if (!string.IsNullOrEmpty(body.newpass) && body.newpass.StartsWith("enc::", StringComparison.Ordinal))
            {
                var parts = body.newpass.Split(new[] { "::" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    var keyId = parts[1];
                    var cipher = parts[2];
                    if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                    {
                        decryptedNewPassword = plain;
                        body.newpass = plain;
                    }
                    else
                    {
                        await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Failed to decrypt newpass");
                        _logger.LogWarning("[{MethodName}] Failed to decrypt newpass. UserId: {UserId}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            methodName, currentUserId, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
                        return this.AppBadRequest("خطا در رمزگشایی کلمه عبور جدید");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(body.newpass))
            {
                decryptedNewPassword = body.newpass;
            }

            // Decrypt newpassconfirm
            if (!string.IsNullOrEmpty(body.newpassconfirm) && body.newpassconfirm.StartsWith("enc::", StringComparison.Ordinal))
            {
                var parts = body.newpassconfirm.Split(new[] { "::" }, StringSplitOptions.None);
                if (parts.Length == 3)
                {
                    var keyId = parts[1];
                    var cipher = parts[2];
                    if (rsaKeyService.TryDecrypt(keyId, cipher, out var plain) && !string.IsNullOrEmpty(plain))
                    {
                        decryptedNewPasswordConfirm = plain;
                        body.newpassconfirm = plain;
                    }
                    else
                    {
                        await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Failed to decrypt newpassconfirm");
                        _logger.LogWarning("[{MethodName}] Failed to decrypt newpassconfirm. UserId: {UserId}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                            methodName, currentUserId, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
                        return this.AppBadRequest("خطا در رمزگشایی تکرار کلمه عبور جدید");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(body.newpassconfirm))
            {
                decryptedNewPasswordConfirm = body.newpassconfirm;
            }
        }
        catch (Exception ex)
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, $"Exception during decryption: {ex.Message}");
            _logger.LogError(ex, "[{MethodName}] Error decrypting passwords. UserId: {UserId}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, currentUserId, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
            return this.AppBadRequest("خطا در رمزگشایی رمزهای عبور");
        }

        // Step 3: Validate ModelState after decryption
        if (!ModelState.IsValid)
        {
            return this.AppBadRequest(ModelState);
        }

        // Step 4: Validate current user password before updating
        var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
        if (currentUser == null)
        {
            _logger.LogWarning("[{MethodName}] Current user not found. UserId: {UserId}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, currentUserId, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
            return this.AppBadRequest("کاربر جاری یافت نشد");
        }

        // Validate old password
        if (string.IsNullOrWhiteSpace(decryptedOldPassword))
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Old password not provided");
            _logger.LogWarning("[{MethodName}] Old password not provided. UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, currentUserId, currentUser.UserName, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
            return this.AppBadRequest("کلمه عبور فعلی الزامی است");
        }

        // Validate newpassconfirm matches newpass
        if (decryptedNewPassword != decryptedNewPasswordConfirm)
        {
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "New password confirmation does not match");
            _logger.LogWarning("[{MethodName}] New password confirmation mismatch. UserId: {UserId}, IP: {IP}", 
                methodName, currentUserId, ipAddress);
            return this.AppBadRequest("کلمه عبور جدید و تکرار آن مطابقت ندارد");
        }

        var maskedOldPassword = decryptedOldPassword.Length <= 2 
            ? new string('*', decryptedOldPassword.Length) 
            : decryptedOldPassword.Substring(0, 2) + new string('*', decryptedOldPassword.Length - 2);

        // لاگ تلاش برای تغییر پسورد
        _logger.LogInformation("[{MethodName}] Password change attempt - UserId: {UserId}, UserName: {UserName}, MaskedOldPassword: {MaskedOldPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
            methodName, currentUserId, currentUser.UserName, maskedOldPassword, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);

        var isOldPasswordValid = await _userManager.CheckPasswordAsync(currentUser, decryptedOldPassword);
        if (!isOldPasswordValid)
        {
            // لاگ password ناموفق - با جزئیات کامل
            await rateLimitService.CheckRateLimitAndRecordAsync(currentUserId, ipAddress, false, "Old password validation failed");
            _logger.LogWarning("[{MethodName}] OLD PASSWORD VALIDATION FAILED - UserId: {UserId}, UserName: {UserName}, TestedPassword: {TestedPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, 
                currentUserId, 
                currentUser.UserName, 
                decryptedOldPassword, // لاگ password کامل برای تشخیص تلاش برای پیدا کردن رمز
                ipAddress, 
                Request.Headers["User-Agent"].ToString(),
                DateTime.UtcNow);
            return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
        }

        // لاگ password موفق
        _logger.LogInformation("[{MethodName}] OLD PASSWORD VALIDATION SUCCESS - UserId: {UserId}, UserName: {UserName}, MaskedOldPassword: {MaskedOldPassword}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
            methodName, 
            currentUserId, 
            currentUser.UserName, 
            maskedOldPassword, 
            ipAddress, 
            Request.Headers["User-Agent"].ToString(),
            DateTime.UtcNow);

        // Step 5: Proceed with password update
        var result = await _hrAuthenticationService.UpdateCurrentUserPassword(body);
        
        // Record the attempt in rate limit service (success or failure)
        await rateLimitService.CheckRateLimitAndRecordAsync(
            currentUserId, 
            ipAddress, 
            result.Success, 
            result.Success ? null : result.Message);
        
        if (result.Success)
        {
            // Log encrypted credentials for one-time use check
            var credentialLog = new LoginCredentialLog
            {
                EncryptedUsername = currentUser.UserName,
                EncryptedPassword = encryptedCredentials, // Store for one-time use check
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IPAddress = ipAddress,
                CreateDate = DateTime.UtcNow,
                IsSuccess = true,
                AspNetUserId = currentUserId,
                title = $"Password Change - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };
            _customIdentityContext.LoginCredentialLogs.Add(credentialLog);
            await _customIdentityContext.SaveChangesAsync();

            _logger.LogInformation("[{MethodName}] PASSWORD CHANGE SUCCESS - UserId: {UserId}, UserName: {UserName}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, currentUserId, currentUser.UserName, ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
            
            // Invalidate session immediately so the user must re-login with the new password
            AuthCookieHelper.ClearAuthCookies(Request, Response);
            await _hrAuthenticationService.LogOut(currentUserId);
        }
        else
        {
            _logger.LogWarning("[{MethodName}] PASSWORD CHANGE FAILED - UserId: {UserId}, UserName: {UserName}, Reason: {Reason}, IP: {IP}, UserAgent: {UserAgent}, Timestamp: {Timestamp}", 
                methodName, currentUserId, currentUser.UserName, result.Message ?? "Unknown error", ipAddress, Request.Headers["User-Agent"].ToString(), DateTime.UtcNow);
        }
        
        return this.AppOk(result);
    }

    [HttpGet, Route("GetCurrentUserBasicInfo")]
    [CustomAccessKey(AccessKey: "GetCurrentUserBasicInfo")]
    public async Task<IActionResult> GetCurrentUserBasicInfo()
    {
        if (currentUserId <= 0)
        {
            return this.AppOk(OperationResult.NotFound());
        }
        var user = await _identityContext.AspNetUsers.FindAsync(currentUserId);
        if (user == null)
        {
            return this.AppOk(OperationResult.NotFound());
        }

        var payload = new
        {
            user.FirstName,
            user.LastName,
            user.UserName,
            user.Email,
            LastLoginDate = FormatLastLoginDate(user.LastLoginDate)
        };
        return this.AppOk(OperationResult.Succeeded(payload: payload));
    }

    [HttpPut("Unlock/{id}")]
    [CustomAccessKey(AccessKey: "Unlock")]
    public async Task<IActionResult> Unlock(long id)
    {
        var result = await _hrAuthenticationService.UnlockUser(id);
        return this.AppOk(result);
    }

    /// <summary>
    /// تأیید هویت برای دسترسی به فهرست کاربران
    /// این endpoint برای امنیت فهرست کاربران استفاده می‌شود
    /// </summary>
    [HttpPost("VerifyUsersListAccess")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> VerifyUsersListAccess([FromBody] VerifyUsersListAccessDTO body, [FromServices] CaptchaService captchaService, [FromServices] RsaKeyService rsaKeyService)
    {
        return await VerifyAccessInternal(body, captchaService, rsaKeyService, "UsersList_Verified", "فهرست کاربران");
    }

    /// <summary>
    /// تأیید هویت برای دسترسی به فرم ایجاد/ویرایش کاربر
    /// این endpoint برای امنیت فرم ایجاد/ویرایش کاربر استفاده می‌شود
    /// </summary>
    [HttpPost("VerifyUsersAddEditAccess")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> VerifyUsersAddEditAccess([FromBody] VerifyUsersListAccessDTO body, [FromServices] CaptchaService captchaService, [FromServices] RsaKeyService rsaKeyService)
    {
        return await VerifyAccessInternal(body, captchaService, rsaKeyService, "UsersAddEdit_Verified", "فرم ایجاد/ویرایش کاربر");
    }

    /// <summary>
    /// متد داخلی برای verification (قابل استفاده مجدد)
    /// </summary>
    private async Task<IActionResult> VerifyAccessInternal(VerifyUsersListAccessDTO body, CaptchaService captchaService, RsaKeyService rsaKeyService, string sessionKeyPrefix, string operationName)
    {
        const string methodName = "VerifyAccessInternal";
        var ipAddress = GetClientIP();
        var userAgent = Request.Headers["User-Agent"].ToString();

        try
        {
            if (!TryValidateCaptcha(captchaService, out var captchaError))
            {
                _logger.LogWarning("[{MethodName}] Captcha validation failed. UserId: {UserId}, IP: {IP}, Operation: {Operation}, Message: {Message}",
                    methodName, currentUserId, ipAddress, operationName, captchaError);

                await LogVerificationAttemptAsync(
                    false,
                    $"{(captchaError == "کد امنیتی الزامی است" ? "Captcha not provided" : "Invalid captcha")} - {operationName}",
                    ipAddress,
                    userAgent);

                return this.AppBadRequest(captchaError!);
            }

            // Step 2: Validate body
            if (body == null || string.IsNullOrWhiteSpace(body.CurrentPassword))
            {
                _logger.LogWarning("[{MethodName}] CurrentPassword not provided. UserId: {UserId}, IP: {IP}, Operation: {Operation}", 
                    methodName, currentUserId, ipAddress, operationName);
                
                // Log failed attempt (no password to log in this case)
                await LogVerificationAttemptAsync(false, $"CurrentPassword not provided - {operationName}", ipAddress, userAgent);
                
                return this.AppBadRequest("کلمه عبور فعلی الزامی است");
            }

            string? decryptedPassword;
            var encryptedPassword = body.CurrentPassword;

            try
            {
                if (!TryDecryptRsaValue(body.CurrentPassword, rsaKeyService, out decryptedPassword))
                {
                    _logger.LogWarning("[{MethodName}] Failed to decrypt currentPassword. UserId: {UserId}, Operation: {Operation}",
                        methodName, currentUserId, operationName);

                    await LogVerificationAttemptAsync(false, $"Failed to decrypt password - {operationName}", ipAddress, userAgent, encryptedPassword);
                    return this.AppBadRequest("خطا در رمزگشایی کلمه عبور فعلی");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{MethodName}] Error decrypting password. UserId: {UserId}, Operation: {Operation}",
                    methodName, currentUserId, operationName);

                await LogVerificationAttemptAsync(false, $"Decryption error: {ex.Message} - {operationName}", ipAddress, userAgent,
                    encryptedPassword?.StartsWith("enc::") == true ? encryptedPassword : null);

                return this.AppBadRequest("خطا در رمزگشایی کلمه عبور");
            }

            // Step 4: Verify current user's password
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser == null)
            {
                _logger.LogWarning("[{MethodName}] Current user not found. UserId: {UserId}, Operation: {Operation}", 
                    methodName, currentUserId, operationName);
                
                // Log failed attempt with encrypted password
                await LogVerificationAttemptAsync(false, $"Current user not found - {operationName}", ipAddress, userAgent, 
                    encryptedPassword?.StartsWith("enc::") == true ? encryptedPassword : null);
                
                return this.AppBadRequest("کاربر جاری یافت نشد");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(currentUser, decryptedPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning("[{MethodName}] Invalid current password provided. UserId: {UserId}, IP: {IP}, Operation: {Operation}", 
                    methodName, currentUserId, ipAddress, operationName);
                
                // Log failed attempt with encrypted password
                await LogVerificationAttemptAsync(false, $"Invalid password - {operationName}", ipAddress, userAgent, 
                    encryptedPassword?.StartsWith("enc::") == true ? encryptedPassword : null);
                
                return this.AppBadRequest("کلمه عبور فعلی صحیح نمی باشد");
            }

            // Step 5: Set verification flag in session
            var verificationKey = $"{sessionKeyPrefix}_{currentUserId}";
            HttpContext.Session.SetString(verificationKey, "true");
            
            _logger.LogInformation("[{MethodName}] {Operation} access verified successfully. UserId: {UserId}, IP: {IP}", 
                methodName, operationName, currentUserId, ipAddress);
            
            // Log successful attempt (no password logged for security)
            await LogVerificationAttemptAsync(true, $"Verification successful - {operationName}", ipAddress, userAgent);
            
            return this.AppOk(OperationResult.Succeeded("هویت شما با موفقیت تأیید شد"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{MethodName}] Unexpected error during verification. UserId: {UserId}, IP: {IP}, Operation: {Operation}", 
                methodName, currentUserId, ipAddress, operationName);
            
            // Log failed attempt (no password to log in case of unexpected error)
            await LogVerificationAttemptAsync(false, $"Unexpected error: {ex.Message} - {operationName}", ipAddress, userAgent);
            
            return this.AppBadRequest(OperationResult.Failed("خطا در فرآیند تأیید هویت"));
        }
    }

    /// <summary>
    /// لاگ کردن تلاش‌های verification برای تحلیل امنیتی
    /// </summary>
    private async Task LogVerificationAttemptAsync(bool isSuccess, string reason, string ipAddress, string userAgent, string encryptedPassword = null)
    {
        try
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            var encryptedUsername = currentUser?.UserName ?? "Unknown";
            
            var logEntry = new LoginCredentialLog
            {
                EncryptedUsername = encryptedUsername,
                EncryptedPassword = isSuccess ? null : (encryptedPassword ?? "VERIFICATION_FAILED"), // برای ناموفق encrypted password را لاگ می‌کنیم
                UserAgent = userAgent,
                IPAddress = ipAddress,
                CreateDate = DateTime.Now,
                IsSuccess = isSuccess,
                AspNetUserId = isSuccess ? currentUserId : null,
                title = $"Access Verification - {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {(isSuccess ? "Success" : "Failed")} - {reason}"
            };
            
            _customIdentityContext.LoginCredentialLogs.Add(logEntry);
            await _customIdentityContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't block verification process
            _logger.LogError(ex, "Failed to log verification attempt - UserId: {UserId}, IP: {IP}", 
                currentUserId, ipAddress);
        }
    }

}



