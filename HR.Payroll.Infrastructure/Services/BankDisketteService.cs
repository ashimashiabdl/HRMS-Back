using AutoMapper;
using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Employee.Core.Entities;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;


namespace HR.Payroll.Infrastructure.Services
{
    public class BankDisketteService : BaseService<BankDiskette, PayrollContext, BankDisketteDTO>, IScopedServices
    {
        private OrganisationCostCenterService _organisationCostCenterService;
        private EmployeeService _employeeService;
        private BankDisketteTemplateService _bankDisketteTemplateService;
        private BankService _bankService;
        private BankDisketteTemplateRowService _bankDisketteTemplateRowService;
        private readonly ILogger<BankDisketteService> _logger;
        private readonly UserResolverService _userResolverService;
        public BankDisketteService(IMapper mapper, BankDisketteTemplateRowService BankDisketteTemplateRowService, BankDisketteTemplateService BankDisketteTemplateService, BankService BankService, EmployeeService EmployeeService, OrganisationCostCenterService OrganisationCostCenterService, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, ILogger<BankDisketteService> logger) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _bankDisketteTemplateRowService = BankDisketteTemplateRowService;
            _bankDisketteTemplateService = BankDisketteTemplateService;
            _bankService = BankService;
            _employeeService = EmployeeService;
            _organisationCostCenterService = OrganisationCostCenterService;
            _logger = logger;
            _userResolverService = userService;
        }
        /// <summary>
        /// گرفتن فهرست دیسکت های تولید شده به تفکیک بانک
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OperationResult GetCurrentDisketteFilesBySepration(long id)
        {
            var batchRequest = _unitOfWork.Context.BatchPayRollRequests.Find(id);

            var BankDisketteGroupAndFiles = _unitOfWork.Context.BankDisketteGroupAndFiles.Include(i => i.BankDisketteTemplate.Bank).Where(i => i.BankDisketteId == batchRequest.BankDisketteId);
            if (BankDisketteGroupAndFiles == null)
            {
                return OperationResult.NotFound();
            }
            else
            {
                if (BankDisketteGroupAndFiles.Any())
                {
                    return OperationResult.Succeeded(payload: BankDisketteGroupAndFiles);
                }
                else
                {
                    return OperationResult.NotFound();
                }
            }
        }

        public OperationResult GetCurrentBankDisketteCostCenters(long id)
        {
            var BankDiskette = GetIdAsync(id).Result;
            if (BankDiskette.CalculateAllFichesInCurrentPeriod == true)
            {
                return OperationResult.NotFound();
            }
            else
            {
                var costCenterList = _unitOfWork.Context.BankDisketteCostCenters.Include(i => i.CostCenter).Where(i => i.BankDisketteId == id);

                if (costCenterList == null)
                {
                    return OperationResult.NotFound();
                }
                else
                {
                    if (costCenterList.Any())
                    {
                        var rsulat = _mapper.Map<List<BankDisketteCostCenterDTO>>(costCenterList.ToList());
                        return OperationResult.Succeeded(payload: rsulat);
                    }
                    else
                    {
                        return OperationResult.NotFound();
                    }
                }
            }

        }

        private const int StuckRequestTimeoutMinutes = 30;
        private const string WorkerIpAddress = "Job";

        private static readonly long[] ValidBatchRequestStates =
        [
            (long)Enums.BatchPayRollRequestState.Initial,
            (long)Enums.BatchPayRollRequestState.TryAgain,
        ];

        private static readonly long[] ValidFicheStatusList =
        [
            (long)Enums.FicheStatus.Initial,
            (long)Enums.FicheStatus.Payed,
        ];

        /// <summary>
        /// محاسبه گروهی دیسکت بانک — پردازش درخواست‌های BatchPayRollRequest در وضعیت Initial/TryAgain.
        /// </summary>
        public void CalculateBankDiskBatch()
        {
            _logger.LogInformation("شروع CalculateBankDiskBatch");

            _unitOfWork.Context.ChangeTracker.Clear();
            var originalAutoDetectChanges = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
            var originalQueryTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            try
            {
                CleanupStuckBankDisketteRequests();

                var readyToCalculateRequests = _unitOfWork.Context.BatchPayRollRequests
                    .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.BankDisketteCalculation
                                && ValidBatchRequestStates.Contains(i.RequestStateId))
                    .OrderBy(i => i.Id)
                    .ToList();

                _logger.LogInformation("تعداد {Count} درخواست دیسکت بانک برای پردازش یافت شد", readyToCalculateRequests.Count);

                foreach (var batchRequest in readyToCalculateRequests)
                {
                    if (IsRequestCancelledByUser(batchRequest.Id))
                    {
                        _logger.LogInformation("درخواست {RequestId} توسط کاربر لغو شده — رد شد", batchRequest.Id);
                        continue;
                    }

                    try
                    {
                        ProcessSingleBankDisketteRequest(batchRequest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطای غیرمنتظره در پردازش درخواست دیسکت بانک {RequestId}", batchRequest.Id);
                        try
                        {
                            FailBankDisketteRequest(
                                batchRequest,
                                null,
                                BuildUserFriendlyErrorMessage(ex, "پردازش درخواست دیسکت بانک"));
                        }
                        catch (Exception failEx)
                        {
                            _logger.LogCritical(failEx, "FailBankDisketteRequest نیز شکست خورد برای {RequestId}", batchRequest.Id);
                            EnsureBatchRequestNotLeftRunning(
                                batchRequest.Id,
                                batchRequest.BankDisketteId,
                                BuildUserFriendlyErrorMessage(ex, "پردازش درخواست دیسکت بانک"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای کلی در CalculateBankDiskBatch");
            }
            finally
            {
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
                _logger.LogInformation("پایان CalculateBankDiskBatch");
            }
        }

        private void CleanupStuckBankDisketteRequests()
        {
            try
            {
                WithTrackingEnabled(() =>
                {
                    var stuckRequests = _unitOfWork.Context.BatchPayRollRequests
                        .Where(i => i.RequestTypeId == (long)Enums.BatchPayRollRequestType.BankDisketteCalculation
                                    && i.RequestStateId == (long)Enums.BatchPayRollRequestState.Running
                                    && i.LastPoolingTime.HasValue
                                    && i.LastPoolingTime.Value < DateTime.Now.AddMinutes(-StuckRequestTimeoutMinutes))
                        .ToList();

                    if (!stuckRequests.Any())
                    {
                        return;
                    }

                    _logger.LogWarning("تعداد {Count} درخواست دیسکت بانک گیرکرده (بیش از {Timeout} دقیقه) یافت شد",
                        stuckRequests.Count, StuckRequestTimeoutMinutes);

                    foreach (var stuckRequest in stuckRequests)
                    {
                        try
                        {
                            var now = DateTime.Now;
                            var msg =
                                $"پردازش قبلی بیش از {StuckRequestTimeoutMinutes} دقیقه پاسخ نداد. سیستم درخواست را برای «تلاش مجدد» آماده کرد. لطفاً چند دقیقه صبر کنید یا از فرم نظارت، «تلاش مجدد» را بزنید.";

                            _unitOfWork.Context.BatchPayRollRequests
                                .Where(r => r.Id == stuckRequest.Id)
                                .ExecuteUpdate(s => s
                                    .SetProperty(r => r.RequestStateId, (long)Enums.BatchPayRollRequestState.TryAgain)
                                    .SetProperty(r => r.ExeptionMessage, msg)
                                    .SetProperty(r => r.LastModifiedDate, now)
                                    .SetProperty(r => r.LastPoolingTime, now));

                            stuckRequest.RequestStateId = (long)Enums.BatchPayRollRequestState.TryAgain;
                            stuckRequest.ExeptionMessage = msg;
                        }
                        catch (Exception cleanupEx)
                        {
                            _logger.LogError(cleanupEx, "خطا در پاکسازی درخواست گیرکرده {RequestId}", stuckRequest.Id);
                            _unitOfWork.Context.ChangeTracker.Clear();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پاکسازی درخواست‌های گیرکرده دیسکت بانک");
            }
            finally
            {
                _unitOfWork.Context.ChangeTracker.Clear();
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        private void ProcessSingleBankDisketteRequest(BatchPayRollRequest batchRequest)
        {
            _logger.LogInformation("شروع پردازش درخواست دیسکت بانک {RequestId}", batchRequest.Id);

            var isRetryRequest = batchRequest.RequestStateId == (long)Enums.BatchPayRollRequestState.TryAgain;
            BankDiskette? bankDiskette = null;
            var enteredRunning = false;

            try
            {
                ApplyOrganisationContext(batchRequest.OrganisationChartId);

                bankDiskette = LoadBankDiskette(batchRequest.BankDisketteId);
                if (bankDiskette == null)
                {
                    FailBankDisketteRequest(
                        batchRequest,
                        null,
                        "اطلاعات دیسکت بانک مربوط به این درخواست در سیستم یافت نشد. لطفاً درخواست را حذف و مجدداً ثبت کنید.");
                    return;
                }

                if (bankDiskette.BankDisketteStatusId == (long)Enums.BankDisketteStatus.Deleted)
                {
                    FailBankDisketteRequest(
                        batchRequest,
                        bankDiskette,
                        "این دیسکت بانک حذف شده است و امکان محاسبه مجدد وجود ندارد.");
                    return;
                }

                if (bankDiskette.BankDisketteStatusId == (long)Enums.BankDisketteStatus.Payed)
                {
                    FailBankDisketteRequest(
                        batchRequest,
                        bankDiskette,
                        "این دیسکت بانک قبلاً پرداخت شده است و امکان محاسبه مجدد وجود ندارد.");
                    return;
                }

                if (bankDiskette.BankDisketteStatusId == (long)Enums.BankDisketteStatus.CalculationFinished
                    && !isRetryRequest)
                {
                    CompleteBankDisketteRequest(batchRequest, bankDiskette, null);
                    _logger.LogInformation("درخواست {RequestId} قبلاً محاسبه شده — بدون پردازش مجدد بسته شد", batchRequest.Id);
                    return;
                }

                SetBatchRequestStateInDatabase(
                    batchRequest,
                    Enums.BatchPayRollRequestState.Running,
                    exceptionMessage: null,
                    setFinishDate: false);
                SetBankDisketteStatusInDatabase(bankDiskette.Id, bankDiskette.BankDisketteStatusId);
                enteredRunning = true;

                var involvedFiches = LoadInvolvedFiches(bankDiskette);
                if (!involvedFiches.Any())
                {
                    FailBankDisketteRequest(
                        batchRequest,
                        bankDiskette,
                        bankDiskette.CalculateAllFichesInCurrentPeriod
                            ? "برای دوره انتخابی هیچ فیش حقوقی مناسب (محاسبه‌شده یا پرداخت‌شده) یافت نشد. ابتدا فیش حقوق را محاسبه کنید."
                            : "برای مراکز هزینه انتخاب‌شده هیچ فیش حقوقی مناسب یافت نشد. مراکز هزینه یا فیش‌های دوره را بررسی کنید.");
                    return;
                }

                SetBatchRequestCountersInDatabase(batchRequest.Id, involvedFiches.Count, successCount: 0);
                batchRequest.EmployeeCount = involvedFiches.Count;
                batchRequest.SuccessCount = 0;

                var referenceData = LoadBankDisketteReferenceData();
                var fileBuilders = new Dictionary<(long DisketteId, long TemplateId), StringBuilder>();
                var fileEndByTemplate = new Dictionary<long, string?>();

                foreach (var fiche in involvedFiches)
                {
                    var currentState = GetBatchRequestStateFromDatabase(batchRequest.Id);
                    if (currentState == (long)Enums.BatchPayRollRequestState.CancelByUser)
                    {
                        // برای فیش‌های باقی‌مانده هم لاگ ثبت شود
                        MarkRemainingFichesAsSkipped(
                            batchRequest,
                            involvedFiches,
                            fiche.Id,
                            "عملیات توسط کاربر متوقف شد.");
                        FailBankDisketteRequest(
                            batchRequest,
                            bankDiskette,
                            "عملیات توسط کاربر متوقف شد.");
                        return;
                    }

                    if (currentState != (long)Enums.BatchPayRollRequestState.Running)
                    {
                        _logger.LogWarning(
                            "درخواست {RequestId} در میانه پردازش وضعیت غیرمنتظره {State} دارد — ادامه با ثبت لاگ برای باقی‌مانده‌ها",
                            batchRequest.Id, currentState);
                        MarkRemainingFichesAsSkipped(
                            batchRequest,
                            involvedFiches,
                            fiche.Id,
                            $"پردازش متوقف شد چون وضعیت درخواست به {currentState} تغییر کرد.");
                        break;
                    }

                    ProcessSingleFiche(
                        batchRequest,
                        bankDiskette,
                        fiche,
                        referenceData,
                        fileBuilders,
                        fileEndByTemplate);
                }

                // تضمین: هر فیش درگیر باید حداقل یک ردیف جزئیات داشته باشد
                EnsureAllFichesHaveDetailLogs(batchRequest, involvedFiches);

                // اگر در حلقه وضعیت از Running خارج شده (غیر از Cancel که return کرده)، Ensure در finally می‌بندد
                if (GetBatchRequestStateFromDatabase(batchRequest.Id) != (long)Enums.BatchPayRollRequestState.Running)
                {
                    return;
                }

                PersistGeneratedDisketteFiles(bankDiskette, fileBuilders, fileEndByTemplate);

                var summaryMessage = batchRequest.SuccessCount == involvedFiches.Count
                    ? null
                    : $"از {involvedFiches.Count} فیش، {batchRequest.SuccessCount} مورد با موفقیت در دیسکت لحاظ شد. جزئیات هر فیش را در لیست ریز عملیات ببینید.";

                CompleteBankDisketteRequest(batchRequest, bankDiskette, summaryMessage);
                _logger.LogInformation(
                    "پایان پردازش درخواست {RequestId}. موفق: {Success}/{Total}",
                    batchRequest.Id, batchRequest.SuccessCount, involvedFiches.Count);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (enteredRunning)
                {
                    EnsureBatchRequestNotLeftRunning(
                        batchRequest.Id,
                        bankDiskette?.Id,
                        "پردازش دیسکت بانک ناقص ماند و سیستم وضعیت درخواست را به‌صورت خودکار بست. لطفاً جزئیات را بررسی و در صورت نیاز «تلاش مجدد» بزنید.");
                }
            }
        }

        private void ProcessSingleFiche(
            BatchPayRollRequest batchRequest,
            BankDiskette bankDiskette,
            Fiche fiche,
            BankDisketteReferenceData referenceData,
            Dictionary<(long DisketteId, long TemplateId), StringBuilder> fileBuilders,
            Dictionary<long, string?> fileEndByTemplate)
        {
            var startedAt = DateTime.Now;

            // اول لاگ را ثبت می‌کنیم تا حتی اگر بعداً خطا شود، در نظارت دیده شود
            PersistBatchRequestDetailMessage(
                batchRequest.Id,
                fiche.EmployeeId,
                fiche.Id,
                "در حال پردازش دیسکت بانک...",
                startedAt,
                bankDisketteItemId: null,
                isFailure: false);

            UpdateBatchRequestProgress(batchRequest, fiche.EmployeeId);

            try
            {
                // پاک‌سازی tracker بین فیش‌ها تا تداخل tracking باعث توقف بقیه نشود
                _unitOfWork.Context.ChangeTracker.Clear();

                try
                {
                    _unitOfWork.CreateTransaction();
                }
                catch (Exception txEx)
                {
                    throw new InvalidOperationException(
                        "امکان شروع تراکنش برای این فیش نبود. لطفاً «تلاش مجدد» بزنید.", txEx);
                }

                try
                {
                    var employee = _employeeService.GetIdAsync(fiche.EmployeeId).GetAwaiter().GetResult();
                    if (employee == null)
                    {
                        throw new InvalidOperationException(
                            $"اطلاعات کارمند (شناسه {fiche.EmployeeId}) یافت نشد. لطفاً پرونده کارمند را بررسی کنید.");
                    }

                    var bankAccount = ResolveActiveBankAccount(fiche.EmployeeId);
                    if (bankAccount == null)
                    {
                        throw new InvalidOperationException(
                            "حساب بانکی فعال برای این کارمند ثبت نشده است. از بخش «اطلاعات بانکی کارمند» یک حساب فعال ثبت کنید.");
                    }

                    if (!bankAccount.BankId.HasValue || bankAccount.BankId.Value <= 0)
                    {
                        throw new InvalidOperationException(
                            "بانک حساب کارمند مشخص نیست. اطلاعات بانکی کارمند را تکمیل کنید.");
                    }

                    var bankId = bankAccount.BankId.Value;
                    var bankName = referenceData.Banks.TryGetValue(bankId, out var bankTitle)
                        ? bankTitle
                        : "نامشخص";

                    if (!referenceData.TemplatesByBankId.TryGetValue(bankId, out var template))
                    {
                        throw new InvalidOperationException(
                            $"قالب دیسکت بانک «{bankName}» در سیستم تعریف نشده است. از منوی «قالب دیسکت بانک» قالب مربوطه را ایجاد کنید.");
                    }

                    var templateRows = referenceData.TemplateRowsByTemplateId.TryGetValue(template.Id, out var rows)
                        ? rows
                        : [];

                    if (!templateRows.Any())
                    {
                        throw new InvalidOperationException(
                            $"ردیف‌های قالب دیسکت بانک «{bankName}» تعریف نشده است. از منوی «ردیف قالب دیسکت بانک» ردیف‌ها را تکمیل کنید.");
                    }

                    var bankDisketteItem = UpsertBankDisketteItem(bankDiskette, fiche, bankAccount);
                    var requestDetail = UpsertBatchPayRollRequestDetail(batchRequest, fiche, bankDisketteItem.Id);

                    var groupFile = UpsertBankDisketteGroupAndFile(bankDiskette, template);
                    bankDisketteItem.BankDisketteGroupAndFileId = groupFile.Id;
                    _unitOfWork.Context.Update(bankDisketteItem);
                    SaveChangesWithAutoDetect();

                    var builderKey = (bankDiskette.Id, template.Id);
                    if (!fileBuilders.TryGetValue(builderKey, out var rowBuilder))
                    {
                        rowBuilder = new StringBuilder();
                        if (!string.IsNullOrWhiteSpace(template.FileHeader))
                        {
                            rowBuilder.AppendLine(template.FileHeader);
                        }

                        fileBuilders[builderKey] = rowBuilder;
                    }

                    if (!fileEndByTemplate.ContainsKey(template.Id))
                    {
                        fileEndByTemplate[template.Id] = template.FileEnd;
                    }

                    var rowContent = BuildDisketteRow(
                        template,
                        templateRows,
                        bankDiskette,
                        bankDisketteItem,
                        bankAccount,
                        employee);

                    rowBuilder.AppendLine(rowContent);

                    var successMessage = $"با موفقیت در دیسکت بانک «{bankName}» لحاظ شد.";
                    requestDetail.FinalMessage = successMessage;
                    requestDetail.RunTimeinMilliseconds = (int)(DateTime.Now - startedAt).TotalMilliseconds;
                    requestDetail.LastModifiedDate = DateTime.Now;
                    requestDetail.DoDatetime = DateTime.Now;
                    _unitOfWork.Context.Update(requestDetail);

                    SaveChangesWithAutoDetect();
                    _unitOfWork.Commit();

                    IncrementBatchRequestSuccessInDatabase(batchRequest);

                    // ثبت مجدد پیام موفقیت خارج از تراکنش (برای اطمینان از دیده شدن در نظارت)
                    PersistBatchRequestDetailMessage(
                        batchRequest.Id,
                        fiche.EmployeeId,
                        fiche.Id,
                        successMessage,
                        startedAt,
                        bankDisketteItem.Id,
                        isFailure: false);
                }
                catch (Exception)
                {
                    SafeRollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                var friendlyMessage = BuildUserFriendlyErrorMessage(
                    ex,
                    $"محاسبه دیسکت برای فیش {fiche.Id} (کارمند {fiche.EmployeeId})");

                _logger.LogError(ex,
                    "خطا در پردازش فیش {FicheId} برای کارمند {EmployeeId} در درخواست {RequestId}",
                    fiche.Id, fiche.EmployeeId, batchRequest.Id);

                PersistBatchRequestDetailMessage(
                    batchRequest.Id,
                    fiche.EmployeeId,
                    fiche.Id,
                    friendlyMessage,
                    startedAt,
                    bankDisketteItemId: null,
                    isFailure: true);
            }
            finally
            {
                _unitOfWork.Context.ChangeTracker.Clear();
            }
        }

        private void PersistGeneratedDisketteFiles(
            BankDiskette bankDiskette,
            Dictionary<(long DisketteId, long TemplateId), StringBuilder> fileBuilders,
            Dictionary<long, string?> fileEndByTemplate)
        {
            foreach (var entry in fileBuilders)
            {
                var templateId = entry.Key.TemplateId;
                var contentBuilder = entry.Value;

                if (fileEndByTemplate.TryGetValue(templateId, out var fileEnd) && !string.IsNullOrWhiteSpace(fileEnd))
                {
                    contentBuilder.AppendLine(fileEnd);
                }

                var groupFile = _unitOfWork.Context.BankDisketteGroupAndFiles
                    .SingleOrDefault(i => i.BankDisketteId == bankDiskette.Id && i.BankDisketteTemplateId == templateId);

                if (groupFile == null)
                {
                    _logger.LogWarning(
                        "فایل گروهی دیسکت برای BankDisketteId={DisketteId}, TemplateId={TemplateId} یافت نشد",
                        bankDiskette.Id, templateId);
                    continue;
                }

                groupFile.Content = contentBuilder.ToString();
                groupFile.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Update(groupFile);
            }

            if (fileBuilders.Any())
            {
                SaveChangesWithAutoDetect();
            }
        }

        private List<Fiche> LoadInvolvedFiches(BankDiskette bankDiskette)
        {
            var query = _unitOfWork.Context.Fiches
                .Where(i => i.PaymentPeriodId == bankDiskette.PaymentPeriodId
                            && ValidFicheStatusList.Contains(i.FicheStatusId));

            if (bankDiskette.CalculateAllFichesInCurrentPeriod != true)
            {
                var selectedCostCenters = _unitOfWork.Context.BankDisketteCostCenters
                    .Where(i => i.BankDisketteId == bankDiskette.Id)
                    .Select(i => i.CostCenterId)
                    .Distinct()
                    .ToList();

                if (!selectedCostCenters.Any())
                {
                    _logger.LogWarning("هیچ مرکز هزینه‌ای برای BankDisketteId={DisketteId} ثبت نشده", bankDiskette.Id);
                    return [];
                }

                query = query.Where(i => selectedCostCenters.Contains(i.CostCenterId));
            }

            return query.OrderBy(i => i.Id).ToList();
        }

        private BankDisketteReferenceData LoadBankDisketteReferenceData()
        {
            var banks = _bankService.All()
                .ToDictionary(i => i.Id, i => i.title ?? "نامشخص");

            var templates = _bankDisketteTemplateService.All().ToList();
            var templatesByBankId = templates
                .GroupBy(i => i.BankId)
                .ToDictionary(g => g.Key, g => g.First());

            var templateRows = _bankDisketteTemplateRowService.All()
                .GroupBy(i => i.BankDisketteTemplateId)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Id).ToList());

            return new BankDisketteReferenceData(banks, templatesByBankId, templateRows);
        }

        private BankDiskette? LoadBankDiskette(long? bankDisketteId)
        {
            if (!bankDisketteId.HasValue || bankDisketteId.Value <= 0)
            {
                return null;
            }

            return _unitOfWork.Context.BankDiskettes.Find(bankDisketteId.Value);
        }

        private void ApplyOrganisationContext(long organisationChartId)
        {
            _currentUserDefaultOrganId = organisationChartId;
            _employeeService._currentUserDefaultOrganId = organisationChartId;
        }

        private BankAccount? ResolveActiveBankAccount(long employeeId)
        {
            return _employeeService._unitOfWork.Context.BankAccounts
                .Where(i => i.EmployeeId == employeeId && i.Status == true && i.IsDeleted != true)
                .OrderByDescending(i => i.CreateDate)
                .FirstOrDefault();
        }

        private BankDisketteItem UpsertBankDisketteItem(BankDiskette bankDiskette, Fiche fiche, BankAccount bankAccount)
        {
            var existingItem = _unitOfWork.Context.BankDisketteItems
                .SingleOrDefault(i => i.BankDisketteId == bankDiskette.Id && i.EmployeeId == fiche.EmployeeId);

            if (existingItem == null)
            {
                existingItem = new BankDisketteItem
                {
                    BankDisketteId = bankDiskette.Id,
                    EmployeeId = fiche.EmployeeId,
                    FicheId = fiche.Id,
                    CostCenterId = fiche.CostCenterId,
                    Amount = fiche.PurePaymentAmount,
                    AccountNo = string.IsNullOrWhiteSpace(fiche.BankAccountNo)
                        ? bankAccount.AccountNumber
                        : fiche.BankAccountNo,
                    CreateDate = DateTime.Now,
                    IPAddress = WorkerIpAddress,
                };
                _unitOfWork.Context.BankDisketteItems.Add(existingItem);
            }
            else
            {
                existingItem.Amount = fiche.PurePaymentAmount;
                existingItem.AccountNo = string.IsNullOrWhiteSpace(fiche.BankAccountNo)
                    ? bankAccount.AccountNumber
                    : fiche.BankAccountNo;
                existingItem.FicheId = fiche.Id;
                existingItem.CostCenterId = fiche.CostCenterId;
                existingItem.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Update(existingItem);
            }

            SaveChangesWithAutoDetect();
            return existingItem;
        }

        private BatchPayRollRequestDetail UpsertBatchPayRollRequestDetail(
            BatchPayRollRequest batchRequest,
            Fiche fiche,
            long bankDisketteItemId)
        {
            var existingDetail = _unitOfWork.Context.BatchPayRollRequestDetails
                .SingleOrDefault(i => i.BatchPayRollRequestId == batchRequest.Id
                                      && i.EmployeeId == fiche.EmployeeId
                                      && i.FicheId == fiche.Id);

            if (existingDetail == null)
            {
                existingDetail = new BatchPayRollRequestDetail
                {
                    BatchPayRollRequestId = batchRequest.Id,
                    EmployeeId = fiche.EmployeeId,
                    FicheId = fiche.Id,
                    BankDisketteItemId = bankDisketteItemId,
                    CreateDate = DateTime.Now,
                    IPAddress = WorkerIpAddress,
                };
                _unitOfWork.Context.BatchPayRollRequestDetails.Add(existingDetail);
            }
            else
            {
                existingDetail.BankDisketteItemId = bankDisketteItemId;
                existingDetail.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Update(existingDetail);
            }

            SaveChangesWithAutoDetect();
            return existingDetail;
        }

        private BankDisketteGroupAndFile UpsertBankDisketteGroupAndFile(
            BankDiskette bankDiskette,
            BankDisketteTemplate template)
        {
            var existingGroupFile = _unitOfWork.Context.BankDisketteGroupAndFiles
                .SingleOrDefault(i => i.BankDisketteId == bankDiskette.Id && i.BankDisketteTemplateId == template.Id);

            if (existingGroupFile == null)
            {
                existingGroupFile = new BankDisketteGroupAndFile
                {
                    BankDisketteId = bankDiskette.Id,
                    BankDisketteTemplateId = template.Id,
                    FileName = template.FileName,
                    Extension = template.FileExtension,
                    CreateDate = DateTime.Now,
                    IPAddress = WorkerIpAddress,
                };
                _unitOfWork.Context.BankDisketteGroupAndFiles.Add(existingGroupFile);
            }
            else
            {
                existingGroupFile.FileName = template.FileName;
                existingGroupFile.Extension = template.FileExtension;
                existingGroupFile.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Update(existingGroupFile);
            }

            SaveChangesWithAutoDetect();
            return existingGroupFile;
        }

        private string BuildDisketteRow(
            BankDisketteTemplate template,
            IReadOnlyList<BankDisketteTemplateRow> templateRows,
            BankDiskette bankDiskette,
            BankDisketteItem bankDisketteItem,
            BankAccount bankAccount,
            HR.Employee.Core.Entities.Employee employee)
        {
            var rowBuilder = new StringBuilder();

            if (template.HasLineStartCharacter == true && !string.IsNullOrEmpty(template.LineStartCharacter))
            {
                rowBuilder.Append(template.LineStartCharacter);
            }

            foreach (var templateRow in templateRows)
            {
                var padChar = string.IsNullOrEmpty(templateRow.PadLeftCharacter) ? '0' : templateRow.PadLeftCharacter[0];

                switch (templateRow.DisketteItemTypeId)
                {
                    case (long)Enums.BankDiskItemFieldType.AcountNo:
                        rowBuilder.Append(
                            string.IsNullOrWhiteSpace(bankDisketteItem.AccountNo)
                                ? new string(padChar, templateRow.Length)
                                : bankDisketteItem.AccountNo.PadLeft(templateRow.Length, padChar));
                        break;

                    case (long)Enums.BankDiskItemFieldType.Sheba:
                        rowBuilder.Append(ResolveShebaValue(bankAccount, templateRow.Length, padChar));
                        break;

                    case (long)Enums.BankDiskItemFieldType.Salary:
                        rowBuilder.Append(bankDisketteItem.Amount.ToString().PadLeft(templateRow.Length, padChar));
                        break;

                    case (long)Enums.BankDiskItemFieldType.BankBranchNo:
                        rowBuilder.Append(ResolveBankBranchCode(bankAccount, templateRow.Length, padChar));
                        break;

                    case (long)Enums.BankDiskItemFieldType.UniqueField:
                        rowBuilder.Append(ResolveUniqueFieldFromSheba(bankAccount, templateRow.Length, padChar));
                        break;

                    case (long)Enums.BankDiskItemFieldType.NameAndLastName:
                        rowBuilder.Append($"{employee.FirstName} {employee.LastName}".Trim());
                        break;

                    case (long)Enums.BankDiskItemFieldType.DescriptionOfTheDeposit:
                        rowBuilder.Append(bankDiskette.DescriptionOfTheDeposit ?? string.Empty);
                        break;

                    case (long)Enums.BankDiskItemFieldType.StaticText:
                        if (string.IsNullOrEmpty(templateRow.StaticText))
                        {
                            rowBuilder.Append(new string(padChar, templateRow.Length));
                        }
                        else
                        {
                            var text = templateRow.StaticText;
                            rowBuilder.Append(text.Length > templateRow.Length
                                ? text[..templateRow.Length]
                                : text.PadLeft(templateRow.Length, padChar));
                        }
                        break;
                }

                if (template.HasLineDelimiterCharacter == true && !string.IsNullOrEmpty(template.LineDelimiterCharacter))
                {
                    rowBuilder.Append(template.LineDelimiterCharacter);
                }
            }

            if (template.HasLineEndCharacter == true && !string.IsNullOrEmpty(template.LineEndCharacter))
            {
                rowBuilder.Append(template.LineEndCharacter);
            }

            return rowBuilder.ToString();
        }

        private string ResolveShebaValue(BankAccount bankAccount, int length, char padChar)
        {
            if (string.IsNullOrWhiteSpace(bankAccount.ShabaNumber))
            {
                throw new InvalidOperationException(
                    "شماره شبا برای حساب بانکی این کارمند ثبت نشده است. لطفاً شماره شبا را در اطلاعات بانکی کارمند تکمیل کنید.");
            }

            return bankAccount.ShabaNumber.Length >= length
                ? bankAccount.ShabaNumber[..length]
                : bankAccount.ShabaNumber.PadLeft(length, padChar);
        }

        private string ResolveUniqueFieldFromSheba(BankAccount bankAccount, int length, char padChar)
        {
            if (string.IsNullOrWhiteSpace(bankAccount.ShabaNumber) || bankAccount.ShabaNumber.Length < 8)
            {
                throw new InvalidOperationException(
                    "شماره شبا برای استخراج «فیلد یکتا» کافی نیست. شماره شبا را در اطلاعات بانکی کارمند اصلاح کنید.");
            }

            var uniquePart = bankAccount.ShabaNumber.ToUpperInvariant()[6..8];
            return uniquePart.Length >= length ? uniquePart[..length] : uniquePart.PadLeft(length, padChar);
        }

        private string ResolveBankBranchCode(BankAccount bankAccount, int length, char padChar)
        {
            if (!bankAccount.BankBranchId.HasValue || bankAccount.BankBranchId.Value <= 0)
            {
                throw new InvalidOperationException(
                    "شعبه بانک برای حساب این کارمند مشخص نشده است. شعبه بانک را در اطلاعات بانکی کارمند ثبت کنید.");
            }

            var branch = _unitOfWork.Context.BankBranchs.Find(bankAccount.BankBranchId.Value);
            if (branch == null || string.IsNullOrWhiteSpace(branch.Code))
            {
                throw new InvalidOperationException(
                    "کد شعبه بانک برای این کارمند یافت نشد. کد شعبه را در «اطلاعات شعب بانک» یا «اطلاعات بانکی کارمند» تکمیل کنید.");
            }

            return branch.Code.Length >= length ? branch.Code[..length] : branch.Code.PadLeft(length, padChar);
        }

        private void UpdateBatchRequestProgress(BatchPayRollRequest batchRequest, long employeeId)
        {
            var now = DateTime.Now;
            _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == batchRequest.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.PoolingEmployeeId, employeeId)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now));

            batchRequest.PoolingEmployeeId = employeeId;
            batchRequest.LastPoolingTime = now;
            batchRequest.LastModifiedDate = now;
        }

        private void IncrementBatchRequestSuccessInDatabase(BatchPayRollRequest batchRequest)
        {
            var now = DateTime.Now;
            _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == batchRequest.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.SuccessCount, r => r.SuccessCount + 1)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now));

            batchRequest.SuccessCount += 1;
            batchRequest.LastPoolingTime = now;
            batchRequest.LastModifiedDate = now;
        }

        private void SetBatchRequestCountersInDatabase(long requestId, int employeeCount, int successCount)
        {
            var now = DateTime.Now;
            _unitOfWork.Context.BatchPayRollRequests
                .Where(r => r.Id == requestId)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.EmployeeCount, employeeCount)
                    .SetProperty(r => r.SuccessCount, successCount)
                    .SetProperty(r => r.LastPoolingTime, now)
                    .SetProperty(r => r.LastModifiedDate, now));
        }

        private long GetBatchRequestStateFromDatabase(long requestId)
        {
            return _unitOfWork.Context.BatchPayRollRequests
                .AsNoTracking()
                .Where(r => r.Id == requestId)
                .Select(r => r.RequestStateId)
                .FirstOrDefault();
        }

        /// <summary>
        /// به‌روزرسانی مستقیم وضعیت درخواست در دیتابیس (بدون اتکا به ChangeTracker).
        /// </summary>
        private void SetBatchRequestStateInDatabase(
            BatchPayRollRequest request,
            Enums.BatchPayRollRequestState newState,
            string? exceptionMessage,
            bool setFinishDate)
        {
            SafeRollback();

            var now = DateTime.Now;
            var stateId = (long)newState;
            var finishDate = setFinishDate ? now : (DateTime?)null;

            int affected;
            if (setFinishDate)
            {
                affected = _unitOfWork.Context.BatchPayRollRequests
                    .Where(r => r.Id == request.Id)
                    .ExecuteUpdate(s => s
                        .SetProperty(r => r.RequestStateId, stateId)
                        .SetProperty(r => r.LastPoolingTime, now)
                        .SetProperty(r => r.LastModifiedDate, now)
                        .SetProperty(r => r.FinishDateTime, finishDate)
                        .SetProperty(r => r.ExeptionMessage, exceptionMessage));
            }
            else
            {
                affected = _unitOfWork.Context.BatchPayRollRequests
                    .Where(r => r.Id == request.Id)
                    .ExecuteUpdate(s => s
                        .SetProperty(r => r.RequestStateId, stateId)
                        .SetProperty(r => r.LastPoolingTime, now)
                        .SetProperty(r => r.LastModifiedDate, now)
                        .SetProperty(r => r.ExeptionMessage, exceptionMessage));
            }

            if (affected == 0)
            {
                throw new InvalidOperationException(
                    $"به‌روزرسانی وضعیت درخواست {request.Id} در پایگاه داده انجام نشد.");
            }

            // همگام‌سازی آبجکت حافظه
            request.RequestStateId = stateId;
            request.LastPoolingTime = now;
            request.LastModifiedDate = now;
            request.ExeptionMessage = exceptionMessage;
            if (setFinishDate)
            {
                request.FinishDateTime = now;
            }

            // تأیید از DB
            var verifiedState = GetBatchRequestStateFromDatabase(request.Id);
            if (verifiedState != stateId)
            {
                _logger.LogCritical(
                    "وضعیت درخواست {RequestId} پس از ExecuteUpdate هنوز {Actual} است (انتظار: {Expected})",
                    request.Id, verifiedState, stateId);
                throw new InvalidOperationException(
                    $"وضعیت درخواست {request.Id} در پایگاه داده درست ذخیره نشد.");
            }

            _logger.LogInformation(
                "وضعیت درخواست {RequestId} در DB به {State} تغییر کرد",
                request.Id, newState);
        }

        private void SetBankDisketteStatusInDatabase(long bankDisketteId, long statusId)
        {
            var now = DateTime.Now;
            _unitOfWork.Context.BankDiskettes
                .Where(d => d.Id == bankDisketteId)
                .ExecuteUpdate(s => s
                    .SetProperty(d => d.BankDisketteStatusId, statusId)
                    .SetProperty(d => d.LastModifiedDate, now));
        }

        private void EnsureBatchRequestNotLeftRunning(long requestId, long? bankDisketteId, string message)
        {
            try
            {
                var currentState = GetBatchRequestStateFromDatabase(requestId);
                if (currentState != (long)Enums.BatchPayRollRequestState.Running)
                {
                    return;
                }

                _logger.LogError(
                    "درخواست دیسکت بانک {RequestId} هنوز Running است — اجبار به EndLoop",
                    requestId);

                var now = DateTime.Now;
                _unitOfWork.Context.BatchPayRollRequests
                    .Where(r => r.Id == requestId
                                && r.RequestStateId == (long)Enums.BatchPayRollRequestState.Running)
                    .ExecuteUpdate(s => s
                        .SetProperty(r => r.RequestStateId, (long)Enums.BatchPayRollRequestState.EndLoop)
                        .SetProperty(r => r.FinishDateTime, now)
                        .SetProperty(r => r.LastPoolingTime, now)
                        .SetProperty(r => r.LastModifiedDate, now)
                        .SetProperty(r => r.ExeptionMessage, message));

                if (bankDisketteId.HasValue && bankDisketteId.Value > 0)
                {
                    SetBankDisketteStatusInDatabase(
                        bankDisketteId.Value,
                        (long)Enums.BankDisketteStatus.CalculationFinished);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "نتوانستیم وضعیت گیرکرده درخواست {RequestId} را از Running خارج کنیم",
                    requestId);
            }
        }

        private void CompleteBankDisketteRequest(
            BatchPayRollRequest batchRequest,
            BankDiskette bankDiskette,
            string? summaryMessage)
        {
            SetBatchRequestStateInDatabase(
                batchRequest,
                Enums.BatchPayRollRequestState.EndLoop,
                summaryMessage,
                setFinishDate: true);

            SetBankDisketteStatusInDatabase(
                bankDiskette.Id,
                (long)Enums.BankDisketteStatus.CalculationFinished);

            bankDiskette.BankDisketteStatusId = (long)Enums.BankDisketteStatus.CalculationFinished;
            bankDiskette.LastModifiedDate = DateTime.Now;
        }

        private void FailBankDisketteRequest(
            BatchPayRollRequest batchRequest,
            BankDiskette? bankDiskette,
            string userMessage)
        {
            _logger.LogWarning("درخواست دیسکت بانک {RequestId} با پیام کاربر متوقف شد: {Message}",
                batchRequest.Id, userMessage);

            try
            {
                SetBatchRequestStateInDatabase(
                    batchRequest,
                    Enums.BatchPayRollRequestState.EndLoop,
                    userMessage,
                    setFinishDate: true);

                if (bankDiskette != null
                    && bankDiskette.BankDisketteStatusId != (long)Enums.BankDisketteStatus.Payed
                    && bankDiskette.BankDisketteStatusId != (long)Enums.BankDisketteStatus.Deleted)
                {
                    SetBankDisketteStatusInDatabase(
                        bankDiskette.Id,
                        (long)Enums.BankDisketteStatus.CalculationFinished);
                    bankDiskette.BankDisketteStatusId = (long)Enums.BankDisketteStatus.CalculationFinished;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FailBankDisketteRequest برای درخواست {RequestId} شکست خورد — Ensure اجرا می‌شود", batchRequest.Id);
                EnsureBatchRequestNotLeftRunning(batchRequest.Id, bankDiskette?.Id, userMessage);
            }
        }

        private bool IsRequestCancelledByUser(long requestId)
        {
            return GetBatchRequestStateFromDatabase(requestId) == (long)Enums.BatchPayRollRequestState.CancelByUser;
        }

        private void MarkRemainingFichesAsSkipped(
            BatchPayRollRequest batchRequest,
            List<Fiche> involvedFiches,
            long fromFicheIdInclusive,
            string message)
        {
            var remaining = involvedFiches
                .Where(f => f.Id >= fromFicheIdInclusive)
                .ToList();

            // اگر Idها ترتیبی نیستند، از ایندکس استفاده کن
            var startIndex = involvedFiches.FindIndex(f => f.Id == fromFicheIdInclusive);
            if (startIndex >= 0)
            {
                remaining = involvedFiches.Skip(startIndex).ToList();
            }

            foreach (var fiche in remaining)
            {
                PersistBatchRequestDetailMessage(
                    batchRequest.Id,
                    fiche.EmployeeId,
                    fiche.Id,
                    message,
                    DateTime.Now,
                    bankDisketteItemId: null,
                    isFailure: true);
            }
        }

        private void EnsureAllFichesHaveDetailLogs(BatchPayRollRequest batchRequest, List<Fiche> involvedFiches)
        {
            try
            {
                var existingKeys = _unitOfWork.Context.BatchPayRollRequestDetails
                    .AsNoTracking()
                    .Where(d => d.BatchPayRollRequestId == batchRequest.Id)
                    .Select(d => new { d.EmployeeId, d.FicheId })
                    .ToList()
                    .Select(d => (d.EmployeeId, FicheId: d.FicheId ?? 0))
                    .ToHashSet();

                foreach (var fiche in involvedFiches)
                {
                    if (existingKeys.Contains((fiche.EmployeeId, fiche.Id)))
                    {
                        continue;
                    }

                    PersistBatchRequestDetailMessage(
                        batchRequest.Id,
                        fiche.EmployeeId,
                        fiche.Id,
                        "برای این فیش جزئیاتی ثبت نشده بود. احتمالاً پردازش قبل از ثبت لاگ متوقف شده است. لطفاً «تلاش مجدد» بزنید.",
                        DateTime.Now,
                        bankDisketteItemId: null,
                        isFailure: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "خطا در تضمین ثبت لاگ جزئیات برای درخواست {RequestId}",
                    batchRequest.Id);
            }
        }

        /// <summary>
        /// ثبت/به‌روزرسانی پیام جزئیات درخواست — مستقل از تراکنش و ChangeTracker.
        /// برای هر نفر باید حتماً یک ردیف در نظارت دیده شود.
        /// </summary>
        private void PersistBatchRequestDetailMessage(
            long batchRequestId,
            long employeeId,
            long ficheId,
            string message,
            DateTime startedAt,
            long? bankDisketteItemId,
            bool isFailure)
        {
            try
            {
                SafeRollback();
                _unitOfWork.Context.ChangeTracker.Clear();

                var now = DateTime.Now;
                var elapsed = Math.Max(0, (now - startedAt).TotalMilliseconds);

                var existing = _unitOfWork.Context.BatchPayRollRequestDetails
                    .FirstOrDefault(d => d.BatchPayRollRequestId == batchRequestId
                                         && d.EmployeeId == employeeId
                                         && d.FicheId == ficheId);

                if (existing == null)
                {
                    existing = new BatchPayRollRequestDetail
                    {
                        BatchPayRollRequestId = batchRequestId,
                        EmployeeId = employeeId,
                        FicheId = ficheId,
                        CreateDate = now,
                        IPAddress = WorkerIpAddress,
                        FinalMessage = message,
                        DoDatetime = now,
                        LastTryDateTime = isFailure ? now : null,
                        RunTimeinMilliseconds = elapsed,
                        BankDisketteItemId = bankDisketteItemId,
                    };
                    _unitOfWork.Context.BatchPayRollRequestDetails.Add(existing);
                }
                else
                {
                    existing.FinalMessage = message;
                    existing.DoDatetime = now;
                    existing.LastModifiedDate = now;
                    existing.RunTimeinMilliseconds = elapsed;
                    if (isFailure)
                    {
                        existing.LastTryDateTime = now;
                    }

                    if (bankDisketteItemId.HasValue)
                    {
                        existing.BankDisketteItemId = bankDisketteItemId;
                    }

                    _unitOfWork.Context.Update(existing);
                }

                SaveChangesWithAutoDetect();
                _unitOfWork.Context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "خطا در PersistBatchRequestDetailMessage برای Request={RequestId}, Fiche={FicheId}, Employee={EmployeeId}",
                    batchRequestId, ficheId, employeeId);

                // آخرین تلاش: فقط پیام را با ExecuteUpdate اگر ردیف وجود دارد
                try
                {
                    var now = DateTime.Now;
                    var updated = _unitOfWork.Context.BatchPayRollRequestDetails
                        .Where(d => d.BatchPayRollRequestId == batchRequestId
                                    && d.EmployeeId == employeeId
                                    && d.FicheId == ficheId)
                        .ExecuteUpdate(s => s
                            .SetProperty(d => d.FinalMessage, message)
                            .SetProperty(d => d.DoDatetime, now)
                            .SetProperty(d => d.LastModifiedDate, now)
                            .SetProperty(d => d.LastTryDateTime, isFailure ? now : null));

                    if (updated == 0)
                    {
                        _logger.LogCritical(
                            "هیچ ردیف جزئیاتی برای Request={RequestId}, Fiche={FicheId} ذخیره نشد. پیام: {Message}",
                            batchRequestId, ficheId, message);
                    }
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogCritical(fallbackEx,
                        "fallback ثبت لاگ جزئیات نیز شکست خورد. Request={RequestId}, Fiche={FicheId}",
                        batchRequestId, ficheId);
                }
            }
        }

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

        private void WithTrackingEnabled(Action action)
        {
            var previousAutoDetect = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
            var previousTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

            try
            {
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = true;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                action();
            }
            finally
            {
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = previousTracking;
            }
        }

        private void SafeRollback()
        {
            try
            {
                _unitOfWork.Rollback();
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "خطا در Rollback تراکنش دیسکت بانک");
            }
        }

        private static string BuildUserFriendlyErrorMessage(Exception ex, string context)
        {
            if (ex is InvalidOperationException invalidOpEx && !string.IsNullOrWhiteSpace(invalidOpEx.Message))
            {
                return invalidOpEx.Message;
            }

            var message = ex.InnerException?.Message ?? ex.Message ?? string.Empty;

            if (message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
                || message.Contains("timed out", StringComparison.OrdinalIgnoreCase))
            {
                return "ارتباط با پایگاه داده کند یا قطع شد. لطفاً چند دقیقه بعد دوباره «تلاش مجدد» را بزنید.";
            }

            if (message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
            {
                return "سیستم موقتاً مشغول است. لطفاً چند دقیقه بعد دوباره تلاش کنید.";
            }

            if (message.Contains("foreign key", StringComparison.OrdinalIgnoreCase))
            {
                return "برخی اطلاعات پایه (بانک، شعبه، قالب دیسکت یا مرکز هزینه) ناقص یا حذف شده است. تنظیمات پایه را بررسی کنید.";
            }

            if (message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || message.Contains("unique", StringComparison.OrdinalIgnoreCase))
            {
                return "رکورد تکراری در دیسکت بانک شناسایی شد. در صورت تکرار، با پشتیبانی تماس بگیرید.";
            }

            return $"{context} با خطا مواجه شد. لطفاً جزئیات را در لیست ریز عملیات ببینید یا با پشتیبانی تماس بگیرید.";
        }

        private sealed record BankDisketteReferenceData(
            Dictionary<long, string> Banks,
            Dictionary<long, BankDisketteTemplate> TemplatesByBankId,
            Dictionary<long, List<BankDisketteTemplateRow>> TemplateRowsByTemplateId);

        public new OperationResult CreateForAsync(BankDisketteDTO entityToCreate)
        {
            try
            {
                _logger.LogInformation("CreateForAsync started for PaymentPeriodId: {PaymentPeriodId}", entityToCreate?.PaymentPeriodId);

                // بررسی null بودن DTO
                if (entityToCreate == null)
                {
                    _logger.LogWarning("CreateForAsync: entityToCreate is null");
                    return OperationResult.Failed("اطلاعات دیسکت بانک ارسال نشده است");
                }

                if (entityToCreate.PaymentPeriodId > 0)
                {
                    List<long> invalidStatusList = new()
                       {
                       (long)Enums.BankDisketteStatus.Initial,
                       (long)Enums.BankDisketteStatus.CalculationFinished,
                       (long)Enums.BankDisketteStatus.Payed,
                       };

                    //if (_unitOfWork.Context.BankDiskettes.Any(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && invalidStatusList.Contains(i.BankDisketteStatusId)))
                    //{
                    //    return OperationResult.Failed("برای دوره ارسالی دیسکت بانک وجود دارد لطفا ابتدا دیسکت قبل را حذف بفرمایید");
                    //}

                    // بررسی وضعیت دوره
                    PaymentPeriod period;
                    try
                    {
                        period = _unitOfWork.Context.PaymentPeriods.Find(entityToCreate.PaymentPeriodId);
                        if (period == null)
                        {
                            _logger.LogWarning("CreateForAsync: PaymentPeriod not found for Id: {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                            return OperationResult.Failed("دوره پرداخت یافت نشد");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CreateForAsync: Error finding PaymentPeriod {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                        return OperationResult.Failed("خطا در دریافت اطلاعات دوره پرداخت");
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
                List<OrganisationCostCenter> costCenterList = new List<OrganisationCostCenter>();
                var mappedTodo = _mapper.Map<BankDiskette>(entityToCreate);
                if (mappedTodo == null)
                {
                    _logger.LogError("CreateForAsync: Mapper returned null for entityToCreate");
                    return OperationResult.Failed("خطا در تبدیل اطلاعات دیسکت بانک");
                }

                mappedTodo.BankDisketteStatusId = (long)Enums.BankDisketteStatus.Initial;

                // دریافت فیش‌ها
                List<Fiche> fiche;
                try
                {
                    fiche = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == entityToCreate.PaymentPeriodId && (i.FicheStatusId == (long)Enums.FicheStatus.Payed || i.FicheStatusId == (long)Enums.FicheStatus.Initial)).ToList();
                    _logger.LogInformation("CreateForAsync: Found {FicheCount} fiches for period {PaymentPeriodId}", fiche?.Count ?? 0, entityToCreate.PaymentPeriodId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateForAsync: Error fetching fiches for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                    return OperationResult.Failed("خطا در دریافت فیش‌های حقوقی");
                }

                // محاسبه تعداد و مبلغ
                try
                {
                    if (entityToCreate.CalculateAllFichesInCurrentPeriod == true)
                    {
                        mappedTodo.AllPersonnelCount = fiche.Count();
                        mappedTodo.SumPaymentAmount = fiche.Sum(i => i.PurePaymentAmount);
                        _logger.LogInformation("CreateForAsync: Calculate all fiches. Count: {Count}, Sum: {Sum}", mappedTodo.AllPersonnelCount, mappedTodo.SumPaymentAmount);
                    }
                    else
                    {
                        if (_organisationCostCenterService == null)
                        {
                            _logger.LogError("CreateForAsync: _organisationCostCenterService is null");
                            return OperationResult.Failed("خطا در دسترسی به سرویس مراکز هزینه");
                        }

                        if (entityToCreate.CostCenterIdList == null || !entityToCreate.CostCenterIdList.Any())
                        {
                            _logger.LogWarning("CreateForAsync: CostCenterIdList is null or empty");
                            return OperationResult.Failed("فهرست مراکز هزینه ارسال نشده است");
                        }

                        var allCostCenters = _organisationCostCenterService.All();
                        if (allCostCenters == null)
                        {
                            _logger.LogError("CreateForAsync: _organisationCostCenterService.All() returned null");
                            return OperationResult.Failed("خطا در دریافت فهرست مراکز هزینه");
                        }

                        costCenterList = allCostCenters.Where(i => entityToCreate.CostCenterIdList.Contains(i.Id)).ToList();
                        _logger.LogInformation("CreateForAsync: Found {CostCenterCount} cost centers", costCenterList?.Count ?? 0);

                        var idList = costCenterList.Select(i => i.CostCenterId).Distinct().ToList();
                        var includeFiches = fiche.Where(i => idList.Contains(i.CostCenterId));
                        mappedTodo.AllPersonnelCount = includeFiches.Count();
                        mappedTodo.SumPaymentAmount = includeFiches.Sum(i => i.PurePaymentAmount);
                        _logger.LogInformation("CreateForAsync: Filtered fiches. Count: {Count}, Sum: {Sum}", mappedTodo.AllPersonnelCount, mappedTodo.SumPaymentAmount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateForAsync: Error calculating fiches for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                    return OperationResult.Failed("خطا در محاسبه اطلاعات فیش‌ها");
                }

                mappedTodo.BatchPayRollRequestId = null;

                // شروع تراکنش
                _logger.LogInformation("CreateForAsync: Starting database transaction for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                _unitOfWork.CreateTransaction();
                try
                {
                    // تنظیم OrganisationChartId
                    if (typeof(BankDiskette).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                    {
                        if (_currentUserDefaultOrganId > 0)
                        {
                            PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                            if (propertyInfo == null)
                            {
                                _logger.LogError("CreateForAsync: OrganisationChartId property not found on BankDiskette");
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("خطای سیستمی: خاصیت OrganisationChartId یافت نشد");
                            }

                            propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                            _logger.LogInformation("CreateForAsync: Set OrganisationChartId to {OrganId}", _currentUserDefaultOrganId);
                        }
                        else
                        {
                            _logger.LogError("CreateForAsync: _currentUserDefaultOrganId is not set");
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("سازمان پیش فرض مشخص نشده است");
                        }
                    }

                    // ذخیره دیسکت بانک
                    try
                    {
                        mappedTodo.IPAddress = "";
                        mappedTodo.CreateDate = DateTime.Now;
                        _unitOfWork.Context.BankDiskettes.Add(mappedTodo);
                        _unitOfWork.Context.SaveChanges();
                        _logger.LogInformation("CreateForAsync: BankDiskette saved with Id: {DisketteId}", mappedTodo.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CreateForAsync: Error saving BankDiskette for period {PaymentPeriodId}", entityToCreate.PaymentPeriodId);
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("خطا در ذخیره دیسکت بانک");
                    }

                    // ذخیره مراکز هزینه
                    if (entityToCreate.CalculateAllFichesInCurrentPeriod == true)
                    {
                        entityToCreate.CostCenterIdList = new List<long>() { };
                        _logger.LogInformation("CreateForAsync: Calculate all fiches mode, no cost centers to save");
                    }
                    else
                    {
                        if (entityToCreate.CostCenterIdList == null || !entityToCreate.CostCenterIdList.Any())
                        {
                            _logger.LogWarning("CreateForAsync: CostCenterIdList is null or empty in non-all-fiches mode");
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("فهرست مراکز هزینه ارسال نشده است");
                        }

                        try
                        {
                            List<BankDisketteCostCenter> CostCenterList = new List<BankDisketteCostCenter>();
                            foreach (var CostCenter in costCenterList)
                            {
                                BankDisketteCostCenter toAdd = new BankDisketteCostCenter()
                                {
                                    BankDisketteId = mappedTodo.Id,
                                    CostCenterId = CostCenter.CostCenterId,
                                    CreateDate = DateTime.Now,
                                    IPAddress = "",
                                };
                                CostCenterList.Add(toAdd);
                            }
                            _unitOfWork.Context.BankDisketteCostCenters.AddRange(CostCenterList);
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
                            RequestTypeId = (long)Enums.BatchPayRollRequestType.BankDisketteCalculation,
                            EmployeeCount = mappedTodo.AllPersonnelCount,
                            IPAddress = "",
                            CreateDate = DateTime.Now,
                            OrganisationChartId = mappedTodo.OrganisationChartId,
                            PaymentPeriodId = mappedTodo.PaymentPeriodId,
                            RequestStateId = (long)Enums.BatchPayRollRequestState.Initial,
                            UserId = _userResolverService.GetUserId(),
                            Username = string.IsNullOrWhiteSpace(_userResolverService.fullname()) ? _userResolverService.GetUser() : _userResolverService.fullname(),
                            SuccessCount = 0,
                            RequsetDescription = "به درخواست صدور دیسکت بانک",
                            BankDisketteId = mappedTodo.Id
                        };
                        _unitOfWork.Context.BatchPayRollRequests.Add(batch);
                        _unitOfWork.Context.SaveChanges();
                        _logger.LogInformation("CreateForAsync: BatchPayRollRequest created with Id: {BatchId}", batch.Id);

                        // به‌روزرسانی دیسکت با شناسه batch
                        mappedTodo.BatchPayRollRequestId = batch.Id;
                        _unitOfWork.Context.Update(mappedTodo);
                        _unitOfWork.Context.SaveChanges();
                        _logger.LogInformation("CreateForAsync: Updated BankDiskette {DisketteId} with BatchPayRollRequestId: {BatchId}", mappedTodo.Id, batch.Id);

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
                    return OperationResult.Failed("خطای غیرمنتظره در ایجاد دیسکت بانک");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateForAsync: Unhandled outer exception for PaymentPeriodId: {PaymentPeriodId}", entityToCreate?.PaymentPeriodId);
                return OperationResult.Failed($"خطای سیستمی در ایجاد دیسکت بانک: {ex.Message}");
            }
        }
        public new OperationResult DeleteRecord(long Id)
        {
            var existingDiskette = _unitOfWork.Context.BankDiskettes.Find(Id);
            if (existingDiskette == null)
            {
                return OperationResult.NotFound();
            }
            else
            {
                if (existingDiskette.BankDisketteStatusId == (long)Enums.BankDisketteStatus.Payed || existingDiskette.BankDisketteStatusId == (long)Enums.BankDisketteStatus.Deleted)
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
                    existingDiskette.BankDisketteStatusId = (long)Enums.BankDisketteStatus.Deleted;
                    _unitOfWork.Context.BankDiskettes.Update(existingDiskette);
                    _unitOfWork.Context.SaveChanges();
                    return OperationResult.Succeeded();
                }
            }
        }

        public OperationResult downloadBankDisk(long Id)
        {
            var diskList = _unitOfWork.Context.BankDisketteGroupAndFiles.Find(Id);
            if (diskList == null)
            {
                return OperationResult.NotFound();
            }
            else
            {
                return OperationResult.Succeeded(payload: diskList);
            }
        }
        public OperationResult CheckIfPeriodIsValidForCreateBankDisk(long PaymentPeriodId)
        {
            try
            {
                _logger.LogInformation("CheckIfPeriodIsValidForCreateBankDisk: Checking period {PaymentPeriodId}", PaymentPeriodId);

                if (PaymentPeriodId <= 0)
                {
                    _logger.LogWarning("CheckIfPeriodIsValidForCreateBankDisk: Invalid PaymentPeriodId: {PaymentPeriodId}", PaymentPeriodId);
                    return OperationResult.Failed("شناسه دوره نامعتبر است");
                }

                List<long> invalidStatusList = new List<long>()
                {
                    (long)Enums.BankDisketteStatus.Initial,
                    (long)Enums.BankDisketteStatus.CalculationFinished,
                    (long)Enums.BankDisketteStatus.Payed,
                };
                //var existingBankDiskette = _unitOfWork.Context.BankDiskettes.Where(i => invalidStatusList.Contains(i.BankDisketteStatusId) && i.PaymentPeriodId == PaymentPeriodId);
                //if (existingBankDiskette == null)
                //{

                //}
                //else
                //{
                //    if (existingBankDiskette.Any())
                //    {
                //        return OperationResult.Failed("برای دوره مورد نظر دیسکت وجود دارد");
                //    }
                //}

                // بررسی وجود دوره
                PaymentPeriod PaymentPeriod;
                try
                {
                    PaymentPeriod = _unitOfWork.Context.PaymentPeriods.Find(PaymentPeriodId);
                    if (PaymentPeriod == null)
                    {
                        _logger.LogWarning("CheckIfPeriodIsValidForCreateBankDisk: PaymentPeriod not found: {PaymentPeriodId}", PaymentPeriodId);
                        return OperationResult.Failed("دوره پرداخت یافت نشد");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CheckIfPeriodIsValidForCreateBankDisk: Error finding PaymentPeriod {PaymentPeriodId}", PaymentPeriodId);
                    return OperationResult.Failed("خطا در دریافت اطلاعات دوره پرداخت");
                }

                // بررسی سازمان
                if (PaymentPeriod.OrganisationChartId != _currentUserDefaultOrganId)
                {
                    _logger.LogWarning("CheckIfPeriodIsValidForCreateBankDisk: OrganisationChartId mismatch. Period OrganId: {PeriodOrganId}, User OrganId: {UserOrganId}",
                        PaymentPeriod.OrganisationChartId, _currentUserDefaultOrganId);
                    return OperationResult.Failed("محل پرداخت پیش فرض با دوره ارسالی مطابقت ندارد");
                }

                // بررسی وضعیت بسته بودن دوره
                if (PaymentPeriod.IsClosed == true)
                {
                    _logger.LogWarning("CheckIfPeriodIsValidForCreateBankDisk: Period {PaymentPeriodId} is closed", PaymentPeriodId);
                    return OperationResult.Failed("دوره مورد نظر بسته است و امکان تهیه دیسکت وجود ندارد");
                }

                // بررسی وجود فیش
                try
                {
                    var ficheCount = _unitOfWork.Context.Fiches.Where(i => i.PaymentPeriodId == PaymentPeriod.Id && (i.FicheStatusId == (long)Enums.FicheStatus.Initial || i.FicheStatusId == (long)Enums.FicheStatus.Payed)).Count();
                    _logger.LogInformation("CheckIfPeriodIsValidForCreateBankDisk: Found {FicheCount} fiches for period {PaymentPeriodId}", ficheCount, PaymentPeriodId);

                    if (ficheCount > 0)
                    {
                        return OperationResult.Succeeded("تعداد " + ficheCount + " فیش با وضعیت محاسبه اولیه برای دوره انتخابی یافت شد ");
                    }
                    else
                    {
                        _logger.LogWarning("CheckIfPeriodIsValidForCreateBankDisk: No fiches found for period {PaymentPeriodId}", PaymentPeriodId);
                        return OperationResult.Failed("برای دوره انتخابی هیچ فیش محاسبه شده ای یافت نشد");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CheckIfPeriodIsValidForCreateBankDisk: Error counting fiches for period {PaymentPeriodId}", PaymentPeriodId);
                    return OperationResult.Failed("خطا در شمارش فیش‌های دوره");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckIfPeriodIsValidForCreateBankDisk: Unhandled exception for period {PaymentPeriodId}", PaymentPeriodId);
                return OperationResult.Failed("خطای غیرمنتظره در بررسی اعتبار دوره");
            }
        }
        public bool Validate(BankDiskette entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
