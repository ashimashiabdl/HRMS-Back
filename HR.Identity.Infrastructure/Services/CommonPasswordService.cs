using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HR.Identity.infrastructure.Data;

namespace HR.Identity.infrastructure.Services;

public class CommonPasswordService : BaseService<CommonPassword, IdentityContext, CommonPasswordDTO>, IScopedServices
{
    private readonly ILogger<CommonPasswordService>? _logger;
    
    public CommonPasswordService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<CommonPasswordService> logger) 
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public bool Validate(CommonPassword entity, object etc = null)
    {
        return true;
    }

    /// <summary>
    /// بررسی می‌کند که آیا کلمه عبور در لیست کلمه‌های عبور متداول وجود دارد یا نه
    /// </summary>
    /// <param name="password">کلمه عبور برای بررسی</param>
    /// <returns>true اگر کلمه عبور متداول باشد، false در غیر این صورت</returns>
    public bool IsCommonPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        // بررسی دقیق (case-sensitive)
        var exactMatch = All(false)
            .Where(cp => !cp.IsDeleted && cp.Password == password)
            .Any();

        if (exactMatch)
        {
            return true;
        }

        // بررسی بدون در نظر گیری حروف بزرگ/کوچک
        var caseInsensitiveMatch = All(false)
            .Where(cp => !cp.IsDeleted && cp.Password.ToLower() == password.ToLower())
            .Any();

        return caseInsensitiveMatch;
    }
}

