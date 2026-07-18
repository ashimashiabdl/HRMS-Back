using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.DTOs;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Infrastructure.Mapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<InterdictOrderWageItem, InterdictOrderWageItemDTO>().ReverseMap();
            CreateMap<InterdictOrderCoefficientItem, InterdictOrderCoefficientItemDTO>().ReverseMap();
            CreateMap<OrganisationEmployeeTypeOrderTypeDescription, OrganisationEmployeeTypeOrderTypeDescriptionDTO>().ReverseMap();


            CreateMap<InterdictOrder, InterdictOrderDTO>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeId))
            .ForMember(dest => dest.PayLocationId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.PayLocationId))
            .ForMember(dest => dest.CostCenterId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.CostCenterId))
            .ForMember(dest => dest.OrganizationUnitId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationUnitId))
            .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : (src.RecruitOrder.OrganizationUnit == null ? "" : src.RecruitOrder.OrganizationUnit.title)))
            .ForMember(dest => dest.WorkPlaceId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.WorkPlaceId))
            .ForMember(dest => dest.WorkPlace, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : (src.RecruitOrder.WorkPlace == null ? "" : src.RecruitOrder.WorkPlace.title)))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.ProjectId))
            .ForMember(dest => dest.EmployeeStatusId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeStatusId))
            .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeTypeId))
            .ForMember(dest => dest.OrganizationJobId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationJobId))
            .ForMember(dest => dest.OrganisationPositionId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganisationPositionId));




            CreateMap<InterdictOrder, InterdictOrderFlatDTO>()
                .ForMember(dest => dest.ImpleDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeId))
                .ForMember(dest => dest.PayLocationId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.PayLocationId))
                .ForMember(dest => dest.CostCenterId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.CostCenterId))
                .ForMember(dest => dest.OrganizationUnitId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationUnitId))
                .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : (src.RecruitOrder.OrganizationUnit == null ? "" : src.RecruitOrder.OrganizationUnit.title)))
                .ForMember(dest => dest.WorkPlaceId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.WorkPlaceId))
                .ForMember(dest => dest.WorkPlace, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : (src.RecruitOrder.WorkPlace == null ? "" : src.RecruitOrder.WorkPlace.title)))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.ProjectId))
                .ForMember(dest => dest.EmployeeStatusId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeStatusId))
                .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeTypeId))
                .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : (src.RecruitOrder.EmployeeType == null ? "" : src.RecruitOrder.EmployeeType.title)))
                .ForMember(dest => dest.OrganizationJobId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationJobId))
                .ForMember(dest => dest.OrganisationPositionId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganisationPositionId));



            CreateMap<BatchRequest, BatchRequestDTO>()
               .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
               .ForMember(dest => dest.RequestState, opt => opt.MapFrom(src => src.RequestState == null ? "" : src.RequestState.title))
               .ForMember(dest => dest.ArchiveState, opt => opt.MapFrom(src => src.ArchiveState == null ? "" : src.ArchiveState.title))
               .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType == null ? "" : src.OrderType.title))

               .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType == null ? "" : src.RequestType.title))
               ;

            CreateMap<BatchRequestDTO, BatchRequest>();


            CreateMap<BatchRequestDetail, BatchRequestDetailDTO>()
               .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
               .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.InterdictOrder == null ? "" : (src.InterdictOrder.Status == null ? "" : src.InterdictOrder.Status.title)))
               .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.InterdictOrder == null ? 0 : src.InterdictOrder.StatusId))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))

                ;
            CreateMap<BatchRequestDetailDTO, BatchRequestDetail>();
            CreateMap<BatchRequestDetailReference, BatchRequestDetailReferenceDTO>().ReverseMap();




            CreateMap<InterdictOrder, PayRollOrderCartableDTO>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.RecruitOrder.Employee == null ? "" : src.RecruitOrder.Employee.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.RecruitOrder.Employee == null ? "" : src.RecruitOrder.Employee.LastName))
            .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.RecruitOrder.Employee == null ? "" : src.RecruitOrder.Employee.NationalNo))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeId))
            .ForMember(dest => dest.PayLocationId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.PayLocationId))
            .ForMember(dest => dest.PayLocation, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.PayLocation.title))
            .ForMember(dest => dest.WorkPlaceId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.WorkPlaceId))
            .ForMember(dest => dest.WorkPlace, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.WorkPlace.title))
            .ForMember(dest => dest.CostCenterId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.CostCenterId))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.RecruitOrder.CostCenter == null ? "" : src.RecruitOrder.CostCenter.title))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.ProjectId))
            .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.Project.title))
            .ForMember(dest => dest.OrganizationUnitId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationUnitId))
            .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.OrganizationUnit.title))
            .ForMember(dest => dest.EmployeeStatusId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeStatusId))
            .ForMember(dest => dest.EmployeeStatus, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.EmployeeStatus.title))
            .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.EmployeeTypeId))
            .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.RecruitOrder == null ? "" : src.RecruitOrder.EmployeeType.title))
            .ForMember(dest => dest.OrganizationJobId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganizationJob.JobId))
            .ForMember(dest => dest.OrganisationPositionId, opt => opt.MapFrom(src => src.RecruitOrder == null ? 0 : src.RecruitOrder.OrganisationPositionId))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.title))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType.title))

            ;

            //public long? OrganisationPositionId { get; set; }
            //public string? OrganisationPosition { get; set; }
            //public long StatusId { get; set; }
            //public string? Status { get; set; }

        }
    }
}
