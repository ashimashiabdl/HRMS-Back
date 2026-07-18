using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class PersonnelLeaveService : BaseService<PersonnelLeave, PayrollContext, PersonnelLeaveDTO>, IScopedServices
    {
        public PersonnelLeaveService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
            : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
        }

        public OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? EmployeeId = null, long? PaymentPeriodId = null)
        {
            IQueryable<PersonnelLeave> dataSource = All(IgnoreExpired)
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .Include(x => x.OrganisationChart)
                .Include(x => x.PaymentPeriod)
                .Include(x => x.PersonnelFunctionExcelFile);
            
            // Filter by EmployeeId if provided
            if (EmployeeId.HasValue && EmployeeId.Value > 0)
            {
                dataSource = dataSource.Where(x => x.EmployeeId == EmployeeId.Value);
            }
            
            // Filter by PaymentPeriodId if provided
            if (PaymentPeriodId.HasValue && PaymentPeriodId.Value > 0)
            {
                dataSource = dataSource.Where(x => x.PaymentPeriodId == PaymentPeriodId.Value);
            }
            
            // Pass CustomDataSource with IgnoreDefaultOrganId = true to prevent base filtering issues
            return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, null, null, dataSource, false);
        }

        public OperationResult GetPagedTempData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", long? PersonnelFunctionExcelFileId = null)
        {
            var dataSource = _unitOfWork.Context.Set<TempPersonnelLeave>()
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .Include(x => x.OrganisationChart)
                .Include(x => x.PaymentPeriod)
                .Include(x => x.PersonnelFunctionExcelFile)
                .Where(x => !x.IsDeleted);
            
            // Filter by PersonnelFunctionExcelFileId if provided
            if (PersonnelFunctionExcelFileId.HasValue && PersonnelFunctionExcelFileId.Value > 0)
            {
                dataSource = dataSource.Where(x => x.PersonnelFunctionExcelFileId == PersonnelFunctionExcelFileId.Value);
            }
            
            // Map TempPersonnelLeave to PersonnelLeave for paging with proper navigation properties
            var mappedDataSource = dataSource.Select(temp => new PersonnelLeave
            {
                Id = temp.Id,
                EmployeeId = temp.EmployeeId,
                Employee = temp.Employee != null ? new Employee.Core.Entities.Employee
                {
                    Id = temp.Employee.Id,
                    FirstName = temp.Employee.FirstName,
                    LastName = temp.Employee.LastName,
                    NationalNo = temp.Employee.NationalNo,
                    PersonelCode = temp.Employee.PersonelCode
                } : null,
                OrganisationChartId = temp.OrganisationChartId,
                OrganisationChart = temp.OrganisationChart,
                PaymentPeriodId = temp.PaymentPeriodId,
                PaymentPeriod = temp.PaymentPeriod,
                LeaveTypeId = temp.LeaveTypeId,
                LeaveType = temp.LeaveType != null ? new HR.BaseInfo.Core.Entities.LeaveType
                {
                    Id = temp.LeaveType.Id,
                    title = temp.LeaveType.title
                } : null,
                PersonnelFunctionExcelFileId = temp.PersonnelFunctionExcelFileId,
                PersonnelFunctionExcelFile = temp.PersonnelFunctionExcelFile,
                Day = temp.Day,
                Hour = temp.Hour,
                Minute = temp.Minute,
                Description = temp.Description,
                title = temp.title,
                CreateDate = temp.CreateDate,
                IPAddress = temp.IPAddress,
                IsDeleted = temp.IsDeleted
            });
            
            // Pass mapped data to base method
            return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, false, null, null, mappedDataSource, false);
        }
    }
}


