using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class AttendanceLocationService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<AttendanceLocation, AttendanceContext, AttendanceLocationDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    public OperationResult GetAsKeyValuePair(long currentUserDefaultOrganId)
    {
        return OperationResult.Succeeded(payload: All()
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId)
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = (string.IsNullOrWhiteSpace(i.Code) ? i.title : i.Code + " - " + i.title)
            }));
    }
}
