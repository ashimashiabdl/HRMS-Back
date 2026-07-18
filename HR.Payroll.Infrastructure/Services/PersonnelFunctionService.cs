using AutoMapper;
using Dapper;
using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Core.DTOs;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace HR.Payroll.Infrastructure.Services;

public class PersonnelFunctionService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, EmployeeService employeeService) : BaseService<PersonnelFunction, PayrollContext, PersonnelFunctionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly EmployeeService _employeeService = employeeService;
    private readonly UserResolverService _userResolverService = userService;


    /// <summary>
    /// فهرست کارکردهای در انتظار تایید حقوق دستمزد
    /// </summary>
    /// <returns></returns>
    public OperationResult getCurrentOrganPayRollPendigFunctions(PayRollOrderPagerDTO req)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (var command = con.CreateCommand())
                {

                    if (req.CostCenterId == null)
                    {
                        req.CostCenterId = 0;
                    }
                    if (req.WorkPlaceId == null)
                    {
                        req.WorkPlaceId = 0;
                    }
                    if (req.OrganizationUnitId == null)
                    {
                        req.OrganizationUnitId = 0;
                    }
                    if (string.IsNullOrEmpty(req.filter))
                    {
                        req.filter = null;
                    }
                    if (string.IsNullOrEmpty(req.activeSortColumn))
                    {
                        req.activeSortColumn = null;
                    }

                    if (string.IsNullOrEmpty(req.Sortdirection))
                    {
                        req.Sortdirection = "ASC";
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[payroll].[GetCurrentOrganPayrollPendigFunctions]";
                    command.Parameters.AddWithValue("@currentUserId", req.currentUserId);
                    command.Parameters.AddWithValue("@currentUserDefaultOrganId", _currentUserDefaultOrganId);
                    command.Parameters.AddWithValue("@PageNo", req.currentPage + 1);
                    command.Parameters.AddWithValue("@PageSize", req.pageSize);
                    command.Parameters.AddWithValue("@year", req.year);
                    command.Parameters.AddWithValue("@month", req.month);
                    command.Parameters.AddWithValue("@filter", req.filter);
                    command.Parameters.AddWithValue("@CostCenterId", req.CostCenterId);
                    command.Parameters.AddWithValue("@OrganizationUnitId", req.OrganizationUnitId);
                    command.Parameters.AddWithValue("@WorkPlaceId", req.WorkPlaceId);
                    command.Parameters.AddWithValue("@activeSortColumn", req.activeSortColumn);
                    command.Parameters.AddWithValue("@Sortdirection", req.Sortdirection);


                    con.Open();
                    SqlDataReader rdr = command.ExecuteReader();
                    var functionList = new List<GetCurrentOrganPayrollPendigFunctions_Result>();
                    int rowCount = 0;
                    while (rdr.Read())
                    {
                        var row = rdr.ConvertToObject<GetCurrentOrganPayrollPendigFunctions_Result>();
                        functionList.Add(row);
                        if (row.Totalcount.HasValue && rowCount == 0)
                        {
                            rowCount = row.Totalcount.Value;
                        }
                    }
                    con.Close();

                    return OperationResult.Succeeded(payload: functionList, rowCount: rowCount);
                }
            }


        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطایی رخ داده");
        }
    }

    /// <summary>
    /// فهرست کارکردهای در انتظار تایید حقوق دستمزد
    /// </summary>
    /// <returns></returns>
    public OperationResult getCurrentOrganPayRollPendigFunctionsOld(PayRollOrderPagerDTO req)
    {
        try
        {
            var all = All(IgnoreExpired: false)
                .Include(i => i.Employee)

                .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId && i.IsConfirmed != true && (req.year == i.Year || req.year == 0) && (req.month == i.Month || req.month == 0))
                .OrderBy(i => i.Employee.LastName).ThenBy(i => i.Employee.FirstName).AsQueryable();
            ;

            int rowCount = 0;

            int currentPage = req.currentPage >= 0 ? req.currentPage : 0;
            var startIndex = req.pageSize * (req.currentPage);

            if (string.IsNullOrEmpty(req.activeSortColumn))
            {
                req.activeSortColumn = "CreateDate";
                req.Sortdirection = "desc";
            }
            if (req.activeSortColumn != "")
            {
                var desc = req.Sortdirection == "asc" ? false : true;
                string command = desc ? "OrderByDescending" : "OrderBy";
                var type = typeof(PersonnelFunction);
                var property = type.GetProperty(req.activeSortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                              all.Expression, Expression.Quote(orderByExpression));
                all.Provider.CreateQuery<PersonnelFunction>(resultExpression);

            }

            if (!string.IsNullOrEmpty(req.filter))
            {
                var type = typeof(PersonnelFunctionDTO);
                var predicate = PredicateBuilder.New<PersonnelFunction>();
                foreach (var property in type.GetProperties())
                {
                    switch (property.Name.ToLower().Trim())
                    {
                        case "firstname":
                            predicate = predicate.Or(s => HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name).Contains(req.filter));
                            break;
                        case "lastname":
                            predicate = predicate.Or(s => HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name).Contains(req.filter));
                            break;
                        case "nationalno":
                            predicate = predicate.Or(s => HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name).Contains(req.filter));
                            break;
                        case "PersonelCode":
                            predicate = predicate.Or(s => HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.Employee, property.Name).Contains(req.filter));
                            break;
                    }
                }
                var AsEnumerable = all.AsEnumerable().Where(predicate);
                all = AsEnumerable.AsQueryable();
            }
            var pagedData = all
                .Skip(startIndex)
                .Take(req.pageSize);

            if (all == null)
            {
                rowCount = 0;
            }
            else
            {
                if (all.Any())
                {
                    rowCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(all.Count())));
                }
                else
                {
                    rowCount = 0;
                }
            }


            var FlatList = _mapper.Map<List<PersonnelFunctionDTO>>(pagedData);

            return OperationResult.Succeeded(payload: FlatList, rowCount: rowCount);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطایی رخ داده");
        }
    }
    public new async Task<OperationResult> UpdateForAsync(PersonnelFunctionDTO entityToUpdate)
    {
        try
        {
            if (_db.Set<Fiche>().Any(i => i.PersonnelFunctionId == entityToUpdate.Id))
            {
                return OperationResult.Failed("برای رکورد مورد نظر فیش حقوقی وجود دارد امکان بروز رسانی وجود ندارد");
            }

            if (!entityToUpdate.Id.HasValue || entityToUpdate.Id.Value <= 0)
            {
                return OperationResult.Failed("شناسه رکورد معتبر نیست");
            }

            var existing = await GetIdAsync(entityToUpdate.Id.Value);
            if (existing == null)
            {
                return OperationResult.Failed("رکورد یافت نشد");
            }

            var mappedTodo = _mapper.Map<PersonnelFunction>(entityToUpdate);

            // حفظ تاریخ‌های گلوگاه؛ فقط در انتقال وضعیت پر می‌شوند
            mappedTodo.ReceiveDate = existing.ReceiveDate ?? DateTime.Now;
            mappedTodo.CreateDate = existing.CreateDate;
            mappedTodo.IPAddress = existing.IPAddress;

            var wasConfirmed = existing.IsConfirmed == true;
            var willConfirm = mappedTodo.IsConfirmed == true;
            if (willConfirm && !wasConfirmed)
            {
                var now = DateTime.Now;
                mappedTodo.ConfirmDate = now;
                mappedTodo.PayRollAproveDate = now;
                if (string.IsNullOrWhiteSpace(mappedTodo.PayRollApproveUser))
                {
                    mappedTodo.PayRollApproveUser = _userResolverService.GetUser();
                }
            }
            else if (willConfirm && wasConfirmed)
            {
                mappedTodo.ConfirmDate = existing.ConfirmDate ?? mappedTodo.ConfirmDate ?? DateTime.Now;
                mappedTodo.PayRollAproveDate = existing.PayRollAproveDate ?? mappedTodo.PayRollAproveDate;
                mappedTodo.PayRollApproveUser = string.IsNullOrWhiteSpace(mappedTodo.PayRollApproveUser)
                    ? existing.PayRollApproveUser
                    : mappedTodo.PayRollApproveUser;
            }
            else
            {
                mappedTodo.ConfirmDate = existing.ConfirmDate;
                mappedTodo.PayRollAproveDate = existing.PayRollAproveDate;
                mappedTodo.PayRollApproveUser = existing.PayRollApproveUser;
            }

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
    /// <summary>
    /// Copy functions from last period
    /// </summary>
    /// <returns></returns>
    public OperationResult CopyFunctionsFromLastPeriod()
    {
        if (_currentUserDefaultPaymentPeriod > 0)
        {
            var currentPeriod = _unitOfWork.Context.PaymentPeriods.Find(_currentUserDefaultPaymentPeriod);
            if (currentPeriod == null)
            {
                return OperationResult.Failed("دوره پیش فرض برای کاربر جاری تعریف نشده است");
            }
            else
            {
                if (currentPeriod.IsClosed)
                {
                    return OperationResult.Failed("دوره جاری بسته و امکان انتقال کارکرد وجود ندارد");
                }
                else
                {
                    long NewPaymentPeriodId = _currentUserDefaultPaymentPeriod;

                    if (currentPeriod.ShamsiMonth == 1)
                    {
                        var LastPaymentPeriod = _unitOfWork.Context.PaymentPeriods.Where(i => i.ShamsiYear == currentPeriod.ShamsiYear - 1 && i.ShamsiMonth == 12);
                        if (LastPaymentPeriod == null)
                        {
                            return OperationResult.Failed("دوره قبلی در جدول دوره ها تعریف نشده است");
                        }
                        else
                        {
                            if (LastPaymentPeriod.Any())
                            {
                                long LastPaymentPeriodId = LastPaymentPeriod.Single().Id;
                                using (SqlConnection con = new SqlConnection(_connectionString))
                                {
                                    using (var command = con.CreateCommand())
                                    {
                                        command.CommandType = CommandType.StoredProcedure;
                                        command.CommandText = "[payroll].[CopyFunctionsFromLastPeriod]";
                                        command.Parameters.AddWithValue("@NewPaymentPeriodId", NewPaymentPeriodId);
                                        command.Parameters.AddWithValue("@LastPaymentPeriodId", LastPaymentPeriodId);
                                        SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                        returnValue.Direction = ParameterDirection.ReturnValue;
                                        con.Open();
                                        command.ExecuteNonQuery();
                                        return OperationResult.Succeeded();
                                    }
                                }
                            }
                            else
                            {
                                return OperationResult.Failed("دوره قبلی در جدول دوره ها تعریف نشده است");
                            }
                        }

                    }
                    else if (currentPeriod.ShamsiMonth > 1)
                    {
                        var LastPaymentPeriod = _unitOfWork.Context.PaymentPeriods.Where(i => i.ShamsiYear == currentPeriod.ShamsiYear && i.ShamsiMonth == currentPeriod.ShamsiMonth - 1);
                        if (LastPaymentPeriod == null)
                        {
                            return OperationResult.Failed("دوره قبلی در جدول دوره ها تعریف نشده است");
                        }
                        else
                        {
                            if (LastPaymentPeriod.Any())
                            {
                                long LastPaymentPeriodId = LastPaymentPeriod.Single().Id;
                                using (SqlConnection con = new SqlConnection(_connectionString))
                                {
                                    using (var command = con.CreateCommand())
                                    {
                                        command.CommandType = CommandType.StoredProcedure;
                                        command.CommandText = "[payroll].[CopyFunctionsFromLastPeriod]";
                                        command.Parameters.AddWithValue("@NewPaymentPeriodId", NewPaymentPeriodId);
                                        command.Parameters.AddWithValue("@LastPaymentPeriodId", LastPaymentPeriodId);
                                        SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                        returnValue.Direction = ParameterDirection.ReturnValue;
                                        con.Open();
                                        command.ExecuteNonQuery();
                                        return OperationResult.Succeeded();
                                    }
                                }
                            }
                            else
                            {
                                return OperationResult.Failed("دوره قبلی در جدول دوره ها تعریف نشده است");
                            }
                        }

                    }
                    else
                    {
                        return OperationResult.Failed("دوره قبلی در جدول دوره ها تعریف نشده است");
                    }
                }
            }
        }
        else
        {
            return OperationResult.Failed("دوره پیش فرض کاربر جاری یافت نشد");
        }
    }

    /// <summary>
    /// 5 سال اخیر را باز می گرداند
    /// </summary>
    /// <returns></returns>
    public OperationResult GetTop5Year()
    {
        List<SharedKernel.Data.KeyValuePair> top5Year = new List<SharedKernel.Data.KeyValuePair>();
        var persianCalendar = new System.Globalization.PersianCalendar();
        int shamsiYear = persianCalendar.GetYear(DateTime.Now);
        for (int i = 0; i < 6; i++)
        {
            SharedKernel.Data.KeyValuePair year = new SharedKernel.Data.KeyValuePair();
            year.key = shamsiYear - i;
            year.id = year.key;
            year.value = (shamsiYear - i).ToString();
            top5Year.Add(year);
        }
        return OperationResult.Succeeded(payload: top5Year);
    }
    public OperationResult GetYearList()
    {
        List<SharedKernel.Data.KeyValuePair> top5Year = new List<SharedKernel.Data.KeyValuePair>();
        var persianCalendar = new System.Globalization.PersianCalendar();
        int shamsiYear = persianCalendar.GetYear(DateTime.Now);
        int startYear = 1368;
        for (int i = startYear; i <= shamsiYear + 1; i++)
        {
            SharedKernel.Data.KeyValuePair year = new()
            {
                key = i
            };
            year.id = year.key;
            year.value = (i).ToString();
            top5Year.Add(year);
        }
        return OperationResult.Succeeded(payload: top5Year.OrderByDescending(i=>i.key));
    }
    public OperationResult GetMonths()
    {
        List<SharedKernel.Data.KeyValuePair> yearMonths = new List<SharedKernel.Data.KeyValuePair>();
        var persianCalendar = new System.Globalization.PersianCalendar();

        for (int i = 1; i < 13; i++)
        {
            SharedKernel.Data.KeyValuePair year = new SharedKernel.Data.KeyValuePair();
            year.key = i;
            year.id = i;


            switch (i)
            {
                case 1:
                    year.value = "فروردین";
                    break;
                case 2:
                    year.value = "اردیبهشت";
                    break;
                case 3:
                    year.value = "خرداد";
                    break;
                case 4:
                    year.value = "تیر";
                    break;
                case 5:
                    year.value = "مرداد";
                    break;
                case 6:
                    year.value = "شهریور";
                    break;
                case 7:
                    year.value = "مهر";
                    break;
                case 8:
                    year.value = "آبان";
                    break;
                case 9:
                    year.value = "آذر";
                    break;
                case 10:
                    year.value = "دی";
                    break;
                case 11:
                    year.value = "بهمن";
                    break;
                case 12:
                    year.value = "اسفند";
                    break;
            }


            yearMonths.Add(year);
        }
        return OperationResult.Succeeded(payload: yearMonths);
    }
    public OperationResult PayRollApprove(FunctionApproveDTO req)
    {
        try
        {
            _unitOfWork.CreateTransaction();
            var Function = GetIdAsync(req.Id).Result;
            if (Function != null)
            {
                if (Function.OrganisationChartId == _currentUserDefaultOrganId)
                {
                    if (_unitOfWork.Context.Entry<PersonnelFunction>(Function).GetDatabaseValues().GetValue<bool?>("IsConfirmed") != true)
                    {
                        var now = DateTime.Now;
                        Function.IsConfirmed = true;
                        // زمان تایید رکورد در سیستم حقوق
                        Function.ConfirmDate = now;
                        // زمان تایید حقوق و دستمزد
                        Function.PayRollAproveDate = now;
                        Function.PayRollApproveUser = _userResolverService.GetUser();
                        _unitOfWork.Context.Update(Function);
                        _unitOfWork.Context.SaveChanges();
                        _unitOfWork.Commit();
                        return OperationResult.Succeeded();
                    }
                }
            }
            _unitOfWork.Rollback();
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed("خطای بحرانی ره داده است");
        }
    }

    /// <summary>
    /// ایجاد کارکرد — زمان دریافت در سامانه ثبت می‌شود
    /// </summary>
    public new async Task<OperationResult> CreateForAsync(PersonnelFunctionDTO entityToCreate)
    {
        if (!entityToCreate.ReceiveDate.HasValue)
        {
            entityToCreate.ReceiveDate = DateTime.Now;
        }

        // رکورد تازه نباید تاییدشده وارد شود مگر صراحتاً ارسال شده باشد
        if (entityToCreate.IsConfirmed != true)
        {
            entityToCreate.IsConfirmed = false;
            entityToCreate.ConfirmDate = null;
            entityToCreate.PayRollAproveDate = null;
            entityToCreate.PayRollApproveUser = null;
        }
        else
        {
            var now = DateTime.Now;
            entityToCreate.ConfirmDate ??= now;
            entityToCreate.PayRollAproveDate ??= now;
            if (string.IsNullOrWhiteSpace(entityToCreate.PayRollApproveUser))
            {
                entityToCreate.PayRollApproveUser = _userResolverService.GetUser();
            }
        }

        return await base.CreateForAsync(entityToCreate);
    }

    public bool Validate(PersonnelFunction entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    public OperationResult GetFilteredPersonnelFunctions(PersonnelFunctionFilterDTO filterDto)
    {
        try
        {
            var query = All(IgnoreExpired: false)
                .Include(p => p.Employee)
                .Where(p => p.OrganisationChartId == _currentUserDefaultOrganId);

            // فیلتر سال
            if (filterDto.Year > 0)
            {
                query = query.Where(p => p.Year == filterDto.Year);
            }

            // فیلتر ماه
            if (filterDto.Month > 0)
            {
                query = query.Where(p => p.Month == filterDto.Month);
            }

            // فیلتر کارمند
            if (filterDto.EmployeeId.HasValue && filterDto.EmployeeId.Value > 0)
            {
                query = query.Where(p => p.EmployeeId == filterDto.EmployeeId.Value);
            }

            // فیلتر نام
            if (!string.IsNullOrWhiteSpace(filterDto.FirstName))
            {
                query = query.Where(p => p.Employee != null && p.Employee.FirstName.Contains(filterDto.FirstName));
            }

            // فیلتر نام خانوادگی
            if (!string.IsNullOrWhiteSpace(filterDto.LastName))
            {
                query = query.Where(p => p.Employee != null && p.Employee.LastName.Contains(filterDto.LastName));
            }

            // فیلتر کد ملی
            if (!string.IsNullOrWhiteSpace(filterDto.NationalNo))
            {
                query = query.Where(p => p.Employee != null && p.Employee.NationalNo.Contains(filterDto.NationalNo));
            }

            // فیلتر کد کارمندی
            if (!string.IsNullOrWhiteSpace(filterDto.PersonnelCode))
            {
                query = query.Where(p => p.Employee != null && p.Employee.PersonelCode.Contains(filterDto.PersonnelCode));
            }

            // فیلتر PaymentPeriod (بر اساس ShamsiYear و ShamsiMonth)
            if (filterDto.PaymentPeriodId.HasValue && filterDto.PaymentPeriodId.Value > 0)
            {
                var paymentPeriod = _db.Set<PaymentPeriod>().FirstOrDefault(pp => pp.Id == filterDto.PaymentPeriodId.Value);
                if (paymentPeriod != null)
                {
                    query = query.Where(p => p.Year == paymentPeriod.ShamsiYear && p.Month == paymentPeriod.ShamsiMonth);
                }
            }

            // فیلتر CostCenter
            if (filterDto.CostCenterId.HasValue && filterDto.CostCenterId.Value > 0)
            {
                query = query.Where(p => p.CostCenterId == filterDto.CostCenterId.Value);
            }

            // فیلتر OrganizationUnit
            if (filterDto.OrganizationUnitId.HasValue && filterDto.OrganizationUnitId.Value > 0)
            {
                query = query.Where(p => p.OrganizationUnitId == filterDto.OrganizationUnitId.Value);
            }

            // فیلتر WorkPlace
            if (filterDto.WorkPlaceId.HasValue && filterDto.WorkPlaceId.Value > 0)
            {
                query = query.Where(p => p.WorkPlaceId == filterDto.WorkPlaceId.Value);
            }

            // فیلترهای InterdictOrder
            if (!string.IsNullOrWhiteSpace(filterDto.InterdictOrderFirstName) || 
                !string.IsNullOrWhiteSpace(filterDto.InterdictOrderNationalNo) || 
                !string.IsNullOrWhiteSpace(filterDto.InterdictOrderName))
            {
                var interdictOrderQuery = _db.Set<HR.Order.Core.Data.InterdictOrder>()
                    .Include(io => io.RecruitOrder)
                    .ThenInclude(ro => ro.Employee)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filterDto.InterdictOrderFirstName))
                {
                    interdictOrderQuery = interdictOrderQuery.Where(io => 
                        (io.FirstName != null && io.FirstName.Contains(filterDto.InterdictOrderFirstName)) ||
                        (io.RecruitOrder != null && io.RecruitOrder.Employee != null && 
                         io.RecruitOrder.Employee.FirstName != null && 
                         io.RecruitOrder.Employee.FirstName.Contains(filterDto.InterdictOrderFirstName)));
                }

                if (!string.IsNullOrWhiteSpace(filterDto.InterdictOrderNationalNo))
                {
                    interdictOrderQuery = interdictOrderQuery.Where(io => 
                        (io.NationalNo != null && io.NationalNo.Contains(filterDto.InterdictOrderNationalNo)) ||
                        (io.RecruitOrder != null && io.RecruitOrder.Employee != null && 
                         io.RecruitOrder.Employee.NationalNo != null && 
                         io.RecruitOrder.Employee.NationalNo.Contains(filterDto.InterdictOrderNationalNo)));
                }

                if (!string.IsNullOrWhiteSpace(filterDto.InterdictOrderName))
                {
                    interdictOrderQuery = interdictOrderQuery.Where(io => 
                        ((io.FirstName != null && io.FirstName.Contains(filterDto.InterdictOrderName)) ||
                         (io.LastName != null && io.LastName.Contains(filterDto.InterdictOrderName))) ||
                        (io.RecruitOrder != null && io.RecruitOrder.Employee != null && 
                         ((io.RecruitOrder.Employee.FirstName != null && io.RecruitOrder.Employee.FirstName.Contains(filterDto.InterdictOrderName)) ||
                          (io.RecruitOrder.Employee.LastName != null && io.RecruitOrder.Employee.LastName.Contains(filterDto.InterdictOrderName)))));
                }

                var employeeIdsFromInterdict = interdictOrderQuery
                    .Where(io => io.RecruitOrder != null && io.RecruitOrder.EmployeeId > 0)
                    .Select(io => io.RecruitOrder.EmployeeId)
                    .Distinct()
                    .ToList();

                if (employeeIdsFromInterdict.Any())
                {
                    query = query.Where(p => employeeIdsFromInterdict.Contains(p.EmployeeId));
                }
                else
                {
                    // اگر هیچ نتیجه‌ای پیدا نشد، هیچ رکوردی برگردانده نشود
                    query = query.Where(p => false);
                }
            }

            // فیلتر عمومی
            if (!string.IsNullOrWhiteSpace(filterDto.Filter))
            {
                query = query.Where(p => 
                    (p.Employee != null && (
                        p.Employee.FirstName.Contains(filterDto.Filter) ||
                        p.Employee.LastName.Contains(filterDto.Filter) ||
                        p.Employee.NationalNo.Contains(filterDto.Filter) ||
                        p.Employee.PersonelCode.Contains(filterDto.Filter)
                    )) ||
                    p.Comment.Contains(filterDto.Filter)
                );
            }

            // Employee access check - filter by accessible employee IDs
            if (_employeeService != null && _userResolverService != null)
            {
                try
                {
                    var currentUserId = _userResolverService.GetUserId();
                    var accessibleEmployeeIds = _employeeService.GetAccessibleEmployeeIds(currentUserId).ToList();
                    
                    if (accessibleEmployeeIds.Any())
                    {
                        query = query.Where(p => accessibleEmployeeIds.Contains(p.EmployeeId));
                    }
                    else
                    {
                        // User has no access to any employees - return empty result
                        query = query.Where(p => false);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail - return results without access filtering
                    // _logger?.LogError(ex, "Error in employee access check");
                }
            }

            // تعداد کل رکوردها
            var totalCount = query.Count();

            // مرتب‌سازی
            if (!string.IsNullOrWhiteSpace(filterDto.ActiveSortColumn))
            {
                var isDescending = filterDto.Sortdirection?.ToLower() == "desc";
                
                query = filterDto.ActiveSortColumn.ToLower() switch
                {
                    "firstname" => isDescending 
                        ? query.OrderByDescending(p => p.Employee.FirstName) 
                        : query.OrderBy(p => p.Employee.FirstName),
                    "lastname" => isDescending 
                        ? query.OrderByDescending(p => p.Employee.LastName) 
                        : query.OrderBy(p => p.Employee.LastName),
                    "nationalno" => isDescending 
                        ? query.OrderByDescending(p => p.Employee.NationalNo) 
                        : query.OrderBy(p => p.Employee.NationalNo),
                    "personelcode" => isDescending 
                        ? query.OrderByDescending(p => p.Employee.PersonelCode) 
                        : query.OrderBy(p => p.Employee.PersonelCode),
                    "year" => isDescending 
                        ? query.OrderByDescending(p => p.Year) 
                        : query.OrderBy(p => p.Year),
                    "month" => isDescending 
                        ? query.OrderByDescending(p => p.Month) 
                        : query.OrderBy(p => p.Month),
                    "createdate" => isDescending 
                        ? query.OrderByDescending(p => p.CreateDate) 
                        : query.OrderBy(p => p.CreateDate),
                    _ => isDescending 
                        ? query.OrderByDescending(p => p.Id) 
                        : query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreateDate);
            }

            // صفحه‌بندی
            var pagedData = query
                .Skip(filterDto.CurrentPage * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .Select(p => new
                {
                    p.Id,
                    recrank = filterDto.CurrentPage * filterDto.PageSize + 1,
                    firstname = p.Employee != null ? p.Employee.FirstName : "",
                    lastname = p.Employee != null ? p.Employee.LastName : "",
                    nationalno = p.Employee != null ? p.Employee.NationalNo : "",
                    personelcode = p.Employee != null ? p.Employee.PersonelCode : "",
                    p.Year,
                    p.Month,
                    p.FunctionDay,
                    p.PersonnelFunctionDay,
                    p.PersonnelHourPresent,
                    p.PersonnelNoEnter,
                    p.PersonnelAbsenceDay,
                    p.PersonnelIllnessDay,
                    p.PersonnelMissionHours,
                    p.PersonnelOverTime,
                    p.PersonnelOverTimeMinutes,
                    p.PersonnelNightWork,
                    p.PersonnelWorkingHolidayHours,
                    p.RemoteWorkHours,
                    p.IsConfirmed,
                    p.RealFunctionDay,
                    p.HolidayFunctionDay,
                    p.PersonnelMissionDay,
                    p.PaylessDay,
                    p.PaylessHour,
                    p.ShiftWork10Percent,
                    p.ShiftWork15Percent,
                    p.ShiftWork22Point5Percent,
                    p.RewardsDay,
                    p.PostType,
                    p.DeservedFunctionInHoliday,
                    p.DeservedFunctionOutHoliday,
                    p.PersonnelNightWorkDay,
                    p.PersonnelWorkingHolidaysDay,
                    p.LinearFunctionDay,
                    p.IsModir,
                    p.PersonnelCeillingOvertime,
                    p.PersonnelOverTimeFixed,
                    p.CarServiceDeduction,
                    p.AttendanceId,
                    p.ShiftWorkAllowance,
                    p.ShiftCount,
                    p.Food,
                    p.AccordReward,
                    p.Reward,
                    p.Arear1,
                    p.Arear2,
                    p.PersonnelHourlyWork,
                    p.PersonnelHourlyWorkMinutes,
                    p.PaylessMinutes,
                    p.Karaneh,
                    p.PersonnelNightWorkMinutes,
                    p.BasijOverTime,
                    p.TravelExpenses,
                    p.MissionExpenses,
                    p.RequestForAdditionalInsuranceForEntry,
                    p.MaximumAmountOfAllowancePayable,
                    p.OtherDeductions,
                    p.HekmatDeductions,
                    p.PersonnelWorkingHolidayMinutes,
                    p.DebtToTheCompany,
                    p.LastMonthDemand,
                    p.BonusCeiling,
                    p.IndividualBonusCeiling,
                    p.OtherBenefits,
                    p.FridayWorkHours,
                    p.FridayWorkAllowance,
                    p.NightWorkAllowance,
                    p.Comment,
                    p.CreateDate,
                    createdate = p.CreateDate
                })
                .ToList();

            // شماره‌گذاری ردیف‌ها
            int rowNumber = filterDto.CurrentPage * filterDto.PageSize + 1;
            var result = pagedData.Select(item => new
            {
                item.Id,
                recrank = rowNumber++,
                item.firstname,
                item.lastname,
                item.nationalno,
                item.personelcode,
                item.Year,
                item.Month,
                item.FunctionDay,
                item.PersonnelFunctionDay,
                item.PersonnelHourPresent,
                item.PersonnelNoEnter,
                item.PersonnelAbsenceDay,
                item.PersonnelIllnessDay,
                item.PersonnelMissionHours,
                item.PersonnelOverTime,
                item.PersonnelOverTimeMinutes,
                item.PersonnelNightWork,
                item.PersonnelWorkingHolidayHours,
                item.RemoteWorkHours,
                item.IsConfirmed,
                item.RealFunctionDay,
                item.HolidayFunctionDay,
                item.PersonnelMissionDay,
                item.PaylessDay,
                item.PaylessHour,
                item.ShiftWork10Percent,
                item.ShiftWork15Percent,
                item.ShiftWork22Point5Percent,
                item.RewardsDay,
                item.PostType,
                item.DeservedFunctionInHoliday,
                item.DeservedFunctionOutHoliday,
                item.PersonnelNightWorkDay,
                item.PersonnelWorkingHolidaysDay,
                item.LinearFunctionDay,
                item.IsModir,
                item.PersonnelCeillingOvertime,
                item.PersonnelOverTimeFixed,
                item.CarServiceDeduction,
                item.AttendanceId,
                item.ShiftWorkAllowance,
                item.ShiftCount,
                item.Food,
                item.AccordReward,
                item.Reward,
                item.Arear1,
                item.Arear2,
                item.PersonnelHourlyWork,
                item.PersonnelHourlyWorkMinutes,
                item.PaylessMinutes,
                item.Karaneh,
                item.PersonnelNightWorkMinutes,
                item.BasijOverTime,
                item.TravelExpenses,
                item.MissionExpenses,
                item.RequestForAdditionalInsuranceForEntry,
                item.MaximumAmountOfAllowancePayable,
                item.OtherDeductions,
                item.HekmatDeductions,
                item.PersonnelWorkingHolidayMinutes,
                item.DebtToTheCompany,
                item.LastMonthDemand,
                item.BonusCeiling,
                item.IndividualBonusCeiling,
                item.OtherBenefits,
                item.FridayWorkHours,
                item.FridayWorkAllowance,
                item.NightWorkAllowance,
                item.Comment,
                item.createdate
            }).ToList();

            return OperationResult.Succeeded(payload: result, rowCount: totalCount);
        }
        catch (Exception ex)
        {
           // _logger.LogError(ex, "Error in GetFilteredPersonnelFunctions");
            return OperationResult.Failed("خطا در دریافت اطلاعات کارکرد");
        }
    }

    public async Task<OperationResult> BulkUpdateAsync(List<PersonnelFunctionDTO> items)
    {
        try
        {
            _unitOfWork.CreateTransaction();
            int successCount = 0;
            int failCount = 0;
            int insertCount = 0;
            int updateCount = 0;
            var errors = new List<string>();

            foreach (var item in items)
            {
                try
                {
                    // Validate required fields
                    if (item.EmployeeId <= 0)
                    {
                        errors.Add($"شناسه کارمند معتبر نیست");
                        failCount++;
                        continue;
                    }

                    // Get Year and Month from DTO, or from _currentUserDefaultPaymentPeriod if not provided
                    int? year = item.Year;
                    int? month = item.Month;

                    // If Year and Month are not provided, get them from _currentUserDefaultPaymentPeriod
                    if (!year.HasValue || !month.HasValue || year.Value <= 0 || month.Value <= 0)
                    {
                        if (_currentUserDefaultPaymentPeriod > 0)
                        {
                            var currentPeriod = _unitOfWork.Context.PaymentPeriods.Find(_currentUserDefaultPaymentPeriod);
                            if (currentPeriod != null)
                            {
                                year = currentPeriod.ShamsiYear;
                                month = currentPeriod.ShamsiMonth;
                            }
                        }
                    }

                    // Final validation: if still no valid year/month, skip this item
                    if (!year.HasValue || !month.HasValue || year.Value <= 0 || month.Value <= 0)
                    {
                        errors.Add($"سال و ماه برای کارمند {item.EmployeeId} معتبر نیست. لطفاً سال و ماه را مشخص کنید یا دوره پرداخت پیش‌فرض را تنظیم کنید.");
                        failCount++;
                        continue;
                    }

                    // Find existing record with highest ID for this EmployeeId, Year, and Month
                    var existingRecord = All(IgnoreExpired: false)
                        .Where(pf => pf.EmployeeId == item.EmployeeId
                                   && pf.Year == year.Value
                                   && pf.Month == month.Value
                                   && pf.OrganisationChartId == _currentUserDefaultOrganId)
                        .OrderByDescending(pf => pf.Id)
                        .FirstOrDefault();

                    PersonnelFunction entityToSave;

                    if (existingRecord != null)
                    {
                        // Update existing record (Upsert - Update part)
                        // Check if fiche exists for this personnel function
                        if (_db.Set<Fiche>().Any(i => i.PersonnelFunctionId == existingRecord.Id))
                        {
                            errors.Add($"برای کارکرد کارمند {item.EmployeeId} در سال {year.Value} و ماه {month.Value} فیش حقوقی وجود دارد و امکان بروزرسانی وجود ندارد");
                            failCount++;
                            continue;
                        }

                        // Map DTO to existing entity, preserving the ID
                        entityToSave = _mapper.Map<PersonnelFunction>(item);
                        entityToSave.Id = existingRecord.Id;
                        entityToSave.Year = year.Value;
                        entityToSave.Month = month.Value;
                        entityToSave.EmployeeId = item.EmployeeId;
                        entityToSave.OrganisationChartId = _currentUserDefaultOrganId;

                        // Preserve original create date and other audit / gateway fields
                        entityToSave.CreateDate = existingRecord.CreateDate;
                        entityToSave.IPAddress = existingRecord.IPAddress;
                        entityToSave.ReceiveDate = existingRecord.ReceiveDate ?? DateTime.Now;
                        entityToSave.ConfirmDate = existingRecord.ConfirmDate;
                        entityToSave.PayRollAproveDate = existingRecord.PayRollAproveDate;
                        entityToSave.PayRollApproveUser = existingRecord.PayRollApproveUser;
                        entityToSave.IsConfirmed = existingRecord.IsConfirmed;

                        Update(entityToSave);
                        updateCount++;
                    }
                    else
                    {
                        // Insert new record (Upsert - Insert part)
                        entityToSave = _mapper.Map<PersonnelFunction>(item);
                        entityToSave.Id = 0; // Ensure new record
                        entityToSave.Year = year.Value;
                        entityToSave.Month = month.Value;
                        entityToSave.EmployeeId = item.EmployeeId;
                        entityToSave.OrganisationChartId = _currentUserDefaultOrganId;
                        entityToSave.ReceiveDate ??= DateTime.Now;
                        entityToSave.IsConfirmed = false;
                        entityToSave.ConfirmDate = null;
                        entityToSave.PayRollAproveDate = null;
                        entityToSave.PayRollApproveUser = null;

                        Add(entityToSave);
                        insertCount++;
                    }

                    // Validate date range overlap
                    if (CheckDateRangeNoOverLap(entityToSave))
                    {
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"در بازه زمانی انتخابی برای کارمند {item.EmployeeId} در سال {year.Value} و ماه {month.Value} ردیف تکراری وجود دارد");
                        failCount++;
                    }
                }
                catch (Exception ex)
                {
                    var employeeIdStr = item.EmployeeId > 0 ? item.EmployeeId.ToString() : "نامشخص";
                    errors.Add($"خطا در پردازش کارکرد کارمند {employeeIdStr}: {ex.Message}");
                    failCount++;
                }
            }

            if (successCount > 0)
            {
                var saved = await _unitOfWork.Save();
                if (saved > 0)
                {
                    _unitOfWork.Commit();
                    var message = $"{successCount} رکورد با موفقیت پردازش شد";
                    if (insertCount > 0)
                    {
                        message += $" ({insertCount} درج";
                    }
                    if (updateCount > 0)
                    {
                        message += insertCount > 0 ? $", {updateCount} بروزرسانی" : $" ({updateCount} بروزرسانی";
                    }
                    if (insertCount > 0 || updateCount > 0)
                    {
                        message += ")";
                    }
                    if (failCount > 0)
                    {
                        message += $". {failCount} رکورد با خطا مواجه شد";
                    }
                    return OperationResult.Succeeded(message, successCount);
                }
                else
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("خطا در ذخیره تغییرات");
                }
            }
            else
            {
                _unitOfWork.Rollback();
                var errorMessage = string.Join("; ", errors);
                return OperationResult.Failed($"هیچ رکوردی پردازش نشد. خطاها: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی گروهی: {ex.Message}");
        }
    }

    public OperationResult GetFunctionBulkCartable(FunctionBulkCartableFilterDTO filterDto)
    {
        try
        {
            var currentUserId = _userResolverService.GetUserId();
            
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[payroll].[FunctionBulkCartable]";
                    
                    command.Parameters.AddWithValue("@EducationGradeId", filterDto.EducationGradeId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@JobNatureId", filterDto.JobNatureId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OrganizationJobId", filterDto.OrganizationJobId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GenderId", filterDto.GenderId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaritalStatusId", filterDto.MaritalStatusId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CostCenterId", filterDto.CostCenterId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OrganizationUnitId", filterDto.OrganizationUnitId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@WorkPlaceId", filterDto.WorkPlaceId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PayLocationId", filterDto.PayLocationId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeStatusId", filterDto.EmployeeStatusId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeTypeId", filterDto.EmployeeTypeId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(filterDto.FirstName) ? (object)DBNull.Value : filterDto.FirstName);
                    command.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(filterDto.LastName) ? (object)DBNull.Value : filterDto.LastName);
                    command.Parameters.AddWithValue("@PersonelCode", string.IsNullOrEmpty(filterDto.PersonelCode) ? (object)DBNull.Value : filterDto.PersonelCode);
                    command.Parameters.AddWithValue("@IdentityNo", string.IsNullOrEmpty(filterDto.IdentityNo) ? (object)DBNull.Value : filterDto.IdentityNo);
                    command.Parameters.AddWithValue("@NationalNo", string.IsNullOrEmpty(filterDto.NationalNo) ? (object)DBNull.Value : filterDto.NationalNo);
                    command.Parameters.AddWithValue("@PaymentPeriodId", filterDto.PaymentPeriodId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PageNo", filterDto.CurrentPage + 1);
                    command.Parameters.AddWithValue("@PageSize", filterDto.PageSize);
                    command.Parameters.AddWithValue("@CurrentUserId", currentUserId);
                    command.Parameters.AddWithValue("@SortBy", filterDto.SortBy ?? "FirstName");
                    command.Parameters.AddWithValue("@SortDirection", filterDto.SortDirection ?? "ASC");

                    con.Open();
                    SqlDataReader rdr = command.ExecuteReader();
                    var results = new List<FunctionBulkCartableDTO>();
                    int totalCount = 0;
                    
                    while (rdr.Read())
                    {
                        FunctionBulkCartableDTO row;
                        try
                        {
                            row = rdr.ConvertToObject<FunctionBulkCartableDTO>();
                        }
                        catch (Exception ex)
                        {
                            // If ConvertToObject fails (e.g., type conversion error), use manual mapping
                            row = MapReaderToFunctionBulkCartableDTO(rdr);
                        }
                        
                        results.Add(row);
                        // Get TotalCount from first row (it's the same in all rows)
                        if (totalCount == 0)
                        {
                            // Try to get TotalCount from the object first
                            if (row.TotalCount.HasValue)
                            {
                                totalCount = row.TotalCount.Value;
                            }
                            else
                            {
                                // If not found in object, try to read directly from reader
                                try
                                {
                                    int totalCountIndex = rdr.GetOrdinal("TotalCount");
                                    if (!rdr.IsDBNull(totalCountIndex))
                                    {
                                        totalCount = rdr.GetInt32(totalCountIndex);
                                    }
                                }
                                catch
                                {
                                    // TotalCount column not found or not accessible
                                }
                            }
                        }
                    }
                    rdr.Close();
                    con.Close();
                    
                    // Debug: Log TotalCount to help diagnose pagination issues
                    System.Diagnostics.Debug.WriteLine($"GetFunctionBulkCartable - TotalCount from SP: {totalCount}, Results Count: {results.Count}, PageNo: {filterDto.CurrentPage + 1}, PageSize: {filterDto.PageSize}");
            
            // If PersonnelFunctionId is not returned from SP, find it from EmployeeId, Year, Month
            foreach (var item in results)
            {
                if (!item.PersonnelFunctionId.HasValue && item.EmployeeId > 0 && item.Year.HasValue && item.Month.HasValue)
                {
                    var pf = All(IgnoreExpired: false)
                        .Where(p => p.EmployeeId == item.EmployeeId 
                                 && p.Year == item.Year.Value 
                                 && p.Month == item.Month.Value
                                 && p.OrganisationChartId == _currentUserDefaultOrganId)
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault();
                    
                    if (pf != null)
                    {
                        item.PersonnelFunctionId = pf.Id;
                    }
                }
            }

            // Add row numbers
            int rowNumber = filterDto.CurrentPage * filterDto.PageSize + 1;
            var resultWithRowNumbers = results.Select((item, index) => new
            {
                recrank = rowNumber + index,
                EmployeeId = item.EmployeeId,
                costCenter = item.CostCenter ?? "",
                firstname = item.FirstName ?? "",
                lastname = item.LastName ?? "",
                nationalno = item.NationalNo ?? "",
                personelcode = item.PersonelCode ?? "",
                job = item.Job ?? "",
                processAreaValue = item.ProcessAreaValue,
                processDescription = item.ProcessDescription ?? "",
                id = item.PersonnelFunctionId ?? 0, // Use PersonnelFunctionId for edit/delete operations
                Year = item.Year,
                Month = item.Month,
                FunctionDay = item.FunctionDay,
                PersonnelFunctionDay = item.PersonnelFunctionDay,
                PersonnelHourPresent = item.PersonnelHourPresent,
                PersonnelNoEnter = item.PersonnelNoEnter,
                PersonnelAbsenceDay = item.PersonnelAbsenceDay,
                PersonnelIllnessDay = item.PersonnelIllnessDay,
                PersonnelMissionHours = item.PersonnelMissionHours,
                PersonnelOverTime = item.PersonnelOverTime,
                PersonnelOverTimeMinutes = item.PersonnelOverTimeMinutes,
                PersonnelNightWork = item.PersonnelNightWork,
                PersonnelWorkingHolidayHours = item.PersonnelWorkingHolidayHours,
                RemoteWorkHours = item.RemoteWorkHours,
                IsConfirmed = item.IsConfirmed,
                RealFunctionDay = item.RealFunctionDay,
                HolidayFunctionDay = item.HolidayFunctionDay,
                PersonnelMissionDay = item.PersonnelMissionDay,
                PaylessDay = item.PaylessDay,
                PaylessHour = item.PaylessHour,
                ShiftWork10Percent = item.ShiftWork10Percent,
                ShiftWork15Percent = item.ShiftWork15Percent,
                ShiftWork22Point5Percent = item.ShiftWork22Point5Percent,
                RewardsDay = item.RewardsDay,
                PostType = item.PostType,
                DeservedFunctionInHoliday = item.DeservedFunctionInHoliday,
                DeservedFunctionOutHoliday = item.DeservedFunctionOutHoliday,
                PersonnelNightWorkDay = item.PersonnelNightWorkDay,
                PersonnelWorkingHolidaysDay = item.PersonnelWorkingHolidaysDay,
                LinearFunctionDay = item.LinearFunctionDay,
                IsModir = item.IsModir,
                PersonnelCeillingOvertime = item.PersonnelCeillingOvertime,
                PersonnelOverTimeFixed = item.PersonnelOverTimeFixed,
                CarServiceDeduction = item.CarServiceDeduction,
                AttendanceId = item.AttendanceId,
                ShiftWorkAllowance = item.ShiftWorkAllowance,
                ShiftCount = item.ShiftCount,
                Food = item.Food,
                AccordReward = item.AccordReward,
                Reward = item.Reward,
                Arear1 = item.Arear1,
                Arear2 = item.Arear2,
                PersonnelHourlyWork = item.PersonnelHourlyWork,
                PersonnelHourlyWorkMinutes = item.PersonnelHourlyWorkMinutes,
                PaylessMinutes = item.PaylessMinutes,
                Karaneh = item.Karaneh,
                PersonnelNightWorkMinutes = item.PersonnelNightWorkMinutes,
                BasijOverTime = item.BasijOverTime,
                TravelExpenses = item.TravelExpenses,
                MissionExpenses = item.MissionExpenses,
                RequestForAdditionalInsuranceForEntry = item.RequestForAdditionalInsuranceForEntry,
                MaximumAmountOfAllowancePayable = item.MaximumAmountOfAllowancePayable,
                OtherDeductions = item.OtherDeductions,
                HekmatDeductions = item.HekmatDeductions,
                PersonnelWorkingHolidayMinutes = item.PersonnelWorkingHolidayMinutes,
                DebtToTheCompany = item.DebtToTheCompany,
                LastMonthDemand = item.LastMonthDemand,
                BonusCeiling = item.BonusCeiling,
                IndividualBonusCeiling = item.IndividualBonusCeiling,
                OtherBenefits = item.OtherBenefits,
                OvertimePerCapita = item.OvertimePerCapita,
                DisciplinaryOvertime = item.DisciplinaryOvertime,
                ApprovedOvertimeHours = item.ApprovedOvertimeHours,
                OvertimeOutsideUnit = item.OvertimeOutsideUnit,
                ServiceRight = item.ServiceRight,
                ShiftReplacementOvertime = item.ShiftReplacementOvertime,
                CashOvertime = item.CashOvertime,
                TotalOvertime = item.TotalOvertime,
                EfficiencyAndBonusRight = item.EfficiencyAndBonusRight,
                MissionAndShift = item.MissionAndShift,
                OtherPaymentsAndDeductions = item.OtherPaymentsAndDeductions,
                Efficiency100Percent = item.Efficiency100Percent,
                ApprovedEfficiency = item.ApprovedEfficiency,
                ApprovedEfficiencyReserve = item.ApprovedEfficiencyReserve,
                FridayWorkHours = item.FridayWorkHours,
                FridayWorkAllowance = item.FridayWorkAllowance,
                NightWorkAllowance = item.NightWorkAllowance,
                Comment = item.Comment,
                createdate = item.ReceiveDate
            }).ToList();

            return OperationResult.Succeeded(payload: resultWithRowNumbers, rowCount: totalCount);
                }
            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت اطلاعات: {ex.Message}");
        }
    }

    /// <summary>
    /// Manual mapping from SqlDataReader to FunctionBulkCartableDTO to handle type conversion errors
    /// </summary>
    private FunctionBulkCartableDTO MapReaderToFunctionBulkCartableDTO(SqlDataReader rdr)
    {
        var dto = new FunctionBulkCartableDTO();
        
        // Helper methods to safely convert values
        long? SafeGetLong(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                
                var value = rdr.GetValue(ordinal);
                if (value == null || value == DBNull.Value)
                    return null;
                
                if (value is long longVal)
                    return longVal;
                if (value is int intVal)
                    return intVal;
                if (value is string strVal && !string.IsNullOrWhiteSpace(strVal) && long.TryParse(strVal, out long parsedLong))
                    return parsedLong;
                
                return Convert.ToInt64(value);
            }
            catch
            {
                return null;
            }
        }
        
        int? SafeGetInt(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                
                var value = rdr.GetValue(ordinal);
                if (value == null || value == DBNull.Value)
                    return null;
                
                if (value is int intVal)
                    return intVal;
                if (value is long longVal)
                    return (int)longVal;
                if (value is string strVal && !string.IsNullOrWhiteSpace(strVal) && int.TryParse(strVal, out int parsedInt))
                    return parsedInt;
                
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }
        
        decimal? SafeGetDecimal(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                
                var value = rdr.GetValue(ordinal);
                if (value == null || value == DBNull.Value)
                    return null;
                
                if (value is decimal decimalVal)
                    return decimalVal;
                if (value is double doubleVal)
                    return (decimal)doubleVal;
                if (value is float floatVal)
                    return (decimal)floatVal;
                if (value is string strVal && !string.IsNullOrWhiteSpace(strVal) && decimal.TryParse(strVal, out decimal parsedDecimal))
                    return parsedDecimal;
                
                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }
        
        float? SafeGetFloat(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                
                var value = rdr.GetValue(ordinal);
                if (value == null || value == DBNull.Value)
                    return null;
                
                if (value is float floatVal)
                    return floatVal;
                if (value is double doubleVal)
                    return (float)doubleVal;
                if (value is decimal decimalVal)
                    return (float)decimalVal;
                if (value is string strVal && !string.IsNullOrWhiteSpace(strVal) && float.TryParse(strVal, out float parsedFloat))
                    return parsedFloat;
                
                return Convert.ToSingle(value);
            }
            catch
            {
                return null;
            }
        }
        
        bool? SafeGetBool(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                
                var value = rdr.GetValue(ordinal);
                if (value == null || value == DBNull.Value)
                    return null;
                
                if (value is bool boolVal)
                    return boolVal;
                if (value is string strVal && !string.IsNullOrWhiteSpace(strVal) && bool.TryParse(strVal, out bool parsedBool))
                    return parsedBool;
                if (value is int intVal)
                    return intVal != 0;
                
                return Convert.ToBoolean(value);
            }
            catch
            {
                return null;
            }
        }
        
        string? SafeGetString(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                return rdr.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        DateTime? SafeGetDateTime(string columnName)
        {
            try
            {
                int ordinal = rdr.GetOrdinal(columnName);
                if (rdr.IsDBNull(ordinal))
                    return null;
                return rdr.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        // Map all properties
        dto.EmployeeId = SafeGetLong("EmployeeId") ?? 0;
        dto.PersonnelFunctionId = SafeGetLong("PersonnelFunctionId");
        dto.FirstName = SafeGetString("FirstName");
        dto.LastName = SafeGetString("LastName");
        dto.FatherName = SafeGetString("FatherName");
        dto.IdentityNo = SafeGetString("IdentityNo");
        dto.NationalNo = SafeGetString("NationalNo");
        dto.PersonelCode = SafeGetString("PersonelCode");
        dto.PayLocationId = SafeGetLong("PayLocationId");
        dto.PayLocation = SafeGetString("PayLocation");
        dto.EmployeeTypeId = SafeGetLong("EmployeeTypeId");
        dto.EmployeeType = SafeGetString("EmployeeType");
        dto.EmployeeStatusId = SafeGetLong("EmployeeStatusId");
        dto.EmployeeStatus = SafeGetString("EmployeeStatus");
        dto.CostCenterId = SafeGetLong("CostCenterId");
        dto.CostCenter = SafeGetString("CostCenter");
        dto.WorkPlaceId = SafeGetLong("WorkPlaceId");
        dto.WorkPlace = SafeGetString("WorkPlace");
        dto.OrganizationUnitId = SafeGetLong("OrganizationUnitId");
        dto.OrganizationUnit = SafeGetString("OrganizationUnit");
        dto.IsEmployed = SafeGetBool("IsEmployed");
        dto.EducationGradeId = SafeGetLong("EducationGradeId");
        dto.EducationGrade = SafeGetString("EducationGrade");
        dto.JobId = SafeGetLong("JobId");
        dto.Job = SafeGetString("Job");
        dto.JobNatureId = SafeGetLong("JobNatureId");
        dto.JobNature = SafeGetString("JobNature");
        dto.JobCode = SafeGetString("JobCode");
        dto.JobDegree = SafeGetInt("JobDegree");
        dto.GenderId = SafeGetLong("GenderId");
        dto.Gender = SafeGetString("Gender");
        dto.MaritalStatusId = SafeGetLong("MaritalStatusId");
        dto.MaritalStatus = SafeGetString("MaritalStatus");
        dto.ProcessAreaId = SafeGetLong("ProcessAreaId");
        dto.ProcessArea = SafeGetString("ProcessArea");
        dto.ProcessAreaValue = SafeGetDecimal("ProcessAreaValue");
        dto.ProcessDescription = SafeGetString("ProcessDescription");
        
        // PersonnelFunction fields
        dto.ArearsStatusId = SafeGetLong("ArearsStatusId");
        dto.OrganisationChartId = SafeGetLong("OrganisationChartId") ?? 0;
        dto.PersonnelFunctionExcelFileId = SafeGetLong("PersonnelFunctionExcelFileId");
        dto.FunctionDay = SafeGetInt("FunctionDay");
        dto.PersonnelFunctionDay = SafeGetInt("PersonnelFunctionDay");
        dto.PersonnelHourPresent = SafeGetInt("PersonnelHourPresent");
        dto.PersonnelNoEnter = SafeGetDecimal("PersonnelNoEnter");
        dto.PersonnelAbsenceDay = SafeGetDecimal("PersonnelAbsenceDay");
        dto.PersonnelIllnessDay = SafeGetDecimal("PersonnelIllnessDay");
        dto.PersonnelMissionHours = SafeGetDecimal("PersonnelMissionHours");
        dto.PersonnelOverTime = SafeGetDecimal("PersonnelOverTime");
        dto.PersonnelOverTimeMinutes = SafeGetDecimal("PersonnelOverTimeMinutes");
        dto.PersonnelNightWork = SafeGetDecimal("PersonnelNightWork");
        dto.PersonnelWorkingHolidayHours = SafeGetDecimal("PersonnelWorkingHolidayHours");
        dto.Year = SafeGetInt("Year");
        dto.Month = SafeGetInt("Month");
        dto.RemoteWorkHours = SafeGetInt("RemoteWorkHours");
        dto.IsConfirmed = SafeGetBool("IsConfirmed");
        dto.PayRollApproveUser = SafeGetString("PayRollApproveUser");
        dto.PayRollAproveDate = SafeGetDateTime("PayRollAproveDate");
        dto.RealFunctionDay = SafeGetDecimal("RealFunctionDay");
        dto.HolidayFunctionDay = SafeGetDecimal("HolidayFunctionDay");
        dto.FunctionTypeId = SafeGetLong("FunctionTypeId");
        dto.PersonnelMissionDay = SafeGetInt("PersonnelMissionDay");
        dto.PaylessDay = SafeGetInt("PaylessDay");
        dto.PaylessHour = SafeGetInt("PaylessHour");
        dto.ShiftWork10Percent = SafeGetInt("ShiftWork10Percent");
        dto.ShiftWork15Percent = SafeGetInt("ShiftWork15Percent");
        dto.ShiftWork22Point5Percent = SafeGetInt("ShiftWork22Point5Percent");
        dto.RewardsDay = SafeGetInt("RewardsDay");
        dto.PostType = SafeGetInt("PostType");
        dto.DeservedFunctionInHoliday = SafeGetInt("DeservedFunctionInHoliday");
        dto.DeservedFunctionOutHoliday = SafeGetInt("DeservedFunctionOutHoliday");
        dto.PersonnelNightWorkDay = SafeGetDecimal("PersonnelNightWorkDay");
        dto.PersonnelWorkingHolidaysDay = SafeGetFloat("PersonnelWorkingHolidaysDay");
        dto.LinearFunctionDay = SafeGetLong("LinearFunctionDay");
        dto.Comment = SafeGetString("Comment");
        dto.ConfirmDate = SafeGetDateTime("ConfirmDate");
        dto.IsModir = SafeGetBool("IsModir");
        dto.PersonnelCeillingOvertime = SafeGetDecimal("PersonnelCeillingOvertime");
        dto.PersonnelOverTimeFixed = SafeGetDecimal("PersonnelOverTimeFixed");
        dto.CarServiceDeduction = SafeGetLong("CarServiceDeduction");
        dto.AttendanceId = SafeGetLong("AttendanceId");
        dto.ReceiveDate = SafeGetDateTime("ReceiveDate");
        dto.Description = SafeGetString("Description");
        dto.AccordReward = SafeGetDecimal("AccordReward");
        dto.Arear1 = SafeGetDecimal("Arear1");
        dto.Arear2 = SafeGetDecimal("Arear2");
        dto.Food = SafeGetDecimal("Food");
        dto.Reward = SafeGetDecimal("Reward");
        dto.ShiftCount = SafeGetInt("ShiftCount");
        dto.ShiftWorkAllowance = SafeGetDecimal("ShiftWorkAllowance");
        dto.BasijOverTime = SafeGetDecimal("BasijOverTime");
        dto.Karaneh = SafeGetDecimal("Karaneh");
        dto.PaylessMinutes = SafeGetInt("PaylessMinutes");
        dto.PersonnelHourlyWork = SafeGetDecimal("PersonnelHourlyWork");
        dto.PersonnelHourlyWorkMinutes = SafeGetDecimal("PersonnelHourlyWorkMinutes");
        dto.PersonnelNightWorkMinutes = SafeGetDecimal("PersonnelNightWorkMinutes");
        dto.BonusCeiling = SafeGetDecimal("BonusCeiling");
        dto.DebtToTheCompany = SafeGetDecimal("DebtToTheCompany");
        dto.HekmatDeductions = SafeGetDecimal("HekmatDeductions");
        dto.IndividualBonusCeiling = SafeGetDecimal("IndividualBonusCeiling");
        dto.LastMonthDemand = SafeGetDecimal("LastMonthDemand");
        dto.MaximumAmountOfAllowancePayable = SafeGetDecimal("MaximumAmountOfAllowancePayable");
        dto.MissionExpenses = SafeGetDecimal("MissionExpenses");
        dto.OtherBenefits = SafeGetDecimal("OtherBenefits");
        dto.OtherDeductions = SafeGetDecimal("OtherDeductions");
        dto.PersonnelWorkingHolidayMinutes = SafeGetDecimal("PersonnelWorkingHolidayMinutes");
        dto.RequestForAdditionalInsuranceForEntry = SafeGetDecimal("RequestForAdditionalInsuranceForEntry");
        dto.TravelExpenses = SafeGetDecimal("TravelExpenses");
        dto.CreatedBy = SafeGetLong("CreatedBy");
        dto.LastModifiedBy = SafeGetLong("LastModifiedBy");
        dto.ApprovedEfficiency = SafeGetDecimal("ApprovedEfficiency");
        dto.ApprovedEfficiencyReserve = SafeGetDecimal("ApprovedEfficiencyReserve");
        dto.FridayWorkHours = SafeGetDecimal("FridayWorkHours");
        dto.FridayWorkAllowance = SafeGetDecimal("FridayWorkAllowance");
        dto.NightWorkAllowance = SafeGetDecimal("NightWorkAllowance");
        dto.ApprovedOvertimeHours = SafeGetDecimal("ApprovedOvertimeHours");
        dto.CashOvertime = SafeGetDecimal("CashOvertime");
        dto.DisciplinaryOvertime = SafeGetDecimal("DisciplinaryOvertime");
        dto.Efficiency100Percent = SafeGetDecimal("Efficiency100Percent");
        dto.EfficiencyAndBonusRight = SafeGetDecimal("EfficiencyAndBonusRight");
        dto.MissionAndShift = SafeGetDecimal("MissionAndShift");
        dto.OtherPaymentsAndDeductions = SafeGetDecimal("OtherPaymentsAndDeductions");
        dto.OvertimeOutsideUnit = SafeGetDecimal("OvertimeOutsideUnit");
        dto.OvertimePerCapita = SafeGetDecimal("OvertimePerCapita");
        dto.ServiceRight = SafeGetDecimal("ServiceRight");
        dto.ShiftReplacementOvertime = SafeGetDecimal("ShiftReplacementOvertime");
        dto.TotalOvertime = SafeGetDecimal("TotalOvertime");
        dto.TotalCount = SafeGetInt("TotalCount");
        
        return dto;
    }
}
