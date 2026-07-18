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

public class RoleReportService : BaseService<RoleReport, IdentityContext, RoleReportDTO>, IScopedServices
{
    public RoleReportService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _mapper = mapper;
    }
    public OperationResult GetSelectedRoleAvailableReports(long Id)
    {
        var isAdmin = Id == 1;

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

        var ret = All().Where(i => i.RoleId == Id).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.DynamicReportId,
            id = i.RoleId,
            value = i.title
        });
        var resultTitleById = results.ToDictionary(x => x.id, x => x.value);
        List<HR.SharedKernel.Data.KeyValuePair> mainRet = new List<SharedKernel.Data.KeyValuePair>();
        foreach (var row in ret.ToList())
        {
            if (resultTitleById.TryGetValue(row.key, out var title))
            {
                row.value = title;
                mainRet.Add(row);
            }
        }
        return OperationResult.Succeeded(payload: mainRet);
    }
    public bool Validate(RoleReport entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}

