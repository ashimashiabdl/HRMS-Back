using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;

using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationEmployeeTypeWageItemService : BaseService<OrganisationEmployeeTypeWageItem, SystemSettingContext, OrganisationEmployeeTypeWageItemDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeWageItemService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public new async Task<OperationResult> CreateForAsync(OrganisationEmployeeTypeWageItemDTO entityToCreate)
        {
            if (entityToCreate == null)
            {
                return OperationResult.Failed("اطلاعات ارسالی معتبر نیست");
            }

            await ClearExclusiveFlagsAsync(
                organisationChartId: _currentUserDefaultOrganId,
                employeeTypeId: entityToCreate.EmployeeTypeId,
                excludeId: null,
                clearDailyAndWage: entityToCreate.IsDailyAndWage == true,
                clearSanavatInc: entityToCreate.IsSanavatINC == true);

            return await base.CreateForAsync(entityToCreate);
        }

        public new async Task<OperationResult> UpdateForAsync(OrganisationEmployeeTypeWageItemDTO entityToUpdate)
        {
            if (entityToUpdate == null || !(entityToUpdate.Id > 0))
            {
                return OperationResult.Failed("اطلاعات ارسالی معتبر نیست");
            }

            var existing = await _unitOfWork.Context.Set<OrganisationEmployeeTypeWageItem>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == entityToUpdate.Id && i.IsDeleted != true);

            if (existing == null)
            {
                return OperationResult.Failed("رکورد یافت نشد");
            }

            var employeeTypeId = entityToUpdate.EmployeeTypeId > 0
                ? entityToUpdate.EmployeeTypeId
                : existing.EmployeeTypeId;

            await ClearExclusiveFlagsAsync(
                organisationChartId: existing.OrganisationChartId,
                employeeTypeId: employeeTypeId,
                excludeId: existing.Id,
                clearDailyAndWage: entityToUpdate.IsDailyAndWage == true,
                clearSanavatInc: entityToUpdate.IsSanavatINC == true);

            return await base.UpdateForAsync(entityToUpdate);
        }

        /// <summary>
        /// فقط یک آیتم برای هر نوع استخدام می‌تواند DSW_ROOZ / DSW_INC باشد.
        /// </summary>
        private async Task ClearExclusiveFlagsAsync(
            long organisationChartId,
            long employeeTypeId,
            long? excludeId,
            bool clearDailyAndWage,
            bool clearSanavatInc)
        {
            if ((!clearDailyAndWage && !clearSanavatInc) || organisationChartId <= 0 || employeeTypeId <= 0)
            {
                return;
            }

            var query = _unitOfWork.Context.Set<OrganisationEmployeeTypeWageItem>()
                .Where(i => i.IsDeleted != true
                    && i.OrganisationChartId == organisationChartId
                    && i.EmployeeTypeId == employeeTypeId);

            if (excludeId > 0)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }

            var siblings = await query.ToListAsync();
            foreach (var sibling in siblings)
            {
                if (clearDailyAndWage && sibling.IsDailyAndWage)
                {
                    sibling.IsDailyAndWage = false;
                }

                if (clearSanavatInc && sibling.IsSanavatINC)
                {
                    sibling.IsSanavatINC = false;
                }
            }
        }

        public bool Validate(OrganisationEmployeeTypeWageItem entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
