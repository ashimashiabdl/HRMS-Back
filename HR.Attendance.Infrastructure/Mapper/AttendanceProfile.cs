using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Core.Entities.EmployeeSpecific;
using EmployeeShiftAssignment = HR.Attendance.Core.Entities.EmployeeSpecific.EmployeeShiftAssignment;
using HR.Attendance.Infrastructure.Services;

namespace HR.Attendance.Infrastructure.Mapper;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<AttendanceLocation, AttendanceLocationDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.RelatedOrganisationChart, opt => opt.MapFrom(src => src.RelatedOrganisationChart == null ? string.Empty : src.RelatedOrganisationChart.title))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.RelatedOrganisationChart, opt => opt.Ignore());

        CreateMap<AttendanceDevice, AttendanceDeviceDTO>()
            .ForMember(dest => dest.AttendanceLocation, opt => opt.MapFrom(src => src.AttendanceLocation == null ? string.Empty : src.AttendanceLocation.title))
            .ForMember(dest => dest.Brand, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.AttendanceLocation, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore());

        CreateMap<EmployeeAttendanceLog, AttendanceLogDTO>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? string.Empty : (src.Employee.FirstName + " " + src.Employee.LastName + " (" + src.Employee.PersonelCode + ")")))
            .ForMember(dest => dest.AttendanceDevice, opt => opt.MapFrom(src => src.AttendanceDevice == null ? string.Empty : src.AttendanceDevice.title))
            .ReverseMap()
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.AttendanceDevice, opt => opt.Ignore());

        CreateMap<AttendanceCalendar, AttendanceCalendarDTO>()
            .ForMember(dest => dest.Holiday, opt => opt.MapFrom(src => src.Holiday == null ? string.Empty : src.Holiday.title))
            .ForMember(dest => dest.WeekDayTitle, opt => opt.MapFrom(src => AttendanceCalendarService.ToPersianWeekDay(src.WeekDay)))
            .ReverseMap()
            .ForMember(dest => dest.Holiday, opt => opt.Ignore());

        CreateMap<ShiftDetail, ShiftDetailDTO>()
            .ForMember(dest => dest.WeekDayTitle, opt => opt.MapFrom(src => AttendanceCalendarService.ToPersianWeekDay(src.WeekDay)))
            .ForMember(dest => dest.RoundTypeTitle, opt => opt.MapFrom(src => ShiftService.ToRoundTypeTitle(src.RoundType)))
            .ReverseMap()
            .ForMember(dest => dest.Shift, opt => opt.Ignore());

        CreateMap<ShiftOverrideDetail, ShiftOverrideDetailDTO>()
            .ForMember(dest => dest.WeekDayTitle, opt => opt.MapFrom(src => AttendanceCalendarService.ToPersianWeekDay(src.WeekDay)))
            .ForMember(dest => dest.RoundTypeTitle, opt => opt.MapFrom(src => ShiftService.ToRoundTypeTitle(src.RoundType)))
            .ReverseMap()
            .ForMember(dest => dest.ShiftOverride, opt => opt.Ignore());

        CreateMap<Shift, ShiftDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details.Where(d => !d.IsDeleted)))
            .ForMember(dest => dest.IsFlexible, opt => opt.Ignore())
            .ForMember(dest => dest.StartTime, opt => opt.Ignore())
            .ForMember(dest => dest.EndTime, opt => opt.Ignore())
            .ForMember(dest => dest.RequiredWorkSeconds, opt => opt.Ignore())
            .ForMember(dest => dest.NightShift, opt => opt.Ignore())
            .ForMember(dest => dest.CrossDay, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore());

        CreateMap<ShiftOverride, ShiftOverrideDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift == null ? string.Empty : (src.Shift.Code + " - " + src.Shift.title)))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details.Where(d => !d.IsDeleted)))
            .ForMember(dest => dest.IsFlexible, opt => opt.Ignore())
            .ForMember(dest => dest.StartTime, opt => opt.Ignore())
            .ForMember(dest => dest.EndTime, opt => opt.Ignore())
            .ForMember(dest => dest.RequiredWorkSeconds, opt => opt.Ignore())
            .ForMember(dest => dest.NightShift, opt => opt.Ignore())
            .ForMember(dest => dest.CrossDay, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Shift, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore());

        CreateMap<EmployeeShiftAssignment, EmployeeShiftAssignmentDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? string.Empty : (src.Employee.FirstName + " " + src.Employee.LastName + " (" + src.Employee.PersonelCode + ")")))
            .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift == null ? string.Empty : (src.Shift.Code + " - " + src.Shift.title)))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Shift, opt => opt.Ignore());

        CreateMap<AbsenceType, AbsenceTypeDTO>().ReverseMap();

        CreateMap<EmployeeExceptionJustificationRequestState, EmployeeExceptionJustificationRequestStateDTO>().ReverseMap();

        CreateMap<EmployeeAttendanceDailyResult, EmployeeAttendanceDailyResultDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? string.Empty : (src.Employee.FirstName + " " + src.Employee.LastName + " (" + src.Employee.PersonelCode + ")")))
            .ForMember(dest => dest.AttendanceCalendar, opt => opt.MapFrom(src => src.AttendanceCalendar == null
                ? string.Empty
                : AttendanceCalendarService.FormatPersianDateLabel(src.AttendanceCalendar.Date, src.AttendanceCalendar.WeekDay)))
            .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift == null ? string.Empty : (src.Shift.Code + " - " + src.Shift.title)))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.AttendanceCalendar, opt => opt.Ignore())
            .ForMember(dest => dest.Shift, opt => opt.Ignore());

        CreateMap<EmployeeAttendanceException, EmployeeAttendanceExceptionDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? string.Empty : (src.Employee.FirstName + " " + src.Employee.LastName + " (" + src.Employee.PersonelCode + ")")))
            .ForMember(dest => dest.AttendanceCalendar, opt => opt.MapFrom(src => src.AttendanceCalendar == null
                ? string.Empty
                : AttendanceCalendarService.FormatPersianDateLabel(src.AttendanceCalendar.Date, src.AttendanceCalendar.WeekDay)))
            .ForMember(dest => dest.AbsenceType, opt => opt.MapFrom(src => src.AbsenceType == null ? string.Empty : src.AbsenceType.title))
            .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift == null ? string.Empty : (src.Shift.Code + " - " + src.Shift.title)))
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.AttendanceCalendar, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore())
            .ForMember(dest => dest.Shift, opt => opt.Ignore());

        CreateMap<EmployeeMonthlySummary, EmployeeMonthlySummaryDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? string.Empty : src.OrganisationChart.title))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? string.Empty : (src.Employee.FirstName + " " + src.Employee.LastName + " (" + src.Employee.PersonelCode + ")")))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter == null ? string.Empty : src.CostCenter.title))
            .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(src => src.OrganizationUnit == null ? string.Empty : src.OrganizationUnit.title))
            .ForMember(dest => dest.WorkPlace, opt => opt.MapFrom(src => src.WorkPlace == null ? string.Empty : src.WorkPlace.title))
            .ForMember(dest => dest.MonthTitle, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.CostCenter, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizationUnit, opt => opt.Ignore())
            .ForMember(dest => dest.WorkPlace, opt => opt.Ignore());

        CreateMap<EmployeeExceptionJustificationRequest, EmployeeExceptionJustificationRequestDTO>()
            .ForMember(dest => dest.EmployeeAttendanceException, opt => opt.MapFrom(src => src.EmployeeAttendanceException == null ? string.Empty : src.EmployeeAttendanceException.title))
            .ForMember(dest => dest.AbsenceType, opt => opt.MapFrom(src => src.AbsenceType == null ? string.Empty : src.AbsenceType.title))
            .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType == null ? string.Empty : src.LeaveType.title))
            .ForMember(dest => dest.EmployeeExceptionJustificationRequestState, opt => opt.MapFrom(src => src.EmployeeExceptionJustificationRequestState == null ? string.Empty : src.EmployeeExceptionJustificationRequestState.title))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeAttendanceException == null ? (long?)null : src.EmployeeAttendanceException.EmployeeId))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.EmployeeAttendanceException == null || src.EmployeeAttendanceException.Employee == null
                ? string.Empty
                : (src.EmployeeAttendanceException.Employee.FirstName + " " + src.EmployeeAttendanceException.Employee.LastName + " (" + src.EmployeeAttendanceException.Employee.PersonelCode + ")")))
            .ReverseMap()
            .ForMember(dest => dest.EmployeeAttendanceException, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenceType, opt => opt.Ignore())
            .ForMember(dest => dest.LeaveType, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeExceptionJustificationRequestState, opt => opt.Ignore());
    }
}
