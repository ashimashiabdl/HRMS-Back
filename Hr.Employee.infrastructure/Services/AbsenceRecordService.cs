using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class AbsenceRecordService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.AbsenceRecord, EmployeeContext, AbsenceRecordDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(HR.Employee.Core.Entities.AbsenceRecord entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}

