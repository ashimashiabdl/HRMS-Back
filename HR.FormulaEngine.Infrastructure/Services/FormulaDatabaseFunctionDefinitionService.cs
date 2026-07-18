using AutoMapper;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.FormulaEngine.Infrastructure.Services
{
    public class FormulaDatabaseFunctionDefinitionService : BaseService<FormulaDatabaseFunctionDefinition, FormulaEngineContext, FormulaDatabaseFunctionDefinitionDTO>, IScopedServices
    {
        public FormulaDatabaseFunctionDefinitionService(IMapper mapper, IUnitOfWork<FormulaEngineContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        /// <summary>
        /// Visible records: current organisation, or system-public functions from any organisation.
        /// </summary>
        private IQueryable<FormulaDatabaseFunctionDefinition> AccessibleQuery(bool ignoreExpired = true)
        {
            return All(ignoreExpired, ignoreDefaultOrganId: true)
                .Where(i => i.IsPublic || i.OrganisationChartId == _currentUserDefaultOrganId);
        }

        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: AccessibleQuery().OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.title + " ( " + (i.EnglishName == null ? "" : i.EnglishName) + " ) "
            }));
        }

        public new OperationResult Get(long id)
        {
            var all = AccessibleQuery(ignoreExpired: false);
            var entityType = _unitOfWork.Context.Model.FindEntityType(typeof(FormulaDatabaseFunctionDefinition));
            if (entityType != null)
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    all = all.Include(navigation.Name);
                }
            }

            var row = all.SingleOrDefault(i => i.Id == id);
            var record = _mapper.Map<FormulaDatabaseFunctionDefinitionDTO>(row);
            if (record == null)
            {
                return OperationResult.NotFound();
            }

            return OperationResult.Succeeded(payload: record);
        }

        public bool Validate(FormulaDatabaseFunctionDefinition entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
