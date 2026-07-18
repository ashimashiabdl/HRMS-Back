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
    public class BankDisketteTemplateRowService : BaseService<BankDisketteTemplateRow, PayrollContext, BankDisketteTemplateRowDTO>, IScopedServices
    {
        public BankDisketteTemplateRowService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(BankDisketteTemplateRow entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
