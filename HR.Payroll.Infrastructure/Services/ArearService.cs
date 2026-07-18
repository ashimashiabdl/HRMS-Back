using AutoMapper;
using HR.Order.Core.Data;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace HR.Payroll.Infrastructure.Services
{
    /// <summary>
    /// سرویس محاسبه گروهی معوقه
    /// </summary>
    public class ArearService : BaseService<Arear, PayrollContext, ArearDTO>, IScopedServices
    {
        private readonly OrderService _orderService;
        private readonly FicheService _ficheService;
        private readonly ILogger<ArearService> _logger;

        private static readonly long[] OrderValidStatesForArears =
        {
            (long)Enums.OrderStatus.LastOrder,
            (long)Enums.OrderStatus.FinalOrder
        };

        /// <summary>
        /// حداکثر تعداد تلاش برای هر حکم؛ بعد از آن حکم تا اصلاح مجدد (تغییر LastModifiedDate) دیگر پردازش و لاگ نمی‌شود
        /// </summary>
        private const int MaxAttemptsPerOrder = 2;

        /// <summary>
        /// عنوان رکورد نشانگر «تلاش پردازش» در جدول Batch_Log؛ شمارش تلاش‌ها بر اساس همین رکوردها انجام می‌شود
        /// </summary>
        private const string AttemptMarkerTitle = "ArearService.Attempt";

        public ArearService(
            IMapper mapper,
            FicheService ficheService,
            OrderService orderService,
            IUnitOfWork<PayrollContext> unitOfWork,
            IDapper dapper,
            IConfiguration configuration,
            UserResolverService userService,
            ILogger<ArearService> logger)
            : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _ficheService = ficheService;
            _orderService = orderService;
            _logger = logger;
        }

        public bool Validate(Arear entity, object etc = null)
        {
            return true;
        }

        /// <summary>
        /// سرکشی احکامی که نیاز به محاسبه معوقه دارند و تولید فیش معوقه + اقلام تغییر یافته
        /// </summary>
        public void CheckArearsAndCalculate()
        {
            var runId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _unitOfWork.Context.ChangeTracker.Clear();

                var needToCheckOrders = LoadOrdersNeedingArearCalculation();

                // حذف احکامی که به سقف تلاش رسیده‌اند تا در هر چرخه دوباره پردازش و لاگ نشوند
                var attemptCounts = LoadAttemptCountsFromBatchLog(needToCheckOrders);
                var eligibleOrders = needToCheckOrders
                    .Where(o => attemptCounts.GetValueOrDefault(o.Id) < MaxAttemptsPerOrder)
                    .ToList();

                if (eligibleOrders.Count == 0)
                {
                    // بدون کاندید جدید؛ عمداً هیچ BatchLog یا لاگ Information ثبت نمی‌شود تا لاگ تکراری تولید نشود
                    _logger.LogDebug("[{RunId}] حکم واجد شرایط جدیدی برای محاسبه معوقه وجود ندارد (کل کاندید: {Total})",
                        runId, needToCheckOrders.Count);
                    return;
                }

                _logger.LogInformation("[{RunId}] شروع CheckArearsAndCalculate — تعداد {Count} حکم جهت محاسبه معوقه", runId, eligibleOrders.Count);
                AddBatchLog(new BatchLog
                {
                    LogDescription = $"[{runId}] تعداد {eligibleOrders.Count} حکم جهت محاسبه معوقه یافت شد"
                });

                foreach (var order in eligibleOrders
                    .OrderBy(i => i.RecruitOrder?.EmployeeId ?? 0)
                    .ThenBy(i => i.Serial))
                {
                    var attempt = attemptCounts.GetValueOrDefault(order.Id) + 1;
                    RecordAttemptMarker(order, runId, attempt);

                    var success = ProcessSingleOrderArear(order, runId);

                    if (!success && attempt >= MaxAttemptsPerOrder)
                    {
                        var msg = $"حکم {order.Id} پس از {attempt} تلاش ناموفق از چرخه محاسبه معوقه کنار گذاشته شد؛ تا اصلاح مجدد حکم دیگر پردازش نمی‌شود.";
                        _logger.LogWarning("[{RunId}] {Message}", runId, msg);
                        AddBatchLog(new BatchLog
                        {
                            EmployeeId = order.RecruitOrder?.EmployeeId,
                            InterdictOrderId = order.Id,
                            LogDescription = $"[{runId}] {msg}"
                        });
                    }
                }

                _logger.LogInformation("[{RunId}] پایان CheckArearsAndCalculate", runId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RunId}] خطای کلی در CheckArearsAndCalculate: {Message}", runId, ex.Message);
                TryAddBatchLog(new BatchLog
                {
                    LogDescription = $"[{runId}] خطای کلی سرویس محاسبه معوقه: {BuildRootCause(ex)}"
                });
            }
        }

        private List<InterdictOrder> LoadOrdersNeedingArearCalculation()
        {
            return _unitOfWork.Context.InterdictOrders
                .Include(i => i.RecruitOrder)
                .Where(i =>
                    i.IsArrears == true
                    && i.PayRollAprove == true
                    && i.PayRollAproveDate != null
                    && i.ApproveTimePaymentPeriod > 0
                    && i.ArearsStatusId == (long)Enums.ArearsStatus.NeedToCalculate
                    && OrderValidStatesForArears.Contains(i.StatusId)
                    && i.IsDeleted != true)
                .AsNoTracking()
                .ToList();
        }

        /// <summary>
        /// شمارش تلاش‌های قبلی هر حکم بر اساس رکوردهای نشانگر ثبت‌شده در جدول Batch_Log.
        /// فقط تلاش‌های بعد از آخرین ویرایش حکم شمرده می‌شوند؛ بنابراین با اصلاح حکم (تغییر LastModifiedDate)
        /// شمارنده صفر شده و حکم دوباره دو فرصت پردازش پیدا می‌کند.
        /// </summary>
        private Dictionary<long, int> LoadAttemptCountsFromBatchLog(List<InterdictOrder> orders)
        {
            if (orders.Count == 0)
            {
                return new Dictionary<long, int>();
            }

            var orderIds = orders.Select(o => o.Id).ToList();

            var markers = _unitOfWork.Context.BatchLogs
                .Where(b =>
                    b.title == AttemptMarkerTitle
                    && b.IsDeleted != true
                    && b.InterdictOrderId != null
                    && orderIds.Contains(b.InterdictOrderId.Value))
                .Select(b => new { b.InterdictOrderId, b.CreateDate })
                .AsNoTracking()
                .ToList();

            return orders.ToDictionary(
                o => o.Id,
                o => markers.Count(m =>
                    m.InterdictOrderId == o.Id
                    && (o.LastModifiedDate == null || m.CreateDate >= o.LastModifiedDate)));
        }

        /// <summary>
        /// ثبت رکورد نشانگر تلاش در Batch_Log (مبنای شمارش سقف 2 تلاش برای هر حکم)
        /// </summary>
        private void RecordAttemptMarker(InterdictOrder order, string runId, int attempt)
        {
            AddBatchLog(new BatchLog
            {
                title = AttemptMarkerTitle,
                EmployeeId = order.RecruitOrder?.EmployeeId,
                InterdictOrderId = order.Id,
                LogDescription = $"[{runId}] تلاش شماره {attempt} از {MaxAttemptsPerOrder} برای محاسبه معوقه حکم {order.Id}"
            });
        }

        /// <returns>true فقط وقتی محاسبه و ذخیره معوقه کامل موفق باشد</returns>
        private bool ProcessSingleOrderArear(InterdictOrder order, string runId)
        {
            var employeeId = order.RecruitOrder?.EmployeeId;
            var orderCtx = $"حکمId={order.Id}، سریال={order.Serial}، کارمندId={employeeId?.ToString() ?? "نامشخص"}";

            try
            {
                if (order.RecruitOrder == null)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: برای {orderCtx} رکورد استخدام (RecruitOrder) بارگذاری نشد؛ بدون آن محل خدمت/نوع استخدام مشخص نیست.");
                    return false;
                }

                if (employeeId is null or <= 0)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: برای {orderCtx} شناسه کارمند روی حکم استخدام خالی است.");
                    return false;
                }

                if (HasExistingArearFiches(order.Id))
                {
                    SkipOrder(order, runId, employeeId.Value,
                        $"رد شد: برای {orderCtx} قبلاً فیش معوقه ثبت شده است. در صورت نیاز ابتدا فیش‌های معوقه قبلی را بررسی/حذف کنید.");
                    return false;
                }

                if (!order.ApproveTimePaymentPeriod.HasValue || order.ApproveTimePaymentPeriod.Value <= 0)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: برای {orderCtx} فیلد ApproveTimePaymentPeriod (دوره حقوقی زمان تایید حکم) خالی است؛ بدون آن دوره‌های بسته قبلی قابل شناسایی نیستند.",
                        employeeId);
                    return false;
                }

                var relatedPeriod = _unitOfWork.Context.PaymentPeriods.Find(order.ApproveTimePaymentPeriod.Value);
                if (relatedPeriod == null)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: برای {orderCtx} دوره حقوقی با شناسه {order.ApproveTimePaymentPeriod.Value} در جدول PaymentPeriod یافت نشد (ممکن است حذف شده باشد).",
                        employeeId);
                    return false;
                }

                if (relatedPeriod.IsClosed)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: دوره «{relatedPeriod.title}» (Id={relatedPeriod.Id}) که مبنای تایید حکم است بسته شده؛ محاسبه معوقه فقط وقتی دوره تایید باز باشد مجاز است.",
                        employeeId, relatedPeriod.Id);
                    return false;
                }

                var blockingHigherSerial = FindBlockingHigherSerialOrders(order);
                if (blockingHigherSerial.Count > 0)
                {
                    var detail = string.Join(" | ", blockingHigherSerial.Select(o =>
                        $"حکمId={o.Id} سریال={o.Serial} PayRollAprove={o.PayRollAprove}"));
                    FailOrder(order, runId,
                        $"ریشه مشکل: برای کارمند {employeeId} احکام با سریال بزرگ‌تر از {order.Serial} هنوز در کارتابل حقوق تعیین تکلیف نشده‌اند. ابتدا آن‌ها را تایید/رد کنید. جزئیات: {detail}",
                        employeeId);
                    return false;
                }

                var includePeriods = FindClosedPeriodsForArear(order, relatedPeriod);
                if (includePeriods.Count == 0)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: هیچ دوره بسته قبلی برای محاسبه معوقه یافت نشد. شرط: سال={relatedPeriod.ShamsiYear}، ماه < {relatedPeriod.ShamsiMonth}، بسته بودن دوره، StartDate بین تاریخ اجرای حکم ({order.StartDate:yyyy/MM/dd}) و شروع دوره تایید ({relatedPeriod.StartDate:yyyy/MM/dd}).",
                        employeeId, relatedPeriod.Id);
                    return false;
                }

                LogOrder(order, runId, employeeId.Value,
                    $"دوره‌های مشمول محاسبه ({includePeriods.Count}): {string.Join("، ", includePeriods.Select(p => p.title))}",
                    relatedPeriod.Id);

                var siblingOrders = LoadSiblingOrders(order);
                var calculatedFiches = new List<FicheDTO>();
                var skippedPeriods = new List<string>();

                foreach (var period in includePeriods)
                {
                    var periodResult = CalculateArearFicheForPeriod(order, period, siblingOrders, runId);
                    if (periodResult.Success && periodResult.Payload != null)
                    {
                        calculatedFiches.Add((FicheDTO)periodResult.Payload);
                    }
                    else
                    {
                        skippedPeriods.Add($"{period.title}: {periodResult.Message}");
                        LogOrder(order, runId, employeeId.Value,
                            $"دوره «{period.title}» محاسبه نشد — {periodResult.Message}",
                            period.Id);
                    }
                }

                if (calculatedFiches.Count == 0)
                {
                    FailOrder(order, runId,
                        $"ریشه مشکل: هیچ فیش معوقه‌ای برای {orderCtx} محاسبه نشد. جزئیات دوره‌ها: {string.Join(" ؛ ", skippedPeriods)}",
                        employeeId, relatedPeriod.Id);
                    return false;
                }

                var persistResult = PersistArearCalculation(order, calculatedFiches, relatedPeriod, runId, skippedPeriods);
                if (!persistResult.Success)
                {
                    FailOrder(order, runId,
                        persistResult.Message ?? "خطای نامشخص در ذخیره فیش معوقه",
                        employeeId, relatedPeriod.Id);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RunId}] خطا در پردازش {OrderCtx}", runId, orderCtx);
                FailOrder(order, runId,
                    $"ریشه مشکل (استثنا): {BuildRootCause(ex)} — زمینه: {orderCtx}",
                    order.RecruitOrder?.EmployeeId);
                return false;
            }
        }

        private bool HasExistingArearFiches(long interdictOrderId)
        {
            return _unitOfWork.Context.ArearFiches
                .Any(i => i.InterdictOrderId == interdictOrderId && i.IsDeleted != true);
        }

        private List<InterdictOrder> FindBlockingHigherSerialOrders(InterdictOrder order)
        {
            var employeeId = order.RecruitOrder!.EmployeeId;
            return _unitOfWork.Context.InterdictOrders
                .Include(i => i.RecruitOrder)
                .Where(i =>
                    OrderValidStatesForArears.Contains(i.StatusId)
                    && i.Id != order.Id
                    && i.RecruitOrder != null
                    && i.RecruitOrder.EmployeeId == employeeId
                    && i.Serial > order.Serial
                    && i.PayRollAprove != true
                    && i.IsDeleted != true)
                .AsNoTracking()
                .ToList();
        }

        private List<PaymentPeriod> FindClosedPeriodsForArear(InterdictOrder order, PaymentPeriod relatedPeriod)
        {
            return _unitOfWork.Context.PaymentPeriods
                .Where(i =>
                    i.IsClosed == true
                    && i.IsDeleted != true
                    && i.ShamsiYear == relatedPeriod.ShamsiYear
                    && i.ShamsiMonth < relatedPeriod.ShamsiMonth
                    && i.StartDate <= relatedPeriod.StartDate
                    && i.StartDate >= order.StartDate)
                .OrderBy(i => i.ShamsiMonth)
                .AsNoTracking()
                .ToList();
        }

        private List<InterdictOrder> LoadSiblingOrders(InterdictOrder order)
        {
            var employeeId = order.RecruitOrder!.EmployeeId;
            return _unitOfWork.Context.InterdictOrders
                .Include(i => i.RecruitOrder)
                .Where(i =>
                    OrderValidStatesForArears.Contains(i.StatusId)
                    && i.Id != order.Id
                    && i.RecruitOrder != null
                    && i.RecruitOrder.EmployeeId == employeeId
                    && i.IsDeleted != true)
                .OrderBy(i => i.Serial)
                .AsNoTracking()
                .ToList();
        }

        private OperationResult CalculateArearFicheForPeriod(
            InterdictOrder order,
            PaymentPeriod period,
            List<InterdictOrder> siblingOrders,
            string runId)
        {
            var employeeId = order.RecruitOrder!.EmployeeId;
            var payLocationId = order.RecruitOrder.PayLocationId;
            long arearsInterdictId = order.Id;

            var midPeriodOrders = siblingOrders
                .Where(o => o.StartDate > period.StartDate && o.StartDate < period.EndDate)
                .ToList();

            if (midPeriodOrders.Count > 0)
            {
                var detail = string.Join("، ", midPeriodOrders.Select(o =>
                    $"حکمId={o.Id} سریال={o.Serial} شروع={o.StartDate:yyyy/MM/dd}"));
                return OperationResult.Failed(
                    $"ریشه مشکل: دوره «{period.title}» نیاز به تقسیم دارد چون حکم(های) دیگر در میانه دوره شروع می‌شوند ({detail}). محاسبه خودکار تقسیم دوره پشتیبانی نمی‌شود؛ ابتدا تاریخ اجرای احکام یا دوره‌ها را اصلاح کنید.");
            }

            var sameStartOrder = siblingOrders
                .Where(o => o.StartDate == period.StartDate && o.StartDate < period.EndDate)
                .OrderByDescending(o => o.Serial)
                .FirstOrDefault();

            if (sameStartOrder != null)
            {
                arearsInterdictId = sameStartOrder.Id;
                _logger.LogInformation(
                    "[{RunId}] برای دوره {Period} از حکم جایگزین ArearsInterdictId={ArearsInterdictId} استفاده شد (شروع یکسان با دوره)",
                    runId, period.title, arearsInterdictId);
            }

            _ficheService._currentUserDefaultPaymentPeriod = period.Id;
            _ficheService._currentUserDefaultOrganId = payLocationId;

            OperationResult ficheResp;
            try
            {
                ficheResp = _ficheService.CalculateFiche(
                    employeeId,
                    period.Id,
                    payLocationId,
                    BuildTreeTrace: false,
                    SaveFiche: false,
                    IsArear: true,
                    ArearsInterdictId: arearsInterdictId);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: استثنا در CalculateFiche برای دوره «{period.title}»: {BuildRootCause(ex)}");
            }

            if (!ficheResp.Success || ficheResp.Payload == null)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: CalculateFiche برای دوره «{period.title}» ناموفق بود. پیام موتور محاسبه: {ficheResp.Message ?? "بدون پیام"}");
            }

            var payload = (FicheDTO)ficheResp.Payload;
            if (payload.OrganisationEmployeeTypeFicheItem == null || !payload.OrganisationEmployeeTypeFicheItem.Any())
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: خروجی محاسبه دوره «{period.title}» فاقد اقلام فیش (OrganisationEmployeeTypeFicheItem) است؛ تنظیمات اقلام نوع استخدام/سازمان را بررسی کنید.");
            }

            return OperationResult.Succeeded(
                msg: $"محاسبه فیش معوقه دوره «{period.title}» موفق",
                payload: payload);
        }

        private OperationResult PersistArearCalculation(
            InterdictOrder order,
            List<FicheDTO> calculatedFiches,
            PaymentPeriod relatedPeriod,
            string runId,
            List<string> skippedPeriods)
        {
            var employeeId = order.RecruitOrder!.EmployeeId;
            var recruit = _unitOfWork.Context.RecruitOrders.Find(order.RecruitOrderId);
            if (recruit == null)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: RecruitOrder با Id={order.RecruitOrderId} برای حکم {order.Id} یافت نشد.");
            }

            if (!order.ApproveTimePaymentPeriod.HasValue)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: ApproveTimePaymentPeriod برای حکم {order.Id} خالی است؛ ذخیره فیش معوقه بدون دوره قصد پرداخت ممکن نیست.");
            }

            _unitOfWork.CreateTransaction();
            var transactionLogs = new List<string>();

            try
            {
                var trackedOrder = _unitOfWork.Context.InterdictOrders.Find(order.Id);
                if (trackedOrder == null)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed($"ریشه مشکل: حکم {order.Id} هنگام ذخیره یافت نشد (ممکن است همزمان حذف شده باشد).");
                }

                var arear = new Arear
                {
                    EmployeeId = employeeId,
                    OrganisationChartId = recruit.PayLocationId,
                    PersonnelFunctionId = calculatedFiches.FirstOrDefault()?.PersonnelFunctionId,
                    InterdictOrderId = order.Id,
                    ArearsStatusId = (long)Enums.ArearsStatus.Calculated,
                    ApproveTimePaymentPeriodId = order.ApproveTimePaymentPeriod.Value,
                    PaymentPeriodIntendToPayId = order.ApproveTimePaymentPeriod.Value,
                    CalculatedDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    IPAddress = "Job",
                    Description = skippedPeriods.Count > 0
                        ? $"محاسبه با رد برخی دوره‌ها: {string.Join(" ؛ ", skippedPeriods)}"
                        : "محاسبه کامل معوقه",
                    StartDate = order.StartDate,
                    EndDate = order.EndDate
                };
                _unitOfWork.Context.Arears.Add(arear);
                _unitOfWork.Context.SaveChanges();

                long totalDifference = 0;
                int changedItemCount = 0;
                int savedFicheCount = 0;

                foreach (var arearsFiche in calculatedFiches)
                {
                    var saveOne = PersistSingleArearFiche(
                        arearsFiche,
                        arear,
                        recruit,
                        order,
                        transactionLogs);

                    if (!saveOne.Success)
                    {
                        _unitOfWork.Rollback();
                        foreach (var line in transactionLogs)
                        {
                            AddBatchLog(new BatchLog
                            {
                                EmployeeId = employeeId,
                                InterdictOrderId = order.Id,
                                PaymentPeriodId = arearsFiche.PaymentPeriodId,
                                LogDescription = line
                            });
                        }
                        return OperationResult.Failed(saveOne.Message);
                    }

                    // Payload null = دوره رد شد (مثلاً بدون فیش عادی) ولی تراکنش ادامه دارد
                    if (saveOne.Payload == null)
                    {
                        continue;
                    }

                    var stats = ((int ChangedCount, long DiffSum))saveOne.Payload;
                    changedItemCount += stats.ChangedCount;
                    totalDifference += stats.DiffSum;
                    savedFicheCount++;
                }

                if (savedFicheCount == 0)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed(
                        $"ریشه مشکل: هیچ فیش معوقه‌ای ذخیره نشد (احتمالاً فیش عادی متناظر یافت نشد). جزئیات: {string.Join(" ؛ ", transactionLogs)}");
                }

                arear.ArearFicheCount = savedFicheCount;
                arear.ChangedItemCount = changedItemCount;
                arear.TotalDifferenceAmount = totalDifference;
                arear.PayableDifferenceAmount = totalDifference;
                arear.LastErrorMessage = null;
                _unitOfWork.Context.Arears.Update(arear);

                trackedOrder.LastModifiedDate = DateTime.Now;
                trackedOrder.ArearsStatusId = (long)Enums.ArearsStatus.Calculated;
                _unitOfWork.Context.InterdictOrders.Update(trackedOrder);

                _unitOfWork.Context.SaveChanges();
                _unitOfWork.Commit();

                var successMsg =
                    $"[{runId}] ذخیره موفق معوقه: ArearId={arear.Id}، فیش={savedFicheCount}، اقلام تغییر یافته={changedItemCount}، جمع تفاوت={totalDifference:N0}";
                _logger.LogInformation("{Message}", successMsg);
                AddBatchLog(new BatchLog
                {
                    EmployeeId = employeeId,
                    InterdictOrderId = order.Id,
                    PaymentPeriodId = relatedPeriod.Id,
                    LogDescription = successMsg
                });

                foreach (var line in transactionLogs)
                {
                    AddBatchLog(new BatchLog
                    {
                        EmployeeId = employeeId,
                        InterdictOrderId = order.Id,
                        LogDescription = line
                    });
                }

                return OperationResult.Succeeded(successMsg, payload: arear.Id);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed($"ریشه مشکل در تراکنش ذخیره: {BuildRootCause(ex)}");
            }
        }

        private OperationResult PersistSingleArearFiche(
            FicheDTO arearsFiche,
            Arear arear,
            RecruitOrder recruit,
            InterdictOrder sourceOrder,
            List<string> transactionLogs)
        {
            var period = _unitOfWork.Context.PaymentPeriods.Find(arearsFiche.PaymentPeriodId);
            var periodTitle = period?.title ?? arearsFiche.PaymentPeriodId.ToString();

            var normalFiches = _unitOfWork.Context.Fiches
                .Where(i =>
                    i.PaymentPeriodId == arearsFiche.PaymentPeriodId
                    && i.EmployeeId == recruit.EmployeeId
                    && i.IsDeleted != true)
                .ToList();

            if (normalFiches.Count == 0)
            {
                transactionLogs.Add(
                    $"دوره «{periodTitle}»: فیش عادی یافت نشد؛ بدون فیش مبنا مغایرت‌گیری ممکن نیست و فیش معوقه ذخیره نشد.");
                return OperationResult.Succeeded("رد دوره بدون فیش عادی", payload: null);
            }

            if (normalFiches.Count > 1)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: برای کارمند {recruit.EmployeeId} در دوره «{periodTitle}» بیش از یک فیش عادی ({normalFiches.Count}) وجود دارد؛ شناسه‌ها: {string.Join(",", normalFiches.Select(f => f.Id))}.");
            }

            var normalFiche = normalFiches.Single();
            var normalFicheItems = _unitOfWork.Context.FicheItems
                .Where(i => i.FicheId == normalFiche.Id && i.IsDeleted != true)
                .ToList();

            if (normalFicheItems.Count == 0)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: فیش عادی Id={normalFiche.Id} دوره «{periodTitle}» هیچ قلمی ندارد؛ مغایرت‌گیری ممکن نیست.");
            }

            if (arearsFiche.OrganisationEmployeeTypeFicheItem == null)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: خروجی محاسبه دوره «{periodTitle}» فاقد OrganisationEmployeeTypeFicheItem است.");
            }

            var arearsCoveredItems = arearsFiche.OrganisationEmployeeTypeFicheItem
                .Where(i => i.ArearsCovered == true)
                .ToList();

            if (arearsCoveredItems.Count == 0)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: در تنظیمات اقلام فیش، هیچ قلمی با ArearsCovered=true برای دوره «{periodTitle}» وجود ندارد؛ بدون قلم معوقه‌پذیر ذخیره معنی ندارد.");
            }

            if (!sourceOrder.ApproveTimePaymentPeriod.HasValue)
            {
                return OperationResult.Failed(
                    $"ریشه مشکل: ApproveTimePaymentPeriod حکم {sourceOrder.Id} خالی است.");
            }

            transactionLogs.Add(
                $"دوره «{periodTitle}»: فیش عادی Id={normalFiche.Id} با {normalFicheItems.Count} قلم؛ {arearsCoveredItems.Count} قلم معوقه‌پذیر.");

            var newFiche = new ArearFiche
            {
                ArearId = arear.Id,
                EmployeeId = recruit.EmployeeId,
                PersonnelFunctionId = arearsFiche.PersonnelFunctionId,
                InterdictOrderId = arearsFiche.InterdictOrderId > 0 ? arearsFiche.InterdictOrderId : sourceOrder.Id,
                CreateDate = DateTime.Now,
                PaymentPeriodId = arearsFiche.PaymentPeriodId,
                EmployeeTypeId = recruit.EmployeeTypeId,
                StartDate = period?.StartDate,
                EndDate = period?.EndDate,
                OrganisationChartId = recruit.PayLocationId,
                DeductedAmount = arearsFiche.DeductionSumAmount,
                PurePaymentAmount = Convert.ToInt64(arearsFiche.PayableAmount),
                IPAddress = "Job",
                PaymentTax = Convert.ToInt64(arearsFiche.TaxCoveredSum),
                PaymentInsuranceCovered = Convert.ToInt64(arearsFiche.PaymentInsuranceCovered),
                PaymentRetiredCovered = arearsFiche.PaymentRetiredCovered,
                DailyFunctionAmount = arearsFiche.DailyFunctionAmount,
                TotalAmount = Convert.ToInt64(arearsFiche.TotalPaymentAmount),
                BillEydiOpadashAmount = arearsFiche.BillEydiOpadashAmount,
                BillBazkharidSanavatAmount = arearsFiche.BillBazkharidSanavatAmount,
                BillSumItemsAmount = arearsFiche.BillSumItemsAmount,
                BillItemsWage = arearsFiche.BillItemsWage,
                FicheStatusId = (long)Enums.FicheStatus.Initial,
                SumCashTaxCoveredAndCountinious = arearsFiche.SumCashTaxCoveredAndCountinious,
                SumNonCashTaxCoveredAndCountinious = arearsFiche.SumNonCashTaxCoveredAndCountinious,
                SumNonCashTaxCoveredAndNotCountinious = arearsFiche.SumNonCashTaxCoveredAndNotCountinious,
                SumCashTaxCoveredAndNotCountinious = arearsFiche.SumCashTaxCoveredAndNotCountinious,
                NotNetCurrentMonthOverTimePayment = arearsFiche.NotNetCurrentMonthOverTimePayment,
                SumOfDelayedCountiniousPaymentInCurrentMonth = arearsFiche.SumOfDelayedCountiniousPaymentInCurrentMonth,
                HouseAmount = arearsFiche.HouseAmount,
                CarAmount = arearsFiche.CarAmount,
                CostCenterId = recruit.CostCenterId,
                PeymanRowId = arearsFiche.PeymanRowId,
                BankAccountNo = arearsFiche.BankAccountNo,
                InsuranceNo = arearsFiche.InsuranceNo,
                UnEmploymentAmount = arearsFiche.UnEmploymentAmount,
                IsActiveInsurance = arearsFiche.IsActiveInsurance,
                MonthJobBenefit = arearsFiche.MonthJobBenefit,
                Description = arearsFiche.Description,
                InsuranceTotal_DSW = arearsFiche.InsuranceTotal_DSW,
                SpouseAmount = arearsFiche.SpouseAmount,
                IncAmount = arearsFiche.IncAmount,
                PaymentPeriodIntendToPayId = sourceOrder.ApproveTimePaymentPeriod.Value
            };

            _unitOfWork.Context.ArearFiches.Add(newFiche);
            _unitOfWork.Context.SaveChanges();

            foreach (var item in arearsFiche.OrganisationEmployeeTypeFicheItem.Where(i => i.IsVirtual != true))
            {
                if (!item.PaymentTypeId.HasValue)
                {
                    return OperationResult.Failed(
                        $"ریشه مشکل: قلم WageItemId={item.WageItemId} در دوره «{periodTitle}» فاقد PaymentTypeId است.");
                }

                var row = new ArearFicheItem
                {
                    ArearFicheId = newFiche.Id,
                    CreateDate = DateTime.Now,
                    IPAddress = "Job",
                    Value = item.Amount,
                    PaymentTypeId = item.PaymentTypeId.Value,
                    WageItemId = item.WageItemId,
                    IsDeleted = false,
                    IsArear = true,
                    IsSubItem = item.IsSubItem,
                    Comment = item.Description
                };
                _unitOfWork.Context.ArearFicheItems.Add(row);
            }

            _unitOfWork.Context.SaveChanges();

            var coveredWageIds = arearsCoveredItems.Select(i => i.WageItemId).ToHashSet();
            int changedCount = 0;
            long diffSum = 0;

            foreach (var item in arearsFiche.OrganisationEmployeeTypeFicheItem.Where(i => coveredWageIds.Contains(i.WageItemId)))
            {
                var matches = normalFicheItems.Where(i => i.WageItemId == item.WageItemId).ToList();
                if (matches.Count > 1)
                {
                    return OperationResult.Failed(
                        $"ریشه مشکل: در فیش عادی Id={normalFiche.Id} بیش از یک قلم با WageItemId={item.WageItemId} وجود دارد.");
                }

                long lastAmount = matches.Count == 1 ? Convert.ToInt64(matches.Single().Value) : 0;
                long currentAmount = Convert.ToInt64(item.Amount);

                if (lastAmount == currentAmount)
                {
                    continue;
                }

                var change = new ArearsChangedFicheItem
                {
                    WageItemId = item.WageItemId,
                    CreateDate = DateTime.Now,
                    LastAmount = lastAmount,
                    CurrentAmount = currentAmount,
                    ArearFicheId = newFiche.Id,
                    IPAddress = "Job"
                };
                _unitOfWork.Context.ArearsChangedFicheItems.Add(change);
                changedCount++;
                diffSum += currentAmount - lastAmount;
            }

            _unitOfWork.Context.SaveChanges();
            transactionLogs.Add(
                $"دوره «{periodTitle}»: ArearFicheId={newFiche.Id} ذخیره شد؛ {changedCount} قلم تغییر یافته؛ جمع تفاوت={diffSum:N0}.");

            return OperationResult.Succeeded(payload: (changedCount, diffSum));
        }

        private void FailOrder(InterdictOrder order, string runId, string rootCause, long? employeeId = null, long? paymentPeriodId = null)
        {
            _logger.LogWarning("[{RunId}] {RootCause}", runId, rootCause);
            AddBatchLog(new BatchLog
            {
                EmployeeId = employeeId ?? order.RecruitOrder?.EmployeeId,
                InterdictOrderId = order.Id,
                PaymentPeriodId = paymentPeriodId,
                LogDescription = $"[{runId}] {rootCause}"
            });

            try
            {
                UpsertArearFailureRecord(order, rootCause);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{RunId}] ثبت رکورد Arear ناموفق برای حکم {OrderId}", runId, order.Id);
            }
        }

        private void SkipOrder(InterdictOrder order, string runId, long employeeId, string reason)
        {
            _logger.LogInformation("[{RunId}] {Reason}", runId, reason);
            AddBatchLog(new BatchLog
            {
                EmployeeId = employeeId,
                InterdictOrderId = order.Id,
                LogDescription = $"[{runId}] {reason}"
            });
        }

        private void LogOrder(InterdictOrder order, string runId, long employeeId, string message, long? paymentPeriodId = null)
        {
            _logger.LogInformation("[{RunId}] حکم {OrderId}: {Message}", runId, order.Id, message);
            AddBatchLog(new BatchLog
            {
                EmployeeId = employeeId,
                InterdictOrderId = order.Id,
                PaymentPeriodId = paymentPeriodId,
                LogDescription = $"[{runId}] {message}"
            });
        }

        private void UpsertArearFailureRecord(InterdictOrder order, string rootCause)
        {
            if (order.RecruitOrder == null)
            {
                return;
            }

            var existing = _unitOfWork.Context.Arears
                .FirstOrDefault(a =>
                    a.InterdictOrderId == order.Id
                    && a.IsDeleted != true
                    && a.ArearsStatusId == (long)Enums.ArearsStatus.NeedToCalculate);

            if (existing != null)
            {
                existing.LastErrorMessage = Truncate(rootCause, 2000);
                existing.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.Arears.Update(existing);
                _unitOfWork.Context.SaveChanges();
                return;
            }

            var arear = new Arear
            {
                EmployeeId = order.RecruitOrder.EmployeeId,
                OrganisationChartId = order.RecruitOrder.PayLocationId,
                InterdictOrderId = order.Id,
                ArearsStatusId = (long)Enums.ArearsStatus.NeedToCalculate,
                ApproveTimePaymentPeriodId = order.ApproveTimePaymentPeriod,
                PaymentPeriodIntendToPayId = order.ApproveTimePaymentPeriod,
                LastErrorMessage = Truncate(rootCause, 2000),
                CreateDate = DateTime.Now,
                IPAddress = "Job",
                StartDate = order.StartDate,
                EndDate = order.EndDate
            };
            _unitOfWork.Context.Arears.Add(arear);
            _unitOfWork.Context.SaveChanges();
        }

        private void AddBatchLog(BatchLog log)
        {
            if (string.IsNullOrWhiteSpace(log.title))
            {
                log.title = "ArearService";
            }

            log.CreateDate = DateTime.Now;
            log.LastModifiedDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(log.IPAddress))
            {
                log.IPAddress = "Job";
            }

            const string sql = @"
INSERT INTO [Payroll].[Batch_Log]
([LogDescription],[ServiceName],[LogTypeId],[InterdictOrderId],[PersonnelFunctionId],[EmployeeId],[title],[CreateDate],[LastModifiedDate],[IPAddress],[IsDeleted],[PaymentPeriodId])
VALUES (@LogDescription, @ServiceName, @LogTypeId, @InterdictOrderId, @PersonnelFunctionId, @EmployeeId, @title, @CreateDate, @LastModifiedDate, @IPAddress, 0, @PaymentPeriodId)";

            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@LogDescription", (object?)log.LogDescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ServiceName", "ArearService");
            cmd.Parameters.AddWithValue("@LogTypeId", (long)Enums.BatchLoggerRecordType.OrderArear);
            cmd.Parameters.AddWithValue("@InterdictOrderId", (object?)log.InterdictOrderId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PersonnelFunctionId", (object?)log.PersonnelFunctionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeId", (object?)log.EmployeeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@title", log.title);
            cmd.Parameters.AddWithValue("@CreateDate", log.CreateDate ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@LastModifiedDate", log.LastModifiedDate ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@IPAddress", log.IPAddress ?? "Job");
            cmd.Parameters.AddWithValue("@PaymentPeriodId", (object?)log.PaymentPeriodId ?? DBNull.Value);
            cn.Open();
            cmd.ExecuteNonQuery();
        }

        private void TryAddBatchLog(BatchLog log)
        {
            try
            {
                AddBatchLog(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ثبت BatchLog ناموفق: {Message}", ex.Message);
            }
        }

        private static string BuildRootCause(Exception ex)
        {
            var sb = new StringBuilder();
            var current = ex;
            var depth = 0;
            while (current != null && depth < 5)
            {
                if (depth > 0)
                {
                    sb.Append(" ← ");
                }
                sb.Append(current.GetType().Name).Append(": ").Append(current.Message);
                current = current.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        private static string Truncate(string value, int max)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= max)
            {
                return value;
            }
            return value[..(max - 3)] + "...";
        }

        public OperationResult GetArearById(long id)
        {
            var entity = _unitOfWork.Context.Arears
                .Include(a => a.ArearsStatus)
                .Include(a => a.ApproveTimePaymentPeriod)
                .Include(a => a.PaymentPeriodIntendToPay)
                .Include(a => a.OrganisationChart)
                .Include(a => a.Employee)
                .Include(a => a.InterdictOrder)
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id && a.IsDeleted != true);

            if (entity == null)
            {
                return OperationResult.NotFound($"معوقه با شناسه {id} یافت نشد.");
            }

            return OperationResult.Succeeded(payload: _mapper.Map<ArearDTO>(entity));
        }

        public OperationResult GetArearsByEmployee(long employeeId, int currentPage, int pageSize)
        {
            if (employeeId <= 0)
            {
                return OperationResult.Failed("ریشه مشکل: شناسه کارمند نامعتبر است.");
            }

            var query = _unitOfWork.Context.Arears
                .Include(a => a.ArearsStatus)
                .Include(a => a.ApproveTimePaymentPeriod)
                .Include(a => a.PaymentPeriodIntendToPay)
                .Include(a => a.InterdictOrder)
                .Where(a => a.EmployeeId == employeeId && a.IsDeleted != true)
                .OrderByDescending(a => a.Id);

            var total = query.Count();
            var page = query.Skip(currentPage * pageSize).Take(pageSize).AsNoTracking().ToList();
            return OperationResult.Succeeded(
                payload: _mapper.Map<List<ArearDTO>>(page),
                rowCount: total);
        }
    }
}
