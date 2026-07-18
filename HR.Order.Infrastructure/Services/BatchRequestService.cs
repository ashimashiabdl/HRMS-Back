using AutoMapper;
using HR.Employee.Core.DTOs;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using HR.SharedKernel;
using HR.SharedKernel.Excel;
using System.Threading.Tasks;
using static HR.SharedKernel.Share.Enums;
using System.Collections;
using HR.Organisation.Infrastructure.Services;
using Hr.Employee.infrastructure.Services;
using HR.BaseInfo.infrastructure.Services;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HR.Order.Infrastructure.Services
{
    public class BatchRequestService : BaseService<BatchRequest, OrderContext, BatchRequestDTO>, IScopedServices
    {
        private EmployeeService _employeeService;
        private WageItemService _wageItemService;
        private UserResolverService _userResolverService;
        private BaseInfo.infrastructure.Services.CoefficientService _coefficientService;
        private readonly SystemSettingContext _systemSettingContext;

        public BatchRequestService(
            IMapper mapper,
            BaseInfo.infrastructure.Services.CoefficientService CoefficientService,
            WageItemService WageItemService,
            EmployeeService EmployeeService,
            SystemSettingContext systemSettingContext,
            IUnitOfWork<OrderContext> unitOfWork,
            IDapper dapper,
            IConfiguration configuration,
            UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _employeeService = EmployeeService;
            _wageItemService = WageItemService;
            _coefficientService = CoefficientService;
            _userResolverService = userService;
            _systemSettingContext = systemSettingContext;
        }
        public OperationResult Get(long id)
        {
            try
            {
                GetBatchRequestById_Result ret = new GetBatchRequestById_Result();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[Order].[GetBatchRequestById]", con);
                    cmd.Parameters.AddWithValue("@BatchRequestId", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ret = rdr.ConvertToObject<GetBatchRequestById_Result>();
                        con.Close();
                        break;
                    }
                }
                return OperationResult.Succeeded(payload: ret);
            }
            catch (Exception ex)
            {

                throw new Exception(" خطا در Get " + ex.Message);
            }


        }
        public OperationResult GetBatchRequestArchiveStatus(long id)
        {
            try
            {
                GetBatchRequestArchiveStatus_Result ret = new GetBatchRequestArchiveStatus_Result();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[Order].[GetBatchRequestArchiveStatus]", con);
                    cmd.Parameters.AddWithValue("@BatchRequestId", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ret = rdr.ConvertToObject<GetBatchRequestArchiveStatus_Result>();
                        con.Close();
                        break;
                    }
                }
                return OperationResult.Succeeded(payload: ret);
            }
            catch (Exception ex)
            {

                throw new Exception(" خطا در Get " + ex.Message);
            }


        }
        public async Task<OperationResult> UpdateState(BatchRequestStateUpdateDTO entityToUpdate)
        {
            var mappedTodo = _unitOfWork.Context.Set<BatchRequest>().Find(entityToUpdate.Id);
            _unitOfWork.Context.Entry(mappedTodo).Property("RequestStateId").CurrentValue = entityToUpdate.NewStateId;
            Update(mappedTodo);
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            else { return OperationResult.Failed(); }
        }

        public async Task<OperationResult> UpdateArchiveState(BatchRequestStateUpdateDTO entityToUpdate)
        {
            var mappedTodo = _unitOfWork.Context.Set<BatchRequest>().Find(entityToUpdate.Id);
            _unitOfWork.Context.Entry(mappedTodo).Property("ArchiveStateId").CurrentValue = entityToUpdate.NewStateId;
            Update(mappedTodo);

            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            else { return OperationResult.Failed(); }

        }
        public OperationResult DeleteAllDrafts(long BatchRequestId)
        {
            try
            {
                var InterdictIdList = _unitOfWork.Context.Set<BatchRequestDetail>().Where(i => i.BatchRequestId == BatchRequestId && i.InterdictId > 0).Select(i => i.InterdictId).ToList();
                // فقط احکام همین درخواست گروهی که وضعیت پیش‌نویس یا ردشده دارند (پرانتز برای اولویت عملگر ضروری است).
                var draftInterdicts = _unitOfWork.Context.Set<InterdictOrder>()
                    .Where(i => InterdictIdList.Contains(i.Id) && (i.StatusId == (long)Enums.OrderStatus.Draft || i.StatusId == (long)Enums.OrderStatus.RejectedOrder))
                    .ToList();
                foreach (var item in draftInterdicts)
                {
                    item.StatusId = (long)Enums.OrderStatus.LogicalDeleted;
                    item.LastModifiedDate = DateTime.Now;
                    _unitOfWork.Context.Update(item);
                }
                _unitOfWork.Context.SaveChanges();

                return OperationResult.Succeeded();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public OperationResult GetBatchRequestFiles(long BatchRequestId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[Order].[GetBatchRequestFiles]", con);
                cmd.Parameters.AddWithValue("@BatchRequestId", BatchRequestId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                var retList = new List<GetBatchRequestFiles_Result>();
                try
                {
                    while (rdr.Read())
                    {
                        retList.Add(rdr.ConvertToObject<GetBatchRequestFiles_Result>());

                    }
                    con.Close();
                    return OperationResult.Succeeded(payload: retList);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }
        public OperationResult downloadFile(long id)
        {
            var File = _db.Set<BatchRequestFile>().Find(id);
            string temp_inBase64 = Convert.ToBase64String(File.Content);
            return OperationResult.Succeeded(payload: temp_inBase64);
        }

        public OperationResult deleteFile(long id)
        {
            var File = _db.Set<BatchRequestFile>().Find(id);
            if (File.FileTypeId == (long)Enums.BatchRequestFileType.CoefficientsExcel
                || File.FileTypeId == (long)Enums.BatchRequestFileType.WageItemsExcel
                || File.FileTypeId == (long)Enums.BatchRequestFileType.OrderDatesExcel)
            {
                return OperationResult.Failed("فایل های پیوست محاسباتی امکان حذف ندارند");
            }
            _db.Remove(File);
            return OperationResult.Succeeded(payload: _db.SaveChangesAsync());
        }


        public OperationResult getCoefExcelPreview(long id, bool isTemp, bool useMappedColumns = false)
        {
            try
            {
                var file = ResolveBatchFile(id, isTemp);
                var retList = useMappedColumns
                    ? EPPlus.ExcelMappedGridToDataTable(new MemoryStream(file.Content), GetCoefficientMappedColumns())
                    : EPPlus.ExcelDataToDataTable(new MemoryStream(file.Content));
                EnrichBatchGridRows(retList, isWage: false);
                return OperationResult.Succeeded(payload: retList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public OperationResult getWageExcelPreview(long id, bool isTemp, bool useMappedColumns = false)
        {
            var file = ResolveBatchFile(id, isTemp);
            var retList = useMappedColumns
                ? EPPlus.ExcelMappedGridToDataTable(new MemoryStream(file.Content), GetWageMappedColumns())
                : EPPlus.ExcelDataToDataTable(new MemoryStream(file.Content));
            EnrichBatchGridRows(retList, isWage: true);
            return OperationResult.Succeeded(payload: retList);
        }

        public OperationResult getDatesExcelPreview(long id, bool isTemp)
        {
            var file = ResolveBatchFile(id, isTemp);
            var retList = EPPlus.ExcelDatesToDataTable(new MemoryStream(file.Content), firstRowIsHeader: true);
            EnrichDateRows(retList);
            return OperationResult.Succeeded(payload: retList);
        }

        public OperationResult getDatesFromWageExcelPreview(long wageFileId, bool isTemp)
        {
            var file = ResolveBatchFile(wageFileId, isTemp);
            var retList = EPPlus.ExcelDatesToDataTable(new MemoryStream(file.Content), firstRowIsHeader: true);
            EnrichDateRows(retList);
            return OperationResult.Succeeded(payload: retList);
        }

        private IbaseFile ResolveBatchFile(long id, bool isTemp)
        {
            if (isTemp)
            {
                return _db.Set<OrderTempFile>().Find(id)
                    ?? throw new InvalidOperationException("فایل موقت یافت نشد");
            }

            return _db.Set<BatchRequestFile>().Find(id)
                ?? throw new InvalidOperationException("فایل پیوست یافت نشد");
        }

        private List<BatchMappedColumnDefinition> GetWageMappedColumns()
        {
            return _systemSettingContext.Set<OrganisationWageItem>()
                .Include(i => i.MappedExcelColumn)
                .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId
                    && o.MappedExcelColumnId != null
                    && o.MappedExcelColumnId > 0
                    && o.MappedExcelColumn != null)
                .Select(i => new BatchMappedColumnDefinition
                {
                    ItemId = i.WageItemId,
                    ColumnOrder = i.MappedExcelColumn!.Order,
                    ColumnLetter = i.MappedExcelColumn!.Value,
                })
                .ToList();
        }

        private List<BatchMappedColumnDefinition> GetCoefficientMappedColumns()
        {
            return _systemSettingContext.Set<OrganisationCoefficient>()
                .Include(i => i.MappedExcelColumn)
                .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId
                    && o.MappedExcelColumnId != null
                    && o.MappedExcelColumnId > 0
                    && o.MappedExcelColumn != null)
                .Select(i => new BatchMappedColumnDefinition
                {
                    ItemId = i.CoefficientId,
                    ColumnOrder = i.MappedExcelColumn!.Order,
                    ColumnLetter = i.MappedExcelColumn!.Value,
                })
                .ToList();
        }

        private void EnrichBatchGridRows(List<BatchGridModelForExcel> retList, bool isWage)
        {
            if (retList == null || !retList.Any())
            {
                return;
            }

            var nationalNoList = retList.Select(i => i.NationalNo).Distinct().ToList();
            var employeeList = _employeeService.All().Where(e => nationalNoList.Contains(e.NationalNo)).ToList();
            foreach (var item in retList)
            {
                var employee = employeeList.FirstOrDefault(e => e.NationalNo == item.NationalNo);
                if (employee != null)
                {
                    item.EmployeeId = employee.Id;
                    item.FullName = employee.FirstName + " " + employee.LastName;
                }
            }

            var itemIdList = retList.Select(w => w.ItemId).Distinct().ToList();
            if (isWage)
            {
                var wageItems = _wageItemService.All().Where(w => itemIdList.Contains(w.Id));
                foreach (var item in wageItems)
                {
                    foreach (var innerItem in retList.Where(i => i.ItemId == item.Id))
                    {
                        innerItem.ItemDesc = item.title;
                    }
                }
            }
            else
            {
                var coefItems = _coefficientService.All().Where(w => itemIdList.Contains(w.Id));
                foreach (var item in coefItems)
                {
                    foreach (var innerItem in retList.Where(i => i.ItemId == item.Id))
                    {
                        innerItem.ItemDesc = item.title;
                    }
                }
            }
        }

        private void EnrichDateRows(List<BatchDateGridModelForExcel> retList)
        {
            if (retList == null || !retList.Any())
            {
                return;
            }

            var nationalNoList = retList.Select(i => i.NationalNo).Distinct().ToList();
            var employeeList = _employeeService.All().Where(e => nationalNoList.Contains(e.NationalNo)).ToList();
            foreach (var item in retList)
            {
                var employee = employeeList.FirstOrDefault(e => e.NationalNo == item.NationalNo);
                if (employee != null)
                {
                    item.EmployeeId = employee.Id;
                    item.FullName = employee.FirstName + " " + employee.LastName;
                }
            }
        }

        private Dictionary<long, BatchDateGridModelForExcel> BuildEmployeeDatesByNationalNo(
            List<BatchDateGridModelForExcel> datesRows,
            IEnumerable<long> employeeIdList)
        {
            var employeeDatesById = new Dictionary<long, BatchDateGridModelForExcel>();
            var nationalNoList = datesRows.Select(i => i.NationalNo).Distinct().ToList();
            var employees = _employeeService.All()
                .Where(e => employeeIdList.Contains(e.Id) && nationalNoList.Contains(e.NationalNo))
                .ToList();

            foreach (var row in datesRows.Where(r => r.StartDate.HasValue))
            {
                var employee = employees.FirstOrDefault(e => e.NationalNo == row.NationalNo);
                if (employee != null)
                {
                    employeeDatesById[employee.Id] = row;
                }
            }

            return employeeDatesById;
        }

        public OperationResult DeleteAllArchives(long BatchRequestId)
        {
            try
            {
                var InterdictIdList = _unitOfWork.Context.Set<BatchRequestDetail>().Where(i => i.BatchRequestId == BatchRequestId && i.InterdictId > 0).Select(i => i.InterdictId).ToList();
                var draftInterdicts = _unitOfWork.Context.Set<InterdictOrderArchive>().Where(i => InterdictIdList.Contains(i.InterdictOrderId)).ToList();
                foreach (var item in draftInterdicts)
                {
                    _unitOfWork.Context.Remove(item);
                }
                _unitOfWork.Context.SaveChanges();
                return OperationResult.Succeeded();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// تمام احکام پیش‌نویس و ردشده این درخواست گروهی را به وضعیت «در حال بررسی» (کارتابل) ارسال می‌کند.
        /// </summary>
        public OperationResult SendAllDraftsToCartable(long BatchRequestId)
        {
            try
            {
                var InterdictIdList = _unitOfWork.Context.Set<BatchRequestDetail>().Where(i => i.BatchRequestId == BatchRequestId && i.InterdictId > 0).Select(i => i.InterdictId).ToList();
                var draftInterdicts = _unitOfWork.Context.Set<InterdictOrder>()
                    .Where(i => InterdictIdList.Contains(i.Id) && (i.StatusId == (long)Enums.OrderStatus.Draft || i.StatusId == (long)Enums.OrderStatus.RejectedOrder))
                    .ToList();
                foreach (var item in draftInterdicts)
                {
                    item.StatusId = (long)Enums.OrderStatus.Pending;
                    item.LastModifiedDate = DateTime.Now;
                    _unitOfWork.Context.Update(item);
                }
                _unitOfWork.Context.SaveChanges();
                return OperationResult.Succeeded(payload: draftInterdicts.Count);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public new OperationResult CreateForAsync(BatchRequestDTO entityToCreate)
        {
            _unitOfWork.CreateTransaction();
            try
            {
                var mappedTodo = _mapper.Map<BatchRequest>(entityToCreate);
                if (typeof(BatchRequest).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                {
                    PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                    propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                }
                mappedTodo.CreateDate = DateTime.Now;
                mappedTodo.FinishDateTime = null;
                mappedTodo.AspNetUsersId = _userResolverService.GetUserId();
                mappedTodo.LastPoolingTime = null;
                mappedTodo.EmployeeCount = entityToCreate.EmployeeIdList.Count();
                Add(mappedTodo);
                _unitOfWork.Context.Entry(mappedTodo).Property("RequestTypeId").CurrentValue = (long)BatchRequestType.BatchOrderIssue;
                _unitOfWork.Context.Entry(mappedTodo).Property("RequestStateId").CurrentValue = (long)BatchRequestState.Initial;
                if (entityToCreate.NeedBatchPrint)
                {
                    _unitOfWork.Context.Entry(mappedTodo).Property("ArchiveStateId").CurrentValue = (long)BatchRequestArchiveState.InitialArchive;
                }
                else
                {
                    _unitOfWork.Context.Entry(mappedTodo).Property("ArchiveStateId").CurrentValue = (long)BatchRequestArchiveState.IgnoreArchive;
                }

                _unitOfWork.Context.SaveChanges();
                OrderTempFile? wageTempFile = null;
                if (entityToCreate.WageFileTempId > 0)
                {
                    wageTempFile = _unitOfWork.Context.Set<OrderTempFile>().Find(entityToCreate.WageFileTempId);
                }

                if (entityToCreate.WageFileTempId > 0)
                {
                    var tempFile = wageTempFile!;
                    var wageExcelFile = new BatchRequestFile()
                    {
                        BatchRequestId = mappedTodo.Id,
                        IPAddress = "",
                        MimeType = tempFile.MimeType,
                        Content = tempFile.Content,
                        Size = tempFile.Size,
                        IsDeleted = false,
                        Description = "آپلود شده زمان ثبت درخواست صدور گروهی",
                        UniqueId = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        title = tempFile.title
                    };
                    _unitOfWork.Context.Set<BatchRequestFile>().Add(wageExcelFile);
                    _unitOfWork.Context.Entry(wageExcelFile).Property("FileTypeId").CurrentValue = (long)Enums.BatchRequestFileType.WageItemsExcel;
                    _unitOfWork.Context.SaveChanges();
                }

                if (entityToCreate.CoefficientFileTempId > 0)
                {
                    var tempFile = _unitOfWork.Context.Set<OrderTempFile>().Find(entityToCreate.CoefficientFileTempId);
                    var coefficientExcelFile = new BatchRequestFile()
                    {
                        BatchRequestId = mappedTodo.Id,
                        IPAddress = "",
                        MimeType = tempFile.MimeType,
                        Content = tempFile.Content,
                        Size = tempFile.Size,
                        IsDeleted = false,
                        Description = "آپلود شده زمان ثبت درخواست صدور گروهی",
                        UniqueId = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        title = tempFile.title
                    };
                    _unitOfWork.Context.Set<BatchRequestFile>().Add(coefficientExcelFile);
                    _unitOfWork.Context.Entry(coefficientExcelFile).Property("FileTypeId").CurrentValue = (long)Enums.BatchRequestFileType.CoefficientsExcel;
                    _unitOfWork.Context.SaveChanges();
                }

                Dictionary<long, BatchDateGridModelForExcel> employeeDatesById = new Dictionary<long, BatchDateGridModelForExcel>();

                if (entityToCreate.DatesFromExcel && entityToCreate.DatesFileTempId > 0)
                {
                    var tempFile = _unitOfWork.Context.Set<OrderTempFile>().Find(entityToCreate.DatesFileTempId);
                    var datesExcelFile = new BatchRequestFile()
                    {
                        BatchRequestId = mappedTodo.Id,
                        IPAddress = "",
                        MimeType = tempFile.MimeType,
                        Content = tempFile.Content,
                        Size = tempFile.Size,
                        IsDeleted = false,
                        Description = "آپلود شده زمان ثبت درخواست صدور گروهی",
                        UniqueId = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        title = tempFile.title
                    };
                    _unitOfWork.Context.Set<BatchRequestFile>().Add(datesExcelFile);
                    _unitOfWork.Context.Entry(datesExcelFile).Property("FileTypeId").CurrentValue = (long)Enums.BatchRequestFileType.OrderDatesExcel;
                    _unitOfWork.Context.SaveChanges();

                    var datesRows = EPPlus.ExcelDatesToDataTable(new MemoryStream(tempFile.Content), firstRowIsHeader: true);
                    employeeDatesById = BuildEmployeeDatesByNationalNo(datesRows, entityToCreate.EmployeeIdList);
                }
                else if (entityToCreate.DatesFromWageExcel && wageTempFile != null)
                {
                    var datesRows = EPPlus.ExcelDatesToDataTable(new MemoryStream(wageTempFile.Content), firstRowIsHeader: true);
                    employeeDatesById = BuildEmployeeDatesByNationalNo(datesRows, entityToCreate.EmployeeIdList);
                }

                foreach (var Employee in entityToCreate.EmployeeIdList)
                {
                    BatchRequestDetail toAdd = new BatchRequestDetail()
                    {
                        BatchRequestId = mappedTodo.Id,
                        EmployeeId = Employee,
                        CreateDate = DateTime.Now,
                        IPAddress = "",
                        DoDatetime = null,
                        LastTryDateTime = null,
                    };
                    if (employeeDatesById.TryGetValue(Employee, out var employeeDates))
                    {
                        toAdd.StartDate = employeeDates.StartDate;
                        toAdd.EndDate = employeeDates.EndDate;
                    }
                    _unitOfWork.Context.Set<BatchRequestDetail>().Add(toAdd);
                }
                if (_unitOfWork.Save().Result > 0)
                {
                    _unitOfWork.Commit();
                    return OperationResult.Succeeded(payload: mappedTodo.Id);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            return OperationResult.Failed();
        }


        public OperationResult GetCurrentRequestExcelReport(long id)
        {
            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("گزارش_شماره_درخواست_گروهی_" + id);
            using (ExcelRange Rng = wsSheet1.Cells[2, 2, 2, 2])
            {
                Rng.Value = "Welcome to Everyday be coding - tutorials for beginners";
                Rng.Style.Font.Size = 16;
                Rng.Style.Font.Bold = true;
                Rng.Style.Font.Italic = true;
            }
            wsSheet1.Protection.IsProtected = false;
            wsSheet1.Protection.AllowSelectLockedCells = false;
            ExcelPkg.SaveAs(new FileInfo(@"D:\New.xlsx"));
            return OperationResult.Succeeded();
        }



        public bool Validate(BatchRequest entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
