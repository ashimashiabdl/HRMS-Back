using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class OrganisationEmployeeTypeFicheItemService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationEmployeeTypeFicheItem, PayrollContext, OrganisationEmployeeTypeFicheItemDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    /// <summary>
    /// گرفتن فهرست اقلام سازمان جاری بر اساس نوع استخدام
    /// </summary>
    /// <param name="OrganisationChartId"></param>
    /// <param name="EmployeeTypeId"></param>
    /// <returns></returns>
    public List<OrganisationEmployeeTypeFicheItemDTO> GetCurrentOrganItemsByEmployeeType(long OrganisationChartId, long EmployeeTypeId)
    {
        var OrganisationFicheItemSetting = _db.Set<OrganisationFicheItem>()
       .Include(i => i.EnterType)
       .Include(i => i.OrganisationFormula)
       .Include(i => i.OrganisationCheckFormula)
       .Include(i => i.PaymentType)
       .Include(i => i.WageItem)
       .Include(i => i.OrganisationFormula == null ? null : i.OrganisationFormula.Formula)
       .Where(DateValidityExtension<OrganisationFicheItem>.GetDateValidationPredicate().And(i => i.OrganisationChartId == OrganisationChartId && i.OrganisationChartId == _currentUserDefaultOrganId)).ToList();

        var employeeTypeSetting = _mapper.Map<List<OrganisationEmployeeTypeFicheItemDTO>>(_db.Set<OrganisationEmployeeTypeFicheItem>()
       .Include(i => i.EnterType)
       .Include(i => i.OrganisationFormula)
       .Include(i => i.OrganisationCheckFormula)
       .Include(i => i.PaymentType)
       .Include(i => i.WageItem)
       .Include(i => i.OrganisationFormula == null ? null : i.OrganisationFormula.Formula)
       .Where(DateValidityExtension<OrganisationEmployeeTypeFicheItem>.GetDateValidationPredicate().And(i => i.EmployeeTypeId == EmployeeTypeId && i.OrganisationChartId == OrganisationChartId && i.OrganisationChartId == _currentUserDefaultOrganId)).ToList());

        var executAbleSetting = new List<OrganisationEmployeeTypeFicheItemDTO>();

        foreach (var item in employeeTypeSetting)
        {
            if (executAbleSetting.Any(i => i.WageItemId == item.WageItemId))
            {
                continue;
            }

            if (OrganisationFicheItemSetting.Any(i => i.WageItemId == item.WageItemId))
            {
                OrganisationEmployeeTypeFicheItemDTO toAdd = new OrganisationEmployeeTypeFicheItemDTO();

                if (item.UseDefaultOrganSetting == true)
                {
                    var organSetting = OrganisationFicheItemSetting.Single(i => i.WageItemId == item.WageItemId);

                    toAdd.WageItemId = organSetting.WageItemId;
                    toAdd.WageItem = organSetting.WageItem.title;
                    toAdd.EnterTypeId = organSetting.EnterTypeId;
                    toAdd.EnterType = organSetting.EnterType.title;
                    toAdd.PaymentTypeId = organSetting.PaymentTypeId;
                    toAdd.PaymentType = organSetting.PaymentType.title;
                    toAdd.OrganisationCheckFormulaId = organSetting.OrganisationCheckFormulaId;
                    toAdd.OrganisationFormulaId = organSetting.OrganisationFormulaId;
                    if (organSetting.OrganisationFormula != null)
                    {
                        toAdd.OrganisationFormula = organSetting.OrganisationFormula.Formula.title;
                    }
                    toAdd.Continuous = organSetting.Continuous;
                    toAdd.ShowZeroInFiche = organSetting.ShowZeroInFiche;
                    toAdd.IsVirtual = organSetting.IsVirtual;
                    toAdd.IsInsuranceCovered = organSetting.IsInsuranceCovered;
                    toAdd.IsTaxCovered = organSetting.IsTaxCovered;
                    toAdd.RetiredCovered = organSetting.RetiredCovered;
                    toAdd.DailyCovered = organSetting.DailyCovered;
                    toAdd.IsDaily = organSetting.IsDaily;
                    toAdd.Priority = organSetting.Priority;
                    toAdd.FixValue = organSetting.FixValue;
                    toAdd.Description = organSetting.Description;
                    toAdd.IsFixed = organSetting.IsFixed;
                    toAdd.OnceInFiche = organSetting.OnceInFiche;
                    toAdd.IsTaminInsurance = organSetting.IsTaminInsurance;
                    toAdd.ArearsCovered = organSetting.ArearsCovered;
                    toAdd.IsEmployerItem = organSetting.IsEmployerItem;
                    toAdd.RetiredCover = organSetting.RetiredCover;
                    toAdd.IsMainTaxItem = organSetting.IsMainTaxItem;
                    toAdd.IsTaxableContinuousCash = organSetting.IsTaxableContinuousCash;
                    toAdd.IsTaxableNonContinuousCash = organSetting.IsTaxableNonContinuousCash;
                    toAdd.IsTaxableContinuousNonCash = organSetting.IsTaxableContinuousNonCash;
                    toAdd.IsTaxableNonContinuousNonCash = organSetting.IsTaxableNonContinuousNonCash;
                    toAdd.ZeroNegativeArears = organSetting.ZeroNegativeArears;
                    toAdd.IsSpecialTax = organSetting.IsSpecialTax;
                    toAdd.IsTaxDiscount = organSetting.IsTaxDiscount;
                    toAdd.CurrentYearArearsCovered = organSetting.CurrentYearArearsCovered;
                    toAdd.Origin = "نوع استخدام";
                    toAdd.OriginId = (int)Enums.OriginOfFicheItem.EmploymentType;
                    if (organSetting.OrganisationCheckFormula != null)
                    {
                        toAdd.OrganisationCheckFormula = organSetting.OrganisationCheckFormula.Formula.title;
                    }
                }
                else
                {
                    toAdd = item;
                }

                if (executAbleSetting.Any(i => i.WageItemId == toAdd.WageItemId))
                {

                }
                else
                {
                    executAbleSetting.Add(toAdd);
                }
            }

        }

        return executAbleSetting;
    }
}
