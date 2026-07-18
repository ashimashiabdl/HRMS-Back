using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Infrastructure.Mapper
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<DynamicReport, DynamicReportDTO>()
                .ForMember(dest => dest.OrganisationMRT, opt => opt.MapFrom(src => src.OrganisationMRT == null ? string.Empty : src.OrganisationMRT.title))
                .ForMember(dest => dest.ExportType, opt => opt.MapFrom(src => src.ExportType == null ? string.Empty : src.ExportType.title))
                .ForMember(dest => dest.FuctionType, opt => opt.MapFrom(src => src.FuctionType == null ? string.Empty : src.FuctionType.title));

            CreateMap<DynamicReportDTO, DynamicReport>();

            CreateMap<DynamicReportParameter, DynamicReportParameterDTO>()
                .ForMember(dest => dest.DynamicReport, opt => opt.MapFrom(src => src.DynamicReport == null ? string.Empty : src.DynamicReport.title))
                .ForMember(dest => dest.Parameter, opt => opt.MapFrom(src => src.Parameter == null ? string.Empty : src.Parameter.title));

            CreateMap<DynamicReportParameterDTO, DynamicReportParameter>();

            CreateMap<FieldDataType, FieldDataTypeDTO>();
            CreateMap<FieldDataTypeDTO, FieldDataType>();

            CreateMap<FieldOperator, FieldOperatorDTO>()
                .ForMember(dest => dest.FieldDataTypeTitle, opt => opt.MapFrom(src => src.FieldDataType == null ? string.Empty : src.FieldDataType.title));

            CreateMap<FieldOperatorDTO, FieldOperator>();

            CreateMap<ReportableEntity, ReportableEntityDTO>();
            CreateMap<ReportableEntityDTO, ReportableEntity>();

            CreateMap<ReportableField, ReportableFieldDTO>()
                .ForMember(dest => dest.ReportableEntityTitle, opt => opt.MapFrom(src => src.ReportableEntity == null ? string.Empty : src.ReportableEntity.title))
                .ForMember(dest => dest.FieldDataTypeTitle, opt => opt.MapFrom(src => src.FieldDataType == null ? string.Empty : src.FieldDataType.title));

            CreateMap<ReportableFieldDTO, ReportableField>();

            CreateMap<PayLocationProgressReport, PayLocationProgressReportDTO>()
                .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title));

            CreateMap<PayLocationProgressReportDTO, PayLocationProgressReport>();
        }
    }
}
