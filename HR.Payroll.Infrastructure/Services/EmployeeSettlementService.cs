using AutoMapper;
using Hr.Employee.infrastructure.Data;
using HR.BaseInfo.Core.Entities;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.Data.EmployeeRelated;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using Dapper;

using HR.WorkFlow.Infrastructure.Data;
using HR.WorkFlow.Infrastructure.Services;
using HR.Order.Core.DTOs;
using HR.Payroll.Core.Helpers;
using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class EmployeeSettlementService(
    IMapper mapper,
    IUnitOfWork<PayrollContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    FicheService ficheService,
    OrganisationEmployeeTypeSettlementItemService settlementItemConfigService,
    OrganisationEmployeeStatusService organisationEmployeeStatusService,
    OrganisationEmployeeTypeOrderTypeService organisationEmployeeTypeOrderTypeService,
    OrganisationEmployeeTypeMRTService organisationEmployeeTypeMRTService,
    OrganisationMRTService organisationMRTService,
    OrganisationContext organisationContext,
    EmployeeContext employeeContext,
    WorkFlowInstanceStarterService workFlowInstanceStarter,
    IUnitOfWork<WorkFlowContext> workFlowUnitOfWork) : BaseService<EmployeeSettlement, PayrollContext, EmployeeSettlementDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly FicheService _ficheService = ficheService;
    private readonly OrganisationEmployeeTypeSettlementItemService _settlementItemConfigService = settlementItemConfigService;
    private readonly OrganisationEmployeeStatusService _organisationEmployeeStatusService = organisationEmployeeStatusService;
    private readonly OrganisationEmployeeTypeOrderTypeService _organisationEmployeeTypeOrderTypeService = organisationEmployeeTypeOrderTypeService;
    private readonly OrganisationEmployeeTypeMRTService _organisationEmployeeTypeMRTService = organisationEmployeeTypeMRTService;
    private readonly OrganisationMRTService _organisationMRTService = organisationMRTService;
    private readonly OrganisationContext _organisationContext = organisationContext;
    private readonly EmployeeContext _employeeContext = employeeContext;
    private readonly WorkFlowInstanceStarterService _workFlowInstanceStarter = workFlowInstanceStarter;
    private readonly IUnitOfWork<WorkFlowContext> _workFlowUnitOfWork = workFlowUnitOfWork;
    private readonly UserResolverService _userResolver = userService;

    /// <summary>
    /// بررسی آمادگی کارمند برای ثبت تسویه حساب بر اساس آخرین حکم نهایی و وضعیت استخدام سازمان.
    /// </summary>
    public OperationResult GetSettlementEligibilityByEmployeeId(long employeeId, DateTime? settlementDate = null)
    {
        if (employeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        if (_currentUserDefaultOrganId <= 0)
        {
            return OperationResult.Failed("سازمان جاری مشخص نیست");
        }

        var effectiveSettlementDate = settlementDate?.Date ?? DateTime.Now.Date;
        var organisationChartId = _currentUserDefaultOrganId;
        var payLocationId = ResolvePayLocationIdForEmployee(employeeId, organisationChartId);
        var orderResolveError = TryResolveSettlementInterdictOrder(
            employeeId,
            payLocationId,
            effectiveSettlementDate,
            out var interdictOrder,
            out var recruitOrder,
            out var lastOrderId);

        if (orderResolveError != null)
        {
            return OperationResult.Succeeded(payload: new EmployeeSettlementEligibilityDTO
            {
                HasFinalInterdict = false,
                Message = orderResolveError.Message ?? "حکم فعال برای کارمند یافت نشد",
            });
        }

        var endDate = interdictOrder!.EndDate;
        var endDateShamsi = endDate.HasValue ? Convertor.ToIranianDate(endDate) : null;
        var (isEmployed, employeeStatusTitle) = ResolveEmploymentInfo(
            recruitOrder!.EmployeeStatusId,
            recruitOrder.PayLocationId,
            effectiveSettlementDate);
        var needSettlement = ResolveNeedSettlement(
            recruitOrder.PayLocationId,
            recruitOrder.EmployeeTypeId,
            interdictOrder.OrderTypeId,
            effectiveSettlementDate);
        var isInCurrentOrgan = recruitOrder.PayLocationId == organisationChartId;
        var (suggestedStartDate, suggestedEndDate) = ResolveLastEmploymentPeriodDates(
            employeeId,
            payLocationId,
            effectiveSettlementDate);
        var suggestedStartDateShamsi = suggestedStartDate.HasValue
            ? Convertor.ToIranianDate(suggestedStartDate)
            : null;
        var suggestedEndDateShamsi = suggestedEndDate.HasValue
            ? Convertor.ToIranianDate(suggestedEndDate)
            : null;
        var eligibilitySnapshot = new GetEmployeeSettlementEligibility_Result
        {
            HasFinalInterdict = true,
            IsInCurrentOrgan = isInCurrentOrgan,
            IsEmployed = isEmployed,
            NeedSettlement = needSettlement,
            InterdictOrderId = lastOrderId,
            InterdictEndDate = endDate,
            EmployeeStatusId = recruitOrder.EmployeeStatusId,
            EmployeeStatusTitle = employeeStatusTitle,
            PayLocationId = recruitOrder.PayLocationId,
        };

        if (!isInCurrentOrgan)
        {
            return OperationResult.Succeeded(payload: new EmployeeSettlementEligibilityDTO
            {
                HasFinalInterdict = true,
                IsInCurrentOrgan = false,
                IsEmployed = isEmployed,
                NeedSettlement = needSettlement,
                InterdictOrderId = lastOrderId,
                InterdictEndDate = endDate,
                InterdictEndDateShamsi = endDateShamsi,
                SuggestedStartDate = suggestedStartDate,
                SuggestedStartDateShamsi = suggestedStartDateShamsi,
                SuggestedEndDate = suggestedEndDate,
                SuggestedEndDateShamsi = suggestedEndDateShamsi,
                EmployeeStatusId = recruitOrder.EmployeeStatusId,
                EmployeeStatusTitle = employeeStatusTitle,
                Message = "کاربر در واحد جاری شاغل نمی‌باشد و امکان تسویه وجود ندارد",
            });
        }

        return OperationResult.Succeeded(payload: new EmployeeSettlementEligibilityDTO
        {
            HasFinalInterdict = true,
            IsInCurrentOrgan = true,
            IsEmployed = isEmployed,
            NeedSettlement = needSettlement,
            InterdictOrderId = lastOrderId,
            InterdictEndDate = endDate,
            InterdictEndDateShamsi = endDateShamsi,
            SuggestedStartDate = suggestedStartDate,
            SuggestedStartDateShamsi = suggestedStartDateShamsi,
            SuggestedEndDate = suggestedEndDate,
            SuggestedEndDateShamsi = suggestedEndDateShamsi,
            EmployeeStatusId = recruitOrder.EmployeeStatusId,
            EmployeeStatusTitle = employeeStatusTitle,
            Message = BuildSettlementEligibilityMessage(eligibilitySnapshot),
            LoanSettlementItemIds = GetActiveLoanSettlementItemIdsForEmployee(employeeId, payLocationId).ToList(),
            DeductionSettlementItemIds = GetActiveDeductionSettlementItemIdsForEmployee(employeeId, payLocationId).ToList(),
        });
    }

    /// <summary>
    /// محاسبه پیش‌نمایش تسویه — بدون هیچ INSERT/UPDATE در دیتابیس.
    /// </summary>
    public OperationResult PreviewSettlementCalculation(EmployeeSettlementDTO request, bool buildTreeTrace = true)
    {
        return CalculateSettlementPreviewCore(request, buildTreeTrace);
    }

    [Obsolete("Use PreviewSettlementCalculation — preview never persists data.")]
    public OperationResult CalculateSettlement(EmployeeSettlementDTO request, bool buildTreeTrace, bool saveSettlement)
    {
        if (saveSettlement)
        {
            return OperationResult.Failed(
                "ذخیره از مسیر محاسبه مجاز نیست. ابتدا پیش‌نمایش بگیرید و سپس از Post یا Put استفاده کنید.");
        }

        return CalculateSettlementPreviewCore(request, buildTreeTrace);
    }

    private OperationResult CalculateSettlementPreviewCore(EmployeeSettlementDTO request, bool buildTreeTrace)
    {
        if (request.EmployeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        if (request.SettlementCauseId <= 0)
        {
            return OperationResult.Failed("علت تسویه الزامی است");
        }

        if (request.SettlementDate == default)
        {
            return OperationResult.Failed("تاریخ تسویه حساب الزامی است");
        }

        var ficheValidationError = ValidateSettlementFichePresence(request);
        if (ficheValidationError != null)
        {
            return ficheValidationError;
        }

        var selectedItemIds = (request.SettlementItems?.Select(i => i.SettlementItemId).Where(i => i > 0).ToList()
            ?? request.SettlementItemIds?.Where(i => i > 0).ToList()
            ?? []);

        if (_currentUserDefaultPaymentPeriod <= 0)
        {
            return OperationResult.Failed("لطفا از بخش تنظیمات حساب کاربری دوره پیش فرض را انتخاب بفرمایید");
        }

        var currentPeriod = _unitOfWork.Context.PaymentPeriods
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == _currentUserDefaultPaymentPeriod);

        if (currentPeriod == null)
        {
            return OperationResult.Failed("دوره پیش فرض یافت نشد");
        }

        var orderImpleDate = ResolveSettlementOrderImpleDate(request.SettlementDate, request.EndDate);

        if (_currentUserDefaultOrganId <= 0)
        {
            return OperationResult.Failed("سازمان جاری مشخص نیست");
        }

        var payLocationId = ResolvePayLocationIdForEmployee(request.EmployeeId, _currentUserDefaultOrganId);
        var orderResolveError = TryResolveSettlementInterdictOrder(
            request.EmployeeId,
            payLocationId,
            orderImpleDate,
            out var interdictOrder,
            out var recruitOrder,
            out var lastOrderId);

        if (orderResolveError != null)
        {
            return orderResolveError;
        }

        var employmentError = ValidateSettlementEmploymentStatus(
            recruitOrder!,
            interdictOrder!,
            _currentUserDefaultOrganId,
            orderImpleDate);
        if (employmentError != null)
        {
            return employmentError;
        }

        selectedItemIds = ApplyLoanSettlementItemSelection(
            selectedItemIds,
            request.EmployeeId,
            request.Loanincluded,
            recruitOrder.PayLocationId,
            out var loanSelectionError);

        if (loanSelectionError != null)
        {
            return loanSelectionError;
        }

        selectedItemIds = ApplyDeductionSettlementItemSelection(
            selectedItemIds,
            request.EmployeeId,
            request.Deductionincluded,
            recruitOrder.PayLocationId,
            out var deductionSelectionError);

        if (deductionSelectionError != null)
        {
            return deductionSelectionError;
        }

        if (selectedItemIds.Count == 0)
        {
            return OperationResult.Failed("حداقل یک آیتم تسویه باید انتخاب شود");
        }

        var loanRemainAmounts = request.Loanincluded
            ? BuildLoanRemainAmountsBySettlementItemId(request.EmployeeId, recruitOrder.PayLocationId)
            : new Dictionary<long, long>();

        var deductionRemainAmounts = request.Deductionincluded
            ? BuildDeductionRemainAmountsBySettlementItemId(request.EmployeeId, recruitOrder.PayLocationId)
            : new Dictionary<long, long>();

        var manualAmounts = BuildManualAmountLookup(request.SettlementItems);
        var manualDescriptions = BuildManualDescriptionLookup(request.SettlementItems);

        var settlementConfigs = _settlementItemConfigService.All()
            .AsNoTracking()
            .Include(i => i.SettlementItem)
            .Include(i => i.PaymentType)
            .Include(i => i.MeasurementUnit)
            .Include(i => i.OrganisationFormula)
                .ThenInclude(f => f!.Formula)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId
                && i.EmployeeTypeId == recruitOrder.EmployeeTypeId
                && selectedItemIds.Contains(i.SettlementItemId))
            .OrderBy(i => i.Priority ?? int.MaxValue)
            .ThenBy(i => i.SettlementItem != null ? i.SettlementItem.title : string.Empty)
            .ThenByDescending(i => i.Id)
            .ToList();

        if (settlementConfigs.Count == 0)
        {
            return OperationResult.NotFound("تنظیمات آیتم‌های تسویه برای نوع استخدام کارمند یافت نشد");
        }

        var configuredItemIds = settlementConfigs.Select(c => c.SettlementItemId).ToHashSet();
        var missingItemIds = selectedItemIds.Where(id => !configuredItemIds.Contains(id)).ToList();
        if (missingItemIds.Count > 0)
        {
            return OperationResult.Failed(
                $"برای {missingItemIds.Count} آیتم انتخاب‌شده تنظیمات سازمان تعریف نشده است (شناسه: {string.Join("، ", missingItemIds)})");
        }

        var coefficientItems = _unitOfWork.Context.InterdictOrderCoefficientItems
            .AsNoTracking()
            .Where(i => i.InterdictOrderId == interdictOrder.Id)
            .ToList();

        var orderWageItems = LoadOrderWageItems(interdictOrder.Id);
        var personnelFunction = LoadPersonnelFunction(request.EmployeeId, currentPeriod.Id, recruitOrder.PayLocationId);

        var formulaImpleDate = ResolveSettlementFormulaImpleDate(request, interdictOrder);
        var formulaRequest = new CommunicateWithFormulaRequest
        {
            BuildTreeTrace = buildTreeTrace,
            DoFinalCalc = true,
            InterdictOrder = interdictOrder,
            RecruitOrder = recruitOrder,
            PaymentPeriod = currentPeriod,
            PersonnelFunction = personnelFunction,
            InterdictOrderCoefficientItems = coefficientItems,
            InterdictOrderWageItems = orderWageItems,
            lastorderId = lastOrderId,
            FormulaImpleDate = formulaImpleDate,
            SettlementItems = [],
            VariableList = BuildSettlementVariableList(request, recruitOrder.PayLocationId, lastOrderId, interdictOrder),
        };

        var calculatedItems = new List<EmployeeSettlementCalculatedItemDTO>();
        var runtimeSettlementItems = new List<SettlementRuntimeItem>();
        var hasCalculationErrors = false;
        var index = 1;
        long paymentSum = 0;
        long deductionSum = 0;

        foreach (var config in settlementConfigs)
        {
            var usesFormula = config.EnterTypeId == (long)Enums.EnterTypeId.UseFormula
                && config.OrganisationFormulaId.HasValue
                && config.OrganisationFormulaId.Value > 0;
            var usesFixValue = config.EnterTypeId == (long)Enums.EnterTypeId.fixValue;
            var isEditAble = config.IsEditAble == true || (!usesFormula && !usesFixValue);

            var row = new EmployeeSettlementCalculatedItemDTO
            {
                Index = index,
                SettlementItemId = config.SettlementItemId,
                SettlementItemTitle = config.SettlementItem?.title,
                PaymentTypeId = config.PaymentTypeId,
                PaymentTypeTitle = config.PaymentType?.title,
                Priority = config.Priority,
                IsEditAble = isEditAble,
                IsVirtual = config.IsVirtual == true,
                OrganisationFormulaId = config.OrganisationFormulaId,
                OrganisationFormula = config.OrganisationFormula?.Formula?.title,
                Description = manualDescriptions.GetValueOrDefault(config.SettlementItemId),
            };

            formulaRequest.SettlementItems = runtimeSettlementItems;

            var resolvedExternally = TryResolveExternalSettlementAmount(
                    config.SettlementItemId,
                    request,
                    loanRemainAmounts,
                    deductionRemainAmounts,
                    out var externalAmount,
                    out var externalMessage);

            // ابتدا مبلغ سیستمی محاسبه می‌شود تا در صورت ویرایش دستی کاربر نیز حفظ گردد.
            if (resolvedExternally)
            {
                row.SystemCalculatedAmount = externalAmount;
                row.IsRowSuccess = true;
                row.FormulaRowMessage = externalMessage;
            }
            else if (usesFixValue)
            {
                row.SystemCalculatedAmount = Math.Max(0L, config.FixValue ?? 0);
                row.IsRowSuccess = true;
                row.FormulaRowMessage = "مقدار ثابت";
            }
            else if (usesFormula)
            {
                var formulaDefinitionId = ResolveOrganisationFormulaDefinitionId(
                    config.OrganisationFormulaId!.Value,
                    _currentUserDefaultOrganId);

                if (!formulaDefinitionId.HasValue)
                {
                    var formulaTitle = config.OrganisationFormula?.Formula?.title
                        ?? config.SettlementItem?.title
                        ?? config.OrganisationFormulaId.Value.ToString(CultureInfo.InvariantCulture);
                    row.SystemCalculatedAmount = 0;
                    row.IsRowSuccess = false;
                    row.FormulaRowMessage =
                        $"متن محاسبه فرمول «{formulaTitle}» در بخش طراحی فرمول‌ها (Formula Definition) تعریف نشده است";
                    hasCalculationErrors = true;
                }
                else
                {
                    row.OrganisationFormulaId = formulaDefinitionId;
                    var formulaResponse = _ficheService.CommunicateWithFormula(formulaDefinitionId.Value, formulaRequest);
                    row.SystemCalculatedAmount = Convert.ToInt64(Math.Round(formulaResponse.Result));
                    row.IsRowSuccess = formulaResponse.Succees;
                    row.SuccessRunTimeInmilliseconds = formulaResponse.SuccessRunTimeInmilliseconds;
                    row.FormulaRowMessage = formulaResponse.Succees ? "Ok" : formulaResponse.ResponseMessage;
                    if (buildTreeTrace)
                    {
                        row.FormulaFriendlyText = formulaResponse.FormulaFriendlyText;
                        row.VariableFriendlyList = formulaResponse.VariableFriendlyList;
                        row.FormulaText = formulaResponse.FormulaText;
                        row.FormulaTreeParser = formulaResponse.FormulaTreeParser;
                        row.FormulaHelpDesc = formulaResponse.FormulaHelpDesc;
                    }

                    if (!formulaResponse.Succees)
                    {
                        hasCalculationErrors = true;
                    }
                }
            }
            else
            {
                row.SystemCalculatedAmount = 0;
                row.IsRowSuccess = true;
                row.FormulaRowMessage = "مبلغ به صورت دستی وارد می‌شود";
            }

            // مبلغ وام/کسور اولویت دارد؛ مبلغ دستی فقط روی آیتم‌های قابل‌ویرایش غیرخارجی اعمال می‌شود.
            if (!resolvedExternally
                && isEditAble
                && manualAmounts.TryGetValue(config.SettlementItemId, out var manualAmount))
            {
                row.Amount = manualAmount;
                row.UsedManualAmount = true;
                row.IsRowSuccess = true;
                row.FormulaRowMessage = row.SystemCalculatedAmount != manualAmount
                    ? "مبلغ دستی (محاسبه سیستم حفظ شد)"
                    : "مبلغ دستی";
            }
            else
            {
                row.Amount = row.SystemCalculatedAmount;
            }

            if (config.IsVirtual != true)
            {
                if (row.PaymentTypeId == (long)Enums.PaymentType.Payment)
                {
                    paymentSum += row.Amount;
                }
                else if (row.PaymentTypeId == (long)Enums.PaymentType.Deduction)
                {
                    deductionSum += row.Amount;
                }
            }

            runtimeSettlementItems.Add(new SettlementRuntimeItem
            {
                SettlementItemId = config.SettlementItemId,
                Value = row.Amount,
            });

            calculatedItems.Add(row);
            index++;
        }

        var purePayment = paymentSum - deductionSum;

        request.InterdictOrderId = lastOrderId;
        request.LastInterdictOrderId = lastOrderId;
        if (request.EmployeeTypeId is null or <= 0)
        {
            request.EmployeeTypeId = recruitOrder.EmployeeTypeId;
        }

        request.PaymentAmount = paymentSum;
        request.DeductionSum = deductionSum;
        request.PurePaymentAmount = purePayment;
        request.SettlementItems = calculatedItems.Select(item => new EmployeeSettlementItemDTO
        {
            SettlementItemId = item.SettlementItemId,
            SettlementItemTitle = item.SettlementItemTitle,
            Amount = item.Amount,
            SystemCalculatedAmount = item.SystemCalculatedAmount,
            Description = item.Description,
            Duration = request.Duration,
        }).ToList();
        request.SettlementItemIds = calculatedItems.Select(i => i.SettlementItemId).ToList();

        var result = new EmployeeSettlementCalculationResultDTO
        {
            Settlement = request,
            CalculatedItems = calculatedItems,
            InterdictOrderId = lastOrderId,
            LastInterdictOrderId = lastOrderId,
            InterdictOrderSerial = interdictOrder.Serial,
            IsPreviewOnly = true,
        };
        PopulateCalculationErrors(result, purePayment);

        if (result.HasCalculationErrors)
        {
            return OperationResult.Failed(BuildCalculationFailureMessage(result), payload: result);
        }

        if (purePayment < 0)
        {
            return OperationResult.Failed("مبلغ خالص پرداختی منفی است؛ امکان ذخیره وجود ندارد", payload: result);
        }

        return OperationResult.Succeeded("پیش‌نمایش محاسبه تسویه آماده است", payload: result);
    }

    private static void PopulateCalculationErrors(EmployeeSettlementCalculationResultDTO result, long purePaymentAmount)
    {
        var failedItems = result.CalculatedItems.Where(item => !item.IsRowSuccess).ToList();
        result.FailedItemCount = failedItems.Count;
        result.ErrorMessages = failedItems
            .Select(item => FormatItemError(item))
            .ToList();

        if (purePaymentAmount < 0)
        {
            result.ErrorMessages.Add($"مبلغ خالص پرداختی منفی است ({purePaymentAmount:N0} ریال)");
        }

        result.HasCalculationErrors = result.FailedItemCount > 0 || purePaymentAmount < 0;
    }

    private static string FormatItemError(EmployeeSettlementCalculatedItemDTO item)
    {
        var title = string.IsNullOrWhiteSpace(item.SettlementItemTitle)
            ? $"آیتم {item.SettlementItemId}"
            : item.SettlementItemTitle;
        var detail = string.IsNullOrWhiteSpace(item.FormulaRowMessage) ? "خطای نامشخص" : item.FormulaRowMessage.Trim();
        return $"«{title}»: {detail}";
    }

    private static string BuildCalculationFailureMessage(EmployeeSettlementCalculationResultDTO result)
    {
        if (result.ErrorMessages.Count == 0)
        {
            return "برخی آیتم‌های تسویه با خطا محاسبه شدند";
        }

        if (result.ErrorMessages.Count == 1)
        {
            return $"محاسبه ناموفق — {result.ErrorMessages[0]}";
        }

        return $"محاسبه {result.FailedItemCount} آیتم ناموفق بود. {string.Join(" | ", result.ErrorMessages.Take(3))}"
            + (result.ErrorMessages.Count > 3 ? " ..." : string.Empty);
    }

    public new OperationResult Get(long id)
    {
        var result = base.Get(id);
        if (!result.Success || result.Payload is not EmployeeSettlementDTO dto)
        {
            return result;
        }

        var items = _unitOfWork.Context.EmployeeSettlementItems
            .Include(i => i.SettlementItem)
            .Where(i => i.EmployeeSettlementId == id && !i.IsDeleted)
            .ToList();

        dto.SettlementItems = _mapper.Map<List<EmployeeSettlementItemDTO>>(items);
        dto.SettlementItemIds = items.Select(i => i.SettlementItemId).ToList();
        return OperationResult.Succeeded(payload: dto);
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeSettlementDTO entityToCreate)
    {
        if (entityToCreate.OrganisationChartId <= 0 && _currentUserDefaultOrganId > 0)
        {
            entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        }

        var organisationChartId = entityToCreate.OrganisationChartId > 0
            ? entityToCreate.OrganisationChartId
            : _currentUserDefaultOrganId;
        var eligibilityError = ValidateSettlementEligibilityRequired(
            entityToCreate.EmployeeId,
            organisationChartId,
            entityToCreate.SettlementDate.Date,
            entityToCreate.EndDate);
        if (eligibilityError != null)
        {
            return eligibilityError;
        }

        var enrichError = TryEnrichSettlementFromEmployeeOrder(entityToCreate);
        if (enrichError != null)
        {
            return enrichError;
        }

        entityToCreate.SettlementStatusId = (long)Enums.SettlementStatus.Initial;

        var overlapError = ValidateSettlementDateRangeOverlap(entityToCreate);
        if (overlapError != null)
        {
            return overlapError;
        }

        var ficheValidationError = ValidateSettlementFichePresence(entityToCreate);
        if (ficheValidationError != null)
        {
            return ficheValidationError;
        }

        var settlementItems = entityToCreate.SettlementItems?.ToList() ?? BuildItemsFromIds(entityToCreate);
        entityToCreate.SettlementItems = null;
        entityToCreate.SettlementItemIds = null;

        var result = await base.CreateForAsync(entityToCreate);
        if (!result.Success)
        {
            return result;
        }

        var settlementId = Convert.ToInt64(result.Payload);
        await SaveSettlementItemsAsync(settlementId, settlementItems, entityToCreate.Duration);
        return result;
    }

    public new async Task<OperationResult> UpdateForAsync(EmployeeSettlementDTO entityToUpdate)
    {
        return await Task.FromResult(
            OperationResult.Failed("ویرایش تسویه حساب پس از ثبت مجاز نیست. در صورت نیاز تسویه جدید ایجاد کنید."));
    }

    public new OperationResult DeleteRecord(long id)
    {
        if (id <= 0)
        {
            return OperationResult.Failed("شناسه تسویه حساب نامعتبر است");
        }

        var settlement = _unitOfWork.Context.EmployeeSettlements
            .FirstOrDefault(s => s.Id == id && !s.IsDeleted);

        if (settlement == null)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        if (settlement.SettlementStatusId != (long)Enums.SettlementStatus.Initial)
        {
            return OperationResult.Failed("فقط تسویه‌های در وضعیت «ایجاد اولیه (پیش‌نویس)» قابل حذف هستند");
        }

        var hasWorkflowInstance = _workFlowUnitOfWork.Context.WorkFlowInstances
            .AsNoTracking()
            .Any(wi => wi.EmployeeSettlementId == id && !wi.IsDeleted);

        if (hasWorkflowInstance)
        {
            return OperationResult.Failed("تسویه‌ای که برای آن گردش کار ایجاد شده قابل حذف نیست");
        }

        return base.DeleteRecord(id);
    }

    /// <summary>
    /// ارسال تسویه حساب به کارتابل تاییدکننده و ایجاد گردش کار.
    /// </summary>
    public OperationResult SendSettlementToCartable(long id)
    {
        if (id <= 0)
        {
            return OperationResult.Failed("شناسه تسویه حساب نامعتبر است");
        }

        var settlement = _unitOfWork.Context.EmployeeSettlements
            .FirstOrDefault(s => s.Id == id && !s.IsDeleted);

        if (settlement == null)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        if (settlement.SettlementStatusId != (long)Enums.SettlementStatus.Initial)
        {
            return OperationResult.Failed("فقط تسویه‌های در وضعیت «ایجاد اولیه» قابل ارسال به گردش کار هستند");
        }

        var eligibilityError = ValidateSettlementEligibilityRequired(
            settlement.EmployeeId,
            settlement.OrganisationChartId > 0 ? settlement.OrganisationChartId : _currentUserDefaultOrganId,
            settlement.SettlementDate.Date,
            settlement.EndDate);
        if (eligibilityError != null)
        {
            return eligibilityError;
        }

        long? workFlowId = null;
        foreach (var orgId in new[] { settlement.OrganisationChartId, _currentUserDefaultOrganId }
                     .Where(id => id > 0)
                     .Distinct())
        {
            workFlowId = _workFlowInstanceStarter.ResolveActiveWorkFlowId(
                (long)Enums.SystemWorkFlowType.Settlement,
                orgId);

            if (workFlowId.HasValue)
            {
                break;
            }
        }

        if (!workFlowId.HasValue)
        {
            return OperationResult.Failed(
                "گردش کار تسویه حساب با مسیرهای معتبر یافت نشد. در «تعریف گردش کار» نوع «تسویه حساب‌ها» را انتخاب کنید، مسیرها را ذخیره کنید و تاریخ اعتبار آن‌ها را بررسی کنید.");
        }

        var hasExistingInstance = _workFlowUnitOfWork.Context.WorkFlowInstances
            .AsNoTracking()
            .Any(wi => wi.EmployeeSettlementId == id && !wi.IsDeleted);

        if (hasExistingInstance)
        {
            return OperationResult.Failed("گردش کار این تسویه قبلاً ایجاد شده است");
        }

        try
        {
            _unitOfWork.CreateTransaction();
            settlement.SettlementStatusId = (long)Enums.SettlementStatus.PendingReview;
            settlement.LastModifiedDate = DateTime.Now;
            _unitOfWork.Context.EmployeeSettlements.Update(settlement);
            _unitOfWork.Context.SaveChanges();

            var creatorName = $"UserId:{_userResolver.GetUserId()}";
            var startResult = _workFlowInstanceStarter.StartInstance(
                workFlowId.Value,
                creatorName,
                employeeSettlementId: id);

            if (!startResult.Success)
            {
                _unitOfWork.Rollback();
                return startResult;
            }

            _unitOfWork.Commit();
            return OperationResult.Succeeded("تسویه حساب به کارتابل تاییدکننده ارسال شد", payload: startResult.Payload);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed("خطا در ارسال تسویه به گردش کار: " + ex.Message);
        }
    }

    /// <summary>
    /// اعمال تایید نهایی تسویه بدون مدیریت تراکنش — برای استفاده در گردش کار یا API مستقل.
    /// </summary>
    public OperationResult ApplyFinalApproveToPendingSettlement(long settlementId, DbContext db)
    {
        var settlement = db.Set<EmployeeSettlement>().Find(settlementId);
        var result = EmployeeSettlementWorkflowStatus.ApplyFinalApprove(settlement);
        if (result.Success && settlement != null)
        {
            db.Set<EmployeeSettlement>().Update(settlement);
        }

        return result;
    }

    /// <summary>
    /// اعمال رد تسویه بدون مدیریت تراکنش — برای استفاده در گردش کار یا API مستقل.
    /// </summary>
    public OperationResult ApplyRejectToPendingSettlement(long settlementId, DbContext db)
    {
        var settlement = db.Set<EmployeeSettlement>().Find(settlementId);
        var result = EmployeeSettlementWorkflowStatus.ApplyReject(settlement);
        if (result.Success && settlement != null)
        {
            db.Set<EmployeeSettlement>().Update(settlement);
        }

        return result;
    }

    /// <summary>
    /// کارتابل سازمانی تسویه حساب — همه رکوردها برای واحد جاری کاربر (بدون فیلتر EmployeeId).
    /// </summary>
    public OperationResult GetOrganisationSettlementCartableList(
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        long? costCenterId = null,
        long? employeeTypeId = null,
        long? settlementStatusId = null)
    {
        if (_currentUserDefaultOrganId <= 0)
        {
            return OperationResult.Failed("واحد سازمانی جاری مشخص نیست");
        }

        var query = _unitOfWork.Context.EmployeeSettlements
            .AsNoTracking()
            .Include(es => es.SettlementCause)
            .Include(es => es.SettlementStatus)
            .Include(es => es.InterdictOrder!)
                .ThenInclude(io => io.RecruitOrder!)
                    .ThenInclude(ro => ro.CostCenter)
            .Include(es => es.InterdictOrder!)
                .ThenInclude(io => io.RecruitOrder!)
                    .ThenInclude(ro => ro.EmployeeType)
            .Where(es => !es.IsDeleted && es.OrganisationChartId == _currentUserDefaultOrganId);

        if (costCenterId.HasValue && costCenterId.Value > 0)
        {
            query = query.Where(es =>
                es.InterdictOrder != null
                && es.InterdictOrder.RecruitOrder != null
                && es.InterdictOrder.RecruitOrder.CostCenterId == costCenterId.Value);
        }

        if (employeeTypeId.HasValue && employeeTypeId.Value > 0)
        {
            query = query.Where(es => es.EmployeeTypeId == employeeTypeId.Value);
        }

        if (settlementStatusId.HasValue && settlementStatusId.Value > 0)
        {
            query = query.Where(es => es.SettlementStatusId == settlementStatusId.Value);
        }

        var settlements = query.ToList();
        if (settlements.Count == 0)
        {
            return OperationResult.Succeeded(payload: new List<EmployeeSettlementOrganisationCartableDTO>(), rowCount: 0);
        }

        var employeeIds = settlements.Select(s => s.EmployeeId).Distinct().ToList();
        var employees = _employeeContext.Employees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .ToDictionary(e => e.Id);

        var rows = settlements.Select(es =>
        {
            employees.TryGetValue(es.EmployeeId, out var employee);

            return new EmployeeSettlementOrganisationCartableDTO
            {
                Id = es.Id,
                EmployeeId = es.EmployeeId,
                FirstName = employee?.FirstName,
                LastName = employee?.LastName,
                NationalNo = employee?.NationalNo,
                EmployeeTypeId = es.EmployeeTypeId,
                EmployeeTypeTitle = es.InterdictOrder?.RecruitOrder?.EmployeeType?.title,
                CostCenterId = es.InterdictOrder?.RecruitOrder?.CostCenterId,
                CostCenterTitle = es.InterdictOrder?.RecruitOrder?.CostCenter?.title,
                SettlementStatusId = es.SettlementStatusId,
                SettlementStatusTitle = es.SettlementStatus?.title,
                SettlementCauseTitle = es.SettlementCause?.title,
                SettlementDate = es.SettlementDate,
                FiscalYear = es.FiscalYear,
                PaymentAmount = es.PaymentAmount,
                PurePaymentAmount = es.PurePaymentAmount,
                DeductionSum = es.DeductionSum,
                CreateDate = es.CreateDate,
                LastModifiedDate = es.LastModifiedDate,
            };
        }).ToList();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var filterNorm = filter.Trim();
            rows = rows.Where(r =>
                    (r.FirstName?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.LastName?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.NationalNo?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.EmployeeTypeTitle?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.CostCenterTitle?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.SettlementStatusTitle?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.SettlementCauseTitle?.Contains(filterNorm, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        rows = ApplyOrganisationSettlementCartableSort(rows, activeSortColumn, sortDirection);

        var rowCount = rows.Count;
        var paged = rows
            .Skip(Math.Max(0, currentPage) * Math.Max(1, pageSize))
            .Take(Math.Max(1, pageSize))
            .ToList();

        return OperationResult.Succeeded(payload: paged, rowCount: rowCount);
    }

    private static List<EmployeeSettlementOrganisationCartableDTO> ApplyOrganisationSettlementCartableSort(
        List<EmployeeSettlementOrganisationCartableDTO> rows,
        string activeSortColumn,
        string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(activeSortColumn))
        {
            return rows.OrderByDescending(r => r.CreateDate ?? DateTime.MinValue).ToList();
        }

        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        Func<EmployeeSettlementOrganisationCartableDTO, object?> keySelector = activeSortColumn.ToLowerInvariant() switch
        {
            "firstname" => r => r.FirstName,
            "lastname" => r => r.LastName,
            "nationalno" => r => r.NationalNo,
            "settlementdate" => r => r.SettlementDate,
            "settlementstatustitle" => r => r.SettlementStatusTitle,
            "employeetypetitle" => r => r.EmployeeTypeTitle,
            "costcentertitle" => r => r.CostCenterTitle,
            "purepaymentamount" => r => r.PurePaymentAmount,
            "createdate" => r => r.CreateDate,
            _ => r => r.Id,
        };

        return desc
            ? rows.OrderByDescending(keySelector).ToList()
            : rows.OrderBy(keySelector).ToList();
    }

    public OperationResult GetSettlementCartableDetail(long settlementId)
    {
        if (settlementId <= 0)
        {
            return OperationResult.Failed("شناسه تسویه حساب نامعتبر است");
        }

        try
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("[wf].[GetSettlementWorkFlowDetails]", con);
            cmd.Parameters.AddWithValue("@EmployeeSettlementId", settlementId);
            cmd.Parameters.AddWithValue("@UserId", _userResolver.GetUserId());
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            using SqlDataReader rdr = cmd.ExecuteReader();
            List<GetinterdictWorkFlowDetails_Result> ret = [];
            var index = 1;
            while (rdr.Read())
            {
                var row = rdr.ConvertToObject<GetinterdictWorkFlowDetails_Result>();
                row.Index = index;
                ret.Add(row);
                index++;
            }

            return OperationResult.Succeeded(payload: ret);
        }
        catch (SqlException ex)
        {
            return OperationResult.Failed($"خطا در دریافت جزئیات گردش کار تسویه: {ex.Message}");
        }
    }

    private OperationResult? TryEnrichSettlementFromEmployeeOrder(EmployeeSettlementDTO dto)
    {
        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        if (dto.OrganisationChartId <= 0 && _currentUserDefaultOrganId > 0)
        {
            dto.OrganisationChartId = _currentUserDefaultOrganId;
        }

        var organisationChartId = dto.OrganisationChartId > 0
            ? dto.OrganisationChartId
            : _currentUserDefaultOrganId;
        var payLocationId = ResolvePayLocationIdForEmployee(dto.EmployeeId, organisationChartId);
        var orderImpleDate = ResolveSettlementOrderImpleDate(dto.SettlementDate, dto.EndDate);
        var orderResolveError = TryResolveSettlementInterdictOrder(
            dto.EmployeeId,
            payLocationId,
            orderImpleDate,
            out _,
            out var recruitOrder,
            out var lastOrderId);

        if (orderResolveError != null)
        {
            return orderResolveError;
        }

        if (recruitOrder.EmployeeTypeId <= 0)
        {
            return OperationResult.Failed("نوع استخدام در حکم کارمند مشخص نیست");
        }

        var employeeTypeExists = _unitOfWork.Context.Set<EmployeeType>()
            .AsNoTracking()
            .Any(employeeType => employeeType.Id == recruitOrder.EmployeeTypeId && !employeeType.IsDeleted);

        if (!employeeTypeExists)
        {
            return OperationResult.Failed("نوع استخدام حکم کارمند در اطلاعات پایه یافت نشد");
        }

        dto.EmployeeTypeId = recruitOrder.EmployeeTypeId;
        dto.InterdictOrderId = lastOrderId;
        dto.LastInterdictOrderId = lastOrderId;

        return null;
    }

    private GetEmployeeSettlementEligibility_Result? LoadSettlementEligibilityFromSp(
        long employeeId,
        long organisationChartId,
        DateTime settlementDate)
    {
        if (employeeId <= 0 || organisationChartId <= 0)
        {
            return null;
        }

        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("[Payroll].[GetEmployeeSettlementEligibility]", con)
        {
            CommandType = CommandType.StoredProcedure,
        };
        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
        cmd.Parameters.AddWithValue("@OrganisationChartId", organisationChartId);
        cmd.Parameters.Add("@SettlementDate", SqlDbType.Date).Value = settlementDate.Date;
        cmd.Parameters.AddWithValue("@PaymentPeriodId", DBNull.Value);
        con.Open();
        using SqlDataReader rdr = cmd.ExecuteReader();
        if (!rdr.Read())
        {
            return null;
        }

        return rdr.ConvertToObject<GetEmployeeSettlementEligibility_Result>();
    }

    private static string BuildSettlementEligibilityMessage(GetEmployeeSettlementEligibility_Result row)
    {
        if (!row.HasFinalInterdict)
        {
            return "حکم فعال برای کارمند یافت نشد";
        }

        if (!row.IsInCurrentOrgan)
        {
            return "کاربر در واحد جاری شاغل نمی‌باشد و امکان تسویه وجود ندارد";
        }

        if (row.NeedSettlement != true)
        {
            return "نوع حکم جاری در تنظیمات سازمان نیازمند تسویه حساب نیست";
        }

        if (row.IsEmployed == true)
        {
            return "کارمند مورد نظر هنوز حکم خاتمه همکاری برایش صادر نشده است";
        }

        if (row.IsEmployed == false)
        {
            return "کارمند آمادگی ثبت تسویه حساب را دارد";
        }

        return row.EmployeeStatusId is null or <= 0
            ? "تنظیمات وضعیت استخدام سازمان برای حکم جاری یافت نشد"
            : "وضعیت استخدام کارمند در تنظیمات سازمان مشخص نیست";
    }

    private OperationResult? ValidateSettlementEligibilityRequired(
        long employeeId,
        long organisationChartId,
        DateTime settlementDate,
        DateTime? settlementEndDate = null)
    {
        if (employeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        if (organisationChartId <= 0)
        {
            return OperationResult.Failed("سازمان جاری مشخص نیست");
        }

        var payLocationId = ResolvePayLocationIdForEmployee(employeeId, organisationChartId);
        var orderImpleDate = ResolveSettlementOrderImpleDate(settlementDate, settlementEndDate);
        var orderResolveError = TryResolveSettlementInterdictOrder(
            employeeId,
            payLocationId,
            orderImpleDate,
            out var interdictOrder,
            out var recruitOrder,
            out _);

        if (orderResolveError != null)
        {
            return orderResolveError;
        }

        return ValidateSettlementEmploymentStatus(
            recruitOrder!,
            interdictOrder!,
            organisationChartId,
            orderImpleDate);
    }

    private static DateTime ResolveSettlementOrderImpleDate(DateTime settlementDate, DateTime? endDate)
    {
        var effectiveDate = settlementDate.Date;
        if (endDate.HasValue && endDate.Value.Date > effectiveDate)
        {
            effectiveDate = endDate.Value.Date;
        }

        return effectiveDate;
    }

    private long GetLatestFinalInterdictOrderId(long employeeId, long payLocationId)
    {
        if (employeeId <= 0 || payLocationId <= 0)
        {
            return 0;
        }

        return (
            from interdictOrder in _unitOfWork.Context.InterdictOrders.AsNoTracking()
            join recruitOrder in _unitOfWork.Context.RecruitOrders.AsNoTracking()
                on interdictOrder.RecruitOrderId equals recruitOrder.Id
            where recruitOrder.EmployeeId == employeeId
                && recruitOrder.PayLocationId == payLocationId
                && !interdictOrder.IsDeleted
                && !recruitOrder.IsDeleted
                && interdictOrder.StatusId == (long)Enums.OrderStatus.FinalOrder
            orderby interdictOrder.Serial descending, interdictOrder.Id descending
            select interdictOrder.Id
        ).FirstOrDefault();
    }

    private (bool? IsEmployed, string? StatusTitle) ResolveEmploymentInfo(
        long employeeStatusId,
        long payLocationId,
        DateTime statusCheckDate)
    {
        if (employeeStatusId <= 0 || payLocationId <= 0)
        {
            return (null, null);
        }

        var mapping = _organisationEmployeeStatusService
            .All(ImpleDate: statusCheckDate)
            .AsNoTracking()
            .Where(status => status.EmployeeStatusId == employeeStatusId
                && status.OrganisationChartId == payLocationId)
            .OrderByDescending(status => status.Id)
            .Select(status => new { status.IsEmployed, Title = status.EmployeeStatus.title })
            .FirstOrDefault();

        return mapping == null ? (null, null) : (mapping.IsEmployed, mapping.Title);
    }

    /// <summary>
    /// آخرین بازه شاغل بودن کارمند را از تایم‌لاین احکام مجاز تشخیص می‌دهد.
    /// احکام مجاز: FinalAprove / LastOrder / FinalOrder.
    /// هر حکم با IsEmployed=false و وضعیت غیر FinalOrder خودش و سریال‌های کوچک‌تر را از بازه جاری خارج می‌کند.
    /// تاریخ پایان: EndDate بزرگ‌ترین سریال با IsEmployed=true.
    /// تاریخ آغاز: StartDate کوچک‌ترین حکم مجاز پس از برش، با نادیده گرفتن FinalOrder.
    /// </summary>
    private (DateTime? SuggestedStartDate, DateTime? SuggestedEndDate) ResolveLastEmploymentPeriodDates(
        long employeeId,
        long payLocationId,
        DateTime statusCheckDate)
    {
        if (employeeId <= 0 || payLocationId <= 0)
        {
            return (null, null);
        }

        long[] allowedStatuses =
        [
            (long)Enums.OrderStatus.FinalAprove,
            (long)Enums.OrderStatus.LastOrder,
            (long)Enums.OrderStatus.FinalOrder,
        ];

        var orders = (
            from interdictOrder in _unitOfWork.Context.InterdictOrders.AsNoTracking()
            join recruitOrder in _unitOfWork.Context.RecruitOrders.AsNoTracking()
                on interdictOrder.RecruitOrderId equals recruitOrder.Id
            where recruitOrder.EmployeeId == employeeId
                && recruitOrder.PayLocationId == payLocationId
                && !interdictOrder.IsDeleted
                && !recruitOrder.IsDeleted
                && allowedStatuses.Contains(interdictOrder.StatusId)
            orderby interdictOrder.Serial ascending, interdictOrder.Id ascending
            select new
            {
                interdictOrder.Id,
                Serial = interdictOrder.Serial ?? 0,
                interdictOrder.StatusId,
                interdictOrder.StartDate,
                interdictOrder.EndDate,
                recruitOrder.EmployeeStatusId,
                recruitOrder.PayLocationId,
            }
        ).ToList();

        if (orders.Count == 0)
        {
            return (null, null);
        }

        var ordersWithEmployment = orders
            .Select(order =>
            {
                var checkDate = order.StartDate?.Date ?? statusCheckDate.Date;
                var (isEmployed, _) = ResolveEmploymentInfo(
                    order.EmployeeStatusId,
                    order.PayLocationId,
                    checkDate);
                return new
                {
                    order.Id,
                    order.Serial,
                    order.StatusId,
                    order.StartDate,
                    order.EndDate,
                    IsEmployed = isEmployed,
                };
            })
            .ToList();

        // برش تایم‌لاین: آخرین حکم خروج غیر FinalOrder و همه سریال‌های کوچک‌تر خارج از بازه جاری‌اند
        short? periodCutSerial = null;
        var periodCutCandidates = ordersWithEmployment
            .Where(order => order.IsEmployed == false
                && order.StatusId != (long)Enums.OrderStatus.FinalOrder)
            .Select(order => order.Serial)
            .ToList();
        if (periodCutCandidates.Count > 0)
        {
            periodCutSerial = periodCutCandidates.Max();
        }

        var lastEmploymentPeriod = periodCutSerial.HasValue
            ? ordersWithEmployment.Where(order => order.Serial > periodCutSerial.Value).ToList()
            : ordersWithEmployment;

        if (lastEmploymentPeriod.Count == 0)
        {
            return (null, null);
        }

        var suggestedEndDate = lastEmploymentPeriod
            .Where(order => order.IsEmployed == true)
            .OrderByDescending(order => order.Serial)
            .ThenByDescending(order => order.Id)
            .Select(order => order.EndDate)
            .FirstOrDefault();

        var suggestedStartDate = lastEmploymentPeriod
            .Where(order => order.StatusId != (long)Enums.OrderStatus.FinalOrder)
            .OrderBy(order => order.Serial)
            .ThenBy(order => order.Id)
            .Select(order => order.StartDate)
            .FirstOrDefault();

        return (suggestedStartDate, suggestedEndDate);
    }

    private bool? ResolveNeedSettlement(
        long organisationChartId,
        long employeeTypeId,
        long orderTypeId,
        DateTime checkDate)
    {
        if (organisationChartId <= 0 || employeeTypeId <= 0 || orderTypeId <= 0)
        {
            return null;
        }

        return _organisationEmployeeTypeOrderTypeService
            .All(ImpleDate: checkDate)
            .AsNoTracking()
            .Where(setting => setting.OrganisationChartId == organisationChartId
                && setting.EmployeeTypeId == employeeTypeId
                && setting.OrderTypeId == orderTypeId)
            .OrderByDescending(setting => setting.Id)
            .Select(setting => setting.NeedSettlement)
            .FirstOrDefault();
    }

    private OperationResult? ValidateSettlementEmploymentStatus(
        RecruitOrder recruitOrder,
        InterdictOrder interdictOrder,
        long organisationChartId,
        DateTime statusCheckDate)
    {
        if (recruitOrder.PayLocationId != organisationChartId)
        {
            return OperationResult.Failed("کاربر در واحد جاری شاغل نمی‌باشد و امکان تسویه وجود ندارد");
        }

        var needSettlement = ResolveNeedSettlement(
            recruitOrder.PayLocationId,
            recruitOrder.EmployeeTypeId,
            interdictOrder.OrderTypeId,
            statusCheckDate);

        if (needSettlement != true)
        {
            return OperationResult.Failed("نوع حکم جاری در تنظیمات سازمان نیازمند تسویه حساب نیست");
        }

        var (isEmployed, _) = ResolveEmploymentInfo(
            recruitOrder.EmployeeStatusId,
            recruitOrder.PayLocationId,
            statusCheckDate);

        if (isEmployed == true)
        {
            return OperationResult.Failed("کارمند مورد نظر هنوز حکم خاتمه همکاری برایش صادر نشده است");
        }

        if (isEmployed != false)
        {
            return OperationResult.Failed(
                recruitOrder.EmployeeStatusId <= 0
                    ? "تنظیمات وضعیت استخدام سازمان برای حکم جاری یافت نشد"
                    : "وضعیت استخدام کارمند در تنظیمات سازمان مشخص نیست");
        }

        return null;
    }

    private long GetLastOrderByImpleDate(
        long employeeId,
        DateTime impleDate,
        long payLocationId,
        bool requireNotEmployed = true)
    {
        if (employeeId <= 0 || payLocationId <= 0)
        {
            return 0;
        }

        using SqlConnection con = new(_connectionString);
        con.Open();
        using var command = con.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "[Order].[GetLastOrderByImpleDate]";
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
        command.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = impleDate.Date;
        command.Parameters.AddWithValue("@CorrectionOrderId", 0L);
        command.Parameters.AddWithValue("@IsForFiche", false);
        command.Parameters.AddWithValue("@PayLocationId", payLocationId);
        command.Parameters.AddWithValue("@PaymentPeriodId", DBNull.Value);
        command.Parameters.AddWithValue("@RequireNotEmployed", requireNotEmployed);
        SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
        returnValue.Direction = ParameterDirection.ReturnValue;
        command.ExecuteNonQuery();
        return Convert.ToInt64(returnValue.Value);
    }

    private OperationResult? TryResolveSettlementInterdictOrder(
        long employeeId,
        long payLocationId,
        DateTime impleDate,
        out InterdictOrder? interdictOrder,
        out RecruitOrder? recruitOrder,
        out long lastOrderId)
    {
        interdictOrder = null;
        recruitOrder = null;

        var resolvedOrderId = GetLastOrderByImpleDate(employeeId, impleDate, payLocationId);
        if (resolvedOrderId <= 0)
        {
            resolvedOrderId = GetLastOrderByImpleDate(employeeId, impleDate, payLocationId, requireNotEmployed: false);
        }

        if (resolvedOrderId <= 0)
        {
            resolvedOrderId = GetLatestFinalInterdictOrderId(employeeId, payLocationId);
        }

        lastOrderId = resolvedOrderId;

        if (resolvedOrderId <= 0)
        {
            return OperationResult.NotFound("حکم فعال برای کارمند یافت نشد");
        }

        var orderData = (
            from io in _unitOfWork.Context.InterdictOrders.AsNoTracking()
            join ro in _unitOfWork.Context.RecruitOrders.AsNoTracking()
                on io.RecruitOrderId equals ro.Id
            where io.Id == resolvedOrderId
            select new { InterdictOrder = io, RecruitOrder = ro }
        ).FirstOrDefault();

        if (orderData == null)
        {
            return OperationResult.NotFound("حکم حقوقی یافت نشد");
        }

        interdictOrder = orderData.InterdictOrder;
        recruitOrder = orderData.RecruitOrder;
        return null;
    }

    private OperationResult? ValidateSettlementDateRangeOverlap(EmployeeSettlementDTO dto)
    {
        if (!dto.StartDate.HasValue || !dto.EndDate.HasValue)
        {
            return OperationResult.Failed("تاریخ آغاز و پایان بازه تسویه حساب الزامی است");
        }

        var requestStart = dto.StartDate.Value.Date;
        var requestEnd = dto.EndDate.Value.Date;

        if (requestEnd < requestStart)
        {
            return OperationResult.Failed("تاریخ پایان بازه تسویه نمی‌تواند قبل از تاریخ آغاز باشد");
        }

        if (dto.EmployeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        var organisationChartId = dto.OrganisationChartId > 0
            ? dto.OrganisationChartId
            : _currentUserDefaultOrganId;
        var excludeId = dto.Id ?? 0;

        var overlappingSettlement = _unitOfWork.Context.EmployeeSettlements
            .AsNoTracking()
            .Where(settlement => !settlement.IsDeleted
                && settlement.EmployeeId == dto.EmployeeId
                && (organisationChartId <= 0 || settlement.OrganisationChartId == organisationChartId)
                && (excludeId <= 0 || settlement.Id != excludeId)
                && settlement.StartDate != null
                && requestStart <= (settlement.EndDate ?? DateTime.MaxValue).Date
                && settlement.StartDate.Value.Date <= requestEnd)
            .OrderByDescending(settlement => settlement.Id)
            .Select(settlement => new
            {
                settlement.Id,
                settlement.StartDate,
                settlement.EndDate,
            })
            .FirstOrDefault();

        if (overlappingSettlement == null)
        {
            return null;
        }

        var requestStartShamsi = Convertor.ToIranianDate(requestStart);
        var requestEndShamsi = Convertor.ToIranianDate(requestEnd);
        var existingStartShamsi = Convertor.ToIranianDate(overlappingSettlement.StartDate);
        var existingEndShamsi = overlappingSettlement.EndDate.HasValue
            ? Convertor.ToIranianDate(overlappingSettlement.EndDate)
            : "نامحدود";

        return OperationResult.Failed(
            $"برای این کارمند در بازه زمانی {requestStartShamsi} تا {requestEndShamsi} تسویه حساب دیگری با شناسه {overlappingSettlement.Id} ثبت شده است (بازه: {existingStartShamsi} تا {existingEndShamsi}). امکان ثبت تسویه همپوشان وجود ندارد.");
    }

    /// <summary>
    /// تعداد فیش‌های حقوقی کارمند در بازه تاریخی تسویه (بر اساس همپوشانی دوره‌های پرداخت).
    /// </summary>
    public OperationResult GetFicheCountInSettlementDateRange(long employeeId, DateTime? startDate, DateTime? endDate)
    {
        if (employeeId <= 0)
        {
            return OperationResult.Failed("شناسه کارمند الزامی است");
        }

        if (!startDate.HasValue || !endDate.HasValue)
        {
            return OperationResult.Failed("تاریخ آغاز و پایان بازه تسویه حساب الزامی است");
        }

        var requestStart = startDate.Value.Date;
        var requestEnd = endDate.Value.Date;

        if (requestEnd < requestStart)
        {
            return OperationResult.Failed("تاریخ پایان بازه تسویه نمی‌تواند قبل از تاریخ آغاز باشد");
        }

        var payLocationId = ResolvePayLocationIdForEmployee(employeeId, _currentUserDefaultOrganId);
        var ficheCount = CountFichesInSettlementDateRange(employeeId, requestStart, requestEnd, payLocationId);
        var startShamsi = Convertor.ToIranianDate(requestStart);
        var endShamsi = Convertor.ToIranianDate(requestEnd);

        return OperationResult.Succeeded(
            payload: new EmployeeSettlementFicheValidationDTO
            {
                FicheCount = ficheCount,
                HasFicheInRange = ficheCount > 0,
                RequiresConfirmation = ficheCount <= 0,
                StartDateShamsi = startShamsi,
                EndDateShamsi = endShamsi,
                Message = ficheCount > 0
                    ? $"تعداد {ficheCount} فیش حقوقی در بازه {startShamsi} تا {endShamsi} یافت شد."
                    : $"در بازه {startShamsi} تا {endShamsi} هیچ فیش حقوقی یافت نشد.",
            });
    }

    private OperationResult? ValidateSettlementFichePresence(EmployeeSettlementDTO dto)
    {
        if (dto.ProceedWithoutFiche)
        {
            return null;
        }

        if (!dto.StartDate.HasValue || !dto.EndDate.HasValue)
        {
            return null;
        }

        var requestStart = dto.StartDate.Value.Date;
        var requestEnd = dto.EndDate.Value.Date;

        if (requestEnd < requestStart)
        {
            return null;
        }

        var payLocationId = ResolvePayLocationIdForEmployee(
            dto.EmployeeId,
            dto.OrganisationChartId > 0 ? dto.OrganisationChartId : _currentUserDefaultOrganId);
        var ficheCount = CountFichesInSettlementDateRange(dto.EmployeeId, requestStart, requestEnd, payLocationId);

        if (ficheCount > 0)
        {
            return null;
        }

        var startShamsi = Convertor.ToIranianDate(requestStart);
        var endShamsi = Convertor.ToIranianDate(requestEnd);

        return OperationResult.Failed(
            $"در بازه {startShamsi} تا {endShamsi} هیچ فیش حقوقی برای این کارمند یافت نشد.",
            payload: new EmployeeSettlementFicheValidationDTO
            {
                FicheCount = 0,
                HasFicheInRange = false,
                RequiresConfirmation = true,
                StartDateShamsi = startShamsi,
                EndDateShamsi = endShamsi,
                Message = $"در بازه {startShamsi} تا {endShamsi} هیچ فیش حقوقی برای این کارمند یافت نشد.",
            });
    }

    private long ResolvePayLocationIdForEmployee(long employeeId, long fallbackOrganId)
    {
        var payLocationId = (
            from interdictOrder in _unitOfWork.Context.InterdictOrders.AsNoTracking()
            join recruitOrder in _unitOfWork.Context.RecruitOrders.AsNoTracking()
                on interdictOrder.RecruitOrderId equals recruitOrder.Id
            where recruitOrder.EmployeeId == employeeId
                && interdictOrder.StatusId == (long)Enums.OrderStatus.FinalOrder
                && !interdictOrder.IsDeleted
            orderby interdictOrder.Id descending
            select recruitOrder.PayLocationId
        ).FirstOrDefault();

        return payLocationId > 0 ? payLocationId : fallbackOrganId;
    }

    private int CountFichesInSettlementDateRange(
        long employeeId,
        DateTime requestStart,
        DateTime requestEnd,
        long payLocationId)
    {
        if (employeeId <= 0 || payLocationId <= 0)
        {
            return 0;
        }

        return (
            from fiche in _unitOfWork.Context.Fiches.AsNoTracking()
            join paymentPeriod in _unitOfWork.Context.PaymentPeriods.AsNoTracking()
                on fiche.PaymentPeriodId equals paymentPeriod.Id
            where fiche.EmployeeId == employeeId
                && fiche.OrganisationChartId == payLocationId
                && !fiche.IsDeleted
                && !paymentPeriod.IsDeleted
                && paymentPeriod.StartDate != null
                && paymentPeriod.EndDate != null
                && paymentPeriod.StartDate.Value.Date <= requestEnd
                && paymentPeriod.EndDate.Value.Date >= requestStart
            select fiche).Count();
    }

    private long? ResolveOrganisationFormulaDefinitionId(long storedFormulaId, long organisationChartId)
    {
        if (storedFormulaId <= 0 || organisationChartId <= 0)
        {
            return null;
        }

        const string sql = """
            SELECT TOP (1) orgFormula.Id
            FROM [Setting].[Organisation_Formula] orgFormula
            INNER JOIN [For].[Formula_Definition] formulaDef
                ON formulaDef.Id = orgFormula.Id
                AND ISNULL(formulaDef.IsDeleted, 0) = 0
            WHERE ISNULL(orgFormula.IsDeleted, 0) = 0
              AND orgFormula.OrganisationChartId = @OrganisationChartId
              AND (orgFormula.Id = @StoredId OR orgFormula.FormulaId = @StoredId)
            ORDER BY CASE WHEN orgFormula.Id = @StoredId THEN 0 ELSE 1 END, orgFormula.Id DESC
            """;

        using var connection = _dapper.GetDbconnection();
        connection.Open();
        return connection.Query<long?>(sql, new { StoredId = storedFormulaId, OrganisationChartId = organisationChartId })
            .FirstOrDefault();
    }

    private static Dictionary<long, long> BuildManualAmountLookup(List<EmployeeSettlementItemDTO>? items)
    {
        var lookup = new Dictionary<long, long>();
        if (items == null)
        {
            return lookup;
        }

        foreach (var item in items.Where(i => i.SettlementItemId > 0 && i.Amount.HasValue))
        {
            lookup[item.SettlementItemId] = item.Amount!.Value;
        }

        return lookup;
    }

    private static Dictionary<long, string?> BuildManualDescriptionLookup(List<EmployeeSettlementItemDTO>? items)
    {
        var lookup = new Dictionary<long, string?>();
        if (items == null)
        {
            return lookup;
        }

        foreach (var item in items.Where(i => i.SettlementItemId > 0))
        {
            lookup[item.SettlementItemId] = item.Description;
        }

        return lookup;
    }

    private static (DateTime Start, DateTime End) ResolveSettlementDateRange(
        EmployeeSettlementDTO request,
        InterdictOrder? interdictOrder)
    {
        var start = request.StartDate?.Date
            ?? interdictOrder?.StartDate?.Date
            ?? request.SettlementDate.Date;
        var end = request.EndDate?.Date
            ?? interdictOrder?.EndDate?.Date
            ?? request.SettlementDate.Date;

        if (end < start)
        {
            end = start;
        }

        return (start, end);
    }

    private static DateTime ResolveSettlementFormulaImpleDate(
        EmployeeSettlementDTO request,
        InterdictOrder? interdictOrder)
    {
        var (start, end) = ResolveSettlementDateRange(request, interdictOrder);
        var candidates = new List<DateTime> { request.SettlementDate.Date, start, end, DateTime.Now.Date };
        return candidates.Where(date => date != default).Max();
    }

    private Dictionary<string, double> BuildSettlementVariableList(
        EmployeeSettlementDTO request,
        long payLocationId,
        long lastOrderId,
        InterdictOrder? interdictOrder = null)
    {
        var variables = new Dictionary<string, double>
        {
            ["FiscalYear"] = request.FiscalYear,
            ["IsYearLong"] = request.IsYearLong ? 1 : 0,
            ["Loanincluded"] = request.Loanincluded ? 1 : 0,
            ["Deductionincluded"] = request.Deductionincluded ? 1 : 0,
        };

        var resolvedLastOrderId = lastOrderId > 0
            ? lastOrderId
            : request.LastInterdictOrderId > 0
                ? request.LastInterdictOrderId
                : request.InterdictOrderId;

        if (resolvedLastOrderId > 0)
        {
            variables["lastorderId"] = resolvedLastOrderId;
            variables["LastInterdictOrderId"] = resolvedLastOrderId;
            variables["InterdictOrderId"] = resolvedLastOrderId;
        }

        if (request.Loanincluded)
        {
            foreach (var loanVariable in BuildLoanRemainVariables(request.EmployeeId, payLocationId))
            {
                variables[loanVariable.Key] = loanVariable.Value;
            }
        }

        if (request.Deductionincluded)
        {
            foreach (var deductionVariable in BuildDeductionRemainVariables(request.EmployeeId, payLocationId))
            {
                variables[deductionVariable.Key] = deductionVariable.Value;
            }
        }

        var (settlementStart, settlementEnd) = ResolveSettlementDateRange(request, interdictOrder);

        if (request.SettlementDate != default)
        {
            variables["SettlementDate"] = Utilities.ConvertDateToNumberMiladi(request.SettlementDate.Date);
        }

        variables["SettlementStartDate"] = Utilities.ConvertDateToNumberMiladi(settlementStart);
        variables["SettlementEndDate"] = Utilities.ConvertDateToNumberMiladi(settlementEnd);

        var (years, months, days) = ParseDuration(request.Duration);
        variables["SettlementDurationYears"] = years;
        variables["SettlementDurationMonths"] = months;
        variables["SettlementDurationDays"] = days;
        variables["PayLocationId"] = payLocationId;
        variables["EmployeeId"] = request.EmployeeId;

        var employeeDateVars = request.EmployeeId > 0
            ? _employeeContext.Employees
                .AsNoTracking()
                .Where(e => e.Id == request.EmployeeId)
                .Select(e => new { e.DeathDate, e.ReleaseDate })
                .FirstOrDefault()
            : null;

        variables["DeathDate"] = employeeDateVars?.DeathDate != null
            ? Utilities.ConvertDateToNumber(employeeDateVars.DeathDate.Value)
            : 0;
        variables["ReleaseDate"] = employeeDateVars?.ReleaseDate != null
            ? Utilities.ConvertDateToNumber(employeeDateVars.ReleaseDate.Value)
            : 0;

        return variables;
    }

    private List<long> ApplyLoanSettlementItemSelection(
        List<long> selectedItemIds,
        long employeeId,
        bool loanIncluded,
        long organisationChartId,
        out OperationResult? error)
    {
        error = null;
        if (organisationChartId <= 0)
        {
            return selectedItemIds;
        }

        var loanLinkedSettlementItemIds = GetLoanLinkedSettlementItemIds(organisationChartId);
        if (loanLinkedSettlementItemIds.Count == 0)
        {
            return selectedItemIds;
        }

        if (loanIncluded)
        {
            var activeLoanSettlementItemIds = GetActiveLoanSettlementItemIdsForEmployee(employeeId, organisationChartId);
            if (activeLoanSettlementItemIds.Count == 0)
            {
                return selectedItemIds;
            }

            return selectedItemIds
                .Union(activeLoanSettlementItemIds)
                .Distinct()
                .ToList();
        }

        var filteredItemIds = selectedItemIds
            .Where(id => !loanLinkedSettlementItemIds.Contains(id))
            .ToList();

        if (selectedItemIds.Count > 0 && filteredItemIds.Count == 0)
        {
            error = OperationResult.Failed("پس از حذف آیتم‌های وام، هیچ آیتم تسویه‌ای برای محاسبه باقی نمانده است");
        }

        return filteredItemIds;
    }

    private HashSet<long> GetLoanLinkedSettlementItemIds(long organisationChartId)
    {
        return _unitOfWork.Context.LoanTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .Select(t => t.SettlementItemId!.Value)
            .ToHashSet();
    }

    private HashSet<long> GetActiveLoanSettlementItemIdsForEmployee(long employeeId, long organisationChartId)
    {
        if (employeeId <= 0 || organisationChartId <= 0)
        {
            return [];
        }

        var loanTypesById = _unitOfWork.Context.LoanTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.IsActive == true
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .ToDictionary(t => t.Id);

        if (loanTypesById.Count == 0)
        {
            return [];
        }

        var loanTypeIds = loanTypesById.Keys.ToList();
        var activeLoanTypeIds = _unitOfWork.Context.PersonnelLoans
            .AsNoTracking()
            .Where(pl => pl.EmployeeId == employeeId
                && !pl.IsDeleted
                && pl.IsActive
                && loanTypeIds.Contains(pl.LoanTypeId))
            .Select(pl => pl.LoanTypeId)
            .Distinct()
            .ToList();

        return activeLoanTypeIds
            .Select(id => loanTypesById[id].SettlementItemId!.Value)
            .ToHashSet();
    }

    private Dictionary<long, long> BuildLoanRemainAmountsBySettlementItemId(long employeeId, long organisationChartId)
    {
        var amounts = new Dictionary<long, long>();

        if (employeeId <= 0 || organisationChartId <= 0)
        {
            return amounts;
        }

        var loanTypesById = _unitOfWork.Context.LoanTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.IsActive == true
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .ToDictionary(t => t.Id);

        if (loanTypesById.Count == 0)
        {
            return amounts;
        }

        var loanTypeIds = loanTypesById.Keys.ToList();
        var activeLoans = _unitOfWork.Context.PersonnelLoans
            .AsNoTracking()
            .Where(pl => pl.EmployeeId == employeeId
                && !pl.IsDeleted
                && pl.IsActive
                && loanTypeIds.Contains(pl.LoanTypeId))
            .ToList();

        if (activeLoans.Count == 0)
        {
            return amounts;
        }

        var loanIds = activeLoans.Select(l => l.Id).ToList();
        var previousLoanPaymentSums = _unitOfWork.Context.PersonnelLoanPayments
            .AsNoTracking()
            .Where(p => loanIds.Contains(p.PersonnelLoanId) && !p.IsDeleted)
            .GroupBy(p => p.PersonnelLoanId)
            .Select(g => new { PersonnelLoanId = g.Key, Sum = g.Sum(i => i.PaymentAmount) })
            .ToDictionary(x => x.PersonnelLoanId, x => x.Sum);

        foreach (var loan in activeLoans)
        {
            if (!loanTypesById.TryGetValue(loan.LoanTypeId, out var loanType)
                || !loanType.SettlementItemId.HasValue
                || loan.AllAmount is not > 0)
            {
                continue;
            }

            previousLoanPaymentSums.TryGetValue(loan.Id, out var paidAmount);
            var remainAmount = Math.Max(0L, loan.AllAmount.Value - paidAmount);
            var settlementItemId = loanType.SettlementItemId.Value;
            amounts[settlementItemId] = amounts.GetValueOrDefault(settlementItemId) + remainAmount;
        }

        return amounts;
    }

    private Dictionary<string, double> BuildLoanRemainVariables(long employeeId, long organisationChartId)
    {
        var variables = new Dictionary<string, double>
        {
            ["TotalRemainLoanAmount"] = 0,
        };

        var amounts = BuildLoanRemainAmountsBySettlementItemId(employeeId, organisationChartId);
        long totalRemain = 0;
        foreach (var (settlementItemId, remainAmount) in amounts)
        {
            variables[$"LoanRemain_si_{settlementItemId}"] = remainAmount;
            totalRemain += remainAmount;
        }

        variables["TotalRemainLoanAmount"] = totalRemain;
        return variables;
    }

    private static bool TryResolveExternalSettlementAmount(
        long settlementItemId,
        EmployeeSettlementDTO request,
        Dictionary<long, long> loanRemainAmounts,
        Dictionary<long, long> deductionRemainAmounts,
        out long amount,
        out string message)
    {
        amount = 0;
        var messages = new List<string>();

        if (request.Loanincluded && loanRemainAmounts.TryGetValue(settlementItemId, out var loanAmount))
        {
            amount += loanAmount;
            messages.Add(loanAmount > 0 ? "مانده وام" : "بدون مانده وام");
        }

        if (request.Deductionincluded && deductionRemainAmounts.TryGetValue(settlementItemId, out var deductionAmount))
        {
            amount += deductionAmount;
            messages.Add(deductionAmount > 0 ? "مانده کسور" : "بدون مانده کسور");
        }

        if (messages.Count == 0)
        {
            message = string.Empty;
            return false;
        }

        message = string.Join(" + ", messages);
        return true;
    }

    private List<long> ApplyDeductionSettlementItemSelection(
        List<long> selectedItemIds,
        long employeeId,
        bool deductionIncluded,
        long organisationChartId,
        out OperationResult? error)
    {
        error = null;
        if (organisationChartId <= 0)
        {
            return selectedItemIds;
        }

        var deductionLinkedSettlementItemIds = GetDeductionLinkedSettlementItemIds(organisationChartId);
        if (deductionLinkedSettlementItemIds.Count == 0)
        {
            return selectedItemIds;
        }

        if (deductionIncluded)
        {
            var activeDeductionSettlementItemIds = GetActiveDeductionSettlementItemIdsForEmployee(employeeId, organisationChartId);
            if (activeDeductionSettlementItemIds.Count == 0)
            {
                return selectedItemIds;
            }

            return selectedItemIds
                .Union(activeDeductionSettlementItemIds)
                .Distinct()
                .ToList();
        }

        var filteredItemIds = selectedItemIds
            .Where(id => !deductionLinkedSettlementItemIds.Contains(id))
            .ToList();

        if (selectedItemIds.Count > 0 && filteredItemIds.Count == 0)
        {
            error = OperationResult.Failed("پس از حذف آیتم‌های کسور، هیچ آیتم تسویه‌ای برای محاسبه باقی نمانده است");
        }

        return filteredItemIds;
    }

    private HashSet<long> GetDeductionLinkedSettlementItemIds(long organisationChartId)
    {
        return _unitOfWork.Context.DeductionTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .Select(t => t.SettlementItemId!.Value)
            .ToHashSet();
    }

    private HashSet<long> GetActiveDeductionSettlementItemIdsForEmployee(long employeeId, long organisationChartId)
    {
        if (employeeId <= 0 || organisationChartId <= 0)
        {
            return [];
        }

        var deductionTypesById = _unitOfWork.Context.DeductionTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.IsActive
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .ToDictionary(t => t.Id);

        if (deductionTypesById.Count == 0)
        {
            return [];
        }

        var deductionTypeIds = deductionTypesById.Keys.ToList();
        var activeDeductionTypeIds = _unitOfWork.Context.EmployeeDeductions
            .AsNoTracking()
            .Where(ed => ed.EmployeeId == employeeId
                && !ed.IsDeleted
                && ed.IsActive
                && deductionTypeIds.Contains(ed.DeductionTypeId))
            .Select(ed => ed.DeductionTypeId)
            .Distinct()
            .ToList();

        return activeDeductionTypeIds
            .Select(id => deductionTypesById[id].SettlementItemId!.Value)
            .ToHashSet();
    }

    private Dictionary<long, long> BuildDeductionRemainAmountsBySettlementItemId(long employeeId, long organisationChartId)
    {
        var amounts = new Dictionary<long, long>();

        if (employeeId <= 0 || organisationChartId <= 0)
        {
            return amounts;
        }

        var deductionTypesById = _unitOfWork.Context.DeductionTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == organisationChartId
                && !t.IsDeleted
                && t.IsActive
                && t.SettlementItemId.HasValue
                && t.SettlementItemId.Value > 0)
            .ToDictionary(t => t.Id);

        if (deductionTypesById.Count == 0)
        {
            return amounts;
        }

        var deductionTypeIds = deductionTypesById.Keys.ToList();
        var activeDeductions = _unitOfWork.Context.EmployeeDeductions
            .AsNoTracking()
            .Where(ed => ed.EmployeeId == employeeId
                && !ed.IsDeleted
                && ed.IsActive
                && deductionTypeIds.Contains(ed.DeductionTypeId))
            .ToList();

        if (activeDeductions.Count == 0)
        {
            return amounts;
        }

        var deductionIds = activeDeductions.Select(d => d.Id).ToList();
        var previousDeductionPaymentSums = _unitOfWork.Context.EmployeeDeductionPayments
            .AsNoTracking()
            .Where(p => deductionIds.Contains(p.EmployeeDeductionId) && !p.IsDeleted)
            .GroupBy(p => p.EmployeeDeductionId)
            .Select(g => new { EmployeeDeductionId = g.Key, Sum = g.Sum(i => i.PaymentAmount) })
            .ToDictionary(x => x.EmployeeDeductionId, x => x.Sum);

        foreach (var deduction in activeDeductions)
        {
            if (!deductionTypesById.TryGetValue(deduction.DeductionTypeId, out var deductionType)
                || !deductionType.SettlementItemId.HasValue
                || deduction.AllAmount is not > 0)
            {
                continue;
            }

            previousDeductionPaymentSums.TryGetValue(deduction.Id, out var paidAmount);
            var remainAmount = Math.Max(0L, deduction.AllAmount.Value - paidAmount);
            var settlementItemId = deductionType.SettlementItemId.Value;
            amounts[settlementItemId] = amounts.GetValueOrDefault(settlementItemId) + remainAmount;
        }

        return amounts;
    }

    private Dictionary<string, double> BuildDeductionRemainVariables(long employeeId, long organisationChartId)
    {
        var variables = new Dictionary<string, double>
        {
            ["TotalRemainDeductionAmount"] = 0,
        };

        var amounts = BuildDeductionRemainAmountsBySettlementItemId(employeeId, organisationChartId);
        long totalRemain = 0;
        foreach (var (settlementItemId, remainAmount) in amounts)
        {
            variables[$"DeductionRemain_si_{settlementItemId}"] = remainAmount;
            totalRemain += remainAmount;
        }

        variables["TotalRemainDeductionAmount"] = totalRemain;
        return variables;
    }

    private static (int years, int months, int days) ParseDuration(string? duration)
    {
        if (string.IsNullOrWhiteSpace(duration) || duration.Length != 6)
        {
            return (0, 0, 0);
        }

        _ = int.TryParse(duration[..2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var years);
        _ = int.TryParse(duration.Substring(2, 2), NumberStyles.Integer, CultureInfo.InvariantCulture, out var months);
        _ = int.TryParse(duration.Substring(4, 2), NumberStyles.Integer, CultureInfo.InvariantCulture, out var days);
        return (years, months, days);
    }

    private List<InterdictOrderWageItem> LoadOrderWageItems(long interdictOrderId)
    {
        var items = new List<InterdictOrderWageItem>();
        using SqlConnection con = new(_connectionString);
        con.Open();
        using SqlCommand cmd = new("SELECT * FROM [Order].Interdict_Order_WageItem WHERE InterdictOrderId = @InterdictOrderId", con);
        cmd.Parameters.Add("@InterdictOrderId", SqlDbType.BigInt).Value = interdictOrderId;
        using SqlDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            items.Add(rdr.ConvertToObject<InterdictOrderWageItem>());
        }

        return items;
    }

    private PersonnelFunction? LoadPersonnelFunction(long employeeId, long paymentPeriodId, long payLocationId)
    {
        using SqlConnection con = new(_connectionString);
        con.Open();
        using SqlCommand cmd = new("[Payroll].[Get_Personnel_Function_ForFiche]", con)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
        cmd.Parameters.AddWithValue("@PaymentPeriodId", paymentPeriodId);
        cmd.Parameters.AddWithValue("@PayLocationId", payLocationId);
        using SqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            return rdr.ConvertToObject<PersonnelFunction>();
        }

        return null;
    }

    private static List<EmployeeSettlementItemDTO> BuildItemsFromIds(EmployeeSettlementDTO dto)
    {
        return dto.SettlementItemIds?
            .Select(id => new EmployeeSettlementItemDTO { SettlementItemId = id })
            .ToList() ?? [];
    }

    private async Task SaveSettlementItemsAsync(long settlementId, List<EmployeeSettlementItemDTO> items, string? duration)
    {
        var existingItems = _unitOfWork.Context.EmployeeSettlementItems
            .Where(i => i.EmployeeSettlementId == settlementId && !i.IsDeleted)
            .ToList();

        foreach (var existingItem in existingItems)
        {
            _unitOfWork.Context.LogicalRemove<EmployeeSettlementItem>(existingItem.Id);
        }

        foreach (var item in items)
        {
            if (item.SettlementItemId <= 0)
            {
                continue;
            }

            var entity = _mapper.Map<EmployeeSettlementItem>(item);
            entity.EmployeeSettlementId = settlementId;
            entity.Duration = duration;
            entity.Amount = item.Amount ?? 0;
            entity.SystemCalculatedAmount = item.SystemCalculatedAmount ?? 0;
            entity.title = string.IsNullOrWhiteSpace(item.SettlementItemTitle)
                ? item.SettlementItemId.ToString()
                : item.SettlementItemTitle;
            _unitOfWork.Context.EmployeeSettlementItems.Add(entity);
        }

        await _unitOfWork.Save();
    }

    public OperationResult DownloadSettlementPDF(long settlementId, bool isRaw)
    {
        var settlement = _unitOfWork.Context.EmployeeSettlements
            .AsNoTracking()
            .Include(i => i.InterdictOrder)
            .ThenInclude(i => i!.RecruitOrder)
            .SingleOrDefault(i => i.Id == settlementId);

        if (settlement?.InterdictOrder?.RecruitOrder == null)
        {
            return OperationResult.NotFound("تسویه حساب یا حکم مرتبط یافت نشد");
        }

        var recruitOrder = settlement.InterdictOrder.RecruitOrder;
        var employeeTypeId = settlement.EmployeeTypeId;
        var payLocationId = recruitOrder.PayLocationId > 0
            ? recruitOrder.PayLocationId
            : settlement.OrganisationChartId;
        var employeeId = settlement.EmployeeId;
        var interdictOrderId = settlement.InterdictOrderId;

        var mrtMappings = _organisationEmployeeTypeMRTService.All().Where(i =>
            i.EmployeeTypeId == employeeTypeId
            && i.OrganisationChartId == payLocationId
            && i.IsRaw == isRaw
            && i.IsManager == false
            && i.SettingTypeId == (long)Enums.MRTtype.Settlement);

        if (!mrtMappings.Any())
        {
            return OperationResult.NotFound("تنظیمات گزارش چاپی تسویه حساب یافت نشد");
        }

        if (mrtMappings.Count() > 1)
        {
            return OperationResult.NotFound("بیش از یک رکورد تنظیمات چاپ تسویه یافت شد؛ لطفا تاریخ تنظیمات را بررسی بفرمایید");
        }

        var mrt = _organisationMRTService.GetIdAsync(mrtMappings.Single().OrganisationMRTId).Result;
        if (mrt == null)
        {
            return OperationResult.NotFound("قالب چاپ تسویه حساب یافت نشد");
        }

        OrganisationChartImage? orgImg = _organisationContext.OrganisationChartImages
            .Where(i => i.OrganisationChartId == payLocationId)
            .FirstOrDefault();

        HR.Employee.Core.Entities.Image? employeeImg = _employeeContext.Images
            .Where(i => i.OrganisationChartId == payLocationId && i.EmployeeId == employeeId)
            .OrderBy(i => i.Id)
            .LastOrDefault();

        try
        {
            var absolutePath = ResolveFontsDirectory();
            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                return OperationResult.Failed("مسیر فونت‌ها یافت نشد: Assets\\Fonts");
            }

            var settlementPdf = new SettlementPrint().GetSettlementPrint(
                settlementId,
                interdictOrderId,
                mrt,
                orgImg,
                employeeImg,
                absolutePath,
                _connectionString);

            return OperationResult.Succeeded(payload: settlementPdf);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطا در تولید PDF تسویه: " + ex.Message);
        }
    }

    private static string? ResolveFontsDirectory()
    {
        var baseDirs = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
        foreach (var baseDir in baseDirs)
        {
            var probe = baseDir;
            for (var up = 0; up <= 5 && !string.IsNullOrEmpty(probe); up++)
            {
                var candidate = Path.Combine(probe, "Assets", "Fonts");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                probe = Path.GetDirectoryName(probe);
            }
        }

        return null;
    }

    public OperationResult GetEmployeeSettlementArchiveStatus(long settlementId)
    {
        var archive = _unitOfWork.Context.EmployeeSettlementArchives
            .AsNoTracking()
            .Where(a => a.EmployeeSettlementId == settlementId)
            .Select(a => new EmployeeSettlementArchiveStatusDto
            {
                HasRawPdf = a.PdfrawByteArray != null && a.PdfrawByteArray.Length > 0,
                HasFormattedPdf = a.PdfbyteArray != null && a.PdfbyteArray.Length > 0,
            })
            .FirstOrDefault();

        return OperationResult.Succeeded(payload: archive ?? new EmployeeSettlementArchiveStatusDto());
    }

    public OperationResult DownloadEmployeeSettlementArchivePdf(long settlementId, bool isRaw)
    {
        var archive = _unitOfWork.Context.EmployeeSettlementArchives
            .AsNoTracking()
            .Where(a => a.EmployeeSettlementId == settlementId)
            .Select(a => new
            {
                a.PdfrawByteArray,
                a.PdfbyteArray,
            })
            .FirstOrDefault();

        if (archive == null)
        {
            return OperationResult.NotFound("آرشیو تسویه حساب یافت نشد");
        }

        var pdfBytes = isRaw ? archive.PdfrawByteArray : archive.PdfbyteArray;
        if (pdfBytes == null || pdfBytes.Length == 0)
        {
            return OperationResult.NotFound(isRaw ? "فایل PDF خام آرشیو موجود نیست" : "فایل PDF قالب‌دار آرشیو موجود نیست");
        }

        return OperationResult.Succeeded(payload: pdfBytes);
    }

    public OperationResult RebuildEmployeeSettlementArchive(long settlementId)
    {
        var settlementExists = _unitOfWork.Context.EmployeeSettlements.AsNoTracking().Any(s => s.Id == settlementId);
        if (!settlementExists)
        {
            return OperationResult.NotFound("تسویه حساب یافت نشد");
        }

        var rawResult = DownloadSettlementPDF(settlementId, true);
        var formattedResult = DownloadSettlementPDF(settlementId, false);

        var rawBytes = rawResult.Success && rawResult.Payload is byte[] rawPayload && rawPayload.Length > 0
            ? rawPayload
            : null;
        var formattedBytes = formattedResult.Success && formattedResult.Payload is byte[] formattedPayload && formattedPayload.Length > 0
            ? formattedPayload
            : null;

        if (rawBytes == null && formattedBytes == null)
        {
            var messages = new List<string>();
            if (rawBytes == null)
            {
                messages.Add(FormatSettlementArchivePdfError(rawResult.Message, isRaw: true));
            }

            if (formattedBytes == null)
            {
                messages.Add(FormatSettlementArchivePdfError(formattedResult.Message, isRaw: false));
            }

            return OperationResult.Failed(string.Join(" | ", messages.Distinct()));
        }

        var hadExistingArchive = EmployeeSettlementArchivePersistence.Exists(_unitOfWork.Context, settlementId);
        EmployeeSettlementArchivePersistence.Replace(
            _unitOfWork.Context,
            settlementId,
            rawBytes,
            formattedBytes);

        var status = new EmployeeSettlementArchiveStatusDto
        {
            HasRawPdf = rawBytes != null,
            HasFormattedPdf = formattedBytes != null,
        };

        var warnings = new List<string>();
        if (rawBytes == null)
        {
            warnings.Add(FormatSettlementArchivePdfError(rawResult.Message, isRaw: true));
        }

        if (formattedBytes == null)
        {
            warnings.Add(FormatSettlementArchivePdfError(formattedResult.Message, isRaw: false));
        }

        if (warnings.Count > 0)
        {
            return OperationResult.Succeeded(
                msg: "آرشیو ذخیره شد، اما برخی فایل‌ها تولید نشدند: " + string.Join(" | ", warnings),
                payload: status);
        }

        return OperationResult.Succeeded(
            msg: hadExistingArchive ? "آرشیو با موفقیت بروزرسانی شد" : "آرشیو با موفقیت ایجاد شد",
            payload: status);
    }

    private static string FormatSettlementArchivePdfError(string? message, bool isRaw)
    {
        var pdfType = isRaw ? "PDF خام" : "PDF قالب‌دار";
        if (string.IsNullOrWhiteSpace(message))
        {
            return $"امکان تولید {pdfType} وجود ندارد";
        }

        if (message.Contains("تنظیمات گزارش چاپی تسویه حساب یافت نشد", StringComparison.Ordinal))
        {
            return isRaw
                ? "تنظیمات قالب چاپ (MRT) برای PDF خام تسویه حساب یافت نشد. لطفاً در تنظیمات سازمان، قالب چاپ تسویه خام را برای نوع استخدام و محل پرداخت مربوطه تعریف کنید."
                : "تنظیمات قالب چاپ (MRT) برای PDF قالب‌دار تسویه حساب یافت نشد. لطفاً در تنظیمات سازمان، قالب چاپ تسویه قالب‌دار را برای نوع استخدام و محل پرداخت مربوطه تعریف کنید.";
        }

        if (message.Contains("بیش از یک رکورد تنظیمات چاپ", StringComparison.Ordinal))
        {
            return $"برای {pdfType} بیش از یک تنظیمات قالب چاپ (MRT) فعال یافت شد. لطفاً تاریخ اعتبار تنظیمات را بررسی کنید.";
        }

        if (message.Contains("مسیر فونت", StringComparison.Ordinal))
        {
            return $"خطا در تولید {pdfType}: {message}";
        }

        return $"{pdfType}: {message}";
    }
}
