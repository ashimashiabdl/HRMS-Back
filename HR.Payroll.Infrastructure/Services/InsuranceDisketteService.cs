using AutoMapper;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.OLD;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

using System.Data;
using System.Data.Common;
using System.Text;
using System.Data.OleDb;
using DotNetDBF;
using Castle.Components.DictionaryAdapter.Xml;
using Hr.Employee.infrastructure.Services;
using HR.Order.Infrastructure.Services;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.infrastructure.Services;

namespace HR.Payroll.Infrastructure.Services;

public class InsuranceDisketteService(IMapper mapper, BaseTableService BaseTableService, InsuranceService InsuranceService, OrganizationJobService OrganizationJobService, OrganisationCostCenterService OrganisationCostCenterService, OrderService OrderService, OrganisationEmployeeTypeFicheItemService OrganisationEmployeeTypeFicheItemService, EmployeeService EmployeeService, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<InsuranceDisketteService> logger) : BaseService<InsuranceDiskette, PayrollContext, InsuranceDisketteDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{

    private EmployeeService _employeeService = EmployeeService;
    private OrganisationEmployeeTypeFicheItemService _organisationEmployeeTypeFicheItemService = OrganisationEmployeeTypeFicheItemService;
    private OrganisationCostCenterService _organisationCostCenterService = OrganisationCostCenterService;
    private OrderService _orderService = OrderService;
    private OrganizationJobService _organizationJobService = OrganizationJobService;
    private InsuranceService _insuranceService = InsuranceService;
    private BaseTableService _baseTableService = BaseTableService;
    private readonly ILogger<InsuranceDisketteService> _logger = logger;
    private readonly UserResolverService _userResolverService = userService;

    /// <summary>
    /// ذخیره تغییرات با فعال‌سازی موقت AutoDetectChanges
    /// </summary>
    private void SaveChangesWithAutoDetect()
    {
        var tempAutoDetect = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
            _unitOfWork.Context.SaveChanges();
        }
        finally
        {
            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = tempAutoDetect;
        }
    }

    public OperationResult GetCurrentInsuranceDisketteCostCenters(long id)
    {
        var InsuranceDiskette = GetIdAsync(id).Result;
        var costCenterList = _unitOfWork.Context.InsuranceDisketteCostCenters.Include(i => i.CostCenter).Where(i => i.InsuranceDisketteId == id);

        if (costCenterList == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (costCenterList.Any())
            {
                var rsulat = _mapper.Map<List<InsuranceDisketteCostCenterDTO>>(costCenterList.ToList());
                return OperationResult.Succeeded(payload: rsulat);
            }
            else
            {
                return OperationResult.NotFound();
            }
        }

    }
    public void CalculateInsuranceDisketteBatch()
    {
        try
        {
            _logger.LogInformation("شروع CalculateInsuranceDisketteBatch - محاسبه دیسکت بیمه گروهی");

            // بهینه‌سازی عملکرد: پاک‌سازی ChangeTracker و غیرفعال‌سازی موقت AutoDetectChanges
            _unitOfWork.Context.ChangeTracker.Clear();
            var originalAutoDetectChanges = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
            var originalQueryTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _logger.LogInformation("بهینه‌سازی ChangeTracker فعال شد: AutoDetectChanges=false, QueryTracking=NoTracking");

            try
            {
                // پاکسازی درخواست‌های گیر کرده در وضعیت Running
                try
                {
                    // فعال کردن موقت tracking برای پاکسازی
                    _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
                    _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

                    // timeout قابل تنظیم: 30 دقیقه
                    int stuckRequestTimeoutMinutes = 30;
                    var stuckRunningRequests = _unitOfWork.Context.BatchPayRollRequests
                        .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.InsuranceDisketteCalculation
                            && i.RequestStateId == (long)Enums.BatchPayRollRequestState.Running
                            && i.LastPoolingTime.HasValue
                            && i.LastPoolingTime.Value < DateTime.Now.AddMinutes(-stuckRequestTimeoutMinutes))
                        .ToList();

                    if (stuckRunningRequests.Any())
                    {
                        _logger.LogWarning("تعداد {Count} درخواست گیر کرده در وضعیت Running یافت شد (timeout: {Timeout} دقیقه). شروع پاکسازی...",
                            stuckRunningRequests.Count, stuckRequestTimeoutMinutes);

                        foreach (var stuckRequest in stuckRunningRequests)
                        {
                            try
                            {
                                _logger.LogWarning("پاکسازی درخواست گیر کرده {RequestId}, آخرین زمان فعالیت: {LastPoolingTime}",
                                    stuckRequest.Id, stuckRequest.LastPoolingTime);

                                stuckRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                stuckRequest.ExeptionMessage = $"درخواست به دلیل عدم پاسخ‌دهی (Timeout بیش از {stuckRequestTimeoutMinutes} دقیقه) متوقف شد. لطفاً مجدداً تلاش کنید.";
                                stuckRequest.FinishDateTime = DateTime.Now;
                                stuckRequest.LastModifiedDate = DateTime.Now;
                                _unitOfWork.Context.Update(stuckRequest);

                                // بروزرسانی وضعیت دیسکت مرتبط
                                if (stuckRequest.InsuranceDisketteId.HasValue && stuckRequest.InsuranceDisketteId.Value > 0)
                                {
                                    var relatedDiskette = _unitOfWork.Context.InsuranceDiskettes.Find(stuckRequest.InsuranceDisketteId.Value);
                                    if (relatedDiskette != null && relatedDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Running)
                                    {
                                        relatedDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.Initial;
                                        relatedDiskette.ErrorMessage = "محاسبه قبلی متوقف شد. می‌توانید مجدداً تلاش کنید.";
                                        relatedDiskette.LastModifiedDate = DateTime.Now;
                                        _unitOfWork.Context.Update(relatedDiskette);
                                    }
                                }

                                _unitOfWork.Context.SaveChanges();
                                _logger.LogInformation("درخواست گیر کرده {RequestId} با موفقیت پاکسازی شد", stuckRequest.Id);
                            }
                            catch (Exception cleanupEx)
                            {
                                _logger.LogError(cleanupEx, "خطا در پاکسازی درخواست گیر کرده {RequestId}", stuckRequest.Id);
                                // پاک کردن تغییرات ناموفق و ادامه با درخواست بعدی
                                _unitOfWork.Context.ChangeTracker.Clear();
                            }
                        }
                    }

                    // بازگرداندن تنظیمات برای پردازش اصلی
                    _unitOfWork.Context.ChangeTracker.Clear();
                    _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
                    _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
                catch (Exception cleanupGlobalEx)
                {
                    _logger.LogError(cleanupGlobalEx, "خطا در فرآیند پاکسازی کلی درخواست‌های گیر کرده");
                    // مطمئن می‌شویم که تنظیمات به حالت اولیه برمی‌گردند
                    _unitOfWork.Context.ChangeTracker.Clear();
                    _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
                    _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }

                var validStateList = new List<long>()
                {
                    (long)Enums.BatchPayRollRequestState.Initial,
                    (long)Enums.BatchPayRollRequestState.TryAgain,
                };

                var readyToCalculateRequests = _unitOfWork.Context.BatchPayRollRequests
                    .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.InsuranceDisketteCalculation
                    && validStateList.Contains(i.RequestStateId)).ToList();

                _logger.LogInformation("تعداد {Count} درخواست دیسکت بیمه یافت شد", readyToCalculateRequests.Count);

                var validFicheStatusList = new List<long>()
            {
                (long)Enums.FicheStatus.Initial,
                (long)Enums.FicheStatus.Payed,
            };

                foreach (var BatchPayRollRequest in readyToCalculateRequests)
                {
                    var currentRequestStateId = _unitOfWork.Context.BatchPayRollRequests
                        .Where(r => r.Id == BatchPayRollRequest.Id)
                        .Select(r => r.RequestStateId)
                        .FirstOrDefault();

                    if (currentRequestStateId == (long)Enums.BatchPayRollRequestState.CancelByUser)
                    {
                        continue;
                    }

                    _logger.LogInformation($"شروع پردازش درخواست دیسکت بیمه: {BatchPayRollRequest.Id}");

                    try
                    {
                        _currentUserDefaultOrganId = BatchPayRollRequest.OrganisationChartId;
                        _organisationEmployeeTypeFicheItemService._currentUserDefaultOrganId = BatchPayRollRequest.OrganisationChartId;
                        _orderService._currentUserDefaultOrganId = BatchPayRollRequest.OrganisationChartId;
                        _logger.LogInformation("شروع پردازش درخواست دیسکت بیمه {RequestId}", BatchPayRollRequest.Id);

                        UpdateRequestAndInsuranceDisketteStateTransactional(
                            BatchPayRollRequest,
                            Enums.BatchPayRollRequestState.Running,
                            null,
                            null
                        );

                        var InsuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Include(i => i.PeymanRow).SingleOrDefault(i => i.Id == BatchPayRollRequest.InsuranceDisketteId);
                        if (InsuranceDiskette == null)
                        {
                            _logger.LogWarning("InsuranceDiskette not found for RequestId={RequestId}, InsuranceDisketteId={DisketteId}", BatchPayRollRequest.Id, BatchPayRollRequest.InsuranceDisketteId);
                            UpdateRequestAndInsuranceDisketteStateTransactional(
                                BatchPayRollRequest,
                                Enums.BatchPayRollRequestState.EndLoop,
                                null,
                                null
                            );
                            continue;
                        }
                        try
                        {
                            InsuranceDiskette.MON_PYM = "000";
                            if (InsuranceDiskette == null)
                            {
                                continue;
                            }
                            var fiches = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == InsuranceDiskette.PaymentPeriodId && validFicheStatusList.Contains(i.FicheStatusId) && i.PeymanRowId == InsuranceDiskette.PeymanRowId).ToList();

                            if (InsuranceDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Deleted)
                            {
                                UpdateRequestAndInsuranceDisketteStateTransactional(
                                    BatchPayRollRequest,
                                    Enums.BatchPayRollRequestState.Deleted,
                                    InsuranceDiskette,
                                    null
                                );
                                continue;
                            }


                            if (InsuranceDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Initial || InsuranceDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.CalculationFinished || InsuranceDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Running)
                            {
                                InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.Running;
                                InsuranceDiskette.LastModifiedDate = DateTime.Now;
                                InsuranceDiskette.DSK_TROOZ = 0;
                                InsuranceDiskette.DSK_TMAH = 0;
                                InsuranceDiskette.DSK_TMAZ = 0;
                                InsuranceDiskette.DSK_TMASH = 0;
                                InsuranceDiskette.DSK_TTOTL = 0;
                                InsuranceDiskette.DSK_BIC = 0;
                                InsuranceDiskette.DSK_TDD = 0;
                                InsuranceDiskette.DSK_TBIME = 0;
                                InsuranceDiskette.DSK_TKOSO = 0;

                                _unitOfWork.Context.Update(InsuranceDiskette);
                                SaveChangesWithAutoDetect();


                                if (fiches == null)
                                {
                                    UpdateRequestAndInsuranceDisketteStateTransactional(
                                        BatchPayRollRequest,
                                        Enums.BatchPayRollRequestState.EndLoop,
                                        InsuranceDiskette,
                                        () => { InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished; }
                                    );
                                }
                                else
                                {
                                    if (fiches.Any())
                                    {
                                        if (InsuranceDiskette.ReportTypeId == (long)Enums.InsuranceDisketteReportType.ByProjectRow)
                                        {
                                            if (InsuranceDiskette.PeymanRowId > 0)
                                            {
                                                fiches = fiches.Where(i => i.PeymanRowId == InsuranceDiskette.PeymanRowId).ToList();
                                            }
                                        }

                                        if (InsuranceDiskette.ReportTypeId == (long)Enums.InsuranceDisketteReportType.ByCostCenter)
                                        {
                                            var costCenterList = _unitOfWork.Context.InsuranceDisketteCostCenters.Where(i => i.InsuranceDisketteId == InsuranceDiskette.Id).Select(i => i.CostCenterId).Distinct().ToList();
                                            fiches = fiches.Where(i => costCenterList.Contains(i.CostCenterId)).ToList();
                                        }

                                        // فقط بروزرسانی شمارنده ها
                                        BatchPayRollRequest.EmployeeCount = fiches.Count;
                                        BatchPayRollRequest.SuccessCount = 0;
                                        BatchPayRollRequest.LastPoolingTime = DateTime.Now;
                                        _unitOfWork.Context.Update(BatchPayRollRequest);
                                        SaveChangesWithAutoDetect();

                                        InsuranceDiskette.DSK_NUM = fiches.Count;
                                        _unitOfWork.Context.Update(InsuranceDiskette);
                                        SaveChangesWithAutoDetect();
                                    }
                                    else
                                    {
                                        UpdateRequestAndInsuranceDisketteStateTransactional(
                                            BatchPayRollRequest,
                                            Enums.BatchPayRollRequestState.EndLoop,
                                            InsuranceDiskette,
                                            () => { InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished; }
                                        );

                                        BatchPayRollRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                        BatchPayRollRequest.EmployeeCount = 0;
                                        BatchPayRollRequest.LastPoolingTime = DateTime.Now;
                                        _unitOfWork.Context.Update(BatchPayRollRequest);
                                        SaveChangesWithAutoDetect();
                                        continue;
                                    }
                                }
                                var organProperty = _unitOfWork.Context.OrganProperties.Where(i => i.OrganisationChartId == InsuranceDiskette.OrganisationChartId);
                                if (organProperty == null)
                                {

                                }
                                else
                                {
                                    if (organProperty.Any())
                                    {
                                        var property = organProperty.SingleOrDefault();
                                        var currentPeriod = _unitOfWork.Context.PaymentPeriods.Find(InsuranceDiskette.PaymentPeriodId);
                                        // کد کارگاه
                                        InsuranceDiskette.DSK_ID = property.InsuranceWorkShopNo;
                                        // نام کارگاه
                                        InsuranceDiskette.DSK_NAME = property.InsuranceWorkShopName;
                                        // نام کارفرما
                                        InsuranceDiskette.DSK_FARM = property.EmployerName;
                                        // آدرس کارگاه
                                        InsuranceDiskette.DSK_ADRS = property.Address;
                                        // نوع فهرست
                                        InsuranceDiskette.DSK_KIND = 0;

                                        if (currentPeriod != null)
                                        {
                                            // سال عملکرد
                                            InsuranceDiskette.DSK_YY = Convert.ToInt32(currentPeriod.ShamsiYear.ToString().Substring(2, 2));
                                            // ماه عملکرد
                                            InsuranceDiskette.DSK_MM = currentPeriod.ShamsiMonth;
                                        }

                                        if (InsuranceDiskette.PeymanRow != null)
                                        {
                                            // ردیف پیمان
                                            InsuranceDiskette.MON_PYM = InsuranceDiskette.PeymanRow.Code;
                                        }

                                        //شرح فهرست
                                        InsuranceDiskette.DSK_DISC = InsuranceDiskette.title;
                                        // نرخ حق بیمه
                                        if (property.InsuranceRate.HasValue)
                                        {
                                            InsuranceDiskette.DSK_RATE = property.InsuranceRate.Value;
                                        }
                                        _unitOfWork.Context.Update(InsuranceDiskette);
                                        SaveChangesWithAutoDetect();
                                    }
                                }
                                foreach (var fiche in fiches)
                                {
                                    try
                                    {
                                        DateTime now = DateTime.Now;
                                        BatchPayRollRequest.PoolingEmployeeId = fiche.EmployeeId;
                                        _unitOfWork.Context.Update(BatchPayRollRequest);
                                        SaveChangesWithAutoDetect();
                                        var interdict = _orderService._unitOfWork.Context.InterdictOrders.Include(i => i.RecruitOrder).SingleOrDefault(i => i.Id == fiche.InterdictOrderId);
                                        if (interdict == null)
                                        {
                                            _logger.LogWarning("Interdict order not found for FicheId={FicheId}, InterdictOrderId={InterdictOrderId}", fiche.Id, fiche.InterdictOrderId);
                                            throw new InvalidOperationException("حکم کارگزینی مرتبط با فیش یافت نشد");
                                        }
                                        //    var PersonnelProperties = _unitOfWork.Context.PersonnelProperties.Where(i => i.EmployeeId == fiche.EmployeeId);

                                        string InsuranceNo = string.Empty;

                                        #region خواندن اطلاعات بیمه از پرونده
                                        var insuranceRecors = _insuranceService.All().Where(i => i.EmployeeId == fiche.EmployeeId);
                                        if (insuranceRecors == null)
                                        {

                                        }
                                        else
                                        {
                                            if (insuranceRecors.Any())
                                            {
                                                if (insuranceRecors.Any(i => i.IsLast == true))
                                                {
                                                    if (insuranceRecors.Count(i => i.IsLast == true) == 1)
                                                    {
                                                        InsuranceNo = insuranceRecors.Single(i => i.IsLast == true).InsuranceNumber;
                                                    }
                                                }
                                            }
                                        }
                                        #endregion خواندن اطلاعات بیمه از پرونده

                                        InsuranceDisketteItem item = null;

                                        var DisketteItem = _unitOfWork.Context.InsuranceDisketteItems.Where(i => i.FicheId == fiche.Id && i.InsuranceDisketteId == InsuranceDiskette.Id);

                                        if (DisketteItem == null)
                                        {
                                            item = new InsuranceDisketteItem()
                                            {
                                                InsuranceDisketteId = InsuranceDiskette.Id,
                                                CreateDate = DateTime.Now,
                                                IPAddress = "",
                                                EmployeeId = fiche.EmployeeId,
                                                FicheId = fiche.Id,
                                                PersonnelFunctionId = fiche.PersonnelFunctionId,
                                                InterdictOrderId = fiche.InterdictOrderId,
                                                CostCenterId = fiche.CostCenterId,
                                                DailyPayment = fiche.DailyFunctionAmount,
                                                InsuranceNo = InsuranceNo
                                            };
                                            _unitOfWork.Context.InsuranceDisketteItems.Add(item);
                                        }
                                        else
                                        {
                                            if (DisketteItem.Any())
                                            {
                                                item = DisketteItem.Single();
                                                item.LastModifiedDate = DateTime.Now;
                                                _unitOfWork.Context.InsuranceDisketteItems.Update(item);
                                            }
                                            else
                                            {
                                                item = new InsuranceDisketteItem()
                                                {
                                                    InsuranceDisketteId = InsuranceDiskette.Id,
                                                    CreateDate = DateTime.Now,
                                                    IPAddress = "",
                                                    EmployeeId = fiche.EmployeeId,
                                                    FicheId = fiche.Id,
                                                    PersonnelFunctionId = fiche.PersonnelFunctionId,
                                                    InterdictOrderId = fiche.InterdictOrderId,
                                                    CostCenterId = fiche.CostCenterId,
                                                    DailyPayment = fiche.DailyFunctionAmount,
                                                    InsuranceNo = InsuranceNo
                                                };
                                                _unitOfWork.Context.InsuranceDisketteItems.Add(item);
                                            }
                                        }

                                        SaveChangesWithAutoDetect();

                                        BatchPayRollRequestDetail detail = null;

                                        var details = _unitOfWork.Context.BatchPayRollRequestDetails.Where(i => i.FicheId == fiche.Id && i.BatchPayRollRequestId == BatchPayRollRequest.Id);

                                        if (details == null)
                                        {
                                            detail = new BatchPayRollRequestDetail()
                                            {
                                                BatchPayRollRequestId = BatchPayRollRequest.Id,
                                                CreateDate = DateTime.Now,
                                                IPAddress = "",
                                                FicheId = fiche.Id,
                                                EmployeeId = fiche.EmployeeId,
                                                InsuranceDisketteItemId = item.Id
                                            };
                                            _unitOfWork.Context.BatchPayRollRequestDetails.Add(detail);
                                        }
                                        else
                                        {
                                            if (details.Any())
                                            {
                                                detail = details.Single();
                                                detail.LastModifiedDate = DateTime.Now;
                                                _unitOfWork.Context.BatchPayRollRequestDetails.Update(detail);
                                            }
                                            else
                                            {
                                                detail = new BatchPayRollRequestDetail()
                                                {
                                                    BatchPayRollRequestId = BatchPayRollRequest.Id,
                                                    CreateDate = DateTime.Now,
                                                    IPAddress = "",
                                                    FicheId = fiche.Id,
                                                    EmployeeId = fiche.EmployeeId,
                                                    InsuranceDisketteItemId = item.Id
                                                };
                                                _unitOfWork.Context.BatchPayRollRequestDetails.Add(detail);
                                            }
                                        }

                                        SaveChangesWithAutoDetect();

                                        try
                                        {
                                            var function = _unitOfWork.Context.PersonnelFunctions.Find(item.PersonnelFunctionId);
                                            var ficheIncludedar = _unitOfWork.Context.Fiches.Include(i => i.Employee)
                                                .Include(i => i.Employee.IssuePlace)
                                                .Include(i => i.Employee.Nationality)
                                                .SingleOrDefault(i => i.Id == fiche.Id);
                                            if (ficheIncludedar == null)
                                            {
                                                _logger.LogWarning("Fiche record not found while calculating diskette item. FicheId={FicheId}", fiche.Id);
                                                throw new InvalidOperationException("فیش حقوقی یافت نشد");
                                            }

                                            item.DailyPayment = 0;
                                            item.MonthPayment = 0;
                                            item.PaymentInsuranceCoveredNotInMonthPayment = 0;
                                            item.PaymentInsuranceCovered = 0;
                                            item.TotalInsurancePayment = 0;
                                            item.PersonnelInsuranceAmount = 0;
                                            item.EmployerInsuranceAmount = 0;
                                            item.UnEmployedInsuranceAmount = 0;


                                            // تعداد روز های کار کرد
                                            var personnelFunctionDays = (function != null && function.PersonnelFunctionDay.HasValue) ? function.PersonnelFunctionDay.Value : 0;
                                            if (personnelFunctionDays <= 0)
                                            {
                                                _logger.LogWarning("PersonnelFunctionDay is null/zero. FicheId={FicheId}, PersonnelFunctionId={PersonnelFunctionId}", fiche.Id, item.PersonnelFunctionId);
                                            }
                                            item.DSW_DD = personnelFunctionDays;
                                            InsuranceDiskette.DSK_TDD = InsuranceDiskette.DSK_TDD + item.DSW_DD;

                                            var DastmozdSetting = _organisationCostCenterService._unitOfWork.Context.OrganisationEmployeeTypeWageItems.Where(i => i.IsDeleted != true && i.OrganisationChartId == interdict.RecruitOrder.PayLocationId && i.EmployeeTypeId == interdict.RecruitOrder.EmployeeTypeId && i.IsDailyAndWage == true);


                                            if (DastmozdSetting != null)
                                            {
                                                if (DastmozdSetting.Any())
                                                {
                                                    if (DastmozdSetting.Count() == 1)
                                                    {
                                                        var singleSetting = DastmozdSetting.SingleOrDefault();

                                                        var relatedOrderWageItem = _orderService._unitOfWork.Context.InterdictOrderWageItems.Where(i => i.InterdictOrderId == interdict.Id && i.WageItemId == singleSetting.WageItemId);

                                                        if (relatedOrderWageItem != null)
                                                        {
                                                            if (relatedOrderWageItem.Any())
                                                            {
                                                                if (relatedOrderWageItem.Count() == 1)
                                                                {

                                                                    if (singleSetting.IsDaily)
                                                                    {
                                                                        // آیتم دستمزد خالص از حکم خوانده می شود
                                                                        item.DSW_ROOZ = relatedOrderWageItem.SingleOrDefault().Value;
                                                                        InsuranceDiskette.DSK_TROOZ = InsuranceDiskette.DSK_TROOZ + item.DSW_ROOZ;
                                                                    }
                                                                    else
                                                                    {

                                                                        // آیتم دستمزد خالص از حکم خوانده می شود
                                                                        item.DSW_ROOZ = relatedOrderWageItem.SingleOrDefault().Value / 30;
                                                                        InsuranceDiskette.DSK_TROOZ = InsuranceDiskette.DSK_TROOZ + item.DSW_ROOZ;

                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            var SanavatSetting = _organisationCostCenterService._unitOfWork.Context.OrganisationEmployeeTypeWageItems.Where(i => i.IsDeleted != true && i.OrganisationChartId == interdict.RecruitOrder.PayLocationId && i.EmployeeTypeId == interdict.RecruitOrder.EmployeeTypeId && i.IsSanavatINC == true);
                                            if (SanavatSetting != null)
                                            {
                                                if (SanavatSetting.Any())
                                                {
                                                    if (SanavatSetting.Count() == 1)
                                                    {
                                                        var singleSetting = SanavatSetting.SingleOrDefault();
                                                        if (singleSetting.IsDaily)
                                                        {
                                                            // Sanavat
                                                            item.DSW_INC = fiche.IncAmount * personnelFunctionDays;
                                                            InsuranceDiskette.DSK_INC = InsuranceDiskette.DSK_INC + item.DSW_INC;
                                                        }
                                                        else
                                                        {
                                                            // var Period = _unitOfWork.Context.PaymentPeriods.Find(InsuranceDiskette.PaymentPeriodId);

                                                            // Sanavat
                                                            item.DSW_INC = fiche.IncAmount / 30 * personnelFunctionDays;
                                                            InsuranceDiskette.DSK_INC = InsuranceDiskette.DSK_INC + item.DSW_INC;
                                                        }
                                                    }
                                                }
                                            }


                                            // Aele
                                            item.DSW_SPOUSE = fiche.SpouseAmount;
                                            InsuranceDiskette.DSK_SPOUSE = InsuranceDiskette.DSK_SPOUSE + item.DSW_SPOUSE;

                                            //  دستمزد ماهانه
                                            item.DSW_MAH = (item.DSW_ROOZ * personnelFunctionDays) + item.DSW_INC;
                                            InsuranceDiskette.DSK_TMAH = InsuranceDiskette.DSK_TMAH + item.DSW_MAH;

                                            //مزایای ماهانه مشمول
                                            item.DSW_MAZ = fiche.PaymentInsuranceCovered;
                                            InsuranceDiskette.DSK_TMAZ = InsuranceDiskette.DSK_TMAZ + item.DSW_MAZ;

                                            // جمع دستمزد و مزایای ماهانه مشمول
                                            item.DSW_MASH = item.DSW_MAH + item.DSW_MAZ;
                                            InsuranceDiskette.DSK_TMASH = InsuranceDiskette.DSK_TMASH + item.DSW_MASH;

                                            //جمع کل دستمزد و مزایای ماهانه
                                            item.DSW_TOTL = item.DSW_MASH;// fiche.InsuranceTotal_DSW;
                                            InsuranceDiskette.DSK_TTOTL = InsuranceDiskette.DSK_TTOTL + item.DSW_TOTL;

                                            #region ItemSetting


                                            var executAbleSetting = _organisationEmployeeTypeFicheItemService.GetCurrentOrganItemsByEmployeeType(fiche.OrganisationChartId, fiche.EmployeeTypeId);


                                            var ficheItems = _unitOfWork.Context.FicheItems.Where(i => i.FicheId == item.FicheId).ToList();

                                            if (ficheItems == null)
                                            {

                                            }
                                            else
                                            {
                                                if (ficheItems.Any())
                                                {
                                                    var taminInsuranceEmployerItemId = executAbleSetting.Where(i => i.IsTaminInsurance == true && i.IsEmployerItem == true);

                                                    if (taminInsuranceEmployerItemId == null)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (taminInsuranceEmployerItemId.Any())
                                                        {
                                                            var taminInsuranceEmployerItem = ficheItems.Where(i => i.WageItemId == taminInsuranceEmployerItemId.Single().WageItemId);

                                                            if (taminInsuranceEmployerItem == null)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                if (taminInsuranceEmployerItem.Any())
                                                                {
                                                                    var InsuranceEmployerItem = taminInsuranceEmployerItem.Single();
                                                                    // مجموع حق بیمه سهم کارفرما
                                                                    // ایق آیتم فقط سر جمع دارد
                                                                    InsuranceDiskette.DSK_TKOSO = InsuranceDiskette.DSK_TKOSO + Convert.ToInt64(InsuranceEmployerItem.Value);
                                                                    //مجموع حق بیمه بیکاری
                                                                    InsuranceDiskette.DSK_BIC = InsuranceDiskette.DSK_BIC + Convert.ToInt64((InsuranceEmployerItem.Value * (0.03)));

                                                                }
                                                            }
                                                        }

                                                    }

                                                    ///////////////////

                                                    var taminInsuranceEmployeeItemId = executAbleSetting.Where(i => i.IsTaminInsurance == true && i.IsEmployerItem != true);
                                                    if (taminInsuranceEmployeeItemId == null)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (taminInsuranceEmployeeItemId.Any())
                                                        {
                                                            var taminInsuranceEmployeeItem = ficheItems.Where(i => i.WageItemId == taminInsuranceEmployeeItemId.Single().WageItemId);

                                                            if (taminInsuranceEmployeeItem == null)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                if (taminInsuranceEmployeeItem.Any())
                                                                {
                                                                    var InsuranceEmployeeItem = taminInsuranceEmployeeItem.Single();
                                                                    // مجموع حق بیمه سهم بیمه شده
                                                                    item.DSW_BIME = Convert.ToInt64(InsuranceEmployeeItem.Value);
                                                                    InsuranceDiskette.DSK_TBIME = InsuranceDiskette.DSK_TBIME + Convert.ToInt64(InsuranceEmployeeItem.Value);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }


                                            #endregion ItemSetting


                                            #region تاریخ استخدام و خاتمه همکاری
                                            List<GetLightOrderListForInsuranceDiskette_Result> ret = new();
                                            using (SqlConnection con = new SqlConnection(_connectionString))
                                            {
                                                SqlCommand cmd = new SqlCommand("[Order].[GetLightOrderListForInsuranceDiskette]", con);
                                                cmd.Parameters.AddWithValue("@OrganisationChartId", _currentUserDefaultOrganId);
                                                cmd.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);

                                                cmd.CommandType = CommandType.StoredProcedure;
                                                con.Open();
                                                SqlDataReader rdr = cmd.ExecuteReader();

                                                while (rdr.Read())
                                                {
                                                    ret.Add(rdr.ConvertToObject<GetLightOrderListForInsuranceDiskette_Result>());
                                                }
                                                con.Close();
                                            }
                                            var employee = _employeeService.GetIdAsync(fiche.EmployeeId).Result;
                                            var firstOrder = ret.OrderBy(i => i.OrderSerial).FirstOrDefault();
                                            /// تاریخ شروع به کار
                                            if (firstOrder == null && (employee.StartWorkDate == null || employee.StartWorkDate == DateTime.MinValue))
                                            {
                                                _logger.LogWarning("No recruit/start order found and employee has no StartWorkDate. EmployeeId={EmployeeId}", fiche.EmployeeId);
                                            }
                                            if (employee.StartWorkDate == null || employee.StartWorkDate == DateTime.MinValue)
                                            {
                                                if (firstOrder != null && firstOrder.StartDate.HasValue)
                                                {
                                                    item.DSW_SDATE = GetNumericalDate(firstOrder.StartDate.Value);
                                                }
                                            }
                                            else
                                            {
                                                item.DSW_SDATE = GetNumericalDate(employee.StartWorkDate.Value);
                                            }
                                            var lastOrder = ret.Where(i => i.OrderStatusId == 9).FirstOrDefault();
                                            if (lastOrder != null && lastOrder.IsEmployed == false && lastOrder.OrderDirectionTypeId == 1163) //  حکم خاتمه دهنده
                                            {
                                                if (lastOrder.StartDate.HasValue)
                                                {
                                                    // تاریخ ترک کار
                                                    item.DSW_EDATE = GetNumericalDate(lastOrder.StartDate.Value);
                                                }

                                            }

                                            #endregion   تاریخ استخدام و خاتمه همکاری
                                            //نرخ پورسانتاژ
                                            item.DSW_PRATE = 0;
                                            InsuranceDiskette.DSK_PRATE = 0;
                                            // کد کارگاه
                                            item.DSW_ID = InsuranceDiskette.DSK_ID;
                                            // سال عملکرد
                                            item.DSW_YY = InsuranceDiskette.DSK_YY;
                                            // ماه عملکرد
                                            item.DSW_MM = InsuranceDiskette.DSK_MM;
                                            // شماره فهرست
                                            item.DSW_LISTNO = InsuranceDiskette.DSK_LISTNO;
                                            // شماره بیمه
                                            item.DSW_ID1 = InsuranceNo;
                                            // نام

                                            if (string.IsNullOrEmpty(interdict.FirstName))
                                            {
                                                item.DSW_FNAME = ficheIncludedar.Employee.FirstName;
                                            }
                                            else
                                            {
                                                item.DSW_FNAME = interdict.FirstName;
                                            }

                                            // نام خانوادگی
                                            if (string.IsNullOrEmpty(interdict.LastName))
                                            {
                                                if (ficheIncludedar.Employee != null)
                                                {
                                                    item.DSW_LNAME = ficheIncludedar.Employee.LastName;
                                                }
                                            }
                                            else
                                            {
                                                item.DSW_LNAME = interdict.LastName;
                                            }

                                            // نام پدر

                                            if (string.IsNullOrEmpty(interdict.FatherName))
                                            {
                                                if (ficheIncludedar.Employee != null)
                                                {
                                                    item.DSW_DNAME = ficheIncludedar.Employee.FatherName;
                                                }
                                            }
                                            else
                                            {
                                                item.DSW_DNAME = interdict.FatherName;
                                            }

                                            // شماره شناسنامه

                                            if (string.IsNullOrEmpty(interdict.IdentityNo))
                                            {
                                                if (ficheIncludedar.Employee != null)
                                                {
                                                    item.DSW_IDNO = ficheIncludedar.Employee.IdentityNo;
                                                }
                                            }
                                            else
                                            {
                                                item.DSW_IDNO = interdict.IdentityNo;
                                            }

                                            if (ficheIncludedar.Employee != null)
                                            {
                                                if (ficheIncludedar.Employee.IssuePlace != null)
                                                {
                                                    // محل صدور
                                                    item.DSW_IDPLC = ficheIncludedar.Employee.IssuePlace.title;
                                                }
                                            }

                                            if (ficheIncludedar.Employee != null)
                                            {
                                                if (ficheIncludedar.Employee.Nationality != null)
                                                {
                                                    // ملیت
                                                    item.DSW_NAT = ficheIncludedar.Employee.Nationality.title;
                                                }
                                            }

                                            if (ficheIncludedar.Employee != null)
                                            {
                                                if (ficheIncludedar.Employee.IssueDate != null)
                                                {
                                                    // تاریخ صدور
                                                    item.DSW_IDATE = GetNumericalDate(ficheIncludedar.Employee.IssueDate.Value);
                                                }
                                            }
                                            if (ficheIncludedar.Employee != null)
                                            {
                                                if (ficheIncludedar.Employee.BirthDate != null)
                                                {
                                                    // تاریخ صدور
                                                    item.DSW_BDATE = GetNumericalDate(ficheIncludedar.Employee.BirthDate.Value);
                                                }
                                            }


                                            if (string.IsNullOrEmpty(interdict.NationalNo))
                                            {
                                                item.PER_NATCOD = interdict.NationalNo;
                                            }
                                            else
                                            {
                                                if (ficheIncludedar.Employee != null)
                                                {
                                                    item.PER_NATCOD = ficheIncludedar.Employee.NationalNo;
                                                }
                                            }
                                            if (ficheIncludedar.Employee != null)
                                            {
                                                // جنسیت
                                                if (ficheIncludedar.Employee.GenderId == 2)
                                                {
                                                    item.DSW_SEX = "مرد";
                                                }
                                                if (ficheIncludedar.Employee.GenderId == 3)
                                                {
                                                    item.DSW_SEX = "زن";
                                                }
                                            }



                                            if (interdict.RecruitOrder.OrganizationJobId > 0)
                                            {
                                                var job = _organizationJobService._unitOfWork.Context.OrganizationJobs.Include(i => i.TaminInsuranceJobList)
                                              .Single(i => i.Id == interdict.RecruitOrder.OrganizationJobId);

                                                if (job.TaminInsuranceJobListId > 0)
                                                {
                                                    if (job.TaminInsuranceJobList != null)
                                                    {
                                                        // کد شغل فهرست بیمه تامین
                                                        item.DSW_JOB = job.TaminInsuranceJobList.Code;
                                                    }
                                                }
                                                if (job.TaminInsuranceJobListId > 0)
                                                {
                                                    if (job.TaminInsuranceJobList != null)
                                                    {
                                                        // کد شغل فهرست بیمه تامین
                                                        item.DSW_OCP = job.TaminInsuranceJobList.title;
                                                    }
                                                }
                                            }

                                            if (ficheIncludedar.Employee.TaminInsuranceJobListId > 0)
                                            {
                                                var taminRecord = _baseTableService._unitOfWork.Context.TaminInsuranceJobLists.Find(ficheIncludedar.Employee.TaminInsuranceJobListId);

                                                if (taminRecord != null)
                                                {
                                                    // کد شغل فهرست بیمه تامین
                                                    item.DSW_JOB = taminRecord.Code;
                                                    // کد شغل فهرست بیمه تامین
                                                    item.DSW_OCP = taminRecord.title;
                                                }
                                            }


                                            BatchPayRollRequest.PoolingEmployeeId = item.EmployeeId;
                                            BatchPayRollRequest.LastPoolingTime = DateTime.Now;
                                            _unitOfWork.Context.Update(BatchPayRollRequest);
                                            SaveChangesWithAutoDetect();


                                            item.LastModifiedDate = DateTime.Now;
                                            _unitOfWork.Context.Update(item);
                                            SaveChangesWithDetailedLogging("Update InsuranceDisketteItem after calculation", item);

                                            detail.DoDatetime = DateTime.Now;
                                            detail.FinalMessage = "لحاظ شده در دیسکت";
                                            detail.RunTimeinMilliseconds = (DateTime.Now - now).TotalMilliseconds;
                                            _unitOfWork.Context.BatchPayRollRequestDetails.Update(detail);
                                            SaveChangesWithDetailedLogging("Mark BatchPayRollRequestDetail as succeeded", detail);

                                            BatchPayRollRequest.SuccessCount = BatchPayRollRequest.SuccessCount + 1;
                                            BatchPayRollRequest.LastModifiedDate = DateTime.Now;
                                            _unitOfWork.Context.Update(BatchPayRollRequest);
                                            SaveChangesWithDetailedLogging("Increment SuccessCount for request", BatchPayRollRequest);
                                        }
                                        catch (Exception ex)
                                        {
                                            try
                                            {
                                                Console.WriteLine(ex.Message);
                                                var relatedRows = _unitOfWork.Context.BatchPayRollRequestDetails.Where(i => i.FicheId == item.FicheId && i.BatchPayRollRequestId == BatchPayRollRequest.Id);

                                                if (relatedRows == null)
                                                {

                                                }
                                                else
                                                {
                                                    if (relatedRows.Any())
                                                    {
                                                        var relatedRow = relatedRows.Single();
                                                        relatedRow.DoDatetime = DateTime.Now;
                                                        relatedRow.FinalMessage = ex.Message;
                                                        _unitOfWork.Context.BatchPayRollRequestDetails.Update(relatedRow);
                                                    }
                                                }


                                                SaveChangesWithDetailedLogging("Persist error message on BatchPayRollRequestDetail", null);
                                            }
                                            catch (Exception exx)
                                            {
                                                _logger.LogError(exx, "خطا در ذخیره پیام خطای فیش {FicheId}", item.FicheId);
                                            }

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "خطا در پردازش فیش {FicheId} در درخواست {RequestId}", fiche.Id, BatchPayRollRequest.Id);
                                    }

                                }


                                SaveChangesWithAutoDetect();

                                BatchPayRollRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                BatchPayRollRequest.FinishDateTime = DateTime.Now;
                                _unitOfWork.Context.Update(BatchPayRollRequest);
                                SaveChangesWithAutoDetect();

                                InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished;
                                InsuranceDiskette.LastModifiedDate = DateTime.Now;
                                _unitOfWork.Context.Update(InsuranceDiskette);
                                SaveChangesWithAutoDetect();

                            }
                            else
                            {
                                BatchPayRollRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                BatchPayRollRequest.FinishDateTime = DateTime.Now;
                                _unitOfWork.Context.Update(BatchPayRollRequest);
                                SaveChangesWithAutoDetect();



                                InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished;
                                InsuranceDiskette.LastModifiedDate = DateTime.Now;
                                _unitOfWork.Context.Update(InsuranceDiskette);
                                SaveChangesWithAutoDetect();
                            }
                            #region FillDBF

                            string CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                            var filePath = CurrentDirectory + "\\Insurance";

                            var dskkar00MainFile = filePath + "\\" + "DSKKAR00.DBF";
                            var dskwor00MainFile = filePath + "\\" + "DSKWOR00.DBF";

                            var filePathTemp = CurrentDirectory + "\\InsuranceTemp";
                            var dskkar00DbFileName = filePathTemp + "\\" + "DSKKAR00.DBF";
                            var dskwor00DbFileName = filePathTemp + "\\" + "DSKWOR00.DBF";
                            if (System.IO.File.Exists(dskkar00DbFileName))
                            {
                                System.IO.File.Delete(dskkar00DbFileName);
                            }

                            if (System.IO.File.Exists(dskwor00DbFileName))
                            {
                                System.IO.File.Delete(dskwor00DbFileName);
                            }
                            System.IO.File.Copy(dskkar00MainFile, dskkar00DbFileName);
                            System.IO.File.Copy(dskwor00MainFile, dskwor00DbFileName);

                            string dskKarConnectionString = "Provider=VFPOLEDB.1;Data Source=" + dskkar00DbFileName;
                            string dskWorConnectionString = "Provider=VFPOLEDB.1;Data Source=" + dskwor00DbFileName;

                            #region WorDisk (DotNetDBF)

                            try
                            {
                                WriteDskWorDbf(dskwor00DbFileName, InsuranceDiskette.Id);
                            }
                            catch (Exception ex)
                            {
                                //  throw new Exception("خطا در نوشتن فایل DSKWOR00: " + ex.Message, ex);
                            }

                            #endregion WorDisk (DotNetDBF)
                            #region DskKar (DotNetDBF)

                            try
                            {
                                WriteDskKarDbf(dskkar00DbFileName, InsuranceDiskette);
                            }
                            catch (Exception ex)
                            {
                                //   throw new Exception("خطا در نوشتن فایل DSKKAR00: " + ex.Message, ex);
                            }

                            #endregion

                            var DSKKAR00 = GetBinaryFile(dskkar00DbFileName);

                            InsuranceDisketteFile DSKKAR00File = new InsuranceDisketteFile()
                            {
                                InsuranceDisketteId = InsuranceDiskette.Id,
                                DiskContent = DSKKAR00,
                                FileTypeId = (long)Enums.InsuranceFileDiskType.DSKKAR00,
                                Extension = "DBF",
                                FileName = "DSKKAR00",
                                IPAddress = "Console",
                                CreateDate = DateTime.Now,
                                title = "فایل ساخته شده توسط سرویس گروهی پس زمینه",
                            };
                            _unitOfWork.Context.InsuranceDisketteFiles.Add(DSKKAR00File);
                            _unitOfWork.Context.SaveChanges();

                            var DSKWOR00 = GetBinaryFile(dskwor00DbFileName);
                            InsuranceDisketteFile DSKWOR00File = new InsuranceDisketteFile()
                            {
                                InsuranceDisketteId = InsuranceDiskette.Id,
                                DiskContent = DSKWOR00,
                                FileTypeId = (long)Enums.InsuranceFileDiskType.DSKWOR00,
                                Extension = "DBF",
                                FileName = "DSKWOR00",
                                IPAddress = "Console",
                                CreateDate = DateTime.Now,
                                title = "فایل ساخته شده توسط سرویس گروهی پس زمینه",
                            };
                            _unitOfWork.Context.InsuranceDisketteFiles.Add(DSKWOR00File);
                            _unitOfWork.Context.SaveChanges();
                            #endregion FillDBF



                            UpdateRequestAndInsuranceDisketteStateTransactional(
                                BatchPayRollRequest,
                                Enums.BatchPayRollRequestState.EndLoop,
                                InsuranceDiskette,
                                () => { InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished; }
                            );
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "خطا در پردازش داخلی دیسکت بیمه {DisketteId} برای درخواست {RequestId}", InsuranceDiskette?.Id, BatchPayRollRequest.Id);

                            try
                            {
                                UpdateRequestAndInsuranceDisketteStateTransactional(
                                    BatchPayRollRequest,
                                    Enums.BatchPayRollRequestState.EndLoop,
                                    InsuranceDiskette,
                                    () =>
                                    {
                                        InsuranceDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished;
                                        InsuranceDiskette.ErrorMessage = ex.Message;
                                    }
                                );
                                _logger.LogInformation("وضعیت خطا برای دیسکت {DisketteId} با موفقیت ذخیره شد", InsuranceDiskette?.Id);
                            }
                            catch (Exception saveEx)
                            {
                                _logger.LogError(saveEx, "خطا در ذخیره وضعیت خطا برای دیسکت {DisketteId}", InsuranceDiskette?.Id);

                                // fallback: تلاش با context پاک شده
                                try
                                {
                                    _unitOfWork.Context.ChangeTracker.Clear();

                                    var requestToUpdate = _unitOfWork.Context.BatchPayRollRequests.Find(BatchPayRollRequest.Id);
                                    if (requestToUpdate != null)
                                    {
                                        requestToUpdate.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                        requestToUpdate.ExeptionMessage = $"خطا در محاسبه: {ex.Message}";
                                        requestToUpdate.FinishDateTime = DateTime.Now;
                                        requestToUpdate.LastModifiedDate = DateTime.Now;
                                        _unitOfWork.Context.Update(requestToUpdate);
                                        _unitOfWork.Context.SaveChanges();
                                    }

                                    if (InsuranceDiskette != null)
                                    {
                                        var disketteToUpdate = _unitOfWork.Context.InsuranceDiskettes.Find(InsuranceDiskette.Id);
                                        if (disketteToUpdate != null)
                                        {
                                            disketteToUpdate.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished;
                                            disketteToUpdate.ErrorMessage = $"خطا: {ex.Message}";
                                            disketteToUpdate.LastModifiedDate = DateTime.Now;
                                            _unitOfWork.Context.Update(disketteToUpdate);
                                            _unitOfWork.Context.SaveChanges();
                                        }
                                    }

                                    _logger.LogInformation("وضعیت با fallback برای درخواست {RequestId} ذخیره شد", BatchPayRollRequest.Id);
                                }
                                catch (Exception fallbackEx)
                                {
                                    _logger.LogCritical(fallbackEx, "خطای Critical: نتوانستیم وضعیت درخواست {RequestId} / دیسکت {DisketteId} را ذخیره کنیم",
                                        BatchPayRollRequest.Id, InsuranceDiskette?.Id);
                                }
                            }
                        }

                        _logger.LogInformation("پایان موفق پردازش درخواست دیسکت بیمه {RequestId}", BatchPayRollRequest.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در پردازش درخواست دیسکت بیمه {RequestId}", BatchPayRollRequest.Id);

                        // تلاش برای ذخیره وضعیت خطا با استفاده از transaction
                        bool stateSaved = false;
                        try
                        {
                            UpdateRequestAndInsuranceDisketteStateTransactional(
                                BatchPayRollRequest,
                                Enums.BatchPayRollRequestState.EndLoop,
                                null,
                                () => { BatchPayRollRequest.ExeptionMessage = ex.Message; }
                            );
                            stateSaved = true;
                            _logger.LogInformation("وضعیت خطای درخواست {RequestId} با موفقیت ذخیره شد", BatchPayRollRequest.Id);
                        }
                        catch (Exception saveEx)
                        {
                            _logger.LogError(saveEx, "خطا در ذخیره وضعیت خطای درخواست {RequestId} با transaction", BatchPayRollRequest.Id);
                        }

                        // اگر transaction موفق نشد، با یک context جدید و ساده تلاش کنیم
                        if (!stateSaved)
                        {
                            try
                            {
                                _logger.LogWarning("تلاش برای ذخیره وضعیت خطا با context جدید برای درخواست {RequestId}", BatchPayRollRequest.Id);

                                // پاک‌سازی تغییرات معلق
                                _unitOfWork.Context.ChangeTracker.Clear();

                                // خواندن مجدد درخواست از دیتابیس
                                var requestToUpdate = _unitOfWork.Context.BatchPayRollRequests.Find(BatchPayRollRequest.Id);
                                if (requestToUpdate != null)
                                {
                                    requestToUpdate.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                                    requestToUpdate.ExeptionMessage = $"خطا در پردازش: {ex.Message}";
                                    requestToUpdate.FinishDateTime = DateTime.Now;
                                    requestToUpdate.LastModifiedDate = DateTime.Now;
                                    requestToUpdate.LastPoolingTime = DateTime.Now;

                                    _unitOfWork.Context.Update(requestToUpdate);
                                    _unitOfWork.Context.SaveChanges();

                                    _logger.LogInformation("وضعیت خطای درخواست {RequestId} با context جدید ذخیره شد", BatchPayRollRequest.Id);
                                    stateSaved = true;
                                }

                                // اگر InsuranceDiskette موجود است، وضعیت آن را هم به‌روز کنیم
                                if (BatchPayRollRequest.InsuranceDisketteId.HasValue && BatchPayRollRequest.InsuranceDisketteId.Value > 0)
                                {
                                    var disketteToUpdate = _unitOfWork.Context.InsuranceDiskettes.Find(BatchPayRollRequest.InsuranceDisketteId.Value);
                                    if (disketteToUpdate != null && disketteToUpdate.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Running)
                                    {
                                        disketteToUpdate.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.CalculationFinished;
                                        disketteToUpdate.ErrorMessage = $"خطا در محاسبه: {ex.Message}";
                                        disketteToUpdate.LastModifiedDate = DateTime.Now;
                                        _unitOfWork.Context.Update(disketteToUpdate);
                                        _unitOfWork.Context.SaveChanges();
                                        _logger.LogInformation("وضعیت دیسکت بیمه {DisketteId} با context جدید ذخیره شد", disketteToUpdate.Id);
                                    }
                                }
                            }
                            catch (Exception fallbackEx)
                            {
                                _logger.LogError(fallbackEx, "خطای نهایی در ذخیره وضعیت برای درخواست {RequestId}. درخواست ممکن است در وضعیت Running باقی بماند!", BatchPayRollRequest.Id);
                            }
                        }

                        if (!stateSaved)
                        {
                            _logger.LogCritical("CRITICAL: نتوانستیم وضعیت درخواست {RequestId} را تغییر دهیم. نیاز به بررسی دستی دارد!", BatchPayRollRequest.Id);
                        }
                    }
                }
                _logger.LogInformation("پایان CalculateInsuranceDisketteBatch");
            }
            finally
            {
                // بازگرداندن تنظیمات ChangeTracker به حالت اولیه
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
                _logger.LogInformation("تنظیمات ChangeTracker به حالت اولیه بازگشت: AutoDetectChanges={AutoDetect}, QueryTracking={QueryTracking}",
                    originalAutoDetectChanges, originalQueryTracking);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در CalculateInsuranceDisketteBatch");
        }
    }
    // همگام سازی تراکنشی وضعیت درخواست و دیسکت بیمه
    private void UpdateRequestAndInsuranceDisketteStateTransactional(BatchPayRollRequest request, Enums.BatchPayRollRequestState newState, InsuranceDiskette? insuranceDiskette, Action? mutateDisketteBeforeSave)
    {
        _unitOfWork.CreateTransaction();
        try
        {
            request.RequestStateId = (long)newState;
            request.LastPoolingTime = DateTime.Now;
            request.LastModifiedDate = DateTime.Now;
            if (newState == Enums.BatchPayRollRequestState.EndLoop)
            {
                request.FinishDateTime = DateTime.Now;
            }
            _unitOfWork.Context.Update(request);

            if (insuranceDiskette != null)
            {
                mutateDisketteBeforeSave?.Invoke();
                insuranceDiskette.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Update(insuranceDiskette);
            }

            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
    public OperationResult downloadDSKKAR00Disk(long BatchPayRollRequestId)
    {
        var InsuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Where(i => i.BatchPayRollRequestId == BatchPayRollRequestId).Single();
        var diskList = _unitOfWork.Context.InsuranceDisketteFiles.Where(i => i.InsuranceDisketteId == InsuranceDiskette.Id && i.FileTypeId == (long)Enums.InsuranceFileDiskType.DSKKAR00);
        if (diskList == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (diskList.Any())
            {

                var last = diskList.OrderBy(i => i.Id).Last();

                last.Content = Convert.ToBase64String(last.DiskContent);
                last.DiskContent = null;
                last.MimeType = System.Net.Mime.MediaTypeNames.Application.Octet;
                return OperationResult.Succeeded(payload: last);
            }
            else
            {
                return OperationResult.NotFound();
            }
        }
    }

    public OperationResult downloadDSKWOR00Disk(long BatchPayRollRequestId)
    {
        var InsuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Where(i => i.BatchPayRollRequestId == BatchPayRollRequestId).Single();
        var diskList = _unitOfWork.Context.InsuranceDisketteFiles.Where(i => i.InsuranceDisketteId == InsuranceDiskette.Id && i.FileTypeId == (long)Enums.InsuranceFileDiskType.DSKWOR00);
        if (diskList == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (diskList.Any())
            {
                var last = diskList.OrderBy(i => i.Id).Last();

                last.Content = Convert.ToBase64String(last.DiskContent);
                last.DiskContent = null;
                last.MimeType = System.Net.Mime.MediaTypeNames.Application.Octet;
                return OperationResult.Succeeded(payload: last);
            }
            else
            {
                return OperationResult.NotFound();
            }
        }
    }
    private byte[] GetBinaryFile(string filename)
    {
        byte[] bytes;
        using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            bytes = new byte[file.Length];
            file.Read(bytes, 0, (int)file.Length);
        }
        return bytes;
    }

    static string? ReverseString(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        else
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }
    /// <summary>
    /// تبدیل تاریخ به فرمت قابل قبول برای سامانه تامین اجتماعی
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public string GetNumericalDate(DateTime date)
    {
        System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
        date = DateTime.Parse(date.ToShortDateString());
        int year = p.GetYear(date);
        int month = p.GetMonth(date);
        int day = p.GetDayOfMonth(date);
        string str = string.Format("{0}{1}{2}", year, month.ToString().PadLeft(2, '0'), day.ToString().PadLeft(2, '0'));
        return str;
    }

    private void SaveChangesWithDetailedLogging(string operationDescription, object? relatedEntity)
    {
        try
        {
            _unitOfWork.Context.SaveChanges();
            _logger.LogInformation("SaveChanges succeeded: {Operation}", operationDescription);
        }
        catch (DbUpdateException dbEx)
        {
            try
            {
                var entries = dbEx.Entries != null ? string.Join(",", dbEx.Entries.Select(e => e.Entity.GetType().Name)) : "";
                _logger.LogError(dbEx, "SaveChanges failed during {Operation}. Affected entries: {Entries}", operationDescription, entries);
            }
            catch
            {
                _logger.LogError(dbEx, "SaveChanges failed during {Operation}", operationDescription);
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error on SaveChanges during {Operation}", operationDescription);
            throw;
        }
    }

    public new OperationResult DeleteRecord(long Id)
    {
        var existingDiskette = _unitOfWork.Context.InsuranceDiskettes.Find(Id);
        if (existingDiskette == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (existingDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Payed || existingDiskette.InsuranceDisketteStatusId == (long)Enums.InsuranceDisketteStatus.Deleted)
            {
                return OperationResult.Failed("وضعیت دیسکت جهت حذف معتبر نمی باشد");
            }
            else
            {
                var relatedPeriod = _unitOfWork.Context.PaymentPeriods.Find(existingDiskette.PaymentPeriodId);
                if (relatedPeriod.IsClosed == true)
                {
                    return OperationResult.Failed("دوره مورد نظر بسته شده است و امکان حذف دیسکت وجود ندارد");
                }
                existingDiskette.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.Deleted;
                _unitOfWork.Context.InsuranceDiskettes.Update(existingDiskette);
                _unitOfWork.Context.SaveChanges();
                return OperationResult.Succeeded();
            }
        }
    }
    public new OperationResult CreateForAsync(InsuranceDisketteDTO entityToCreate)
    {
        try
        {
            _logger.LogInformation("CreateForAsync started for PaymentPeriodId: {PaymentPeriodId}", entityToCreate?.PaymentPeriodId);

            // بررسی null بودن DTO
            if (entityToCreate == null)
            {
                _logger.LogWarning("CreateForAsync: entityToCreate is null");
                return OperationResult.Failed("اطلاعات دیسکت بیمه ارسال نشده است");
            }

            if (entityToCreate.PaymentPeriodId > 0)
            {
                List<long> invalidStatusList =
                [
                (long)Enums.InsuranceDisketteStatus.Initial,
                    (long)Enums.InsuranceDisketteStatus.CalculationFinished,
                    (long)Enums.InsuranceDisketteStatus.Payed,
                ];

                try
                {
                    if (_unitOfWork.Context.InsuranceDiskettes.Any(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && invalidStatusList.Contains(i.InsuranceDisketteStatusId)))
                    {
                        // return OperationResult.Failed("برای دوره ارسالی دیسکت بیمه وجود دارد لطفا ابتدا دیسکت قبل را حذف بفرمایید");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateForAsync: Error checking existing diskettes for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                    return OperationResult.Failed("خطا در بررسی دیسکت‌های موجود");
                }

                // بررسی وضعیت دوره
                var period = _unitOfWork.Context.PaymentPeriods.Find(entityToCreate.PaymentPeriodId);
                if (period == null)
                {
                    _logger.LogWarning("CreateForAsync: PaymentPeriod not found for Id: {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                    return OperationResult.Failed("دوره پرداخت یافت نشد");
                }

                if (period.IsClosed == true)
                {
                    _logger.LogWarning("CreateForAsync: PaymentPeriod {PaymentPeriodId} is closed", entityToCreate.PaymentPeriodId);
                    return OperationResult.Failed("دوره جاری پیش فرض بسته می باشد و امکان تهیه دیسکت وجود ندارد");
                }
            }
            else
            {
                _logger.LogWarning("CreateForAsync: Invalid PaymentPeriodId: {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                return OperationResult.Failed("دوره محاسبه ارسال نشده است");
            }

            // Mapping
            var mappedTodo = _mapper.Map<InsuranceDiskette>(entityToCreate);
            if (mappedTodo == null)
            {
                _logger.LogError("CreateForAsync: Mapper returned null for entityToCreate");
                return OperationResult.Failed("خطا در تبدیل اطلاعات دیسکت بیمه");
            }

            mappedTodo.InsuranceDisketteStatusId = (long)Enums.InsuranceDisketteStatus.Initial;

            // دریافت فیش‌ها
            List<Fiche> fiches;
            try
            {
                fiches = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && (i.FicheStatusId == (long)Enums.FicheStatus.Initial || i.FicheStatusId == (long)Enums.FicheStatus.Payed)).ToList();
                _logger.LogInformation("CreateForAsync: Found {FicheCount} fiches for period {PaymentPeriodId}", fiches?.Count ?? 0, entityToCreate.PaymentPeriodId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateForAsync: Error fetching fiches for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                return OperationResult.Failed("خطا در دریافت فیش‌های حقوقی");
            }

            //mappedTodo.DSK_NUM = fiches.Count();
            mappedTodo.BatchPayRollRequestId = null;

            List<InsuranceDisketteCostCenter> CostCenterList = new List<InsuranceDisketteCostCenter>();
            if (entityToCreate.ReportTypeId == (long)Enums.InsuranceDisketteReportType.ByProjectRow)
            {
                if (entityToCreate.PeymanRowId > 0)
                {
                    _logger.LogInformation("CreateForAsync: Report type is ByProjectRow with PeymanRowId: {PeymanRowId}", entityToCreate.PeymanRowId);
                }
                else
                {
                    _logger.LogWarning("CreateForAsync: PeymanRowId not provided for ByProjectRow report type");
                    return OperationResult.Failed("ردیف پیمان ار سال نشده است ");
                }
            }

            if (entityToCreate.ReportTypeId == (long)Enums.InsuranceDisketteReportType.ByCostCenter)
            {
                if (entityToCreate.CostCenterIdList == null)
                {
                    _logger.LogWarning("CreateForAsync: CostCenterIdList is null for ByCostCenter report type");
                    return OperationResult.Failed("فهرست مرکز هزینه ارسال نشده است");
                }
                else
                {
                    if (entityToCreate.CostCenterIdList.Any())
                    {
                        List<OrganisationCostCenter> costCenterList;
                        try
                        {
                            if (_organisationCostCenterService == null)
                            {
                                _logger.LogError("CreateForAsync: _organisationCostCenterService is null");
                                return OperationResult.Failed("خطا در دسترسی به سرویس مراکز هزینه");
                            }

                            var allCostCenters = _organisationCostCenterService.All();
                            if (allCostCenters == null)
                            {
                                _logger.LogError("CreateForAsync: _organisationCostCenterService.All() returned null");
                                return OperationResult.Failed("خطا در دریافت فهرست مراکز هزینه");
                            }

                            costCenterList = allCostCenters.Where(i => entityToCreate.CostCenterIdList.Contains(i.Id)).ToList();
                            _logger.LogInformation("CreateForAsync: Found {CostCenterCount} cost centers", costCenterList?.Count ?? 0);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "CreateForAsync: Error fetching cost centers");
                            return OperationResult.Failed("خطا در دریافت اطلاعات مراکز هزینه");
                        }

                        foreach (var item in costCenterList)
                        {
                            if (mappedTodo.PeymanRowId > 0)
                            {
                                if (item.PeymanRowId.HasValue)
                                {
                                    if (mappedTodo.PeymanRowId == item.PeymanRowId.Value)
                                    {

                                    }
                                    else
                                    {
                                        return OperationResult.Failed("همه مراکز هزینه های انتخابی در یک ردیف پیمان نیستند");
                                    }
                                }
                            }
                            else
                            {
                                if (item.PeymanRowId.HasValue)
                                {
                                    mappedTodo.PeymanRowId = item.PeymanRowId.Value;
                                }
                            }

                            InsuranceDisketteCostCenter toAdd = new InsuranceDisketteCostCenter()
                            {
                                CostCenterId = item.CostCenterId,
                                CreateDate = DateTime.Now,
                                IPAddress = "",
                            };
                            CostCenterList.Add(toAdd);
                        }
                    }
                    else
                    {
                        return OperationResult.Failed("فهرست مرکز هزینه ارسال نشده است");
                    }
                }
            }


            // شروع تراکنش
            _logger.LogInformation("CreateForAsync: Starting database transaction for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
            _unitOfWork.CreateTransaction();
            try
            {
                // تنظیم OrganisationChartId
                if (typeof(InsuranceDiskette).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                {
                    if (_currentUserDefaultOrganId > 0)
                    {
                        PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                        if (propertyInfo == null)
                        {
                            _logger.LogError("CreateForAsync: OrganisationChartId property not found on InsuranceDiskette");
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("خطای سیستمی: خاصیت OrganisationChartId یافت نشد");
                        }

                        propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                        _logger.LogInformation("CreateForAsync: Set OrganisationChartId to {OrganId}", _currentUserDefaultOrganId);

                        // بررسی تنظیمات سازمان
                        try
                        {
                            var orgChartSetting = _unitOfWork.Context.OrganProperties.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
                            if (orgChartSetting == null || !orgChartSetting.Any())
                            {
                                _logger.LogWarning("CreateForAsync: Organ properties not found for OrganId: {OrganId}", _currentUserDefaultOrganId);
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("مشخصات بیمه ای محل پرداخت مورد نظر تعریف نشده است");
                            }
                            _logger.LogInformation("CreateForAsync: Organ properties validated for OrganId: {OrganId}", _currentUserDefaultOrganId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "CreateForAsync: Error checking organ properties for OrganId: {OrganId}", _currentUserDefaultOrganId);
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("خطا در بررسی تنظیمات بیمه‌ای محل پرداخت");
                        }
                    }
                    else
                    {
                        _logger.LogError("CreateForAsync: _currentUserDefaultOrganId is not set");
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("سازمان پیش فرض مشخص نشده است");
                    }
                }

                // ذخیره دیسکت بیمه
                try
                {
                    mappedTodo.IPAddress = "";
                    mappedTodo.CreateDate = DateTime.Now;
                    _unitOfWork.Context.InsuranceDiskettes.Add(mappedTodo);
                    _unitOfWork.Context.SaveChanges();
                    _logger.LogInformation("CreateForAsync: InsuranceDiskette saved with Id: {DisketteId}", mappedTodo.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateForAsync: Error saving InsuranceDiskette for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("خطا در ذخیره دیسکت بیمه");
                }

                // ذخیره مراکز هزینه
                if (entityToCreate.ReportTypeId == (long)Enums.InsuranceDisketteReportType.ByCostCenter)
                {
                    try
                    {
                        foreach (var CostCenter in CostCenterList)
                        {
                            CostCenter.InsuranceDisketteId = mappedTodo.Id;
                            _unitOfWork.Context.InsuranceDisketteCostCenters.Add(CostCenter);
                        }
                        _unitOfWork.Context.SaveChanges();
                        _logger.LogInformation("CreateForAsync: Saved {CostCenterCount} cost centers for diskette {DisketteId}", CostCenterList.Count, mappedTodo.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CreateForAsync: Error saving cost centers for diskette {DisketteId}", mappedTodo.Id);
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("خطا در ذخیره مراکز هزینه دیسکت");
                    }
                }

                // ایجاد درخواست محاسبه batch
                try
                {
                    BatchPayRollRequest batch = new BatchPayRollRequest()
                    {
                        RequestTypeId = (long)Enums.BatchPayRollRequestType.InsuranceDisketteCalculation,
                        EmployeeCount = mappedTodo.DSK_NUM,
                        IPAddress = "",
                        CreateDate = DateTime.Now,
                        OrganisationChartId = mappedTodo.OrganisationChartId,
                        PaymentPeriodId = mappedTodo.PaymentPeriodId,
                        RequestStateId = (long)Enums.BatchPayRollRequestState.Initial,
                        UserId = _userResolverService.GetUserId(),
                        Username = string.IsNullOrWhiteSpace(_userResolverService.fullname()) ? _userResolverService.GetUser() : _userResolverService.fullname(),
                        SuccessCount = 0,
                        RequsetDescription = "به درخواست صدور دیسکت بیمه",
                        InsuranceDisketteId = mappedTodo.Id
                    };
                    _unitOfWork.Context.BatchPayRollRequests.Add(batch);
                    _unitOfWork.Context.SaveChanges();
                    _logger.LogInformation("CreateForAsync: BatchPayRollRequest created with Id: {BatchId}", batch.Id);

                    // به‌روزرسانی دیسکت با شناسه batch
                    mappedTodo.BatchPayRollRequestId = batch.Id;
                    _unitOfWork.Context.Update(mappedTodo);
                    _unitOfWork.Context.SaveChanges();
                    _logger.LogInformation("CreateForAsync: Updated InsuranceDiskette {DisketteId} with BatchPayRollRequestId: {BatchId}", mappedTodo.Id, batch.Id);

                    // Commit تراکنش
                    _unitOfWork.Commit();
                    _logger.LogInformation("CreateForAsync: Transaction committed successfully for period {PaymentPeriodId}. BatchId: {BatchId}", entityToCreate.PaymentPeriodId, batch.Id);
                    return OperationResult.Succeeded(payload: batch.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateForAsync: Error creating BatchPayRollRequest for diskette {DisketteId}", mappedTodo.Id);
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("خطا در ایجاد درخواست محاسبه دیسکت");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateForAsync: Unhandled exception in transaction for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                try
                {
                    _unitOfWork.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "CreateForAsync: Error during rollback");
                }
                return OperationResult.Failed("خطای غیرمنتظره در ایجاد دیسکت بیمه");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateForAsync: Unhandled outer exception for PaymentPeriodId: {PaymentPeriodId}", entityToCreate?.PaymentPeriodId);
            return OperationResult.Failed($"خطای سیستمی در ایجاد دیسکت بیمه: {ex.Message}");
        }
    }
    public OperationResult CheckIfPeriodIsValidForCreateInsuranceDiskette(long PaymentPeriodId)
    {
        try
        {
            _logger.LogInformation("CheckIfPeriodIsValidForCreateInsuranceDiskette: Checking period {PaymentPeriodId}", PaymentPeriodId);

            if (PaymentPeriodId <= 0)
            {
                _logger.LogWarning("CheckIfPeriodIsValidForCreateInsuranceDiskette: Invalid PaymentPeriodId: {PaymentPeriodId}", PaymentPeriodId);
                return OperationResult.Failed("شناسه دوره نامعتبر است");
            }

            // بررسی دیسکت‌های موجود
            try
            {
                List<long> invalidStatusList = new List<long>()
                {
                    (long)Enums.InsuranceDisketteStatus.Initial,
                    (long)Enums.InsuranceDisketteStatus.CalculationFinished,
                    (long)Enums.InsuranceDisketteStatus.Payed,
                };

                var existingInsuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Where(i => invalidStatusList.Contains(i.InsuranceDisketteStatusId) && i.PaymentPeriodId == PaymentPeriodId);
                if (existingInsuranceDiskette != null && existingInsuranceDiskette.Any())
                {
                    _logger.LogInformation("CheckIfPeriodIsValidForCreateInsuranceDiskette: Found {Count} existing diskettes for period {PaymentPeriodId}", existingInsuranceDiskette.Count(), PaymentPeriodId);
                    // return OperationResult.Failed("برای دوره مورد نظر دیسکت وجود دارد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckIfPeriodIsValidForCreateInsuranceDiskette: Error checking existing diskettes for period {PaymentPeriodId}", PaymentPeriodId);
                return OperationResult.Failed("خطا در بررسی دیسکت‌های موجود");
            }

            // بررسی وجود دوره
            PaymentPeriod PaymentPeriod;
            try
            {
                PaymentPeriod = _unitOfWork.Context.PaymentPeriods.Find(PaymentPeriodId);
                if (PaymentPeriod == null)
                {
                    _logger.LogWarning("CheckIfPeriodIsValidForCreateInsuranceDiskette: PaymentPeriod not found: {PaymentPeriodId}", PaymentPeriodId);
                    return OperationResult.Failed("دوره پرداخت یافت نشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckIfPeriodIsValidForCreateInsuranceDiskette: Error finding PaymentPeriod {PaymentPeriodId}", PaymentPeriodId);
                return OperationResult.Failed("خطا در دریافت اطلاعات دوره پرداخت");
            }

            // بررسی سازمان
            if (PaymentPeriod.OrganisationChartId != _currentUserDefaultOrganId)
            {
                _logger.LogWarning("CheckIfPeriodIsValidForCreateInsuranceDiskette: OrganisationChartId mismatch. Period OrganId: {PeriodOrganId}, User OrganId: {UserOrganId}",
                    PaymentPeriod.OrganisationChartId, _currentUserDefaultOrganId);
                return OperationResult.Failed("محل پرداخت پیش فرض با دوره ارسالی مطابقت ندارد");
            }

            // بررسی وضعیت بسته بودن دوره
            if (PaymentPeriod.IsClosed == true)
            {
                _logger.LogWarning("CheckIfPeriodIsValidForCreateInsuranceDiskette: Period {PaymentPeriodId} is closed", PaymentPeriodId);
                return OperationResult.Failed("دوره مورد نظر بسته است و امکان تهیه دیسکت وجود ندارد");
            }

            // بررسی وجود فیش
            try
            {
                var ficheCount = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == PaymentPeriod.Id && (i.FicheStatusId == (long)Enums.FicheStatus.Initial || i.FicheStatusId == (long)Enums.FicheStatus.Payed)).Count();
                _logger.LogInformation("CheckIfPeriodIsValidForCreateInsuranceDiskette: Found {FicheCount} fiches for period {PaymentPeriodId}", ficheCount, PaymentPeriodId);

                if (ficheCount > 0)
                {
                    return OperationResult.Succeeded();
                }
                else
                {
                    _logger.LogWarning("CheckIfPeriodIsValidForCreateInsuranceDiskette: No fiches found for period {PaymentPeriodId}", PaymentPeriodId);
                    return OperationResult.Failed("برای دوره انتخابی هیچ فیش محاسبه شده ای یافت نشد");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckIfPeriodIsValidForCreateInsuranceDiskette: Error counting fiches for period {PaymentPeriodId}", PaymentPeriodId);
                return OperationResult.Failed("خطا در شمارش فیش‌های دوره");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckIfPeriodIsValidForCreateInsuranceDiskette: Unhandled exception for period {PaymentPeriodId}", PaymentPeriodId);
            return OperationResult.Failed("خطای غیرمنتظره در بررسی اعتبار دوره");
        }
    }
    /// <summary>
    /// دانلود آخرین دیسکت بیمه تولید شده
    /// </summary>
    /// <param name="InsuranceDisketteId"></param>
    /// <returns></returns>
    public OperationResult downloadInsuranceDiskette(long InsuranceDisketteId)
    {
        var InsuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Where(i => i.BatchPayRollRequestId == InsuranceDisketteId).Single();
        var diskList = _unitOfWork.Context.InsuranceDisketteFiles.Where(i => i.InsuranceDisketteId == InsuranceDiskette.Id);
        if (diskList == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            if (diskList.Any())
            {
                return OperationResult.Succeeded(payload: diskList.OrderBy(i => i.Id).Last());
            }
            else
            {
                return OperationResult.NotFound();
            }
        }
    }
    public bool Validate(InsuranceDiskette entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    private void WriteDskWorDbf(string dbfPath, long insuranceDisketteId)
    {
        var details = _unitOfWork.Context.InsuranceDisketteItems.Where(i => i.InsuranceDisketteId == insuranceDisketteId).ToList();
        if (details == null || details.Count == 0)
        {
            return;
        }

        DBFField[] fields;
        using (FileStream readFs = new FileStream(dbfPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var reader = new DBFReader(readFs))
        {
            reader.CharEncoding = Encoding.GetEncoding(1256);
            fields = reader.Fields;
        }

        var nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < fields.Length; i++)
        {
            nameToIndex[fields[i].Name] = i;
        }

        using (FileStream writeFs = new FileStream(dbfPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        using (var writer = new DBFWriter())
        {
            writer.CharEncoding = Encoding.GetEncoding(1256);
            writer.Fields = fields;

            foreach (var item in details)
            {
                object[] row = new object[fields.Length];

                SetRowValue(row, nameToIndex, "DSW_ID", ConvertToIranSystem.ToIranSystem(item.DSW_ID));
                SetRowValue(row, nameToIndex, "DSW_YY", SafeNumeric(item.DSW_YY));
                SetRowValue(row, nameToIndex, "DSW_MM", SafeNumeric(item.DSW_MM));
                SetRowValue(row, nameToIndex, "DSW_LISTNO", ConvertToIranSystem.ToIranSystem(item.DSW_LISTNO));
                SetRowValue(row, nameToIndex, "DSW_ID1", ConvertToIranSystem.ToIranSystem(item.DSW_ID1));
                SetRowValue(row, nameToIndex, "DSW_FNAME", ConvertToIranSystem.ToIranSystem(item.DSW_FNAME));
                SetRowValue(row, nameToIndex, "DSW_LNAME", ConvertToIranSystem.ToIranSystem(item.DSW_LNAME));
                SetRowValue(row, nameToIndex, "DSW_DNAME", ConvertToIranSystem.ToIranSystem(item.DSW_DNAME));
                SetRowValue(row, nameToIndex, "DSW_IDNO", ConvertToIranSystem.ToIranSystem(item.DSW_IDNO));
                SetRowValue(row, nameToIndex, "DSW_IDPLC", ConvertToIranSystem.ToIranSystem(item.DSW_IDPLC));
                SetRowValue(row, nameToIndex, "DSW_IDATE", ConvertToIranSystem.ToIranSystem(ReverseString(ConvertToIranSystem.ToIranSystem(item.DSW_IDATE))));
                SetRowValue(row, nameToIndex, "DSW_BDATE", ConvertToIranSystem.ToIranSystem(ReverseString(ConvertToIranSystem.ToIranSystem(item.DSW_BDATE))));
                SetRowValue(row, nameToIndex, "DSW_SEX", ConvertToIranSystem.ToIranSystem(item.DSW_SEX));
                SetRowValue(row, nameToIndex, "DSW_NAT", ConvertToIranSystem.ToIranSystem(item.DSW_NAT));
                SetRowValue(row, nameToIndex, "DSW_OCP", ConvertToIranSystem.ToIranSystem(item.DSW_OCP));
                SetRowValue(row, nameToIndex, "DSW_SDATE", ConvertToIranSystem.ToIranSystem(ReverseString(ConvertToIranSystem.ToIranSystem(item.DSW_SDATE))));
                SetRowValue(row, nameToIndex, "DSW_EDATE", ConvertToIranSystem.ToIranSystem(ReverseString(ConvertToIranSystem.ToIranSystem(item.DSW_EDATE))));
                SetRowValue(row, nameToIndex, "DSW_DD", SafeNumeric(item.DSW_DD));
                SetRowValue(row, nameToIndex, "DSW_ROOZ", SafeNumeric(item.DSW_ROOZ));
                SetRowValue(row, nameToIndex, "DSW_MAH", SafeNumeric(item.DSW_MAH));
                SetRowValue(row, nameToIndex, "DSW_MAZ", SafeNumeric(item.DSW_MAZ));
                SetRowValue(row, nameToIndex, "DSW_MASH", SafeNumeric(item.DSW_MASH));
                SetRowValue(row, nameToIndex, "DSW_TOTL", SafeNumeric(item.DSW_TOTL));
                SetRowValue(row, nameToIndex, "DSW_BIME", SafeNumeric(item.DSW_BIME));
                SetRowValue(row, nameToIndex, "DSW_PRATE", SafeNumeric(item.DSW_PRATE));
                SetRowValue(row, nameToIndex, "DSW_JOB", ConvertToIranSystem.ToIranSystem(item.DSW_JOB));
                SetRowValue(row, nameToIndex, "PER_NATCOD", ConvertToIranSystem.ToIranSystem(item.PER_NATCOD));
                SetRowValue(row, nameToIndex, "DSW_INC", SafeNumeric(item.DSW_INC));
                SetRowValue(row, nameToIndex, "DSW_SPOUSE", ConvertToIranSystem.ToIranSystem(new string(item.DSW_SPOUSE.ToString().Reverse().ToArray())));

                writer.AddRecord(row);
            }
            writer.Write(writeFs);
        }
    }

    private void WriteDskKarDbf(string dbfPath, InsuranceDiskette query)
    {
        DBFField[] fields;
        using (FileStream readFs = new FileStream(dbfPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var reader = new DBFReader(readFs))
        {
            reader.CharEncoding = Encoding.GetEncoding(1256);
            fields = reader.Fields;
        }

        var nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < fields.Length; i++)
        {
            nameToIndex[fields[i].Name] = i;
        }

        using (FileStream writeFs = new FileStream(dbfPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        using (var writer = new DBFWriter())
        {
            writer.CharEncoding = Encoding.GetEncoding(1256);
            writer.Fields = fields;

            object[] row = new object[fields.Length];
            SetRowValue(row, nameToIndex, "DSK_ID", ConvertToIranSystem.ToIranSystem(query.DSK_ID));
            SetRowValue(row, nameToIndex, "DSK_NAME", ConvertToIranSystem.ToIranSystem(query.DSK_NAME));
            SetRowValue(row, nameToIndex, "DSK_FARM", ConvertToIranSystem.ToIranSystem(query.DSK_FARM));
            SetRowValue(row, nameToIndex, "DSK_ADRS", ConvertToIranSystem.ToIranSystem(query.DSK_ADRS));
            SetRowValue(row, nameToIndex, "DSK_KIND", SafeNumeric(query.DSK_KIND));
            SetRowValue(row, nameToIndex, "DSK_YY", SafeNumeric(query.DSK_YY));
            SetRowValue(row, nameToIndex, "DSK_MM", SafeNumeric(query.DSK_MM));
            SetRowValue(row, nameToIndex, "DSK_LISTNO", ConvertToIranSystem.ToIranSystem(query.DSK_LISTNO));
            SetRowValue(row, nameToIndex, "DSK_DISC", ConvertToIranSystem.ToIranSystem(query.DSK_DISC));
            SetRowValue(row, nameToIndex, "DSK_NUM", SafeNumeric(query.DSK_NUM));
            SetRowValue(row, nameToIndex, "DSK_TDD", SafeNumeric(query.DSK_TDD));
            SetRowValue(row, nameToIndex, "DSK_TROOZ", SafeNumeric(query.DSK_TROOZ));
            SetRowValue(row, nameToIndex, "DSK_TMAH", SafeNumeric(query.DSK_TMAH));
            SetRowValue(row, nameToIndex, "DSK_TMAZ", SafeNumeric(query.DSK_TMAZ));
            SetRowValue(row, nameToIndex, "DSK_TMASH", SafeNumeric(query.DSK_TMASH));
            SetRowValue(row, nameToIndex, "DSK_TTOTL", SafeNumeric(query.DSK_TTOTL));
            SetRowValue(row, nameToIndex, "DSK_TBIME", SafeNumeric(query.DSK_TBIME));
            SetRowValue(row, nameToIndex, "DSK_TKOSO", SafeNumeric(query.DSK_TKOSO));
            SetRowValue(row, nameToIndex, "DSK_BIC", SafeNumeric(query.DSK_BIC));
            SetRowValue(row, nameToIndex, "DSK_RATE", SafeNumeric(query.DSK_RATE));
            SetRowValue(row, nameToIndex, "DSK_PRATE", SafeNumeric(query.DSK_PRATE));
            SetRowValue(row, nameToIndex, "DSK_BIMH", SafeNumeric(query.DSK_BIMH));
            SetRowValue(row, nameToIndex, "MON_PYM", ConvertToIranSystem.ToIranSystem(query.MON_PYM));
            SetRowValue(row, nameToIndex, "DSK_INC", SafeNumeric(query.DSK_INC));
            SetRowValue(row, nameToIndex, "DSK_SPOUSE", query.DSK_SPOUSE.ToString());

            writer.AddRecord(row);
            writer.Write(writeFs);
        }
    }

    private static object SafeNumeric(object? value)
    {
        if (value == null) return 0;
        try
        {
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return 0;
                if (long.TryParse(s, out var l)) return l;
                if (double.TryParse(s, out var d)) return d;
            }
            if (value is int || value is long || value is short || value is byte || value is double || value is float || value is decimal)
            {
                return value;
            }
            return Convert.ToDouble(value);
        }
        catch
        {
            return 0;
        }
    }

    private static void SetRowValue(object[] row, Dictionary<string, int> nameToIndex, string fieldName, object? value)
    {
        if (nameToIndex.TryGetValue(fieldName, out var idx))
        {
            row[idx] = value ?? string.Empty;
        }
    }

    /// <summary>
    /// Read and parse DBF file (IranSystem format) and return structured data
    /// </summary>
    public OperationResult ReadDbfFile(Stream fileStream)
    {
        try
        {
            var result = new DbfFileDataDTO();

            // Use DbfDataReader with Windows-1256 encoding
            var options = new DbfDataReader.DbfDataReaderOptions
            {
                Encoding = Encoding.GetEncoding(1256)
            };

            using var dbfReader = new DbfDataReader.DbfDataReader(fileStream, options);

            // Get schema and build column names
            var schemaTable = dbfReader.GetSchemaTable();
            foreach (System.Data.DataRow schemaRow in schemaTable.Rows)
            {
                var columnName = schemaRow["ColumnName"]?.ToString() ?? "";
                result.ColumnNames.Add(columnName);
            }

            // Read all records
            while (dbfReader.Read())
            {
                var rowDict = new Dictionary<string, object>();

                for (int i = 0; i < result.ColumnNames.Count; i++)
                {
                    var columnName = result.ColumnNames[i];
                    object value = DBNull.Value;

                    if (!dbfReader.IsDBNull(i))
                    {
                        var rawValue = dbfReader.GetValue(i);

                        // Convert string values from IranSystem to Unicode
                        if (rawValue is string strValue && !string.IsNullOrWhiteSpace(strValue))
                        {
                            var bytes = Encoding.GetEncoding(1256).GetBytes(strValue);
                            value = HR.SharedKernel.Converters.IranSystemConverter.FromIranSystem(bytes);
                        }
                        else
                        {
                            value = rawValue ?? DBNull.Value;
                        }
                    }

                    rowDict[columnName] = value;
                }

                result.Rows.Add(rowDict);
            }

            result.TotalRecords = result.Rows.Count;

            return OperationResult.Succeeded(payload: result);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در خواندن فایل DBF: {ex.Message}");
        }
    }
}
