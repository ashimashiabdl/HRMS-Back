using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using DynamicExpressions.Mapping;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HR.Identity.infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.Security;
namespace HR.Identity.infrastructure.Services;

public class HRAuthenticationService(IdentityContext Context,
     UserManager<Core.Entities.AspNetUsers> userManager,
    RoleManager<Core.Entities.AspNetRoles> roleManager,
    UserResolverService UserResolverService,
    CustomIdentityContext CustomIdentityContext,
    SignInManager<AspNetUsers> signInManager,
    IMapper mapper,
    IOptions<Identitysetting> config,
    ILogger<HRAuthenticationService> logger,
    IDapper dapper,
    OrganisationChartService OrganisationChartService,
    RefreshTokenService refreshTokenService,
    UserIdEncryptionService userIdEncryptionService,
    CommonPasswordService commonPasswordService,
    BlockedIpSecurityService blockedIpSecurityService)  //: BaseService<AspNetUsers, IdentityContext, AspNetUsersDTO>, IScopedServices
{
    IdentityContext _identityContext = Context;
    CustomIdentityContext _customIdentityContext = CustomIdentityContext;
    RefreshTokenService _refreshTokenService = refreshTokenService;
    UserIdEncryptionService _userIdEncryptionService = userIdEncryptionService;
    CommonPasswordService _commonPasswordService = commonPasswordService;
    BlockedIpSecurityService _blockedIpSecurityService = blockedIpSecurityService;

    public long _currentUserDefaultOrganId { set; get; }

    private readonly IOptions<Identitysetting> _config = config;
    private readonly ILogger<HRAuthenticationService> _logger = logger;
    public readonly UserManager<Core.Entities.AspNetUsers> _userManager = userManager;
    private readonly RoleManager<Core.Entities.AspNetRoles> _roleManager = roleManager;
    private readonly SignInManager<AspNetUsers> _signInManager = signInManager;


    UserResolverService _userResolverService = UserResolverService;

    /// <summary>
    /// بررسی پیچیدگی کلمه عبور
    /// </summary>
    /// <param name="password">کلمه عبور</param>
    /// <returns>نتیجه بررسی</returns>
    private OperationResult ValidatePasswordComplexity(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return OperationResult.Failed("کلمه عبور نمی تواند خالی باشد");
        }

        // بررسی اینکه آیا کلمه عبور در لیست کلمه‌های عبور متداول است یا نه
        if (_commonPasswordService.IsCommonPassword(password))
        {
            return OperationResult.Failed("کلمه عبور انتخاب شده ساده و متداول است. لطفاً کلمه عبور قوی‌تری انتخاب کنید.");
        }

        var errors = new List<string>();

        // حداقل 8 کاراکتر
        if (password.Length < 8)
        {
            errors.Add("کلمه عبور باید حداقل 8 کاراکتر باشد");
        }

        // شامل حروف کوچک
        if (!password.Any(char.IsLower))
        {
            errors.Add("کلمه عبور باید شامل حروف کوچک انگلیسی باشد");
        }

        // شامل حروف بزرگ
        if (!password.Any(char.IsUpper))
        {
            errors.Add("کلمه عبور باید شامل حروف بزرگ انگلیسی باشد");
        }

        // شامل اعداد
        if (!password.Any(char.IsDigit))
        {
            errors.Add("کلمه عبور باید شامل اعداد باشد");
        }

        // شامل کاراکترهای خاص
        var specialChars = "!@#$%^&*()_+-=[]{}|;':\".,<>?/";
        if (!password.Any(c => specialChars.Contains(c)))
        {
            errors.Add("کلمه عبور باید شامل کاراکترهای خاص باشد (!@#$%^&*...)");
        }

        if (errors.Any())
        {
            return OperationResult.Failed("کلمه عبور شرایط پیچیدگی را ندارد: " + string.Join(" | ", errors));
        }

        return OperationResult.Succeeded();
    }



    public async Task<OperationResult> LogOut(long UserId)
    {
        // Idempotent: cookies may already be cleared while the JWT is expired (anonymous logout).
        if (UserId <= 0)
        {
            return OperationResult.Succeeded("خروج از سیستم با موفقیت انجام شد");
        }

        if (UserId > 0)
        {
            await _signInManager.SignOutAsync();
            var user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user != null)
            {
                var ipAddress = _userResolverService.GetIP();
                await _refreshTokenService.RevokeAllUserTokensAsync(UserId, ipAddress, "User logout");

                try
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // User was already updated (e.g. concurrent idle logout from another tab). Treat as success.
                    _logger.LogWarning(ex, "Concurrency on logout SecurityStamp update for UserId {UserId} - already logged out", UserId);
                }
                _logger.LogInformation("{UserName} Logged out Success", user.UserName);
            }

            return OperationResult.Succeeded();
        }

        return OperationResult.Succeeded("خروج از سیستم با موفقیت انجام شد");
    }
    /// <summary>
    /// کتد احراز هویت و ورد به سیستم
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<OperationResult> Login(LoginDTO user)
    {
        var clientIp = _userResolverService.GetIP();
        var userAgent = _userResolverService.GetUserAgent();
        OperationResult loginResult = null;

        try
        {
            var Currentuser = await _userManager.FindByNameAsync(user.UserName);

            if (Currentuser == null)
            {
                _logger.LogInformation("Login attempt by unknown user: {UserName}", user.UserName.ToLower().Trim());
                try
                {
                    _logger.LogWarning("Login failed (unknown user) - UserName:{UserName} - IP:{IP}", user.UserName.ToLower().Trim(), clientIp);
                    
                    // ثبت تلاش ناموفق در تاریخچه
                    await _blockedIpSecurityService.RecordLoginAttemptAsync(clientIp, null, false, userAgent, "کاربر یافت نشد");
                    
                    // بررسی و بلاک کردن IP در صورت نیاز
                    await _blockedIpSecurityService.CheckAndBlockIpIfNeededAsync(clientIp, null, user.UserName);
                }
                catch { }
                
                loginResult = OperationResult.Failed("نام کاربری یا کلمه عبور صحیح نمی باشد");
                return loginResult;
            }
            else
            {
                if (Currentuser.LockoutEnabled)
                {
                    // SECURITY FAILURE LOG: Locked account login attempt
                    var ip = _userResolverService.GetIP();
                    _logger.LogWarning("SECURITY FAILURE - Login attempt on locked account - UserId:{UserId} - UserName:{UserName} - IP:{IP}",
                        Currentuser.Id, user.UserName.ToLower().Trim(), ip);
                    // Do not leak lockout state to the client
                    
                    // ثبت تلاش ناموفق در تاریخچه
                    try
                    {
                        await _blockedIpSecurityService.RecordLoginAttemptAsync(clientIp, Currentuser.Id, false, userAgent, "حساب کاربری قفل شده است");
                        await UpdateUserLoginHistoryTitleAsync(Currentuser.Id, clientIp);
                    }
                    catch { }
                    
                    loginResult = OperationResult.Failed("نام کاربری یا کلمه عبور صحیح نمی باشد");
                    return loginResult;
                }

                long WrongPassAttemptCount = 0;
                var WrongPassAttemptCountSetting = "5";// GetSettingById(10007);
                if (string.IsNullOrEmpty(WrongPassAttemptCountSetting))
                {
                    WrongPassAttemptCount = HR.SharedKernel.Share.Constants.defaultMaxWrongPassAttemptCount;
                }
                else
                {
                    WrongPassAttemptCount = Convert.ToInt64(WrongPassAttemptCountSetting);
                }


                if (await _userManager.CheckPasswordAsync(Currentuser, user.Password))
                {
                    // بررسی محدودیت IP در صورت وجود
                    if (!string.IsNullOrWhiteSpace(Currentuser.AllowedIP))
                    {
                        var currentIP = _userResolverService.GetIP();
                        if (currentIP != Currentuser.AllowedIP)
                        {
                            try
                            {
                                var ip = _userResolverService.GetIP();
                                _logger.LogWarning("SECURITY FAILURE - Login attempt from unauthorized IP - UserId:{UserId} - UserName:{UserName} - AllowedIP:{AllowedIP} - AttemptIP:{AttemptIP}",
                                    Currentuser.Id, user.UserName.ToLower().Trim(), Currentuser.AllowedIP, ip);
                            }
                            catch { }
                            // Do not disclose IP restriction policy to the client
                            
                            // ثبت تلاش ناموفق در تاریخچه
                            try
                            {
                                await _blockedIpSecurityService.RecordLoginAttemptAsync(clientIp, Currentuser.Id, false, userAgent, $"IP غیرمجاز - IP مجاز: {Currentuser.AllowedIP}");
                                await UpdateUserLoginHistoryTitleAsync(Currentuser.Id, clientIp);
                            }
                            catch { }
                            
                            loginResult = OperationResult.Failed("نام کاربری یا کلمه عبور صحیح نمی باشد");
                            return loginResult;
                        }
                    }

                    // Rotate security stamp on successful login via UserManager so previous tokens are invalidated
                    await _userManager.UpdateSecurityStampAsync(Currentuser);

                    // Reload stamp so JWT claims and later saves never overwrite with a stale value
                    var reloadedForStamp = await _userManager.FindByIdAsync(Currentuser.Id.ToString());
                    if (reloadedForStamp?.SecurityStamp != null)
                    {
                        Currentuser.SecurityStamp = reloadedForStamp.SecurityStamp;
                    }

                    // Invalidate every existing refresh token (single active session policy)
                    await _refreshTokenService.RevokeAllUserTokensAsync(Currentuser.Id, clientIp, "New login - previous sessions revoked");

                    // clientIp و userAgent قبلاً تعریف شده‌اند
                    // Build claims through the shared builder so login and refresh always emit identical claim sets
                    var authClaims = await BuildUserClaimsAsync(Currentuser, clientIp, userAgent);

                    var token = GetToken(authClaims);

                    // ثبت تلاش موفق در تاریخچه
                    await _blockedIpSecurityService.RecordLoginAttemptAsync(clientIp, Currentuser.Id, true, userAgent);
                    await UpdateUserLoginHistoryTitleAsync(Currentuser.Id, clientIp);

                    try
                    {
                        var ip = _userResolverService.GetIP();
                        _logger.LogInformation("Login success - UserId:{UserId} - UserName:{UserName} - IP:{IP}", Currentuser.Id, user.UserName.ToLower().Trim(), ip);
                    }
                    catch { }
                    ClientStorageDTO resp = new();

                    resp.Token = new JwtSecurityTokenHandler().WriteToken(token);
                    resp.expiresOn = token.ValidTo;
                    resp.UserFullPersianName = Currentuser.FirstName + " " + Currentuser.LastName;
                    var isExpired = Currentuser.PasswordExpirationDate.HasValue && DateTime.Now >= Currentuser.PasswordExpirationDate.Value;
                    resp.RequiresPasswordChange = Currentuser.MustChangePassword || isExpired;

                    // Generate refresh token
                    var ipAddress = _userResolverService.GetIP();
                    var refreshTokenResult = await _refreshTokenService.CreateRefreshTokenAsync(Currentuser.Id, ipAddress);
                    if (refreshTokenResult.Success && refreshTokenResult.Payload != null)
                    {
                        resp.RefreshToken = refreshTokenResult.Payload.ToString();
                        resp.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenService.RefreshTokenDays);
                    }

                    var loginTime = DateTime.Now;
                    Currentuser.LastLoginDate = loginTime;

                    // Update only login-related fields; never overwrite SecurityStamp set by UserManager above
                    await _identityContext.AspNetUsers
                        .Where(u => u.Id == Currentuser.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(u => u.AccessFailedCount, 0)
                            .SetProperty(u => u.ExpiresOn, token.ValidTo)
                            .SetProperty(u => u.LastLoginDate, loginTime));

                    // بررسی و ایجاد رکورد UserDefaultSetting در صورت نیاز
                    try
                    {
                        var existingUserDefaultSetting = _customIdentityContext.UserDefaultSettings
                            .FirstOrDefault(uds => uds.UserId == Currentuser.Id && !uds.IsDeleted);

                        if (existingUserDefaultSetting == null)
                        {
                            // اگر رکوردی در UserDefaultSetting وجود ندارد، بررسی UserPayLocation
                            var userPayLocations = _customIdentityContext.UserPayLocations
                                .Where(upl => upl.UserId == Currentuser.Id && !upl.IsDeleted)
                                .ToList();

                            if (userPayLocations != null && userPayLocations.Count == 1)
                            {
                                // اگر دقیقا 1 رکورد در UserPayLocation وجود دارد، یک رکورد در UserDefaultSetting ایجاد کن
                                var newUserDefaultSetting = new UserDefaultSetting
                                {
                                    UserId = Currentuser.Id,
                                    DefaultOrganId = userPayLocations[0].PayLocationId,
                                    CreateDate = DateTime.Now,
                                    IPAddress = _userResolverService.GetIP(),
                                    IsDeleted = false,
                                    title = $"Default Setting for User {Currentuser.Id}"
                                };

                                _customIdentityContext.UserDefaultSettings.Add(newUserDefaultSetting);
                                _customIdentityContext.SaveChanges();

                                _logger.LogInformation("Auto-created UserDefaultSetting for UserId:{UserId} with DefaultOrganId:{DefaultOrganId}",
                                    Currentuser.Id, userPayLocations[0].PayLocationId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // در صورت خطا، لاگ کن ولی لاگین را مختل نکن
                        _logger.LogWarning("Error while checking/creating UserDefaultSetting for UserId:{UserId}: {Error}",
                            Currentuser.Id, ex.Message);
                    }

                    loginResult = OperationResult.Succeeded(payload: resp);
                    return loginResult;

                }
                else
                {
                    // افزایش AccessFailedCount (برای گزارش‌گیری)
                    Currentuser.AccessFailedCount = Currentuser.AccessFailedCount + 1;
                    Currentuser.LastWrongAttemptDatetime = DateTime.Now;
                    
                    // ثبت تلاش ناموفق در تاریخچه
                    await _blockedIpSecurityService.RecordLoginAttemptAsync(clientIp, Currentuser.Id, false, userAgent, "کلمه عبور اشتباه است");
                    await UpdateUserLoginHistoryTitleAsync(Currentuser.Id, clientIp);
                    
                    // بررسی و بلاک کردن IP و کاربر در صورت نیاز (بر اساس UserLoginHistory)
                    var blockResult = await _blockedIpSecurityService.CheckAndBlockIpIfNeededAsync(
                        clientIp, Currentuser.Id, user.UserName);
                    
                    // اگر IP بلاک شد، کاربر را هم غیرفعال کن (اگر هنوز غیرفعال نشده)
                    if (blockResult.Success == false && !Currentuser.LockoutEnabled)
                    {
                        Currentuser.LockoutEnabled = true;
                        Currentuser.DeactivationReason = $"غیرفعال شدن به دلیل بلاک شدن IP به دلیل تلاش‌های ناموفق لاگین. IP: {clientIp}";
                        _logger.LogError("SECURITY FAILURE - Account locked due to failed attempts - UserId:{UserId} - UserName:{UserName} - IP:{IP}",
                            Currentuser.Id, user.UserName.ToLower().Trim(), clientIp);
                    }
                    
                    // اگر AccessFailedCount به حد مجاز رسید، کاربر را غیرفعال کن (برای سازگاری با منطق قبلی)
                    if (Currentuser.AccessFailedCount >= WrongPassAttemptCount && !Currentuser.LockoutEnabled)
                    {
                        Currentuser.LockoutEnabled = true;
                        Currentuser.DeactivationReason = $"غیرفعال شدن به دلیل رسیدن تعداد تلاش‌های ناموفق به حد مجاز ({Currentuser.AccessFailedCount} تلاش ناموفق). IP: {clientIp}";
                        _logger.LogError("SECURITY FAILURE - Account locked due to AccessFailedCount threshold - UserId:{UserId} - UserName:{UserName} - FailedAttempts:{FailedAttempts} - IP:{IP}",
                            Currentuser.Id, user.UserName.ToLower().Trim(), Currentuser.AccessFailedCount, clientIp);
                    }
                    
                    _identityContext.AspNetUsers.Update(Currentuser);
                    _identityContext.SaveChanges();

                    try
                    {
                        var ip = _userResolverService.GetIP();
                        _logger.LogWarning("Login failed (wrong password) - UserId:{UserId} - UserName:{UserName} - IP:{IP}", Currentuser.Id, user.UserName.ToLower().Trim(), ip);
                    }
                    catch { }
                    
                    loginResult = OperationResult.Failed("نام کاربری یا کلمه عبور صحیح نمی باشد");
                    return loginResult;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process - UserName:{UserName} - IP:{IP}", user?.UserName, clientIp);
            loginResult = OperationResult.Failed("خطا در فرآیند ورود به سیستم");
        }

        return loginResult ?? OperationResult.Failed("خطا در فرآیند ورود به سیستم");
    }

    /// <summary>
    /// تنظیم title برای آخرین رکورد UserLoginHistory بر اساس DefaultOrganId
    /// </summary>
    private async Task UpdateUserLoginHistoryTitleAsync(long? userId, string ipAddress)
    {
        if (!userId.HasValue)
            return;

        try
        {
            // پیدا کردن آخرین رکورد UserLoginHistory برای این کاربر و IP
            var lastLoginHistory = await _customIdentityContext.UserLoginHistories
                .Where(h => h.AspNetUserId == userId.Value && h.IPAddress == ipAddress)
                .OrderByDescending(h => h.CreateDate)
                .FirstOrDefaultAsync();

            if (lastLoginHistory != null)
            {
                // پیدا کردن DefaultOrganId برای کاربر
                var existingDefaultOrgan = _customIdentityContext.UserDefaultSettings
                    .FirstOrDefault(uds => uds.UserId == userId.Value && !uds.IsDeleted);

                if (existingDefaultOrgan != null && existingDefaultOrgan.DefaultOrganId.HasValue)
                {
                    var organResult = await OrganisationChartService.GetIdAsync(existingDefaultOrgan.DefaultOrganId.Value);
                    if (organResult != null && !string.IsNullOrEmpty(organResult.title))
                    {
                        lastLoginHistory.title = organResult.title;
                        _customIdentityContext.UserLoginHistories.Update(lastLoginHistory);
                        await _customIdentityContext.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // در صورت خطا، لاگ کن ولی متد Login را مختل نکن
            _logger.LogWarning(ex, "Error updating UserLoginHistory title for UserId:{UserId}", userId);
        }
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.Key));

        var accessTokenMinutes = Math.Max(5, _config.Value?.AccessTokenMinutes ?? 20);

        var token = new JwtSecurityToken(
            issuer: _config.Value.JWTIssuer,
            audience: _config.Value.Audience,
            expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    /// <summary>
    /// تولید توکن دسترسی (JWT) از روی claimها. برای استفاده توسط مسیر refresh-token.
    /// با این متد دیگر نیازی به فراخوانی reflection برای متد خصوصی نیست.
    /// </summary>
    public JwtSecurityToken CreateAccessToken(List<Claim> authClaims) => GetToken(authClaims);

    /// <summary>
    /// ساخت مجموعه claimهای استاندارد کاربر. هم در ورود و هم در تازه‌سازی توکن استفاده می‌شود
    /// تا claimها (از جمله isAdmin) همیشه یکسان باشند.
    /// </summary>
    public async Task<List<Claim>> BuildUserClaimsAsync(AspNetUsers user, string ipAddress, string userAgent)
    {
        // Encrypt userId for security - prevents information leakage if token is compromised
        var encryptedUserId = _userIdEncryptionService.EncryptUserId(user.Id);

        var authClaims = new List<Claim>
        {
            // Username removed from token for security - use encrypted userId to identify user
            new Claim("userId", encryptedUserId),
            new Claim("fullname", (user.FirstName ?? string.Empty) + " " + (user.LastName ?? string.Empty)),
            new Claim("security_stamp", user.SecurityStamp ?? string.Empty),
            new Claim("ip_address", ipAddress ?? "unknown"),
            new Claim("user_agent", userAgent ?? "unknown"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (user.EmployeeId > 0)
        {
            authClaims.Add(new Claim("currentUserEmployeeId", user.EmployeeId.ToString()));
        }

        // بررسی اینکه آیا کاربر RoleId = 1 (ادمین) دارد یا نه
        bool isAdmin = await _identityContext.Set<UserRole>()
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == 1);

        if (isAdmin)
        {
            authClaims.Add(new Claim("isAdmin", "true"));
        }
        else
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
        }

        return authClaims;
    }
    /// <summary>
    /// متد ایجاد یک یوزر کاملا جدید
    /// </summary>
    /// <param name="entityToCreate"></param>
    /// <returns></returns>
    public async Task<OperationResult> CreateForAsync(RegisterUserDTO entityToCreate)
    {
        // Validate password complexity
        var passwordValidation = ValidatePasswordComplexity(entityToCreate.Password);
        if (!passwordValidation.Success)
        {
            return passwordValidation;
        }

        var identityUser = new Core.Entities.AspNetUsers()
        {
            FirstName = entityToCreate.FirstName,
            LastName = entityToCreate.LastName,
            NationalNo = entityToCreate.NationalNo,
            Email = entityToCreate.Email,
            UserName = entityToCreate.UserName,
            PhoneNumber = entityToCreate.PhoneNumber,
            Description = entityToCreate.Description,
            AccessFailedCount = 0,
            CreateDate = DateTime.Now,
            LockoutEnabled = entityToCreate.Disabled,
            EmailConfirmed = entityToCreate.EmailConfirmed,
            PhoneNumberConfirmed = entityToCreate.PhoneNumberConfirmed,
            TwoFactorEnabled = entityToCreate.TwoFactorEnabled,
            EmployeeId = entityToCreate.EmployeeId,
            AllowedIP = entityToCreate.AllowedIP,
            SecurityStamp = Guid.NewGuid().ToString(),
            MustChangePassword = true,
            PasswordExpirationDate = entityToCreate.PasswordExpirationDate,
            ConfidentialityLevelId = entityToCreate.ConfidentialityLevelId,
            DeactivationReason = !entityToCreate.Disabled ? (entityToCreate.DeactivationReason ?? string.Empty) : null,
        };

        var createResult = await _userManager.CreateAsync(identityUser, entityToCreate.Password);
        if (createResult.Succeeded)
        {
            return OperationResult.Succeeded(payload: identityUser.Id);
        }

        var errorMessage = string.Join(" | ", createResult.Errors.Select(e => e.Description));
        _logger.LogWarning("User creation failed for {UserName}: {Errors}", entityToCreate.UserName, errorMessage);
        return OperationResult.Failed(errorMessage);
    }
    public async Task<OperationResult> UpdateAsync(RegisterUserDTO entityToCreate)
    {
        AspNetUsers toUpdateUser = await _userManager.FindByIdAsync(entityToCreate.Id.ToString());

        if (toUpdateUser != null)
        {
            if (!string.IsNullOrEmpty(entityToCreate.pasvord))
            {
                // Validate password complexity
                var passwordValidation = ValidatePasswordComplexity(entityToCreate.pasvord);
                if (!passwordValidation.Success)
                {
                    return passwordValidation;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(toUpdateUser);
                var resetResult = await _userManager.ResetPasswordAsync(toUpdateUser, token, entityToCreate.pasvord);
                if (!resetResult.Succeeded)
                {
                    var errorMessage = string.Join(" | ", resetResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for user {UserId}: {Errors}", toUpdateUser.Id, errorMessage);
                    return OperationResult.Failed(errorMessage);
                }

                // Ensure tokens are invalidated after password reset
                await _userManager.UpdateSecurityStampAsync(toUpdateUser);

                // Reload SecurityStamp to ensure it's updated in tracked entity
                // This fixes the issue where SecurityStamp might be null in tracked entity after UpdateSecurityStampAsync
                var reloadedUserForPassword = await _userManager.FindByIdAsync(entityToCreate.Id.ToString());
                if (reloadedUserForPassword == null)
                {
                    return OperationResult.NotFound();
                }
                // Copy SecurityStamp from reloaded entity to tracked entity
                toUpdateUser.SecurityStamp = reloadedUserForPassword.SecurityStamp;

                // Force user to change password on next login after admin reset
                toUpdateUser.MustChangePassword = true;

            }

            // Track sensitive changes that should invalidate existing tokens
            bool originalLockoutEnabled = toUpdateUser.LockoutEnabled;
            bool originalTwoFactorEnabled = toUpdateUser.TwoFactorEnabled;
            string originalEmail = toUpdateUser.Email;
            string originalPhone = toUpdateUser.PhoneNumber;
            string originalUserName = toUpdateUser.UserName;
            int originalAccessFailedCount = toUpdateUser.AccessFailedCount;

            // Enforce deactivation reason when transitioning to Disabled
            if (!originalLockoutEnabled && !entityToCreate.Disabled)
            {
                if (string.IsNullOrWhiteSpace(entityToCreate.DeactivationReason))
                {
                    //   return OperationResult.Failed("ذکر دلیل برای غیر فعال کردن کاربر الزامی است");
                }
            }

            toUpdateUser.FirstName = entityToCreate.FirstName;
            toUpdateUser.LastName = entityToCreate.LastName;
            toUpdateUser.NationalNo = entityToCreate.NationalNo;
            toUpdateUser.Email = entityToCreate.Email;
            toUpdateUser.UserName = entityToCreate.UserName;
            toUpdateUser.PhoneNumber = entityToCreate.PhoneNumber;
            toUpdateUser.Description = entityToCreate.Description;
            toUpdateUser.AccessFailedCount = entityToCreate.AccessFailedCount;
            toUpdateUser.LockoutEnabled = entityToCreate.Disabled;
            toUpdateUser.EmailConfirmed = entityToCreate.EmailConfirmed;
            toUpdateUser.PhoneNumberConfirmed = entityToCreate.PhoneNumberConfirmed;
            toUpdateUser.TwoFactorEnabled = entityToCreate.TwoFactorEnabled;
            toUpdateUser.EmployeeId = entityToCreate.EmployeeId;
            toUpdateUser.AllowedIP = entityToCreate.AllowedIP;
            toUpdateUser.PasswordExpirationDate = entityToCreate.PasswordExpirationDate;
            toUpdateUser.ConfidentialityLevelId = entityToCreate.ConfidentialityLevelId;
            toUpdateUser.DeactivationReason = entityToCreate.Disabled ? (entityToCreate.DeactivationReason ?? string.Empty) : null;

            // Log AccessFailedCount changes
            if (originalAccessFailedCount != toUpdateUser.AccessFailedCount)
            {
                var currentUser = _userResolverService.GetUser() ?? "System";
                _logger.LogInformation("AccessFailedCount updated by {UpdatedBy} for UserId:{UserId} UserName:{UserName} - From:{OldValue} To:{NewValue}",
                    currentUser, toUpdateUser.Id, toUpdateUser.UserName, originalAccessFailedCount, toUpdateUser.AccessFailedCount);
            }

            // Log Username changes
            if (!string.Equals(originalUserName, toUpdateUser.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var currentUser = _userResolverService.GetUser() ?? "System";
                _logger.LogInformation("Username updated by {UpdatedBy} for UserId:{UserId} - From:{OldValue} To:{NewValue}",
                    currentUser, toUpdateUser.Id, originalUserName, toUpdateUser.UserName);
            }

            // Always rotate security stamp after any update (token invalidation policy)

            // Ensure SecurityStamp is not null before UpdateAsync (fixes production environment issue)
            // This is critical: UserManager.UpdateAsync validates SecurityStamp and will fail if it's null
            // Note: Even if SecurityStamp exists in DB, Entity Framework tracking might have null value
            bool securityStampWasNull = string.IsNullOrEmpty(toUpdateUser.SecurityStamp);
            if (securityStampWasNull)
            {
                await _userManager.UpdateSecurityStampAsync(toUpdateUser);
                // Reload user to ensure SecurityStamp is updated in tracked entity
                // This fixes the issue where SecurityStamp might be null in tracked entity after UpdateSecurityStampAsync
                var reloadedUser = await _userManager.FindByIdAsync(entityToCreate.Id.ToString());
                if (reloadedUser == null)
                {
                    return OperationResult.NotFound();
                }
                // Copy SecurityStamp from reloaded entity to tracked entity
                toUpdateUser.SecurityStamp = reloadedUser.SecurityStamp;
            }

            var updateResult = await _userManager.UpdateAsync(toUpdateUser);
            if (!updateResult.Succeeded)
            {
                var errorMessage = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User update failed for {UserId}: {Errors}", toUpdateUser.Id, errorMessage);
                return OperationResult.Failed(errorMessage);
            }

            // Only rotate SecurityStamp if it wasn't null (if it was null, we already generated it above)
            if (!securityStampWasNull)
            {
                await _userManager.UpdateSecurityStampAsync(toUpdateUser);
            }
        }
        else
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: 1);
    }

    public async Task<OperationResult> UnlockUser(long userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return OperationResult.NotFound();
        }
        user.LockoutEnabled = false;
        user.AccessFailedCount = 0;
        user.LastWrongAttemptDatetime = null;

        // Ensure SecurityStamp is not null before UpdateAsync (fixes production environment issue)
        bool securityStampWasNull = string.IsNullOrEmpty(user.SecurityStamp);
        if (securityStampWasNull)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Unlock failed for user {UserId}: {Errors}", user.Id, errorMessage);
            return OperationResult.Failed(errorMessage);
        }

        // Only rotate SecurityStamp if it wasn't null (if it was null, we already generated it above)
        // This invalidates existing tokens after unlock
        if (!securityStampWasNull)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }
        return OperationResult.Succeeded();
    }



    public async Task<OperationResult> UpdateCurrentUserPassword(UpdatePassCurrentUserDTO entityToCreate)
    {
        AspNetUsers toUpdateUser = await _userManager.FindByIdAsync(UserResolverService.GetUserId().ToString());

        if (toUpdateUser != null)
        {
            if (!string.IsNullOrEmpty(entityToCreate.newpass))
            {
                // Validate password complexity for new password
                var passwordValidation = ValidatePasswordComplexity(entityToCreate.newpass);
                if (!passwordValidation.Success)
                {
                    return passwordValidation;
                }

                // validate confirms (only new password confirmation is required)
                if (entityToCreate.newpass != entityToCreate.newpassconfirm)
                {
                    return OperationResult.Failed("تایید رمز عبور جدید صحیح نیست");
                }
                if (entityToCreate.newpass == entityToCreate.oldpass)
                {
                    return OperationResult.Failed("رمز عبور جدید نباید با رمز عبور فعلی یکسان باشد");
                }

                // Enforce minimum character difference between old and new password
                int minDiff = Math.Max(1, _config.Value?.PasswordMinDiffChars ?? 4);
                if (CalculateLevenshteinDistance(entityToCreate.oldpass, entityToCreate.newpass) < minDiff)
                {
                    return OperationResult.Failed($"رمز عبور جدید باید حداقل {minDiff} کاراکتر با رمز قبلی تفاوت داشته باشد");
                }

                var oldPassOk = await _userManager.CheckPasswordAsync(toUpdateUser, entityToCreate.oldpass);
                if (!oldPassOk)
                {
                    return OperationResult.Failed("رمز عبور فعلی صحیح نیست");
                }

                // Enforce password history (disallow reuse of last N passwords)
                int historyCount = Math.Max(0, _config.Value?.PasswordHistoryDisallowCount ?? 5);
                if (historyCount > 0)
                {
                    var recentPasswords = _customIdentityContext.UserPasswordHistories
                        .Where(h => h.AspNetUserId == toUpdateUser.Id)
                        .OrderByDescending(h => h.CreateDate)
                        .Take(historyCount)
                        .Select(h => new { h.PasswordHash, h.Salt })
                        .ToList();

                    var passwordHasher = (CustomPasswordHasher)_userManager.PasswordHasher;
                    foreach (var prev in recentPasswords)
                    {
                        var result = passwordHasher.VerifyPasswordWithSalt(prev.PasswordHash, entityToCreate.newpass, prev.Salt);
                        if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                        {
                            return OperationResult.Failed($"نمی‌توانید از {historyCount} رمز عبور اخیر خود مجدداً استفاده کنید");
                        }
                    }
                }

                // Save current password hash into history after successful change
                var currentHash = toUpdateUser.PasswordHash;
                var currentSalt = toUpdateUser.salt;
                var changeResult = await _userManager.ChangePasswordAsync(toUpdateUser, entityToCreate.oldpass, entityToCreate.newpass);
                if (!changeResult.Succeeded)
                {
                    var errorMessage = string.Join(" | ", changeResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("ChangePassword failed for user {UserId}: {Errors}", toUpdateUser.Id, errorMessage);
                    return OperationResult.Failed(errorMessage);
                }

                // Ensure tokens are invalidated after password change
                await _userManager.UpdateSecurityStampAsync(toUpdateUser);
                await _refreshTokenService.RevokeAllUserTokensAsync(
                    toUpdateUser.Id,
                    _userResolverService.GetIP(),
                    "Password changed - all sessions revoked");

                // Clear mandatory password change flag after a successful change
                toUpdateUser.MustChangePassword = false;
                // Set next password expiration date (default: 1 month)
                toUpdateUser.PasswordExpirationDate = DateTime.Now.AddMonths(1);

                // Record previous password hash in history
                if (!string.IsNullOrEmpty(currentHash))
                {
                    var history = new UserPasswordHistory
                    {
                        AspNetUserId = toUpdateUser.Id,
                        PasswordHash = currentHash,
                        Salt = currentSalt,
                        CreateDate = DateTime.Now,
                        IPAddress = _userResolverService.GetIP()
                    };
                    _customIdentityContext.UserPasswordHistories.Add(history);
                    _customIdentityContext.SaveChanges();
                }

                // Security check: اگر کاربر بیش از 5 بار در 15 دقیقه اقدام به تغییر رمز عبور کرده باشد
                var timeWindow = DateTime.Now.AddMinutes(-15);
                var passwordChangeCount = await _customIdentityContext.UserPasswordHistories
                    .Where(h => h.AspNetUserId == toUpdateUser.Id && 
                                h.CreateDate >= timeWindow && 
                                !h.IsDeleted)
                    .CountAsync();

                if (passwordChangeCount > 5)
                {
                    var clientIp = _userResolverService.GetIP();
                    var userAgent = _userResolverService.GetUserAgent();
                    
                    _logger.LogError(
                        "SECURITY ALERT - Excessive password change attempts detected - UserId:{UserId} - UserName:{UserName} - Attempts:{Attempts} - IP:{IP}",
                        toUpdateUser.Id, toUpdateUser.UserName, passwordChangeCount, clientIp);

                    // 1. بلاک کردن IP
                    try
                    {
                        await _blockedIpSecurityService.CheckAndBlockIpIfNeededAsync(clientIp, toUpdateUser.Id, toUpdateUser.UserName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error blocking IP during password change security check - UserId:{UserId} - IP:{IP}", 
                            toUpdateUser.Id, clientIp);
                    }

                    // 2. غیرفعال کردن کاربر
                    if (!toUpdateUser.LockoutEnabled)
                    {
                        toUpdateUser.LockoutEnabled = true;
                        toUpdateUser.DeactivationReason = $"غیرفعال شدن به دلیل تلاش‌های مکرر برای تغییر رمز عبور ({passwordChangeCount} تلاش). IP: {clientIp}";
                        _logger.LogError(
                            "SECURITY ALERT - User locked due to excessive password change attempts - UserId:{UserId} - UserName:{UserName} - Attempts:{Attempts} - IP:{IP}",
                            toUpdateUser.Id, toUpdateUser.UserName, passwordChangeCount, clientIp);
                    }

                    // 3. به‌روزرسانی SecurityStamp برای باطل کردن توکن‌ها
                    await _userManager.UpdateSecurityStampAsync(toUpdateUser);

                    // 4. ذخیره تغییرات کاربر
                    var securityUpdateResult = await _userManager.UpdateAsync(toUpdateUser);
                    if (!securityUpdateResult.Succeeded)
                    {
                        _logger.LogError("Failed to update user after security lock - UserId:{UserId} - Errors:{Errors}", 
                            toUpdateUser.Id, string.Join(" | ", securityUpdateResult.Errors.Select(e => e.Description)));
                    }

                    return OperationResult.Failed("به دلیل تلاش‌های مکرر برای تغییر رمز عبور، حساب کاربری شما غیرفعال شده و آدرس IP شما مسدود شده است. لطفاً با مدیر سیستم تماس بگیرید.");
                }
            }

            // Ensure SecurityStamp is not null before UpdateAsync (fixes production environment issue)
            if (string.IsNullOrEmpty(toUpdateUser.SecurityStamp))
            {
                await _userManager.UpdateSecurityStampAsync(toUpdateUser);
            }

            var updateResult = await _userManager.UpdateAsync(toUpdateUser);
            if (!updateResult.Succeeded)
            {
                var errorMessage = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User update after password change failed for {UserId}: {Errors}", toUpdateUser.Id, errorMessage);
                return OperationResult.Failed(errorMessage);
            }
        }

        return OperationResult.Succeeded(payload: 1);
    }

    private static int CalculateLevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return string.IsNullOrEmpty(b) ? 0 : b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        var n = a.Length;
        var m = b.Length;
        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }


}

