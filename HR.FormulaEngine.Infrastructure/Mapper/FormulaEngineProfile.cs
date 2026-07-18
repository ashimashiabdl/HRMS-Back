using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Infrastructure.Mapper
{
    public class FormulaEngineProfile : Profile
    {
        public FormulaEngineProfile()
        {
            CreateMap<FormulaDefinition, FormulaDefinitionDTO>()
                                .ForMember(dest => dest.OrganisationFormula, opt => opt.MapFrom(src => src.OrganisationFormula == null ? "" : (src.OrganisationFormula.Formula == null ? "" : src.OrganisationFormula.Formula.title)));
            ;
            CreateMap<FormulaDefinitionDTO, FormulaDefinition>();    
            
            
            
            
            CreateMap<FormulaDatabaseFunctionDefinition, FormulaDatabaseFunctionDefinitionDTO>()
                                .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : (src.OrganisationChart.title == null ? "" : src.OrganisationChart.title)))
                                .ForMember(dest => dest.FuctionType, opt => opt.MapFrom(src => src.FuctionType == null ? "" : (src.FuctionType.title == null ? "" : src.FuctionType.title)));
            ;
            CreateMap<FormulaDatabaseFunctionDefinitionDTO, FormulaDatabaseFunctionDefinition>();    
            
            
            
            
 
            
            
            
            CreateMap<FormulaTable, FormulaTableDTO>()
                                .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : (src.OrganisationChart.title == null ? "" : src.OrganisationChart.title)))
                                .ForMember(dest => dest.TableType, opt => opt.MapFrom(src => src.TableType == null ? "" : (src.TableType.title == null ? "" : src.TableType.title)));
            ;
            CreateMap<FormulaTableDTO, FormulaTable>();  
            
            
            
            CreateMap<FormulaTableValue, FormulaTableValueDTO>()
                                .ForMember(dest => dest.FormulaTable, opt => opt.MapFrom(src => src.FormulaTable == null ? "" : (src.FormulaTable.title == null ? "" : src.FormulaTable.title)))
                                .ForMember(dest => dest.TableType, opt => opt.MapFrom(src => src.FormulaTable == null ? "" : (src.FormulaTable.TableType == null ? "" : src.FormulaTable.TableType.title)))
                                .ForMember(dest => dest.TableTypeId, opt => opt.MapFrom(src => src.FormulaTable == null ? 0 : (src.FormulaTable.TableType == null ? 0 : src.FormulaTable.TableType.Id)))
                                ;
            ;
            CreateMap<FormulaTableValueDTO, FormulaTableValue>();



            CreateMap<FormulaOperandDTO, FormulaOperand>().ReverseMap();
        }
    }
}
