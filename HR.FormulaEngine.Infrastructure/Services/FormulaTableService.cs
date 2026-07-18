using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.FormulaEngine.Infrastructure.Services
{
    public class FormulaTableService : BaseService<FormulaTable, FormulaEngineContext, FormulaTableDTO>, IScopedServices
    {
        public FormulaTableService(IMapper mapper, IUnitOfWork<FormulaEngineContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.TableType).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.title + " ( " + (i.TableType == null ? "" : i.TableType.title) + " ) "
            }));
        }
        public bool Validate(FormulaTable entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
