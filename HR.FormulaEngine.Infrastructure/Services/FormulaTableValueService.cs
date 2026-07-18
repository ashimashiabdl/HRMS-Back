using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.FormulaEngine.Infrastructure.Services;

public class FormulaTableValueService(IMapper mapper, IUnitOfWork<FormulaEngineContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<FormulaTableValue, FormulaEngineContext, FormulaTableValueDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
}
