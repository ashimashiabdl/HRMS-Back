using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using HR.SharedKernel.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hr.Employee.infrastructure.Services
{
    public class PunishmentEncourageService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<HR.Employee.Core.Entities.PunishmentEncourage, EmployeeContext, PunishmentEncourageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
    {


        public new async Task<OperationResult> UpdateForAsync(PunishmentEncourageDTO entityToUpdate)
        {
            try
            {
                var mappedTodo = _mapper.Map<HR.Employee.Core.Entities.PunishmentEncourage>(entityToUpdate);
                var IsGroup = Convert.ToBoolean(_db.Entry(mappedTodo).GetDatabaseValues().GetValue<object>("IsGroup"));
                mappedTodo.IsGroup = IsGroup;
                Update(mappedTodo);
                if (CheckDateRangeNoOverLap(mappedTodo))
                {
                    if (await _unitOfWork.Save() > 0)
                    {
                        return OperationResult.Succeeded(payload: 1);
                    }
                    else { return OperationResult.Failed(); }
                }
                else
                {
                    return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
                }
            }
            catch (Exception ex)
            {
                return OperationResult.Failed();
            }

        }

        public bool Validate(HR.Employee.Core.Entities.PunishmentEncourage entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
