using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Report.Infrastructure.Services;

public class FieldDataTypeService(IMapper mapper, IUnitOfWork<ReportContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<FieldDataType, ReportContext, FieldDataTypeDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
}

