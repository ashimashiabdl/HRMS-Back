using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Identity.infrastructure.Services;

public class UserMenuService : BaseService<UserMenu, IdentityContext, UserMenuDTO>, IScopedServices
{
    public UserMenuService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// حذف فیزیکی رکورد از دیتابیس (نه soft delete)
    /// </summary>
    public new OperationResult DeleteRecord(long id)
    {
        var entity = _unitOfWork.Context.Set<UserMenu>()
                .FirstOrDefault(e => e.Id == id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        // حذف فیزیکی
        _unitOfWork.Context.Set<UserMenu>().Remove(entity);

        if (_unitOfWork.Save().Result > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }
        else
        {
            return OperationResult.Failed();
        }
    }

    /// <summary>
    /// بازگرداندن تمام رکوردها بدون فیلتر IsDeleted (چون حذف فیزیکی است)
    /// </summary>
    public new IQueryable<UserMenu> All(bool IgnoreExpired = true, DateTime? ImpleDate = null)
    {
        // چون حذف فیزیکی است، نیازی به فیلتر IsDeleted نیست
        return _unitOfWork.Context.Set<UserMenu>()
            .Where(HR.SharedKernel.Extensions.DateValidityExtension<UserMenu>.GetDateValidationPredicate(IgnoreExpired, ImpleDate));
    }
}
