using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using HR.WorkFlow.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.WorkFlow.Infrastructure.Mapper
{
    public class WorkFlowProfile : Profile
    {
        public WorkFlowProfile()
        {
            CreateMap<Core.Data.Action, ActionDTO>();
            //.ForMember(dest => dest.OrderTypeGroupTitle, opt => opt.MapFrom(src => src.OrderTypeGroup == null ? "" : src.OrderTypeGroup.title))
            CreateMap<ActionDTO, Core.Data.Action>();


            CreateMap<Core.Data.ActivityTemplate, ActivityTemplateDTO>()
                .ForMember(dest => dest.WorkFlowInstance, opt => opt.MapFrom(src =>
                    src.WorkFlowInstance == null
                        ? src.WorkFlowInstanceId.ToString()
                        : src.WorkFlowInstance.InterdictOrder != null
                            ? (string.IsNullOrWhiteSpace(src.WorkFlowInstance.InterdictOrder.title)
                                ? src.WorkFlowInstance.InterdictOrderId.ToString()
                                : src.WorkFlowInstance.InterdictOrder.title)
                            : src.WorkFlowInstance.EmployeeSettlementId.HasValue
                                ? $"تسویه {src.WorkFlowInstance.EmployeeSettlementId}"
                                : $"نمونه {src.WorkFlowInstanceId}"))
                .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src =>
                    src.WorkFlowInstance == null || src.WorkFlowInstance.WorkFlow == null
                        ? ""
                        : src.WorkFlowInstance.WorkFlow.title))
                .ForMember(dest => dest.FromNode, opt => opt.MapFrom(src =>
                    src.FromNodeId == null
                        ? "شروع فرآیند"
                        : (src.FromNode == null ? "" : src.FromNode.title)))
                .ForMember(dest => dest.ToNode, opt => opt.MapFrom(src =>
                    src.ToNodeId == null
                        ? "پایان فرآیند"
                        : (src.ToNode == null ? "" : src.ToNode.title)))
                .ForMember(dest => dest.ActionDesc, opt => opt.MapFrom(src =>
                    src.Action == null ? "" : src.Action.title))
                .ForMember(dest => dest.UserSignature, opt => opt.MapFrom(src =>
                    src.UserSignature == null
                        ? ""
                        : src.UserSignature.AspNetUsers == null
                            ? (src.UserSignature.SignTitle ?? "")
                            : src.UserSignature.AspNetUsers.FirstName + " " + src.UserSignature.AspNetUsers.LastName))
            ;


            CreateMap<ActivityTemplateDTO, Core.Data.ActivityTemplate>();




            CreateMap<Core.Data.Definition, DefinitionDTO>()
            .ForMember(dest => dest.FromNode, opt => opt.MapFrom(src => src.FromNodeId == null ? "شروع فرآیند" : (src.FromNode == null ? "" : src.FromNode.title)))
            .ForMember(dest => dest.ToNode, opt => opt.MapFrom(src => src.ToNodeId == null ? "پایان فرآیند (گره نهایی)" : (src.ToNode == null ? "" : src.ToNode.title)))
            .ForMember(dest => dest.ActionDesc, opt => opt.MapFrom(src => src.Action == null ? "" : src.Action.title))
            .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title))
            ;


            CreateMap<DefinitionDTO, Core.Data.Definition>();


            CreateMap<Core.Data.UserSignature, UserSignatureDTO>()
            .ForMember(dest => dest.AspNetUsers, opt => opt.MapFrom(src => src.AspNetUsers == null ? "" : src.AspNetUsers.FirstName + " " + src.AspNetUsers.LastName))
            //.ForMember(dest => dest.ToNode, opt => opt.MapFrom(src => src.ToNode == null ? "" : src.ToNode.title))
            //.ForMember(dest => dest.ActionDesc, opt => opt.MapFrom(src => src.Action == null ? "" : src.Action.title))
            //.ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title))
            ;


            CreateMap<UserSignatureDTO, Core.Data.UserSignature>();



            CreateMap<Core.Data.NodeUserRel, NodeUserRelDTO>()
         .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " ( " + src.Employee.UserName + " ) "))
         .ForMember(dest => dest.Node, opt => opt.MapFrom(src => src.Node == null ? "" : src.Node.title))
         .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title))
         ;


            CreateMap<NodeUserRelDTO, Core.Data.NodeUserRel>();


            CreateMap<Core.Data.NodeRoleRel, NodeRoleRelDTO>()
         .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == null ? "" : (string.IsNullOrEmpty(src.Role.PersianName) ? src.Role.Name : src.Role.PersianName)))
         .ForMember(dest => dest.Node, opt => opt.MapFrom(src => src.Node == null ? "" : src.Node.title))
         .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title))
         ;

            CreateMap<NodeRoleRelDTO, Core.Data.NodeRoleRel>();


            CreateMap<Core.Data.WorkFlow, WorkFlowDTO>()
         .ForMember(dest => dest.WorkFlowType, opt => opt.MapFrom(src => src.WorkFlowType == null ? "" : src.WorkFlowType.title))

         ;


            CreateMap<WorkFlowDTO, Core.Data.WorkFlow>();



            CreateMap<Core.Data.WorkFlowInstance, WorkFlowInstanceDTO>()
                .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title))
                .ForMember(dest => dest.InterdictOrder, opt => opt.MapFrom(src =>
                    src.InterdictOrder == null
                        ? (src.InterdictOrderId.HasValue ? src.InterdictOrderId.Value.ToString() : "")
                        : (string.IsNullOrWhiteSpace(src.InterdictOrder.title)
                            ? src.InterdictOrder.Id.ToString()
                            : src.InterdictOrder.title)))
                .ForMember(dest => dest.EmployeeSettlement, opt => opt.MapFrom(src =>
                    src.EmployeeSettlementId.HasValue ? src.EmployeeSettlementId.Value.ToString() : ""));
            CreateMap<WorkFlowInstanceDTO, Core.Data.WorkFlowInstance>();



            CreateMap<Core.Data.WorkFlowType, WorkFlowTypeDTO>()

         ;


            CreateMap<WorkFlowTypeDTO, Core.Data.WorkFlowType>();



            CreateMap<Core.Data.Node, NodeDTO>()
                .ForMember(dest => dest.WorkFlow, opt => opt.MapFrom(src => src.WorkFlow == null ? "" : src.WorkFlow.title));
            CreateMap<NodeDTO, Core.Data.Node>();

        }
    }
}
