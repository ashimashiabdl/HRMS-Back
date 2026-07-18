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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Infrastructure.Services
{
    public class OrganPropertyService : BaseService<OrganProperty, PayrollContext, OrganPropertyDTO>, IScopedServices
    {
        public OrganPropertyService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public new OperationResult Get(long id)
        {
            try
            {
                var all = All(false);
                var row = all.Where(i => i.OrganisationChartId == id);
                if (row == null)
                {
                    return OperationResult.Succeeded(payload: new OrganPropertyDTO()
                    {
                        OrganisationChartId = id,
                    });
                }
                else
                {
                    if (row.Any())
                    {
                        var record = _mapper.Map<OrganPropertyDTO>(row.Single());
                        return OperationResult.Succeeded(payload: record);
                    }
                    else
                    {
                        return OperationResult.Succeeded(payload: new OrganPropertyDTO()
                        {
                            OrganisationChartId = id,
                        });
                    }
                }
      
            }
            catch (Exception ex)
            {
                return OperationResult.Failed();
            }
        }
        public bool Validate(OrganProperty entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
