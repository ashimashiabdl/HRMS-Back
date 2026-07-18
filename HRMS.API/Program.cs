using AutoMapper;
using Hr.Employee.infrastructure;
using Hr.Employee.infrastructure.Data;
using Hr.Employee.infrastructure.Mapper;
using Hr.SystemSetting.Infrastructure;
using Hr.SystemSetting.Infrastructure.Data;
using Hr.SystemSetting.Infrastructure.Mapper;
using HR.BaseInfo.infrastructure;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Mapper;
using HR.FormulaEngine.Infrastructure;
using HR.FormulaEngine.Infrastructure.Data;
using HR.FormulaEngine.Infrastructure.Mapper;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Mapper;
using HR.Identity.infrastructure.Services;
using HR.Order.Infrastructure;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Mapper;
using HR.Organisation.Infrastructure;
using HR.Organisation.Infrastructure.Data;
using HR.Organisation.Infrastructure.Mapper;
using HR.Payroll.Infrastructure;
using HR.Payroll.Infrastructure.Data;
using HR.Payroll.Infrastructure.Mapper;
using HR.Report.Infrastructure;
using HR.Report.Infrastructure.Data;
using HR.Report.Infrastructure.Mapper;
using HR.Report.Infrastructure.Services;

using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Middleware;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using HR.SharedKernel.Upload;
using HR.Attendance.Infrastructure.Data;
using HR.Attendance.Infrastructure.Mapper;
using HR.WorkFlow.Infrastructure;
using HR.WorkFlow.Infrastructure.Data;
using HR.WorkFlow.Infrastructure.Mapper;
using HRMS.API.Cache;
using HRMS.API.HostedServices;
using HRMS.API.HostedServices.HeavyJobStatus;
using HRMS.API.Infrastructure;
using HRMS.API.Infrastructure.Security;
using HRMS.API.Infrastructure.SMS;
using HRMS.API.Infrastructure.Upload;
using HRMS.API.Scanner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using NLog;
using NLog.Targets;
using NLog.Web;

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;

using HR.Report.Infrastructure.GlobalDBContext;
using Microsoft.OpenApi;
using OfficeOpenXml;

// Initialize NLog - try to load from nlog.config file first, fallback to appsettings
Logger logger;
try
{
    var configPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
    if (File.Exists(configPath))
    {
        logger = LogManager.Setup().LoadConfigurationFromFile(configPath).GetCurrentClassLogger();
    }
    else
    {
        logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    }
}
catch (Exception ex)
{
    // Fallback to console if NLog fails to initialize
    logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    logger.Warn(ex, "Failed to load nlog.config, using appsettings fallback");
}

// -----------------------
// Last-resort crash diagnostics (process-level)
// چون لاگ‌گیری اصلی روی دیتابیس است، خطایی که باعث سقوط کل پراسس (w3wp) می‌شود معمولاً
// هرگز در دیتابیس ثبت نمی‌شود. این هندلرها خطاهای مدیریت‌نشده‌ی سطح پراسس را در یک فایل محلی
// (logs/crash-*.log) ثبت می‌کنند تا علت واقعی توقف سایت مشخص شود.
// -----------------------
var crashLogDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
void WriteCrashLog(string source, Exception? ex)
{
    try
    {
        Directory.CreateDirectory(crashLogDirectory);
        var file = Path.Combine(crashLogDirectory, $"crash-{DateTime.Now:yyyyMMdd}.log");
        var content =
            $"==== {DateTimeOffset.Now:O} | {source} ===={Environment.NewLine}" +
            $"{ex}{Environment.NewLine}{Environment.NewLine}";
        File.AppendAllText(file, content);
    }
    catch { /* در آخرین لایه‌ی دفاعی نباید خطایی پرتاب شود */ }
}

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    var ex = e.ExceptionObject as Exception;
    WriteCrashLog($"AppDomain.UnhandledException (IsTerminating={e.IsTerminating})", ex);
    try { logger.Fatal(ex, "Unhandled exception – process terminating={Terminating}", e.IsTerminating); } catch { }
};

// خطاهای Task که هرگز await/observe نشده‌اند را مهار و ثبت می‌کنیم تا باعث رفتار غیرمنتظره نشوند.
TaskScheduler.UnobservedTaskException += (_, e) =>
{
    WriteCrashLog("TaskScheduler.UnobservedTaskException", e.Exception);
    try { logger.Error(e.Exception, "Unobserved task exception"); } catch { }
    e.SetObserved();
};

// EPPlus 8+ requires license to be set once at startup before any ExcelPackage use
ExcelPackage.License.SetNonCommercialOrganization("HRMS");

StimulsoftAssemblyResolver.EnsureRegistered();
StimulsoftBootstrap.EnsureInitialized();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // -----------------------
    // Logging Setup
    // -----------------------
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Logging.AddConsole();
    // Reduce console noise: keep our own Info logs, warn+ for framework, error for others
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>("Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning);
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>("System", Microsoft.Extensions.Logging.LogLevel.Warning);
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>("HR", Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>("Hr", Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Logging.AddFilter<Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider>("*", Microsoft.Extensions.Logging.LogLevel.Error);
    builder.Host.UseNLog();

    ConfigureNLogDatabaseTarget(builder.Configuration, logger);
    logger.Info("HRMS Is Starting");

    // -----------------------
    // Configuration & Services
    // -----------------------

    // SMS Providers
    builder.Services.AddScoped<ISMSProvider1, AsanakSMSProvider>();
    builder.Services.AddScoped<ISMSProvider2, KaveNegarSMSProvider>();
    builder.Services.AddScoped<ISMSProvider, SMSProvider>();
    builder.Services.Configure<AsanakSettingModel>(builder.Configuration.GetSection("smsproviders:AsanakSetting"));
    builder.Services.Configure<KaveNegarSettingModel>(builder.Configuration.GetSection("smsproviders:KaveNegarSetting"));

    // User Resolver & Auth
    builder.Services.AddScoped<UserResolverService>();
    builder.Services.AddScoped<RefreshTokenService>();
    // Stateless crypto helper derived from configuration; singleton avoids redundant key derivation per request
    builder.Services.AddSingleton<HR.SharedKernel.Security.UserIdEncryptionService>();
    builder.Services.AddScoped<HRAuthenticationService>();
    builder.Services.AddScoped<HRMS.API.Infrastructure.Security.SecurityAuditService>();

    // اضافه کردن Distributed Cache (Memory Cache به صورت پیش‌فرض)
    // اگر Redis موجود است، می‌توانید از AddStackExchangeRedisCache استفاده کنید
    builder.Services.AddDistributedMemoryCache();
    // برای استفاده از Redis، خط زیر را uncomment کنید و connection string را تنظیم کنید:
    // builder.Services.AddStackExchangeRedisCache(options =>
    // {
    //     options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    // });

    // Session configuration for sensitive operations verification
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expires after 30 minutes of inactivity
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Will be true in HTTPS
    });

    // Stateless data-access helpers (no per-request state; safe as singleton)
    builder.Services.AddSingleton<IDapper, HR.SharedKernel.Dapper.Dapper>();

    // Align with scoped DbContext: one UnitOfWork per context type per HTTP request
    builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
    builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

    // HttpContextAccessor must be singleton so scoped services can read the current request
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    // HttpClient for KeepAlive service
    builder.Services.AddHttpClient();

    // Named HttpClient for KeepAlive with optional cert validation bypass for IP-over-HTTPS
    builder.Services.AddHttpClient("KeepAlive")
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var skip = builder.Configuration.GetValue<bool>("KeepAlive:SkipCertificateValidation");
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) =>
                {
                    if (errors == SslPolicyErrors.None)
                    {
                        return true;
                    }

                    if (!skip)
                    {
                        return false;
                    }

                    var host = message?.RequestUri?.Host;
                    if (!string.IsNullOrEmpty(host) && IPAddress.TryParse(host, out _))
                    {
                        // Allow when explicitly configured and the host is an IP address
                        return true;
                    }

                    return false;
                }
            };
        });

    // Exception & Validation
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddScoped<ValidationFilterAttribute>();
    builder.Services.AddScoped<PermissionScanner>();
    builder.Services.AddScoped<DefaultPermissionRouteSeeder>();
    builder.Services.AddScoped<HR.Identity.infrastructure.Services.PermissionsService>();
    builder.Services.AddScoped(typeof(LogActionCallsAttribute));
    builder.Services.AddScoped<HeavyJobStuckRequestRecovery>();
    builder.Services.AddHostedService<HeavyJobWorker>();
    builder.Services.AddHostedService<BatchSettlementWorker>();
    builder.Services.AddHostedService<KeepAliveHostedService>();
    builder.Services.AddHostedService<DefaultPermissionRouteHostedService>();
    builder.Services.AddHostedService<ReportDataSourceUpdateService>();

    // جلوگیری از توقف کل سایت در صورت بروز خطای مدیریت‌نشده در سرویس‌های پس‌زمینه.
    // در .NET 6+ مقدار پیش‌فرض BackgroundServiceExceptionBehavior برابر StopHost است؛ یعنی اگر
    // یکی از BackgroundServiceها در ExecuteAsync خطای مهارنشده بدهد، کل هاست (و در حالت inprocess
    // کل w3wp) متوقف می‌شود. با Ignore، خطای یک Worker فقط لاگ می‌شود و سایت پابرجا می‌ماند.
    builder.Services.Configure<HostOptions>(options =>
    {
        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    });

    // Captcha
    builder.Services.AddMemoryCache();
    builder.Services.Configure<CaptchaOptions>(builder.Configuration.GetSection("Security:Captcha"));
    builder.Services.AddSingleton<CaptchaService>();

    // RSA key service for client-side password encryption support
    builder.Services.AddSingleton<RsaKeyService>();

    // File Upload Validation
    builder.Services.Configure<FileUploadValidationOptions>(builder.Configuration.GetSection("FileUploadValidation"));
    builder.Services.AddScoped<FileUploadValidationFilter>();


    // MVC Setup with Filters
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<LogActionCallsAttribute>();
        options.Filters.Add<FileUploadValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        // Accept numeric payloads sent as JSON strings (e.g. "175" for int/decimal fields)
        // to avoid model-binding failures across forms in production.
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });
    builder.Services.AddMvc(options => options.Filters.Add<GlobalModelStateValidatorAttribute>());

    // Configure FormOptions with safe limits to avoid memory pressure/overflow
    var configuredMaxFileSize = builder.Configuration.GetValue<long>("FileUploadValidation:MaxFileSizeBytes", 200L * 1024 * 1024);
    builder.Services.Configure<FormOptions>(o =>
    {
        o.ValueLengthLimit = 1 * 1024 * 1024; // 1 MB per value
        o.MultipartBodyLengthLimit = configuredMaxFileSize + (5L * 1024 * 1024); // headroom above max file
        o.MemoryBufferThreshold = 1 * 1024 * 1024; // push large bodies to disk buffers
    });

    // -----------------------
    // Kestrel Binding (listen on all interfaces if no URLs specified)
    // -----------------------
    var behindReverseProxy = builder.Configuration.GetValue<bool>("Hosting:BehindReverseProxy");
    var hostingHttpsPort = builder.Configuration.GetValue<int?>("Hosting:HttpsPort") ?? 5001;
    var kestrelCertPath = builder.Configuration["Kestrel:Certificates:Default:Path"];
    var kestrelCertPassword = builder.Configuration["Kestrel:Certificates:Default:Password"];

    // Always suppress the 'Server' header added by Kestrel
    builder.WebHost.ConfigureKestrel((context, options) =>
    {
        options.AddServerHeader = false;
        options.Limits.MaxRequestHeadersTotalSize = 32 * 1024; // 32 KB
        options.Limits.MaxRequestBodySize = configuredMaxFileSize + (5L * 1024 * 1024);

        var configuredUrls = context.Configuration["ASPNETCORE_URLS"] ?? context.Configuration["urls"];
        if (!string.IsNullOrWhiteSpace(configuredUrls))
            return;

        var httpPort = context.Configuration.GetValue<int?>("HttpPort") ?? 5000;
        options.ListenAnyIP(httpPort);

        // Behind IIS/nginx: only HTTP; TLS is terminated at the reverse proxy with the site certificate.
        if (context.Configuration.GetValue<bool>("Hosting:BehindReverseProxy"))
            return;

        if (string.IsNullOrWhiteSpace(kestrelCertPath))
            return;

        var fullCertPath = Path.IsPathRooted(kestrelCertPath)
            ? kestrelCertPath
            : Path.Combine(AppContext.BaseDirectory, kestrelCertPath);
        if (!File.Exists(fullCertPath))
        {
            logger.Warn("Kestrel HTTPS certificate not found at {CertPath}. Listening on HTTP only.", fullCertPath);
            return;
        }

        var certificate = new X509Certificate2(
            fullCertPath,
            kestrelCertPassword ?? string.Empty,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
        options.ListenAnyIP(hostingHttpsPort, listen => listen.UseHttps(certificate));
    });

    // -----------------------
    // Database Context Registrations
    // -----------------------

    var rawConnectionString = builder.Configuration.GetConnectionString("HRMSConnection");



    var decrypted = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(rawConnectionString);
    var connectionString = decrypted ?? rawConnectionString;

    if (!string.IsNullOrWhiteSpace(rawConnectionString) && !HR.SharedKernel.Security.ConnectionStringProtector.IsEncrypted(rawConnectionString))
    {
        logger.Warn("Connection string is not encrypted in configuration. Consider storing as enc::dpapi-* value.");
    }

    void ConfigureBaseDbContext<TContext>(string migrationSchema) where TContext : BaseDbContext
    {
        builder.Services.AddDbContext<TContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                sql.MigrationsHistoryTable("__EFMigrationsHistory", migrationSchema);
            });
        });

        // Register with UserResolverService injection
        builder.Services.AddScoped<TContext>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<TContext>>();
            var userResolverService = serviceProvider.GetRequiredService<UserResolverService>();
            return (TContext)Activator.CreateInstance(typeof(TContext), options, userResolverService);
        });
    }

    // Register all your DbContexts like this:
    ConfigureBaseDbContext<BaseInfoContext>("BaseInfo");
    ConfigureBaseDbContext<EmployeeContext>("Employee");
    ConfigureBaseDbContext<FormulaEngineContext>("For");
    ConfigureBaseDbContext<OrderContext>("Order");
    ConfigureBaseDbContext<OrganisationContext>("Org");
    ConfigureBaseDbContext<PayrollContext>("PayRoll");
    ConfigureBaseDbContext<ReportContext>("rpt");
    ConfigureBaseDbContext<SystemSettingContext>("Setting");
    ConfigureBaseDbContext<WorkFlowContext>("wf");
    ConfigureBaseDbContext<AttendanceContext>("att");

    builder.Services.AddDbContext<GlobalDbContext>((serviceProvider, options) =>
    {
        options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(GlobalDbContext).Assembly.FullName);
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "global");
        });
    });

    ConfigureBaseDbContext<CustomIdentityContext>("Identity");

    builder.Services.AddDbContext<IdentityContext>((serviceProvider, options) =>
    {
        options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName);
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "Identity");
        });
        // Snapshot is behind EF 10 Identity model; allow applying explicit migrations (e.g. RefreshToken SecurityStamp).
        options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    });

    // Register IdentityContext with UserResolverService injection
    builder.Services.AddScoped<IdentityContext>(serviceProvider =>
    {
        var options = serviceProvider.GetRequiredService<DbContextOptions<IdentityContext>>();
        var userResolverService = serviceProvider.GetRequiredService<UserResolverService>();
        return new IdentityContext(options, userResolverService);
    });

    // -----------------------
    // Identity & Authentication
    // -----------------------

    builder.Services.AddIdentity<AspNetUsers, AspNetRoles>(options =>
    {
        // Password complexity requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 1;
    })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

    // Register custom password hasher with per-user salt (backward-compatible)
    builder.Services.AddScoped<IPasswordHasher<AspNetUsers>, CustomPasswordHasher>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        // Enforce HTTPS metadata except during local development
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:JWTIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };

        // Configure JWT bearer to read token from cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Try to get token from Authorization header first (fallback)
                var token = context.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last();

                // If no token in header, try to get from cookie
                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request.Cookies["jwt"];
                }

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                // Enforce SecurityStamp check to invalidate old tokens after password change
                var userIdClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "userId");
                var userIdValue = userIdClaim?.Value;

                // Decrypt userId from token (supports both encrypted and plain format for backward compatibility)
                long userIdLong = -1;
                if (!string.IsNullOrEmpty(userIdValue))
                {
                    var encryptionService = context.HttpContext.RequestServices.GetService<HR.SharedKernel.Security.UserIdEncryptionService>();
                    if (encryptionService != null)
                    {
                        var decryptedUserId = encryptionService.DecryptUserId(userIdValue);
                        if (decryptedUserId > 0)
                        {
                            userIdLong = decryptedUserId;
                        }
                        else if (long.TryParse(userIdValue, out userIdLong))
                        {
                            // Fallback: plain number (backward compatibility)
                        }
                    }
                    else if (long.TryParse(userIdValue, out userIdLong))
                    {
                        // Fallback: plain number if encryption service not available
                    }
                }

                var userId = userIdLong > 0 ? userIdLong.ToString() : userIdValue;
                var tokenStamp = context.Principal?.Claims.FirstOrDefault(c => c.Type == "security_stamp")?.Value;
                var tokenIp = context.Principal?.Claims.FirstOrDefault(c => c.Type == "ip_address")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenStamp))
                {
                    context.HttpContext.Items["auth_error"] = "invalid_token";
                    context.Fail("invalid_token");
                    return;
                }

                // Validate IP address - token should only work from the IP it was issued to
                var currentIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(tokenIp) && !string.IsNullOrEmpty(currentIp) && tokenIp != "unknown")
                {
                    if (tokenIp != currentIp)
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogWarning("SECURITY: Token IP mismatch - Token IP: {TokenIp}, Current IP: {CurrentIp}, UserId: {UserId}, Endpoint: {Endpoint}",
                            tokenIp, currentIp, userId, context.HttpContext.Request.Path);

                        // Log suspicious activity
                        // Security: Username is not in token - use userId for identification
                        var auditService = context.HttpContext.RequestServices.GetService<HRMS.API.Infrastructure.Security.SecurityAuditService>();
                        var currentUserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                        if (auditService != null)
                        {
                            await auditService.LogSuspiciousActivityAsync(
                                userIdLong > 0 ? userIdLong : null,
                                null, // Username not available in token for security reasons
                                "TokenIPMismatch",
                                currentIp,
                                currentUserAgent,
                                $"Token issued to IP {tokenIp}, attempted use from IP {currentIp}",
                                context.HttpContext.Request.Path.Value);
                        }

                        context.HttpContext.Items["auth_error"] = "ip_mismatch";
                        context.Fail("ip_mismatch");
                        return;
                    }
                }

                // Validate User-Agent - token should only work from the same browser/client
                var tokenUserAgent = context.Principal?.Claims.FirstOrDefault(c => c.Type == "user_agent")?.Value;
                var currentUserAgent2 = context.HttpContext.Request.Headers["User-Agent"].ToString();
                if (!string.IsNullOrEmpty(tokenUserAgent) && !string.IsNullOrEmpty(currentUserAgent2) && tokenUserAgent != "unknown")
                {
                    // Allow some flexibility for minor version changes in User-Agent
                    if (!IsUserAgentSimilar(tokenUserAgent, currentUserAgent2))
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogWarning("SECURITY: Token User-Agent mismatch - Token UA: {TokenUA}, Current UA: {CurrentUA}, UserId: {UserId}, IP: {IP}, Endpoint: {Endpoint}",
                            tokenUserAgent, currentUserAgent2, userId, currentIp, context.HttpContext.Request.Path);

                        // Log suspicious activity
                        // Security: Username is not in token - use userId for identification
                        var auditService = context.HttpContext.RequestServices.GetService<HRMS.API.Infrastructure.Security.SecurityAuditService>();
                        if (auditService != null)
                        {
                            await auditService.LogSuspiciousActivityAsync(
                                userIdLong > 0 ? userIdLong : null,
                                null, // Username not available in token for security reasons
                                "TokenUserAgentMismatch",
                                currentIp,
                                currentUserAgent2,
                                $"Token issued to UA '{tokenUserAgent?.Substring(0, Math.Min(100, tokenUserAgent?.Length ?? 0))}...', attempted use from UA '{currentUserAgent2?.Substring(0, Math.Min(100, currentUserAgent2?.Length ?? 0))}...'",
                                context.HttpContext.Request.Path.Value);
                        }

                        context.HttpContext.Items["auth_error"] = "user_agent_mismatch";
                        context.Fail("user_agent_mismatch");
                        return;
                    }
                }

                try
                {
                    var scoped = context.HttpContext.RequestServices;
                    var identityDb = scoped.GetRequiredService<IdentityContext>();
                    var resolvedUserId = userIdLong > 0 ? userIdLong : (long.TryParse(userId, out var parsedId) ? parsedId : -1L);
                    if (resolvedUserId <= 0)
                    {
                        context.HttpContext.Items["auth_error"] = "invalid_token";
                        context.Fail("invalid_token");
                        return;
                    }

                    var userAuthState = await identityDb.AspNetUsers.AsNoTracking()
                        .Where(u => u.Id == resolvedUserId)
                        .Select(u => new
                        {
                            u.SecurityStamp,
                            u.MustChangePassword,
                            u.PasswordExpirationDate
                        })
                        .FirstOrDefaultAsync();

                    if (userAuthState is null)
                    {
                        context.HttpContext.Items["auth_error"] = "user_not_found";
                        context.Fail("user_not_found");
                        return;
                    }

                    var currentSecurityStamp = userAuthState.SecurityStamp;

                    // Fix: If SecurityStamp is null or empty, generate it (fixes production environment issue)
                    if (string.IsNullOrWhiteSpace(currentSecurityStamp))
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        logger?.LogWarning("SECURITY: User SecurityStamp is null or empty for UserId: {UserId}, Endpoint: {Endpoint}. Generating new SecurityStamp.",
                            userId, context.HttpContext.Request.Path);

                        var userManager = scoped.GetService<UserManager<AspNetUsers>>();
                        if (userManager is null)
                        {
                            context.HttpContext.Items["auth_error"] = "user_manager_unavailable";
                            context.Fail("user_manager_unavailable");
                            return;
                        }

                        var trackedUser = await userManager.FindByIdAsync(resolvedUserId.ToString());
                        if (trackedUser is null)
                        {
                            context.HttpContext.Items["auth_error"] = "user_not_found";
                            context.Fail("user_not_found");
                            return;
                        }

                        // Generate new SecurityStamp for users with null SecurityStamp
                        await userManager.UpdateSecurityStampAsync(trackedUser);
                        currentSecurityStamp = trackedUser.SecurityStamp;

                        // After generating SecurityStamp, the existing token will be invalid
                        // User needs to re-authenticate
                        if (string.IsNullOrWhiteSpace(currentSecurityStamp))
                        {
                            context.HttpContext.Items["auth_error"] = "security_stamp_generation_failed";
                            context.Fail("security_stamp_generation_failed");
                            return;
                        }
                    }

                    // Validate SecurityStamp matches token
                    if (!string.Equals(currentSecurityStamp, tokenStamp, StringComparison.Ordinal))
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                        var userStampPreview = !string.IsNullOrEmpty(currentSecurityStamp)
                            ? currentSecurityStamp.Substring(0, Math.Min(8, currentSecurityStamp.Length))
                            : "null";
                        var tokenStampPreview = !string.IsNullOrEmpty(tokenStamp)
                            ? tokenStamp.Substring(0, Math.Min(8, tokenStamp.Length))
                            : "null";
                        logger?.LogWarning("SECURITY: SecurityStamp mismatch for UserId: {UserId}, Endpoint: {Endpoint}. UserStamp: {UserStamp}, TokenStamp: {TokenStamp}",
                            userId, context.HttpContext.Request.Path, userStampPreview, tokenStampPreview);

                        context.HttpContext.Items["auth_error"] = "security_stamp_mismatch";
                        context.Fail("security_stamp_mismatch");
                        return;
                    }

                    context.HttpContext.Items[AuthorizationContextKeys.StampValidatedUserId] = resolvedUserId;

                    // Enforce password change requirement for users flagged as MustChangePassword or password expired
                    var passwordExpired = userAuthState.PasswordExpirationDate.HasValue && DateTime.Now >= userAuthState.PasswordExpirationDate.Value;
                    if (userAuthState.MustChangePassword || passwordExpired)
                    {
                        var path = context.HttpContext.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
                        // Allow only specific endpoints while password change is required
                        var allowedWhileChanging = new[]
                        {
                        "/api/aspnetusers/updatecurrentuserpassword",
                        "/api/identity/logout",
                        "/api/identity/getcurrentusername",
                        "/api/identity/public-key",
                        "/api/identity/captcha-required",
                        "/api/identity/isauthenticated",
                        "/api/userpaylocation/getaskeyvaluepair",
                        "/api/userloginhistory/getpageddatafail",
                        "/api/userloginhistory/getpageddatasuccess",
                        "/api/fiche/getfichecount",
                        "/api/permissionroute/getcurrentusermenu",
                        "/api/permissionroute/getrouteaccesscontext",
                        "/api/userdefaultsetting/getcurrentuserdefultsettingandinsertifnotexist",
                        "/api/userorganizationunit/getaskeyvaluepair",
                        "/api/userworkplace/getaskeyvaluepair",
                        "/api/usercostcenter/getaskeyvaluepair",
                        "/api/paymentperiod/getaskeyvaluepair",
                        "/api/paymentperiod/getcurrentuserdefaultpaymentpeiodid",
                        "/api/message/getunreadmessagecount",
                        "/api/identity/getcurrentuseremployeeid",
                        "/api/aspnetusers/getcurrentuserlastlogindate",
                        "/api/organisationchart/getcurrentuserdefaultorganid",
                        "/api/aspnetusers/getcurrentuserbasicinfo",
                        "/api/image/userimage"
                        };
                        var isAllowed = allowedWhileChanging.Any(p => path.StartsWith(p, StringComparison.Ordinal));
                        if (!isAllowed)
                        {
                            context.HttpContext.Items["auth_error"] = "password_change_required";
                            context.Fail("password_change_required");
                            return;
                        }
                    }
                }
                catch
                {
                    context.HttpContext.Items["auth_error"] = "security_stamp_validation_error";
                    context.Fail("security_stamp_validation_error");
                }
            },
            OnAuthenticationFailed = context =>
            {
                // IMPORTANT: do NOT write to the response here.
                // If we wrote a body now, [AllowAnonymous] endpoints (e.g. /api/identity/refresh-token,
                // /api/identity/login, /api/identity/public-key) would receive a committed 401 even though
                // they are designed to run anonymously and refresh / re-issue the session.
                // Only stash the failure reason so OnChallenge (which only fires when authorization
                // actually denies the request) can produce the final JSON response.
                var code = context.Exception is Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException
                    ? "token_expired" : "authentication_failed";
                context.HttpContext.Items["auth_error"] = code;
                // Detailed reason (exception type + validation message) used ONLY for security logging,
                // never returned to the client.
                context.HttpContext.Items["auth_error_detail"] =
                    $"{context.Exception.GetType().Name}: {context.Exception.Message}";
                context.NoResult();
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                // Return managed JSON response instead of default WWW-Authenticate header only
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var code = (context.HttpContext.Items["auth_error"] as string) ?? context.Error ?? "unauthorized";
                    string message = code switch
                    {
                        "token_expired" => "نشست شما منقضی شده است. لطفاً دوباره وارد شوید.",
                        "authentication_failed" => "احراز هویت ناموفق بود. لطفاً دوباره وارد شوید.",
                        "password_change_required" => "برای ادامه کار باید کلمه عبور را تغییر دهید.",
                        "security_stamp_mismatch" => "نشست شما منقضی شده است. لطفاً دوباره وارد شوید.",
                        "security_stamp_is_null" => "نشست شما منقضی شده است. لطفاً دوباره وارد شوید.",
                        "security_stamp_generation_failed" => "خطای بررسی نشست. لطفاً دوباره وارد شوید.",
                        "invalid_token" => "نشست نامعتبر است. لطفاً دوباره وارد شوید.",
                        "user_not_found" => "کاربر یافت نشد. لطفاً دوباره وارد شوید.",
                        "user_manager_unavailable" => "خطای داخلی احراز هویت. لطفاً دوباره وارد شوید.",
                        "security_stamp_validation_error" => "خطای بررسی نشست. لطفاً دوباره وارد شوید.",
                        "ip_mismatch" => "دسترسی از IP غیرمجاز. لطفاً دوباره وارد شوید.",
                        "user_agent_mismatch" => "دسترسی از مرورگر غیرمجاز. لطفاً دوباره وارد شوید.",
                        _ => "دسترسی غیرمجاز. لطفاً دوباره وارد شوید."
                    };
                    var payload = "{\"success\":false,\"message\":\"" + message.Replace("\"", "\\\"") + "\",\"code\":\"" + code + "\"}";
                    await context.Response.WriteAsync(payload);
                }
            }
        };
    });
    builder.Services.Configure<Identitysetting>(
        builder.Configuration.GetSection("JWT"));

    // -----------------------
    // Global Cookie Policy (Secure + HttpOnly)
    // -----------------------
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.Secure = CookieSecurePolicy.Always;
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    });

    // -----------------------
    // AutoMapper Setup
    // -----------------------

    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddMaps(typeof(WorkFlowProfile).Assembly);
        cfg.AddMaps(typeof(BaseInfoProfile).Assembly);
        cfg.AddMaps(typeof(EmployeeProfile).Assembly);
        cfg.AddMaps(typeof(FormulaEngineProfile).Assembly);
        cfg.AddMaps(typeof(IdentityProfile).Assembly);
        cfg.AddMaps(typeof(OrderProfile).Assembly);
        cfg.AddMaps(typeof(OrganisationProfile).Assembly);
        cfg.AddMaps(typeof(PayrollProfile).Assembly);
        cfg.AddMaps(typeof(ReportProfile).Assembly);
        cfg.AddMaps(typeof(SystemSettingProfile).Assembly);
        cfg.AddMaps(typeof(AttendanceProfile).Assembly);
    });

    // -----------------------
    // Scoped domain services (assembly scan via IScopedServices marker)
    // -----------------------

    RegisterScopedDomainServices(builder.Services, new[]
    {
    typeof(WorkFlowProfile).Assembly,
    typeof(BaseInfoProfile).Assembly,
    typeof(EmployeeProfile).Assembly,
    typeof(OrganisationProfile).Assembly,
    typeof(FormulaEngineProfile).Assembly,
    typeof(IdentityProfile).Assembly,
    typeof(OrderProfile).Assembly,
    typeof(PayrollProfile).Assembly,
    typeof(ReportProfile).Assembly,
    typeof(SystemSettingProfile).Assembly,
    typeof(AttendanceProfile).Assembly,
});

    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.BaseInfo.infrastructure.Import.SimpleEntityImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.EmployeeDeductionImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.EmployeeFundImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.EmployeeLeaveEntitlementImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.BlockedAccountImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.TaxNonCashPaymentImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.EmployeeImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.EducationImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.FamilyImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.RecruitOrderImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.InterdictOrderImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.InterdictOrderWageItemImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.InterdictOrderCoefficientItemImportHandler>());
    builder.Services.AddScoped<HR.BaseInfo.infrastructure.Import.IImportTargetHandler>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.GenericEmployeeEntityImportHandler>());
    builder.Services.AddScoped<HR.SharedKernel.Import.IImportOrganScopedFkValidator>(
        sp => sp.GetRequiredService<HR.Payroll.Infrastructure.Import.ImportPayrollOrganScopedFkValidator>());
    builder.Services.AddScoped<HR.SharedKernel.Import.IImportOrganScopedFkValidator>(
        sp => sp.GetRequiredService<HR.BaseInfo.infrastructure.Import.ImportBaseInfoContextFkValidator>());

    // -----------------------
    // Swagger / OpenAPI
    // -----------------------

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HRMS API",
            Version = "v1",
            Description = "سامانه سرمایه انسانی — مستندات API"
        });
        c.CustomSchemaIds(type => type.FullName);
        c.IgnoreObsoleteActions();
        c.IgnoreObsoleteProperties();

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization. فقط توکن را وارد کنید (Swagger خودش پیشوند Bearer را اضافه می‌کند).",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    // -----------------------
    // CORS Setup
    // -----------------------

    var allowedCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            if (allowedCorsOrigins.Length > 0)
            {
                policy
                    .WithOrigins(allowedCorsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        });
    });

    // -----------------------
    // API Behavior
    // -----------------------

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        // Don't suppress - we want InvalidModelStateResponseFactory to be called
        options.SuppressModelStateInvalidFilter = false;
        // Custom response factory for model binding errors (JSON deserialization, etc.)
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                .ToList();

            // Check if any error is a JSON conversion error
            var jsonError = errors.FirstOrDefault(e =>
                e?.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) == true ||
                e?.Contains("JSON value could not be converted", StringComparison.OrdinalIgnoreCase) == true ||
                e?.Contains("body", StringComparison.OrdinalIgnoreCase) == true ||
                e?.Contains("required", StringComparison.OrdinalIgnoreCase) == true);

            string detail;
            if (jsonError != null)
            {
                // Extract field name from error message if possible
                var pathMatch = Regex.Match(jsonError, @"Path:\s*\$\\.([^\s|]+)");
                var fieldName = pathMatch.Success ? pathMatch.Groups[1].Value.Trim() : null;

                if (!string.IsNullOrEmpty(fieldName))
                    detail = $"داده‌های ارسالی نامعتبر است. مقدار فیلد «{fieldName}» باید عدد صحیح باشد. لطفاً فیلدهای فرم را بررسی کنید.";
                else if (jsonError.Contains("body", StringComparison.OrdinalIgnoreCase) || jsonError.Contains("required", StringComparison.OrdinalIgnoreCase))
                    detail = "بدنه درخواست خالی یا نامعتبر است. لطفاً اطلاعات را ارسال کنید.";
                else
                    detail = "داده‌های ارسالی نامعتبر است. لطفاً فیلدهای فرم (از جمله فیلدهای عددی مانند وزن، قد، سایز) را به درستی پر کنید.";
            }
            else
            {
                // Generic validation error
                detail = string.Join(" ", errors);
            }

            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "درخواست نامعتبر",
                Detail = detail
            };

            return new BadRequestObjectResult(problemDetails);
        };
    });

    // HSTS only when explicitly enabled (avoid pinning broken certs on test hosts)
    if (builder.Configuration.GetValue<bool>("Hosting:UseHsts"))
    {
        builder.Services.AddHsts(options =>
        {
            options.Preload = builder.Configuration.GetValue<bool>("HSTS:Preload");
            options.IncludeSubDomains = builder.Configuration.GetValue<bool>("HSTS:IncludeSubDomains");
            options.MaxAge = TimeSpan.TryParse(builder.Configuration["HSTS:MaxAge"], out var maxAge)
                ? maxAge
                : TimeSpan.FromMinutes(10);
        });
    }

    // Forwarded headers when behind IIS/nginx (must run before HTTPS redirection)
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        options.RequireHeaderSymmetry = false;

        if (behindReverseProxy)
        {
            // Trust reverse proxy on the same host (IIS ARR / nginx); required for correct Request.Scheme
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            options.ForwardLimit = null;
        }

        var knownProxies = builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>();
        foreach (var proxy in knownProxies)
        {
            if (IPAddress.TryParse(proxy, out var ip))
                options.KnownProxies.Add(ip);
        }

        var knownNetworks = builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ?? Array.Empty<string>();
        foreach (var network in knownNetworks)
        {
            var parts = network.Split('/').Select(p => p.Trim()).ToArray();
            if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var prefix) && int.TryParse(parts[1], out var prefixLength))
                options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefix, prefixLength));
        }
    });

    // Optional, config-driven rate limiting (disabled by default)
    var rateLimitingSection = builder.Configuration.GetSection("Security:RateLimiting");
    var rateLimitingEnabled = rateLimitingSection.GetValue<bool>("Enabled");
    if (rateLimitingEnabled)
    {
        var permitLimit = Math.Max(1, rateLimitingSection.GetValue<int>("FixedWindow:PermitLimit", 100));
        var windowSeconds = Math.Max(1, rateLimitingSection.GetValue<int>("FixedWindow:WindowSeconds", 60));
        var queueLimit = Math.Max(0, rateLimitingSection.GetValue<int>("FixedWindow:QueueLimit", 0));

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds),
                    QueueLimit = queueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
    }


    // -----------------------
    // Helper Functions
    // -----------------------

    static bool IsUserAgentSimilar(string tokenUA, string currentUA)
    {
        // Exact match is always valid
        if (tokenUA == currentUA)
            return true;

        // Extract core browser information (ignore minor version differences)
        var tokenCore = ExtractUserAgentCore(tokenUA);
        var currentCore = ExtractUserAgentCore(currentUA);

        return tokenCore == currentCore;
    }

    static string ExtractUserAgentCore(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return string.Empty;

        // Extract major browser and OS information, ignore minor version numbers
        // Example: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0"
        // Becomes: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome"

        var parts = userAgent.Split(' ');
        var coreParts = new System.Collections.Generic.List<string>();

        foreach (var part in parts)
        {
            // Keep OS and platform info
            if (part.Contains("Windows") || part.Contains("Mac") || part.Contains("Linux") ||
                part.Contains("Android") || part.Contains("iOS") || part.Contains("x64") ||
                part.Contains("Win64") || part.Contains("ARM"))
            {
                coreParts.Add(part);
            }
            // Keep browser name without version
            else if (part.StartsWith("Chrome/") || part.StartsWith("Firefox/") ||
                     part.StartsWith("Safari/") || part.StartsWith("Edge/"))
            {
                var browserName = part.Split('/')[0];
                coreParts.Add(browserName);
            }
            // Keep Mozilla prefix
            else if (part.StartsWith("Mozilla/"))
            {
                coreParts.Add("Mozilla");
            }
        }

        return string.Join(" ", coreParts);
    }

    // -----------------------
    // Build & Middleware Pipeline
    // -----------------------

    var app = builder.Build();

    // Outermost: finally runs after exception handler so status codes are accurate
    app.UseMiddleware<RequestLoggingMiddleware>();

    // Remove 'Server' header from all app responses
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Remove("Server");
            return System.Threading.Tasks.Task.CompletedTask;
        });
        await next();
    });

    // Reverse proxy / TLS: honor X-Forwarded-* before any scheme-based middleware
    app.UseForwardedHeaders();

    var useHttpsRedirection = app.Configuration.GetValue<bool>("Hosting:UseHttpsRedirection");
    var useHsts = app.Configuration.GetValue<bool>("Hosting:UseHsts");

    if (!app.Environment.IsDevelopment() && useHsts)
        app.UseHsts();

#if !DEBUG
if (useHttpsRedirection)
    app.UseHttpsRedirection();
#endif

    // API-only: no wwwroot/SPA. Return JSON for non-success status codes instead of /404.html re-execute.
    app.UseStatusCodePages(async context =>
    {
        var response = context.HttpContext.Response;
        if (response.HasStarted || response.StatusCode < 400)
            return;

        response.ContentType = "application/json; charset=utf-8";
        var message = response.StatusCode switch
        {
            StatusCodes.Status404NotFound => "منبع درخواستی یافت نشد",
            StatusCodes.Status403Forbidden => "دسترسی مجاز نیست",
            _ => "خطا در پردازش درخواست"
        };
        await response.WriteAsJsonAsync(new { success = false, message, status = response.StatusCode });
    });

    // Enforce cookie policy before authentication/authorization
    app.UseCookiePolicy();

    // Session middleware (must be after UseCookiePolicy and before UseRouting)
    app.UseSession();

    // Global No-Cache Headers to prevent intermediaries from caching sensitive responses
    //app.Use(async (context, next) =>
    //{
    //    await next();

    //    // Apply only to dynamic API responses (not static files). If needed, narrow by path.
    //    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0, private";
    //    context.Response.Headers["Pragma"] = "no-cache";
    //    context.Response.Headers["Expires"] = "0";
    //});

    // Additional Security Headers
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            // Remove server header (already handled above but ensuring it's removed)
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");

            // Add security headers
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY"; // or SAMEORIGIN
                                                                  // X-XSS-Protection is deprecated and ineffective in modern browsers
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            // Harden CSP: strict for API, relaxed only for Swagger UI
            var requestPath = context.Request.Path.Value ?? string.Empty;
            if (requestPath.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                // Allow Swagger UI assets while keeping origins constrained to self
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; connect-src 'self'; font-src 'self' data:; frame-ancestors 'none'; base-uri 'none'";
            }
            else
            {
                // For API responses, block all active content by default
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'none'; frame-ancestors 'none'; base-uri 'none'";
            }
            // HSTS is managed by UseHsts(); avoid setting it manually here

            // Add no-cache headers safely just before headers are sent
            context.Response.Headers["Cache-Control"] = "s-maxage=0, max-age=0, post-check=0, must-revalidate, pre-check=0";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";

            return System.Threading.Tasks.Task.CompletedTask;
        });
        await next();
    });

    // JSON deserialization error middleware (catches errors before model binding)
    app.UseMiddleware<HR.SharedKernel.Extensions.JsonDeserializationErrorMiddleware>();

    // Use modern exception handler (IExceptionHandler)
    app.UseExceptionHandler();

    // Swagger must run before auth so /swagger stays publicly reachable
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "سامانه سرمایه انسانی");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "HRMS API";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableTryItOutByDefault();
    });

    app.MapGet("/", () => "سرویس سمت سرور سامانه در حال سرویس دهی می باشد -- OK");

    app.UseRouting();
    // Enable rate limiting if configured
    if (rateLimitingEnabled)
    {
        app.UseRateLimiter();
    }

    // CSRF protection for state-changing requests: require same-origin Origin/Referer
    app.Use(async (context, next) =>
    {
        var method = context.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
        {
            await next();
            return;
        }

        bool IsAllowedUri(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var u)) return false;

            var reqAuthority = context.Request.Host.Value;
            if (string.Equals(u.Authority, reqAuthority, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var o in allowedCorsOrigins)
            {
                if (Uri.TryCreate(o, UriKind.Absolute, out var ao) &&
                    string.Equals(ao.Authority, u.Authority, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        var origin = context.Request.Headers["Origin"].FirstOrDefault();
        var referer = context.Request.Headers["Referer"].FirstOrDefault();

        var allowed = false;
        if (!string.IsNullOrEmpty(origin))
        {
            allowed = IsAllowedUri(origin);
        }
        else if (!string.IsNullOrEmpty(referer))
        {
            allowed = IsAllowedUri(referer);
        }
        else
        {
            // No Origin/Referer present (legacy clients) → allow
            allowed = true;
        }

        if (!allowed)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"success\":false,\"message\":\"forbidden\",\"code\":\"csrf_blocked\"}");
            return;
        }

        await next();
    });

    app.UseCors("CorsPolicy");

    // Middleware برای بررسی IPهای بلاک شده (قبل از Authentication)
    app.UseMiddleware<HRMS.API.Infrastructure.BlockedIpMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    // Populate HttpContext.Items for NLog after authentication
    app.UseMiddleware<NLogUserMiddleWare>();
    app.UseMiddleware<SecurityLoggingMiddleware>();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    try { logger.Fatal(ex, "HRMS stopped because of an exception"); } catch { /* last-resort */ }
    throw;
}
finally
{
    LogManager.Shutdown();
}

static void RegisterScopedDomainServices(IServiceCollection services, Assembly[] assemblies)
{
    foreach (var assembly in assemblies)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IScopedServices).IsAssignableFrom(t)
                        && t != typeof(IScopedServices));

        foreach (var type in types)
        {
            services.AddScoped(type);
        }
    }
}

static void ConfigureNLogDatabaseTarget(IConfiguration configuration, Logger bootstrapLogger)
{
    try
    {
        var rawConnectionStringForNLog = configuration.GetConnectionString("HRMSConnection");
        if (string.IsNullOrEmpty(rawConnectionStringForNLog))
        {
            bootstrapLogger.Warn("HRMSConnection connection string not found in configuration. NLog database logging may not work.");
            return;
        }

        var decryptedForNLog = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(rawConnectionStringForNLog);
        var connectionStringForNLog = decryptedForNLog ?? rawConnectionStringForNLog;

        var nlogConfig = LogManager.Configuration;
        if (nlogConfig == null)
        {
            bootstrapLogger.Warn("NLog configuration is null. Check if nlog.config file exists and is accessible.");
            return;
        }

        var dbTarget = nlogConfig.FindTargetByName("dbTarget") as DatabaseTarget;
        if (dbTarget == null)
        {
            bootstrapLogger.Warn("NLog database target 'dbTarget' not found in configuration.");
            return;
        }

        if (string.IsNullOrEmpty(connectionStringForNLog))
        {
            bootstrapLogger.Warn("NLog connection string is empty. Database logging may not work.");
            return;
        }

        dbTarget.ConnectionString = connectionStringForNLog;
        LogManager.Configuration = nlogConfig;
        LogManager.ReconfigExistingLoggers();
        bootstrapLogger.Info("NLog database target configured with connection string from appsettings");
    }
    catch (Exception ex)
    {
        bootstrapLogger.Error(ex, "Error configuring NLog database target. Logging to database may not work.");
    }
}
