using AutoMapper;
using DynamicExpressions.Mapping;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.Core.Interfaces;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HR.Identity.infrastructure.Data;

namespace HR.Identity.infrastructure.Services;

public class UserReportService : BaseService<UserReport, IdentityContext, UserReportDTO>, IScopedServices
{
    public UserReportService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
    }
    public OperationResult GetSelectedUserAvailableReports(long Id)
    {
        var isAdmin = _unitOfWork.Context.Set<UserRole>()
            .AsNoTracking()
            .Any(ur => ur.UserId == Id && ur.RoleId == 1);

        var results = _unitOfWork.Context.Database.SqlQuery<HR.SharedKernel.Data.KeyValuePair>($"SELECT \r\n[Id] as [key],\r\n[Id] as [id]\r\n     \r\n      ,[title] as [value]\r\n    \r\n  FROM [rpt].[Dynamic_Report] where IsActive = 1 \r\n").ToList();

        if (isAdmin)
        {
            var adminRet = results.Select(r => new HR.SharedKernel.Data.KeyValuePair
            {
                key = r.id,
                id = Id,
                value = r.value
            }).ToList();
            return OperationResult.Succeeded(payload: adminRet);
        }

        // Get DynamicReportIds from UserReport table
        var userReportIds = _unitOfWork.Context.Set<UserReport>()
            .AsNoTracking()
            .Where(i => i.UserId == Id)
            .Select(i => i.DynamicReportId)
            .ToList();

        // Get user's role IDs
        var userRoleIds = _unitOfWork.Context.Set<UserRole>()
            .AsNoTracking()
            .Where(ur => ur.UserId == Id)
            .Select(ur => ur.RoleId)
            .ToList();

        // Get DynamicReportIds from RoleReport table based on user's roles
        var roleReportIds = _unitOfWork.Context.Set<RoleReport>()
            .AsNoTracking()
            .Where(rr => userRoleIds.Contains(rr.RoleId))
            .Select(rr => rr.DynamicReportId)
            .ToList();

        // Combine both lists (union) to get unique DynamicReportIds
        var allowedReportIds = userReportIds.Union(roleReportIds).Distinct().ToList();

        // Filter results to only include allowed reports
        var resultTitleById = results.ToDictionary(x => x.id, x => x.value);
        List<HR.SharedKernel.Data.KeyValuePair> mainRet = new List<SharedKernel.Data.KeyValuePair>();
        
        foreach (var reportId in allowedReportIds)
        {
            if (resultTitleById.TryGetValue(reportId, out var title))
            {
                mainRet.Add(new HR.SharedKernel.Data.KeyValuePair
                {
                    key = reportId,
                    id = Id,
                    value = title
                });
            }
        }

        return OperationResult.Succeeded(payload: mainRet);
    }
    public bool Validate(UserReport entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
