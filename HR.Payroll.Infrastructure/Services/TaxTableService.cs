using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class TaxTableService : BaseService<TaxTable, PayrollContext, TaxTableDTO>, IScopedServices
    {
        public TaxTableService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public IQueryable<TaxTable> GetPagedQuery(bool ignoreExpired, long? selectedEmployeeTypeId)
        {
            var predicate = DateValidityExtension<TaxTable>.GetDateValidationPredicate(ignoreExpired)
                .And(i => i.OrganisationChartId == _currentUserDefaultOrganId);

            if (selectedEmployeeTypeId is > 0)
            {
                var employeeTypeId = selectedEmployeeTypeId.Value;
                predicate = predicate.And(i => i.Tax != null && i.Tax.EmployeeTypeId == employeeTypeId);
            }

            return _db.Set<TaxTable>()
                .Include(i => i.Tax)
                .Where(predicate);
        }
     
        public bool Validate(TaxTable entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
