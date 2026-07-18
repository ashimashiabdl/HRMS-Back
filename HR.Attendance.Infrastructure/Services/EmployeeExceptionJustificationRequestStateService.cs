using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class EmployeeExceptionJustificationRequestStateService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<EmployeeExceptionJustificationRequestState, AttendanceContext, EmployeeExceptionJustificationRequestStateDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
}
