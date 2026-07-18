using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.Core.Interfaces;
using System.Linq;

using System.Data.Entity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.BaseInfo.Core.DTOs;
using System;
using HR.SharedKernel.Interaces;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class TaminInsuranceJobListService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<TaminInsuranceJobList, BaseInfoContext, TaminInsuranceJobListDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new dynamic GetAsKeyValuePair()
    {
        // Return a small default set for initial load (no filter)
        return OperationResult.Succeeded(payload:
            All()
                .OrderBy(i => i.title)
                .Take(50)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair
                {
                    key = i.Id,
                    value = (i.title ?? "") + (string.IsNullOrWhiteSpace(i.Code) ? "" : " ( " + i.Code + " ) ")
                })
        ).Payload;
    }

    public dynamic GetAsKeyValuePair(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Trim().Length < 3)
        {
            return OperationResult.Succeeded(payload: new List<HR.SharedKernel.Data.KeyValuePair> { }).Payload;
        }

        var normalized = filter.Trim().ToLower();

        return OperationResult.Succeeded(payload:
            All()
                .Where(i => (i.title != null && i.title.ToLower().Contains(normalized))
                         || (!string.IsNullOrEmpty(i.Code) && i.Code.ToLower().Contains(normalized)))
                .OrderBy(i => i.title)
                .Take(100)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair
                {
                    key = i.Id,
                    value = (i.title ?? "") + (string.IsNullOrWhiteSpace(i.Code) ? "" : " ( " + i.Code + " ) ")
                })
        ).Payload;
    }
    public bool Validate(TaminInsuranceJobList entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
