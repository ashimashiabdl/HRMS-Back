using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;

namespace HR.Identity.infrastructure.Mapper;

public class IdentityProfile : Profile
{
    public IdentityProfile()
    {
        CreateMap<AspNetUsers, LoginDTO>().ReverseMap();


        CreateMap<PermissionRoute, PermissionRouteDTO>().ReverseMap();
      



        CreateMap<AspNetUsers, AspNetUsersDTO>().ReverseMap();
        
        CreateMap<UserLoginHistory, UserLoginHistoryDTO>()
         .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AspNetUser == null ? "" : src.AspNetUser.UserName))
         .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AspNetUser == null ? "" : src.AspNetUser.FirstName))
         .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AspNetUser == null ? "" : src.AspNetUser.LastName))
         .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.AspNetUser == null ? "" : src.AspNetUser.NationalNo))
         .ForMember(dest => dest.FailReason, opt => opt.MapFrom(src => src.FailReason))
         ;

        CreateMap<AspNetRolesDTO, AspNetRoles>();

        CreateMap<AspNetRoles, AspNetRolesDTO>()
         //    .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             ;

        CreateMap<UserReportDTO, UserReport>();

        CreateMap<UserReport, UserReportDTO>()
             .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.FirstName + " " + src.User.LastName + " ( " + src.User.UserName + " ) "))
             ;

        CreateMap<RoleReportDTO, RoleReport>();

        CreateMap<RoleReport, RoleReportDTO>()
             .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == null ? "" : src.Role.PersianName))
             ;

        CreateMap<Message, MessageDTO>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender == null ? "" : src.Sender.UserName))
            .ForMember(dest => dest.SenderFullName, opt => opt.MapFrom(src => src.Sender == null ? "" : (src.Sender.FirstName ?? "") + " " + (src.Sender.LastName ?? "")))
            .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver == null ? "" : src.Receiver.UserName))
            .ForMember(dest => dest.ReceiverFullName, opt => opt.MapFrom(src => src.Receiver == null ? "" : (src.Receiver.FirstName ?? "") + " " + (src.Receiver.LastName ?? "")))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Subject));

        CreateMap<MessageDTO, Message>()
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Subject));

        CreateMap<MessageAttachment, MessageAttachmentDTO>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.FileName));

        CreateMap<MessageAttachmentDTO, MessageAttachment>()
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.Content, opt => opt.Ignore()); // Content should be set manually from base64

    

        //CreateMap<SystemTreeDTO, SystemTree>();




        //CreateMap<SystemTree, SystemTreeDTO>()
        //.ForMember(dest => dest.NodeType, opt => opt.MapFrom(src => src.NodeType == null ? "" : src.NodeType.title))
        //.ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Parent == null ? "" : src.Parent.title))
        //;



        CreateMap<UserWorkPlaceDTO, UserWorkPlace>();

        CreateMap<UserWorkPlace, UserWorkPlaceDTO>()
        .ForMember(dest => dest.WorkPlace, opt => opt.MapFrom(src => src.WorkPlace == null ? "" : src.WorkPlace.title))
        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))
        ;



 

        CreateMap<UserCostCenterDTO, UserCostCenter>();

        CreateMap<UserCostCenter, UserCostCenterDTO>()
        .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? "" : src.CostCenter.title))
        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))

        ;

        CreateMap<UserDefaultSettingDTO, UserDefaultSetting>();

        CreateMap<UserDefaultSetting, UserDefaultSettingDTO>()

        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))

        ;


        CreateMap<UserOrganizationUnitDTO, UserOrganizationUnit>();

        CreateMap<UserOrganizationUnit, UserOrganizationUnitDTO>()

        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))
        .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.OrganizationUnit == null ? "" : src.OrganizationUnit.title))

        ;



        CreateMap<UserPayLocationDTO, UserPayLocation>();

        CreateMap<UserPayLocation, UserPayLocationDTO>()

        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))
        .ForMember(dest => dest.PayLocation, opt => opt.MapFrom(src => src.PayLocation == null ? "" : src.PayLocation.title))

        ;


        CreateMap<UserRoleDTO, UserRole>();

        CreateMap<UserRole, UserRoleDTO>()

        // .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.UserName))

        // .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == null ? "" : src.Role.Name))

        ;

        CreateMap<RoleReportableEntityDTO, RoleReportableEntity>();

        CreateMap<RoleReportableEntity, RoleReportableEntityDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == null ? "" : src.Role.Name))
            .ForMember(dest => dest.ReportableEntity, opt => opt.Ignore()); // Soft FK - will be handled in service

        CreateMap<UserReportableEntityDTO, UserReportableEntity>();

        CreateMap<UserReportableEntity, UserReportableEntityDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User == null ? "" : src.User.FirstName + " " + src.User.LastName + " ( " + src.User.UserName + " ) "))
            .ForMember(dest => dest.ReportableEntity, opt => opt.Ignore()); // Soft FK - will be handled in service

        CreateMap<CommonPasswordDTO, CommonPassword>()
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Password));

        CreateMap<CommonPassword, CommonPasswordDTO>()
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Password));

        CreateMap<BlockedIpDTO, BlockedIp>()
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.IpAddress));

        CreateMap<BlockedIp, BlockedIpDTO>()
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.IpAddress));

    }

}
