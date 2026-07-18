using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class PersonnelFunctionExcelFileService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<PersonnelFunctionExcelFile, PayrollContext, PersonnelFunctionExcelFileDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
    {

        /// <summary>
        /// دانلود فایل
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperationResult DownloadFile(long id)
        {
            try
            {
                var file = _db.Set<PersonnelFunctionExcelFile>()
                    .Where(f => f.Id == id && f.OrganisationChartId == _currentUserDefaultOrganId)
                    .Select(f => new PersonnelFunctionExcelFileDTO
                    {
                        Id = f.Id,
                        FileName = f.title,
                        Content = f.Content,
                        MimeType = f.MimeType,
                        Extension = f.Extension,
                        Size = f.Size
                    })
                    .FirstOrDefault();

                if (file == null)
                {
                    return OperationResult.Failed("فایل یافت نشد");
                }

                return OperationResult.Succeeded(payload: file);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed("خطا در دانلود فایل: " + ex.Message);
            }
        }

        /// <summary>
        /// دریافت فهرست فایل‌ها بدون محتوا (برای بهینه‌سازی)
        /// </summary>
        public  OperationResult GetPagedData(int currentPage = 0, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? paymentPeriodId = null, long? employeeTypeId = null)
        {
            try
            {
                // Build query with filters
                var query = _db.Set<PersonnelFunctionExcelFile>()
                    .Include(x => x.OrganisationChart)
                    .Include(x => x.PaymentPeriod)
                    .Include(x => x.EmployeeType)
                    .Include(x => x.AspNetUsers)
                    .Where(x => x.OrganisationChartId == _currentUserDefaultOrganId && !x.IsDeleted);

                // Filter by PaymentPeriodId if provided
                if (paymentPeriodId.HasValue && paymentPeriodId.Value > 0)
                {
                    query = query.Where(x => x.PaymentPeriodId == paymentPeriodId.Value);
                }

                // Filter by EmployeeTypeId if provided
                if (employeeTypeId.HasValue && employeeTypeId.Value > 0)
                {
                    query = query.Where(x => x.EmployeeTypeId == employeeTypeId.Value);
                }

                // Exclude Content field for performance
                var projectedQuery = query.Select(x => new PersonnelFunctionExcelFile
                    {
                        Id = x.Id,
                        title = x.title,
                        OrganisationChartId = x.OrganisationChartId,
                        OrganisationChart = x.OrganisationChart,
                        Extension = x.Extension,
                        UniqueId = x.UniqueId,
                        Size = x.Size,
                        MimeType = x.MimeType,
                        CreateDate = x.CreateDate,
                        AspNetUsers = x.AspNetUsers,
                        AspNetUsersId = x.AspNetUsersId,
                        IsDeleted = x.IsDeleted,
                        PaymentPeriodId = x.PaymentPeriodId,
                        PaymentPeriod = x.PaymentPeriod,
                        EmployeeTypeId = x.EmployeeTypeId,
                        EmployeeType = x.EmployeeType,
                        // Content is intentionally excluded
                        Content = null!
                    });

                return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, CustomDataSource : projectedQuery);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed("خطا در دریافت فهرست: " + ex.Message);
            }
        }
        
    }
}

