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


using Microsoft.Extensions.Configuration;
using static HR.SharedKernel.Share.Enums;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationEmployeeTypeOrderTypeWageItemService : BaseService<OrganisationEmployeeTypeOrderTypeWageItem, SystemSettingContext, OrganisationEmployeeTypeOrderTypeWageItemDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeOrderTypeWageItemService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public new async Task<OperationResult> CreateForAsync(OrganisationEmployeeTypeOrderTypeWageItemDTO entityToCreate)
        {
            var validation = ValidateEnterType(entityToCreate);
            if (!validation.Success)
            {
                return validation;
            }

            NormalizeEnterType(entityToCreate);
            return await base.CreateForAsync(entityToCreate);
        }

        public new async Task<OperationResult> UpdateForAsync(OrganisationEmployeeTypeOrderTypeWageItemDTO entityToUpdate)
        {
            var validation = ValidateEnterType(entityToUpdate);
            if (!validation.Success)
            {
                return validation;
            }

            NormalizeEnterType(entityToUpdate);
            return await base.UpdateForAsync(entityToUpdate);
        }

        private static OperationResult ValidateEnterType(OrganisationEmployeeTypeOrderTypeWageItemDTO dto)
        {
            if (!(dto.EnterTypeId > 0))
            {
                return OperationResult.Failed("انتخاب کردن نحوه محاسبه الزامی می باشد");
            }

            if (dto.EnterTypeId == (long)EnterTypeId.UseFormula)
            {
                if (!(dto.OrganisationFormulaId is > 0))
                {
                    return OperationResult.Failed("فرمول محاسبه انتخاب نشده است");
                }
            }

            if (dto.Max.HasValue && dto.Min.HasValue && !(dto.Max.Value > dto.Min.Value))
            {
                return OperationResult.Failed("حداکثر و حداقل مبلغ را به صورت صحیح وارد بفرمایید");
            }

            return OperationResult.Succeeded();
        }

        private static void NormalizeEnterType(OrganisationEmployeeTypeOrderTypeWageItemDTO dto)
        {
            if (dto.EnterTypeId != (long)EnterTypeId.UseFormula)
            {
                dto.OrganisationFormulaId = null;
            }

            if (dto.EnterTypeId != (long)EnterTypeId.fixValue)
            {
                dto.FixValue = null;
            }
        }

        public bool Validate(OrganisationEmployeeTypeOrderTypeWageItem entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
