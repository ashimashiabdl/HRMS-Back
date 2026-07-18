using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
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

namespace HR.Payroll.Infrastructure.Services
{
    public class PersonnelFicheItemService : BaseService<PersonnelFicheItem, PayrollContext, PersonnelFicheItemDTO>, IScopedServices
    {
        public PersonnelFicheItemService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(PersonnelFicheItem entity, object etc = null)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetPaymentDetailsByFicheItemId(long personnelFicheItemId)
        {
            try
            {
                var payments = _unitOfWork.Context.PersonnelPayments
                    .Include(i => i.PaymentType)
                    .Include(i => i.BankBranch)
                    .Include(i => i.Employee)
                    .Include(i => i.Fiche)
                    .ThenInclude(f => f!.PaymentPeriod)
                    .Include(i => i.Fiche)
                    .ThenInclude(f => f!.FicheStatus)
                    .AsNoTracking()
                    .Where(i => i.PersonnelFicheItemId == personnelFicheItemId && !i.IsDeleted)
                    .OrderByDescending(i => i.PaymentDate)
                    .ToList();

                var dto = _mapper.Map<List<PersonnelPaymentDTO>>(payments);
                if (dto.Count == 0)
                {
                    return OperationResult.Succeeded("پرداختی برای این قلم یافت نشد", payload: dto, rowCount: 0);
                }

                return OperationResult.Succeeded(payload: dto, rowCount: dto.Count);
            }
            catch (Exception)
            {
                return OperationResult.Failed("بازیابی جزئیات پرداخت با خطا مواجه شد");
            }
        }
    }
}
