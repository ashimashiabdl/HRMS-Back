using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Infrastructure.Mapper
{
    public class SystemSettingProfile : Profile
    {
        public SystemSettingProfile()
        {
            CreateMap<Core.Entities.OrganisationCoefficient, OrganisationCoefficientDTO>()
            .ForMember(dest => dest.CoefficientTitle, opt => opt.MapFrom(src => src.Coefficient == null ? "" : src.Coefficient.title))
            .ForMember(dest => dest.MappedExcelColumnTitle, opt => opt.MapFrom(src => src.MappedExcelColumn == null ? "" : src.MappedExcelColumn.title))
            ;
            CreateMap<OrganisationCoefficientDTO, Core.Entities.OrganisationCoefficient>();

            CreateMap<Core.Entities.OrganisationCostCenter, OrganisationCostCenterDTO>()
            .ForMember(dest => dest.CostCenterTitle, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
            .ForMember(dest => dest.PeymanRow, opt => opt.MapFrom(src => src.PeymanRow == null ? "" : src.PeymanRow.title + (" - " + src.PeymanRow.Code)))
            ;
            CreateMap<OrganisationCostCenterDTO, Core.Entities.OrganisationCostCenter>();



            CreateMap<Core.Entities.OrganisationPeymanRow, OrganisationPeymanRowDTO>()
            ;
            CreateMap<OrganisationPeymanRowDTO, Core.Entities.OrganisationPeymanRow>();



            CreateMap<Core.Entities.OrganisationEmployeeStatus, OrganisationEmployeeStatusDTO>().ForMember(
            dest => dest.EmployeeStatusGroupTitle, opt => opt.MapFrom(src => src.EmployeeStatusGroup == null ? "" : src.EmployeeStatusGroup.title)).ForMember(
            dest => dest.EmployeeStatusTitle, opt => opt.MapFrom(src => src.EmployeeStatus == null ? "" : src.EmployeeStatus.title));
            CreateMap<OrganisationEmployeeStatusDTO, Core.Entities.OrganisationEmployeeStatus>();

            CreateMap<Core.Entities.OrganisationOrderType, OrganisationOrderTypeDTO>()
           .ForMember(dest => dest.OrderTypeGroupTitle, opt => opt.MapFrom(src => src.OrderTypeGroup == null ? "" : src.OrderTypeGroup.title))
           .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
           .ForMember(dest => dest.OrderDirectionTypeTitle, opt => opt.MapFrom(src => src.OrderDirectionType == null ? "" : src.OrderDirectionType.title))
            ;

            CreateMap<OrganisationOrderTypeDTO, Core.Entities.OrganisationOrderType>();

            CreateMap<OrganisationMRTDTO, Core.Entities.OrganisationMRT>().ReverseMap();

            CreateMap<OrganisationEmployeeTypeMRTDTO, OrganisationEmployeeTypeMRT>();
            CreateMap<OrganisationEmployeeTypeMRT, OrganisationEmployeeTypeMRTDTO>()
           .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
           .ForMember(dest => dest.SettingType, opt => opt.MapFrom(src => src.SettingType == null ? "" : src.SettingType.title))
            ;

            CreateMap<Core.Entities.OrganisationEmployeeType, OrganisationEmployeeTypeDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.EmployeeTypeGroupTitle, opt => opt.MapFrom(src => src.EmployeeTypeGroup == null ? "" : src.EmployeeTypeGroup.title));
            CreateMap<OrganisationEmployeeTypeDTO, Core.Entities.OrganisationEmployeeType>();


            CreateMap<Core.Entities.OrganisationWageItem, OrganisationWageItemDTO>()
            .ForMember(dest => dest.WageItemTitle, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
            .ForMember(dest => dest.MappedExcelColumnTitle, opt => opt.MapFrom(src => src.MappedExcelColumn == null ? "" : src.MappedExcelColumn.title))
            ;
            CreateMap<OrganisationWageItemDTO, Core.Entities.OrganisationWageItem>();

            CreateMap<Core.Entities.OrganisationSettlementCause, OrganisationSettlementCauseDTO>()
            .ForMember(dest => dest.SettlementCauseTitle, opt => opt.MapFrom(src => src.SettlementCause == null ? "" : src.SettlementCause.title));
            CreateMap<OrganisationSettlementCauseDTO, Core.Entities.OrganisationSettlementCause>();

            CreateMap<Core.Entities.OrganisationSettlementItem, OrganisationSettlementItemDTO>()
            .ForMember(dest => dest.SettlementItemTitle, opt => opt.MapFrom(src => src.SettlementItem == null ? "" : src.SettlementItem.title));
            CreateMap<OrganisationSettlementItemDTO, Core.Entities.OrganisationSettlementItem>();

            CreateMap<OrganisationAgentOfPunishmentEncourage, OrganisationAgentOfPunishmentEncourageDTO>()
            .ForMember(dest => dest.AgentOfPunishmentEncourage, opt => opt.MapFrom(src => src.AgentOfPunishmentEncourage == null ? "" : src.AgentOfPunishmentEncourage.title))
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
            .ForMember(dest => dest.AgentOfPunishmentEncourageGroup, opt => opt.MapFrom(src => src.AgentOfPunishmentEncourageGroup == null ? "" : src.AgentOfPunishmentEncourageGroup.title))
            ;
            CreateMap<OrganisationAgentOfPunishmentEncourageDTO, OrganisationAgentOfPunishmentEncourage>();


            CreateMap<OrganisationAgentOfPunishmentEncourageScoreInterval, OrganisationAgentOfPunishmentEncourageScoreIntervalDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
            ;
            CreateMap<OrganisationAgentOfPunishmentEncourageScoreIntervalDTO, OrganisationAgentOfPunishmentEncourageScoreInterval>();

            CreateMap<Core.Entities.OrganisationFormula, OrganisationFormulaDTO>()
            .ForMember(dest => dest.FormulaTitle, opt => opt.MapFrom(src => src.Formula == null ? "" : src.Formula.title))
            .ForMember(dest => dest.FormulaUsageLocationTitle, opt => opt.MapFrom(src => src.FormulaUsageLocation == null ? "" : src.FormulaUsageLocation.title));
            CreateMap<OrganisationFormulaDTO, Core.Entities.OrganisationFormula>();


            CreateMap<Core.Entities.OrganisationSetting, OrganisationSettingDTO>()
           .ForMember(dest => dest.SettingTitle, opt => opt.MapFrom(src => src.Setting == null ? "" : src.Setting.title));
            CreateMap<OrganisationSettingDTO, Core.Entities.OrganisationSetting>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderType, OrganisationEmployeeTypeOrderTypeDTO>()
           .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
           .ForMember(dest => dest.OrderLevelType, opt => opt.MapFrom(src => src.OrderLevelType == null ? "" : src.OrderLevelType.title))
           .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title));
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeDTO, Core.Entities.OrganisationEmployeeTypeOrderType>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeWageItem, OrganisationEmployeeTypeWageItemDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.WageItemTitle, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title));
            ;
            CreateMap<OrganisationEmployeeTypeWageItemDTO, Core.Entities.OrganisationEmployeeTypeWageItem>();

            CreateMap<Core.Entities.OrganisationEmployeeTypes.OrganisationEmployeeTypeSettlementItem, OrganisationEmployeeTypeSettlementItemDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.SettlementItemTitle, opt => opt.MapFrom(src => src.SettlementItem == null ? "" : src.SettlementItem.title))
            .ForMember(dest => dest.PaymentTypeTitle, opt => opt.MapFrom(src => src.PaymentType == null ? "" : src.PaymentType.title))
            .ForMember(dest => dest.EnterTypeTitle, opt => opt.MapFrom(src => src.EnterType == null ? "" : src.EnterType.title))
            .ForMember(dest => dest.OrganisationFormulaTitle, opt => opt.MapFrom(src =>
                src.OrganisationFormula != null && src.OrganisationFormula.Formula != null
                    ? src.OrganisationFormula.Formula.title
                    : ""))
            .ForMember(dest => dest.MeasurementUnitTitle, opt => opt.MapFrom(src => src.MeasurementUnit == null ? "" : src.MeasurementUnit.title));
            CreateMap<OrganisationEmployeeTypeSettlementItemDTO, Core.Entities.OrganisationEmployeeTypes.OrganisationEmployeeTypeSettlementItem>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeCoefficient, OrganisationEmployeeTypeCoefficientDTO>()
            .ForMember(dest => dest.CoefficientTitle, opt => opt.MapFrom(src => src.Coefficient == null ? "" : src.Coefficient.title))
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            ;
            CreateMap<OrganisationEmployeeTypeCoefficientDTO, Core.Entities.OrganisationEmployeeTypeCoefficient>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeCanChange, OrganisationEmployeeTypeOrderTypeCanChangeDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
            .ForMember(dest => dest.DefaultEmpTypeTitle, opt => opt.MapFrom(src => src.DefaultEmpType == null ? "" : src.DefaultEmpType.title))
            .ForMember(dest => dest.DefaultEmpStatusTitle, opt => opt.MapFrom(src => src.DefaultEmpStatus == null ? "" : src.DefaultEmpStatus.title))
            ;

            CreateMap<OrganisationEmployeeTypeOrderTypeCanChangeDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeCanChange>();
            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeCheck, OrganisationEmployeeTypeOrderTypeCheckDTO>()
           .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
           .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
           .ForMember(dest => dest.CheckTypeTitle, opt => opt.MapFrom(src => src.CheckType == null ? "" : src.CheckType.title))
           .ForMember(dest => dest.OrganisationFormulaTitle, opt => opt.MapFrom(src =>
               src.OrganisationFormula != null && src.OrganisationFormula.Formula != null
                   ? src.OrganisationFormula.Formula.title
                   : ""))
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeCheckDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeCheck>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeCoefficient, OrganisationEmployeeTypeOrderTypeCoefficientDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
            .ForMember(dest => dest.Coefficient, opt => opt.MapFrom(src => src.Coefficient == null ? "" : src.Coefficient.title))
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeCoefficientDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeCoefficient>();
            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeDescription, OrganisationEmployeeTypeOrderTypeDescriptionDTO>()
           .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
           .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeDescriptionDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeDescription>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeSummaryCalc, OrganisationEmployeeTypeOrderTypeSummaryCalcDTO>()
            .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
            .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeSummaryCalcDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeSummaryCalc>();

            CreateMap<Core.Entities.OrganisationEmployeeTypeOrderTypeWageItem, OrganisationEmployeeTypeOrderTypeWageItemDTO>()
           .ForMember(dest => dest.EmployeeTypeTitle, opt => opt.MapFrom(src => src.EmployeeType == null ? "" : src.EmployeeType.title))
           .ForMember(dest => dest.OrderTypeTitle, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))
           .ForMember(dest => dest.WageItem, opt => opt.MapFrom(src => src.WageItem == null ? "" : src.WageItem.title))
            ;
            CreateMap<OrganisationEmployeeTypeOrderTypeWageItemDTO, Core.Entities.OrganisationEmployeeTypeOrderTypeWageItem>();

        }
    }
}
