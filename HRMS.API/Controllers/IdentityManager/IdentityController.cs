using HR.SharedKernel.Attribute;
using AutoMapper;

using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.Core.Interfaces;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Security;
using HR.SharedKernel.DTOs;
using HRMS.API.Scanner;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Service;
using System.ComponentModel;
using HRMS.API.Cache;
using HRMS.API.Infrastructure.Security;
using HR.Identity.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/Identity")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("احراز هویت")]
public class IdentityController(HRAuthenticationService HRAuthenticationService, RefreshTokenService refreshTokenService, PermissionScanner PermissionScanner, ILogger<IdentityController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService, IWebHostEnvironment environment, HR.SharedKernel.Security.UserIdEncryptionService userIdEncryptionService, CustomIdentityContext customIdentityContext) : AppBaseController(UserResolverService, logger, accessor, null, dapper)
{
    private readonly HRAuthenticationService _hrAuthenticationService = HRAuthenticationService;
    private readonly RefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly CustomIdentityContext _customIdentityContext = customIdentityContext;

    /// <summary>
    /// خروج از سیستم
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Logout")]
    [AllowAnonymous]
    [CustomAccessKey(AccessKey: "Logout")]
    public async Task<IActionResult> Logout()
    {
        return this.AppOk(await ProcessLogoutAsync("User logout"));
    }

    /// <summary>
    /// Clears auth cookies and revokes refresh token. Works even when the access token is expired.
    /// </summary>
    private async Task<OperationResult> ProcessLogoutAsync(string revokeReason)
    {
        var refreshToken = Request.Cookies[AuthCookieHelper.RefreshTokenCookieName];
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, ipAddress, revokeReason);
        }

        AuthCookieHelper.ClearAuthCookies(Request, Response);

        return await _hrAuthenticationService.LogOut(currentUserId);
    }
    [HttpGet, Route("GetCurrentUserName")]
    [CustomAccessKey(AccessKey: "GetCurrentUserName")]
    public IActionResult GetCurrentUserName()
    {
        return this.AppOk(CurrentUserFullName);
    }
    /// <summary>
    /// احراز هویت کاربر و تولید توکین
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDTO id)
    {
        // Step 0: Store encrypted credentials BEFORE decryption for security logging
        // همه تلاش‌های ورود (موفق و ناموفق) باید لاگ شوند
        var encryptedUsername = id?.UserName ?? string.Empty;
        var encryptedPassword = id?.Password ?? string.Empty;
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Step 0.5: Check if this encrypted credential combination was used before (one-time use check)
        // فقط برای payload رمزنگاری‌شده (enc::) — هر encrypt nonce/timestamp یکتا دارد.
        // ورود plaintext روی HTTPS (مثلاً Firefox) همیشه همان رشته است و نباید one-time چک شود.
        var isEncryptedTransport =
            encryptedUsername.StartsWith("enc::", StringComparison.Ordinal)
            && encryptedPassword.StartsWith("enc::", StringComparison.Ordinal);

        if (isEncryptedTransport)
        {
            try
            {
                var previousUsage = await _customIdentityContext.LoginCredentialLogs
                    .Where(log => log.EncryptedUsername == encryptedUsername 
                               && log.EncryptedPassword == encryptedPassword
                               && !log.IsDeleted)
                    .OrderByDescending(log => log.CreateDate)
                    .FirstOrDefaultAsync();

                if (previousUsage != null)
                {
                    // این encrypted credentials قبلاً استفاده شده است - یک بار مصرف هستند
                    _logger.LogWarning(
                        "SECURITY ALERT - Attempted reuse of previously used encrypted credentials - " +
                        "PreviousUsageDate: {PreviousUsageDate}, PreviousIP: {PreviousIP}, " +
                        "CurrentIP: {CurrentIP}, PreviousSuccess: {PreviousSuccess}",
                        previousUsage.CreateDate,
                        previousUsage.IPAddress,
                        ipAddress,
                        previousUsage.IsSuccess);

                    return this.AppOk(OperationResult.Failed(
                        "این اعتبارنامه قبلاً استفاده شده است و یک بار مصرف می‌باشد. لطفاً مجدداً وارد شوید."));
                }
            }
            catch (Exception ex)
            {
                // Log error but don't block login process if check fails
                _logger.LogError(ex, "Failed to check for encrypted credential reuse - IP: {IP}", ipAddress);
            }
        }

        // Create credential log entry (will be updated after authentication)
        // EncryptedPassword در اینجا لاگ می‌شود و در صورت موفقیت، بعداً null می‌شود
        LoginCredentialLog credentialLog = null;
        
        try
        {
            credentialLog = new LoginCredentialLog
            {
                EncryptedUsername = encryptedUsername,
                EncryptedPassword = encryptedPassword, // در صورت موفقیت، بعداً null می‌شود
                UserAgent = userAgent,
                IPAddress = ipAddress,
                CreateDate = DateTime.Now,
                IsSuccess = false,
                AspNetUserId = null,
                title = $"Login Attempt - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            };
            _customIdentityContext.LoginCredentialLogs.Add(credentialLog);
            await _customIdentityContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't block login process
            _logger.LogError(ex, "Failed to log encrypted credentials - IP: {IP}", ipAddress);
        }

        // Helper method to update credential log before returning
        async Task<IActionResult> UpdateLogAndReturn(OperationResult result)
        {
            if (credentialLog != null)
            {
                try
                {
                    credentialLog.IsSuccess = result.Success;
                    
                    // EncryptedPassword همیشه لاگ می‌شود (چه موفق چه ناموفق)
                    // این برای ردیابی و تحلیل امنیتی است
                    // توجه: EncryptedPassword به صورت encrypted لاگ می‌شود (نه بعد از decrypt)
                    
                    if (result.Success && !string.IsNullOrEmpty(id?.UserName))
                    {
                        // Get UserManager from service provider to find user
                        var userManager = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Identity.UserManager<AspNetUsers>>();
                        if (userManager != null)
                        {
                            try
                            {
                                var user = await userManager.FindByNameAsync(id.UserName);
                                if (user != null)
                                {
                                    credentialLog.AspNetUserId = user.Id;
                                }
                            }
                            catch { }
                        }
                    }
                    _customIdentityContext.LoginCredentialLogs.Update(credentialLog);
                    await _customIdentityContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log error but don't block login process
                    _logger.LogError(ex, "Failed to update credential log - IP: {IP}", ipAddress);
                }
            }
            return this.AppOk(result);
        }

        // Step 1: Validate captcha FIRST (before any other processing)
        var captchaService = HttpContext.RequestServices.GetService<HRMS.API.Infrastructure.Security.CaptchaService>();
        if (captchaService == null)
        {
            // لاگ تلاش ناموفق (خطا در سیستم)
            return await UpdateLogAndReturn(OperationResult.Failed("خطا در سیستم احراز هویت"));
        }
        
        // Check if we're in debug/development mode
        var isDevelopment = _environment.IsDevelopment();
        
        // Check if captcha is required based on rate limiting
        // Skip in debug mode UNLESS explicitly enabled via configuration
        bool captchaRequired = (!isDevelopment || captchaService.IsEnabledInDevelopment) 
                              && captchaService.IsCaptchaRequired(ipAddress);
        
        // Get captcha from headers
        var captchaId = Request.Headers["X-Captcha-Id"].FirstOrDefault() ?? Request.Query["captchaId"].FirstOrDefault();
        var captchaCode = Request.Headers["X-Captcha-Code"].FirstOrDefault() ?? Request.Query["captchaCode"].FirstOrDefault();
        
        // Validate captcha FIRST if it's required (before password decryption and other validations)
        if (captchaRequired)
        {
            // Check if captcha is provided
            if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(captchaCode))
            {
                // Track this failed attempt before returning
                captchaService.TrackFailedAttempt(ipAddress);
                _logger.LogWarning("Login attempt without captcha - IP: {IP}, CaptchaRequired: true", ipAddress);
                // لاگ تلاش ناموفق (کد امنیتی ارسال نشده)
                return await UpdateLogAndReturn(OperationResult.Failed("به دلیل تلاش‌های ناموفق متعدد، وارد کردن کد امنیتی الزامی است"));
            }

            // Validate captcha
            var isCaptchaValid = captchaService.Validate(captchaId, captchaCode);
            if (!isCaptchaValid)
            {
                // Track this failed captcha attempt before returning
                captchaService.TrackFailedAttempt(ipAddress);
                _logger.LogWarning("Invalid captcha - IP: {IP}, CaptchaId: {CaptchaId}", ipAddress, captchaId);
                // لاگ تلاش ناموفق (کد امنیتی اشتباه)
                return await UpdateLogAndReturn(OperationResult.Failed("کد امنیتی صحیح نیست"));
            }
            
            // Captcha validated successfully - log for tracking
            _logger.LogInformation("Captcha validated successfully - IP: {IP}", ipAddress);
        }

        // Step 2: Decrypt client-encrypted username and password AFTER captcha validation (format: enc::<keyId>::<base64>)
        string decryptedUsername = null;
        string decryptedPassword = null;
        
        try
        {
            // Decrypt username if encrypted
            if (!string.IsNullOrEmpty(id?.UserName) && id.UserName.StartsWith("enc::", StringComparison.Ordinal))
            {
                var userNameParts = id.UserName.Split(new[] {"::"}, StringSplitOptions.None);
                if (userNameParts.Length == 3)
                {
                    var keyId = userNameParts[1];
                    var cipher = userNameParts[2];
                    var rsa = HttpContext.RequestServices.GetService<RsaKeyService>();
                    if (rsa != null && rsa.TryDecrypt(keyId, cipher, out var plainUsername) && !string.IsNullOrEmpty(plainUsername))
                    {
                        decryptedUsername = plainUsername;
                        id.UserName = plainUsername;
                    }
                }
            }
            else
            {
                decryptedUsername = id?.UserName;
            }
            
            // Decrypt password if encrypted
            if (!string.IsNullOrEmpty(id?.Password) && id.Password.StartsWith("enc::", StringComparison.Ordinal))
            {
                var passwordParts = id.Password.Split(new[] {"::"}, StringSplitOptions.None);
                if (passwordParts.Length == 3)
                {
                    var keyId = passwordParts[1];
                    var cipher = passwordParts[2];
                    var rsa = HttpContext.RequestServices.GetService<RsaKeyService>();
                    if (rsa != null && rsa.TryDecrypt(keyId, cipher, out var plainPassword) && !string.IsNullOrEmpty(plainPassword))
                    {
                        decryptedPassword = plainPassword;
                        id.Password = plainPassword;
                    }
                }
            }
            else
            {
                decryptedPassword = id?.Password;
            }
        }
        catch { }

        // Proceed with actual login
        var result = await _hrAuthenticationService.Login(id);
        
        // Update credential log with authentication result
        if (credentialLog != null)
        {
            try
            {
                credentialLog.IsSuccess = result.Success;
                
                // EncryptedPassword همیشه لاگ می‌شود (چه موفق چه ناموفق)
                // این برای ردیابی و تحلیل امنیتی است
                // توجه: EncryptedPassword به صورت encrypted لاگ می‌شود (نه بعد از decrypt)
                
                if (result.Success && !string.IsNullOrEmpty(id?.UserName))
                {
                    // Get UserManager from service provider to find user
                    var userManager = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Identity.UserManager<AspNetUsers>>();
                    if (userManager != null)
                    {
                        var user = await userManager.FindByNameAsync(id.UserName);
                        if (user != null)
                        {
                            credentialLog.AspNetUserId = user.Id;
                        }
                    }
                }
                _customIdentityContext.LoginCredentialLogs.Update(credentialLog);
                await _customIdentityContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't block login process
                _logger.LogError(ex, "Failed to update credential log - IP: {IP}", ipAddress);
            }
        }
        
        if (result.Success && result.Payload != null)
        {
            var clientData = result.Payload as ClientStorageDTO;
            if (clientData != null)
            {
                // Reset failed attempts on successful login
                captchaService.ResetFailedAttempts(ipAddress);
                _logger.LogInformation("Login successful - IP: {IP}, User: {UserName}", ipAddress, id.UserName);
                
                AuthCookieHelper.AppendJwtCookie(Request, Response, clientData.Token, clientData.expiresOn);

                if (!string.IsNullOrEmpty(clientData.RefreshToken))
                {
                    AuthCookieHelper.AppendRefreshTokenCookie(Request, Response, clientData.RefreshToken, clientData.RefreshTokenExpiresOn);
                }
                
                // Include token in response for client-side claim reading (fullname)
                // Token is also stored in HttpOnly cookie for security
                var secureResponse = new
                {
                    token = clientData.Token, // Include token for reading claims in frontend
                    userFullPersianName = clientData.UserFullPersianName,
                    expiresOn = clientData.expiresOn,
                    refreshTokenExpiresOn = clientData.RefreshTokenExpiresOn,
                    requiresPasswordChange = clientData.RequiresPasswordChange
                };
                
                return this.AppOk(OperationResult.Succeeded(payload: secureResponse));
            }
        }
        else
        {
            // Track failed attempt for password/username errors
            captchaService.TrackFailedAttempt(ipAddress);
            _logger.LogWarning("Login failed - IP: {IP}, User: {UserName}, Reason: {Reason}", 
                ipAddress, id.UserName, result.Message ?? "Unknown");
        }
        
        // Return result (credential log already updated above)
        return this.AppOk(result);
    }

    /// <summary>
    /// دریافت کلید عمومی برای رمزنگاری کلمه عبور در فرانت
    /// </summary>
    [AllowAnonymous]
    [HttpGet, Route("public-key")]
    public IActionResult GetPublicKey([FromServices] RsaKeyService rsa)
    {
        var (keyId, publicKeyPem) = rsa.GetCurrentPublicKey();
        return Ok(new { keyId, publicKeyPem });
    }

    /// <summary>
    /// بررسی نیاز به کپچا برای کاربر جاری
    /// </summary>
    [AllowAnonymous]
    [HttpGet, Route("captcha-required")]
    public IActionResult IsCaptchaRequired([FromServices] CaptchaService captcha)
    {
        // Get client IP address
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Check if we're in debug/development mode
        var isDevelopment = _environment.IsDevelopment();
        var enabledInDev = captcha.IsEnabledInDevelopment;
        
        // Log for debugging
        _logger.LogInformation("Captcha requirement check - IP: {IP}, IsDevelopment: {IsDevelopment}, EnabledInDev: {EnabledInDev}", 
            ipAddress, isDevelopment, enabledInDev);
        
        // Captcha is not required in development mode UNLESS explicitly enabled
        if (isDevelopment && !enabledInDev)
        {
            _logger.LogInformation("Captcha not required - development mode and not explicitly enabled");
            return Ok(new { required = false, reason = "development_mode" });
        }
        
        // Check if captcha is required based on rate limiting
        bool required = captcha.IsCaptchaRequired(ipAddress);
        
        _logger.LogInformation("Captcha requirement result for IP {IP}: {Required}", ipAddress, required);
        return Ok(new { required, reason = required ? "rate_limited" : "normal" });
    }

    /// <summary>
    /// ایجاد کپچا (اختیاری برای امنیت فرم ورود)
    /// </summary>
    [AllowAnonymous]
    [HttpGet, Route("captcha")]
    public IActionResult GetCaptcha([FromServices] CaptchaService captcha)
    {
        var (id, bytes, _) = captcha.Generate();
        Response.Headers.Add("X-Captcha-Id", id);
        return File(bytes, "image/png", enableRangeProcessing: false);
    }

    /// <summary>
    /// دریافت کپچا به صورت Base64 + شناسه برای استفاده سمت کلاینت
    /// </summary>
    [AllowAnonymous]
    [HttpGet, Route("captcha-data")]
    public IActionResult GetCaptchaData([FromServices] CaptchaService captcha)
    {
        var (id, bytes, _) = captcha.Generate();
        var base64 = Convert.ToBase64String(bytes);
        return Ok(new { id, imageBase64 = $"data:image/png;base64,{base64}" });
    }


    
    [HttpGet, Route("GetcurrentUserEmployeeId")]
    [CustomAccessKey(AccessKey: "GetcurrentUserEmployeeId")]
    public IActionResult GetcurrentUserEmployeeId()
    {
        return this.AppOk(currentUserEmployeeId);
    }

    [HttpGet, Route("GetCurrentUserId")]
    [CustomAccessKey(AccessKey: "GetCurrentUserId")]
    public IActionResult GetCurrentUserId()
    {
        return this.AppOk(currentUserId);
    }

    /// <summary>
    /// بررسی وضعیت احراز هویت کاربر از طریق JWT Cookie
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("IsAuthenticated")]
    [AllowAnonymous]
    public IActionResult IsAuthenticated()
    {
        try 
        {
            var jwtCookie = Request.Cookies["jwt"];
            var hasAuthHeader = Request.Headers.ContainsKey("Authorization");
            
            _logger.LogInformation("IsAuthenticated called - JWT Cookie exists: {HasCookie}, Auth Header: {HasAuthHeader}, CurrentUserId: {UserId}", 
                !string.IsNullOrEmpty(jwtCookie), hasAuthHeader, currentUserId);
            
            if (currentUserId > 0)
            {
                return this.AppOk(new { isAuthenticated = true, userId = currentUserId });
            }

            return this.AppOk(new { isAuthenticated = false, message = "No valid user found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in IsAuthenticated");
            return this.AppOk(new { isAuthenticated = false, message = ex.Message });
        }
    }

    /// <summary>
    /// تازه‌سازی توکن دسترسی با استفاده از توکن تازه‌سازی
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            // Get refresh token from cookie
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token not found in cookie");
                return Unauthorized(new { success = false, message = "توکن تازه‌سازی یافت نشد" });
            }

            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            // Validate refresh token
            var validationResult = await _refreshTokenService.ValidateRefreshTokenAsync(refreshToken, ipAddress);
            
            if (!validationResult.Success || validationResult.Payload == null)
            {
                _logger.LogWarning("Invalid refresh token - IP: {IP}", ipAddress);
                
                AuthCookieHelper.ClearAuthCookies(Request, Response);

                return Unauthorized(new { success = false, message = validationResult.Message ?? "توکن تازه‌سازی معتبر نیست" });
            }

            var oldRefreshTokenEntity = validationResult.Payload as RefreshToken;
            if (oldRefreshTokenEntity == null)
            {
                return Unauthorized(new { success = false, message = "خطا در پردازش توکن تازه‌سازی" });
            }

            // Generate new access token
            var user = oldRefreshTokenEntity.User;
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Build claims through the shared builder (identical to login claims, including isAdmin)
            var authClaims = await _hrAuthenticationService.BuildUserClaimsAsync(user, ipAddress, userAgent);

            // Generate new JWT token via the public token factory (no reflection)
            var newAccessToken = _hrAuthenticationService.CreateAccessToken(authClaims);

            if (newAccessToken == null)
            {
                _logger.LogError("Failed to generate new access token");
                return StatusCode(500, new { success = false, message = "خطا در تولید توکن جدید" });
            }

            // Rotate refresh token (replace old with new)
            var rotateResult = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken, ipAddress);
            
            if (!rotateResult.Success || rotateResult.Payload == null)
            {
                _logger.LogError("Failed to rotate refresh token - UserId: {UserId}", user.Id);
                return StatusCode(500, new { success = false, message = "خطا در تازه‌سازی توکن" });
            }

            var newRefreshToken = rotateResult.Payload.ToString();
            var refreshTokenExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenService.RefreshTokenDays);

            AuthCookieHelper.AppendJwtCookie(Request, Response, new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(newAccessToken), newAccessToken.ValidTo);
            AuthCookieHelper.AppendRefreshTokenCookie(Request, Response, newRefreshToken, refreshTokenExpiresOn);

            _logger.LogInformation("Token refreshed successfully - UserId: {UserId}, IP: {IP}", user.Id, ipAddress);

            // Return success response (without tokens for security)
            return Ok(new 
            { 
                success = true, 
                message = "توکن با موفقیت تازه‌سازی شد",
                data = new
                {
                    userFullPersianName = user.FirstName + " " + user.LastName,
                    expiresOn = newAccessToken.ValidTo,
                    refreshTokenExpiresOn = refreshTokenExpiresOn
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new { success = false, message = "خطا در تازه‌سازی توکن" });
        }
    }

    /// <summary>
    /// باطل کردن توکن تازه‌سازی
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("revoke-token")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> RevokeToken()
    {
        try
        {
            // Get refresh token from cookie
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return this.AppOk(OperationResult.Failed("توکن تازه‌سازی یافت نشد"));
            }

            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            var result = await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, ipAddress, "Revoked by user");
            
            if (result.Success)
            {
                AuthCookieHelper.ClearAuthCookies(Request, Response);
            }

            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return this.AppOk(OperationResult.Failed("خطا در باطل کردن توکن"));
        }
    }

    /// <summary>
    /// باطل کردن تمام توکن‌های فعال کاربر
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("revoke-all-tokens")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> RevokeAllTokens()
    {
        try
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            var result = await _refreshTokenService.RevokeAllUserTokensAsync(currentUserId, ipAddress, "Revoked all by user");
            
            if (result.Success)
            {
                AuthCookieHelper.ClearAuthCookies(Request, Response);
            }

            return this.AppOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user {UserId}", currentUserId);
            return this.AppOk(OperationResult.Failed("خطا در باطل کردن توکن‌ها"));
        }
    }
}



