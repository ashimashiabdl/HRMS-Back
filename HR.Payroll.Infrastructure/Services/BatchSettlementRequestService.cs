using AutoMapper;
using Hr.SystemSetting.Infrastructure.Services;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class BatchSettlementRequestService(
    IMapper mapper,
    IUnitOfWork<PayrollContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    OrganisationEmployeeStatusService organisationEmployeeStatusService,
    EmployeeSettlementService employeeSettlementService) : BaseService<BatchSettlementRequest, PayrollContext, BatchSettlementRequestDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly UserResolverService _userResolverService = userService;
    private readonly OrganisationEmployeeStatusService _organisationEmployeeStatusService = organisationEmployeeStatusService;
    private readonly EmployeeSettlementService _employeeSettlementService = employeeSettlementService;

    public OperationResult UpdateRequestState(long batchSettlementRequestId, Enums.BatchSettlementRequestState newState)
    {
        var request = _unitOfWork.Context.BatchSettlementRequests.Find(batchSettlementRequestId);
        if (request == null)
        {
            return OperationResult.NotFound();
        }

        if (request.RequestStateId == (long)Enums.BatchSettlementRequestState.Deleted)
        {
            return OperationResult.Failed("وضعیت درخواست حذف شده می‌باشد؛ امکان تغییر وضعیت وجود ندارد");
        }

        request.RequestStateId = (long)newState;
        request.LastModifiedDate = DateTime.Now;
        _unitOfWork.Context.Update(request);
        _unitOfWork.Context.SaveChanges();
        return OperationResult.Succeeded();
    }

    /// <summary>
    /// کاندیداهای تسویه حساب گروهی — همان منطق کارتابل تسویه (احکام نهایی + غیرشاغل) منهای کارکنانی که تسویه فعال دارند.
    /// </summary>
    public OperationResult GetEligibleSettlementCandidates(long? payLocationId, long? costCenterId)
    {
        try
        {
            if (_currentUserDefaultOrganId <= 0)
            {
                return OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است");
            }

            _employeeSettlementService._currentUserDefaultOrganId = _currentUserDefaultOrganId;
            _employeeSettlementService._currentUserDefaultPaymentPeriod = _currentUserDefaultPaymentPeriod;

            var candidates = QuerySettlementCartableCandidates(payLocationId, costCenterId);
            var index = 1;
            foreach (var candidate in candidates)
            {
                candidate.Id = index++;
                var eligibility = _employeeSettlementService.GetSettlementEligibilityByEmployeeId(candidate.EmployeeId);
                if (eligibility.Success && eligibility.Payload is EmployeeSettlementEligibilityDTO dto)
                {
                    candidate.IsEligible = dto.IsEligibleForSettlement;
                    candidate.EligibilityMessage = dto.Message;
                }
                else
                {
                    candidate.IsEligible = false;
                    candidate.EligibilityMessage = eligibility.Message ?? "امکان بررسی صلاحیت وجود ندارد";
                }
            }

            return OperationResult.Succeeded(payload: candidates, rowCount: candidates.Count);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت فهرست کاندیداها: {ex.Message}");
        }
    }

    private List<BatchSettlementCandidateDTO> QuerySettlementCartableCandidates(long? payLocationId, long? costCenterId)
    {
        var effectivePayLocationId = _currentUserDefaultOrganId > 0
            ? _currentUserDefaultOrganId
            : payLocationId;

        var now = DateTime.Now.Date;
        var nonEmployedMappings = _organisationEmployeeStatusService
            .All(ImpleDate: now)
            .AsNoTracking()
            .Where(o => o.IsDeleted != true
                && o.IsEmployed == false
                && (o.StartDate == null || o.StartDate.Value.Date <= now)
                && (o.EndDate == null || o.EndDate > now))
            .Select(o => new { o.EmployeeStatusId, o.OrganisationChartId })
            .ToList();

        if (effectivePayLocationId.HasValue && effectivePayLocationId.Value > 0)
        {
            nonEmployedMappings = nonEmployedMappings
                .Where(o => o.OrganisationChartId == effectivePayLocationId.Value)
                .ToList();
        }

        if (nonEmployedMappings.Count == 0)
        {
            return [];
        }

        var mappingSet = nonEmployedMappings
            .Select(m => (m.EmployeeStatusId, m.OrganisationChartId))
            .ToHashSet();

        var employeeStatusIds = nonEmployedMappings.Select(m => m.EmployeeStatusId).Distinct().ToList();
        var payLocationIds = nonEmployedMappings.Select(m => m.OrganisationChartId).Distinct().ToList();

        var employeesWithActiveSettlement = _unitOfWork.Context.EmployeeSettlements
            .AsNoTracking()
            .Where(s => !s.IsDeleted
                && s.SettlementStatusId != (long)Enums.SettlementStatus.NotApproved)
            .Select(s => s.EmployeeId)
            .Distinct()
            .ToHashSet();

        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r!.Employee)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r!.PayLocation)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r!.CostCenter)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r!.EmployeeStatus)
            .Include(i => i.OrderType)
            .Where(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder
                && !i.IsDeleted
                && i.RecruitOrder != null
                && employeeStatusIds.Contains(i.RecruitOrder.EmployeeStatusId)
                && payLocationIds.Contains(i.RecruitOrder.PayLocationId));

        if (effectivePayLocationId.HasValue && effectivePayLocationId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder!.PayLocationId == effectivePayLocationId.Value);
        }

        if (costCenterId.HasValue && costCenterId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder!.CostCenterId == costCenterId.Value);
        }

        return query
            .OrderByDescending(i => i.CreateDate)
            .AsEnumerable()
            .Where(i => mappingSet.Contains((i.RecruitOrder!.EmployeeStatusId, i.RecruitOrder.PayLocationId)))
            .Where(i => !employeesWithActiveSettlement.Contains(i.RecruitOrder!.EmployeeId))
            .GroupBy(i => i.RecruitOrder!.EmployeeId)
            .Select(g => g.First())
            .Select(i => new BatchSettlementCandidateDTO
            {
                EmployeeId = i.RecruitOrder!.EmployeeId,
                InterdictOrderId = i.Id,
                FirstName = i.RecruitOrder.Employee?.FirstName,
                LastName = i.RecruitOrder.Employee?.LastName,
                NationalNo = i.RecruitOrder.Employee?.NationalNo,
                PayLocation = i.RecruitOrder.PayLocation?.title,
                CostCenter = i.RecruitOrder.CostCenter?.title,
                EmployeeStatus = i.RecruitOrder.EmployeeStatus?.title,
                OrderType = i.OrderType?.title,
                OrderStartDate = i.StartDate,
                OrderEndDate = i.EndDate,
            })
            .ToList();
    }

    public new OperationResult CreateForAsync(BatchSettlementRequestDTO entityToCreate)
    {
        try
        {
            if (_currentUserDefaultOrganId <= 0)
            {
                return OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است");
            }

            if (entityToCreate.SettlementCauseId <= 0)
            {
                return OperationResult.Failed("علت تسویه الزامی است");
            }

            if (entityToCreate.SettlementDate == default)
            {
                return OperationResult.Failed("تاریخ تسویه حساب الزامی است");
            }

            if (entityToCreate.SettlementStartDate == default || entityToCreate.SettlementEndDate == default)
            {
                return OperationResult.Failed("بازه زمانی تسویه (تاریخ آغاز و پایان) الزامی است");
            }

            if (entityToCreate.SettlementEndDate.Date < entityToCreate.SettlementStartDate.Date)
            {
                return OperationResult.Failed("تاریخ پایان بازه تسویه نمی‌تواند قبل از تاریخ آغاز باشد");
            }

            List<BatchSettlementCandidateDTO> candidates;
            if (entityToCreate.EmployeeIds is { Count: > 0 })
            {
                var allCandidates = QuerySettlementCartableCandidates(null, entityToCreate.CostCenterId);
                var selectedIds = entityToCreate.EmployeeIds.ToHashSet();
                candidates = allCandidates.Where(c => selectedIds.Contains(c.EmployeeId)).ToList();
                if (candidates.Count != selectedIds.Count)
                {
                    var missing = selectedIds.Except(candidates.Select(c => c.EmployeeId)).Count();
                    return OperationResult.Failed($"{missing} کارمند انتخاب‌شده در فهرست کاندیدای تسویه یافت نشد یا قبلاً تسویه فعال دارد");
                }
            }
            else
            {
                candidates = QuerySettlementCartableCandidates(null, entityToCreate.CostCenterId);
            }

            if (candidates.Count == 0)
            {
                return OperationResult.Failed("هیچ کارمند واجد شرایطی برای صدور تسویه حساب گروهی یافت نشد");
            }

            var paymentPeriodId = entityToCreate.PaymentPeriodId ?? _currentUserDefaultPaymentPeriod;
            if (paymentPeriodId <= 0)
            {
                return OperationResult.Failed("دوره پیش‌فرض پرداخت از تنظیمات کاربر مشخص نشده است");
            }

            var toAdd = new BatchSettlementRequest
            {
                OrganisationChartId = _currentUserDefaultOrganId,
                UserId = entityToCreate.UserId,
                RequestStateId = (long)Enums.BatchSettlementRequestState.Initial,
                RequestTypeId = (long)Enums.BatchSettlementRequestType.NormalSettlement,
                SettlementCauseId = entityToCreate.SettlementCauseId,
                PaymentPeriodId = paymentPeriodId,
                PayLocationId = _currentUserDefaultOrganId,
                CostCenterId = entityToCreate.CostCenterId,
                SettlementDate = entityToCreate.SettlementDate.Date,
                SettlementStartDate = entityToCreate.SettlementStartDate.Date,
                SettlementEndDate = entityToCreate.SettlementEndDate.Date,
                FiscalYear = entityToCreate.FiscalYear > 0 ? entityToCreate.FiscalYear : DateTime.Now.Year,
                IsYearLong = entityToCreate.IsYearLong,
                Loanincluded = entityToCreate.Loanincluded,
                Deductionincluded = entityToCreate.Deductionincluded,
                SendToCartable = entityToCreate.SendToCartable,
                ProceedWithoutFiche = entityToCreate.ProceedWithoutFiche,
                Username = string.IsNullOrWhiteSpace(_userResolverService.fullname())
                    ? _userResolverService.GetUser()
                    : _userResolverService.fullname(),
                RequsetDescription = entityToCreate.RequsetDescription,
                EmployeeCount = candidates.Count,
                title = $"تسویه گروهی-{DateTime.Now:yyyyMMddHHmmss}",
                CreateDate = DateTime.Now,
                IPAddress = string.Empty,
            };

            _unitOfWork.CreateTransaction();
            try
            {
                _unitOfWork.Context.Add(toAdd);
                _unitOfWork.Context.SaveChanges();

                foreach (var candidate in candidates)
                {
                    var row = new BatchSettlementRequestDetail
                    {
                        BatchSettlementRequestId = toAdd.Id,
                        EmployeeId = candidate.EmployeeId,
                        InterdictOrderId = candidate.InterdictOrderId,
                        title = $"ردیف-{candidate.EmployeeId}",
                        IPAddress = string.Empty,
                        CreateDate = DateTime.Now,
                    };
                    _unitOfWork.Context.Add(row);
                }

                _unitOfWork.Context.SaveChanges();
                _unitOfWork.Commit();
                return OperationResult.Succeeded(
                    $"درخواست تسویه گروهی برای {candidates.Count} نفر ثبت شد",
                    payload: toAdd.Id);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed($"خطا در ثبت درخواست: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطای غیرمنتظره در ایجاد درخواست: {ex.Message}");
        }
    }

    public OperationResult Get(long id, long currentUserId, bool isAdmin)
    {
        var properties = typeof(BatchSettlementRequest).GetProperties();
        var all = All(false);
        foreach (var property in properties)
        {
            if (property.PropertyType.BaseType == typeof(BatchSettlementRequest).BaseType)
            {
                all = all.Include(property.Name);
            }
        }

        if (!isAdmin)
        {
            all = all.Where(i => i.UserId == currentUserId);
        }

        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<BatchSettlementRequestDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        PopulateRequestMetadata(record);
        return OperationResult.Succeeded(payload: record);
    }

    public OperationResult GetPagedData(int currentPage, int pageSize, string filter, string activeSortColumn, string Sortdirection, bool IgnoreExpired, long currentUserId, bool isAdmin)
    {
        var properties = typeof(BatchSettlementRequest).GetProperties();
        var all = All(IgnoreExpired);
        foreach (var property in properties)
        {
            if (property.PropertyType.BaseType == typeof(BatchSettlementRequest).BaseType)
            {
                all = all.Include(property.Name);
            }
        }

        if (!isAdmin)
        {
            all = all.Where(i => i.UserId == currentUserId);
            all = all.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
        }

        var flatList = _mapper.Map<List<BatchSettlementRequestDTO>>(
            PagerUtility<BatchSettlementRequest>.GetPagedData(all, out var rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection))
            ?? [];

        flatList.ForEach(PopulateRequestMetadata);
        return OperationResult.Succeeded(payload: flatList, rowCount: rowCount);
    }

    private static void PopulateRequestMetadata(BatchSettlementRequestDTO? dto)
    {
        if (dto == null)
        {
            return;
        }

        dto.RequestState = GetRequestStateTitle(dto.RequestStateId);
        dto.RequestType = GetRequestTypeTitle(dto.RequestTypeId);
    }

    private static string GetRequestStateTitle(long? requestStateId) => requestStateId switch
    {
        (long)Enums.BatchSettlementRequestState.Initial => "ایجاد اولیه",
        (long)Enums.BatchSettlementRequestState.EndLoop => "پایان حلقه",
        (long)Enums.BatchSettlementRequestState.TryAgain => "تلاش مجدد",
        (long)Enums.BatchSettlementRequestState.CancelByUser => "انصراف کاربر",
        (long)Enums.BatchSettlementRequestState.Running => "درحال اجرا",
        (long)Enums.BatchSettlementRequestState.Deleted => "حذف شده",
        _ => string.Empty
    };

    private static string GetRequestTypeTitle(long? requestTypeId) => requestTypeId switch
    {
        (long)Enums.BatchSettlementRequestType.NormalSettlement => "صدور تسویه حساب گروهی",
        _ => string.Empty
    };

    public bool Validate(BatchSettlementRequest entity, object? etc = null) => throw new NotImplementedException();
}
