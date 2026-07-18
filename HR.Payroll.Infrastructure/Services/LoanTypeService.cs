
using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class LoanTypeService : BaseService<LoanType, PayrollContext, LoanTypeDTO>, IScopedServices
    {
        public LoanTypeService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public new async Task<OperationResult> UpdateForAsync(LoanTypeDTO entityToUpdate)
        {
            var mappedTodo = _mapper.Map<LoanType>(entityToUpdate);
            if (string.IsNullOrEmpty(mappedTodo.title))
            {
                mappedTodo.title = "";
            }

            if (!CheckDateRangeNoOverLap(mappedTodo))
            {
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }

            Update(mappedTodo);

            if (entityToUpdate.IsActive != true && entityToUpdate.Id.HasValue)
            {
                var personnelLoans = _unitOfWork.Context.PersonnelLoans
                    .Where(pl => pl.LoanTypeId == entityToUpdate.Id.Value && pl.IsActive)
                    .ToList();
                foreach (var personnelLoan in personnelLoans)
                {
                    personnelLoan.IsActive = false;
                }
            }

            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }

            return OperationResult.Failed();
        }

        public bool Validate(LoanType entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
