using AutoMapper;
using DynamicExpressions.Linq;
using Hr.Employee.infrastructure.Data;
using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Services;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Services;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.SharedKernel.Excel;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Helpers;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
using HR.Report.Infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.Data.SqlClient;
using System.Data;

using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Dynamic;
using System.Globalization;

using System.Linq.Dynamic.Core;

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static HR.SharedKernel.Share.Enums;
using Dapper;

namespace HR.Order.Infrastructure.Services;


public class OrderService(OrganisationEmployeeTypeOrderTypeCanChangeService OrganisationEmployeeTypeOrderTypeCanChangeService,
OrganisationEmployeeTypeOrderTypeSummaryCalcService OrganisationEmployeeTypeOrderTypeSummaryCalcService,
OrganisationEmployeeTypeWageItemService OrganisationEmployeeTypeWageItemService,
UserResolverService UserResolverService,
CompetencyService CompetencyService,
OrganizationJobCompetencyQualificationService OrganizationJobCompetencyQualificationService,
ProjectService ProjectService,
CourseService CourseService,
CharacterService CharacterService,
EmployeeSoftwareService EmployeeSoftwareService,
ForignLanguageService ForeignLanguageService,
EmployeeStatusService EmployeeStatusService,
BaseInfo.infrastructure.Services.CoefficientService CoefficientService,
FormulaEngine.Infrastructure.Services.FormulaService FormulaService,
OrganisationEmployeeTypeOrderTypeCheckService OrganisationEmployeeTypeOrderTypeCheckService,
OrganisationEmployeeTypeOrderTypeDescriptionService OrganisationEmployeeTypeOrderTypeDescriptionService,
OrganisationPositionService OrganisationPositionService,
OrganisationEmployeeTypeOrderTypeService OrganisationEmployeeTypeOrderTypeService,
OrganizationJobService OrganizationJobService,
OrganisationFormulaService OrganisationFormulaService,
OrganisationWageItemService OrganisationWageItemService,
OrganisationCoefficientService OrganisationCoefficientService,
OrganisationOrderTypeService OrganisationOrderTypeService,
EmployeeTypeService EmployeeTypeService,
FormulaOperandService FormulaOperandService,
FormulaDatabaseFunctionDefinitionService FormulaDatabaseFunctionDefinitionService,
FormulaTableValueService FormulaTableValueService,
FormulaTableService FormulaTableService,
BaseTableValueService BaseTableValueService,
OrganisationEmployeeTypeService OrganisationEmployeeTypeService,
EducationGradeService EducationGradeService,
OrganisationMRTService OrganisationMRTService,
JobService JobService,

WageItemService WageItemService,
OrderTypeService OrderTypeService,
OrganizationJobInitialCourseQualificationService OrganizationJobInitialCourseQualificationService,
OrganizationJobEducationGradeQualificationService OrganizationJobEducationGradeQualificationService,
OrganizationJobEducationFieldQualificationService OrganizationJobEducationFieldQualificationService,
OrganisationEmployeeTypeMRTService OrganisationEmployeeTypeMRTService,
OrganisationChartService OrganisationChartService,
AbilityService AbilityService,
OrganizationJobRequiredSoftwaresQualificationService OrganizationJobRequiredSoftwaresQualificationService,
OrganizationJobRequiredCharacterQualificationService OrganizationJobRequiredCharacterQualificationService,
OrganizationJobForeignLanguageQualificationService OrganizationJobForeignLanguageQualificationService,
OrganizationJobAbilityQualificationService OrganizationJobAbilityQualificationService,
SystemSettingContext SystemSettingContext,
EmployeeContext EmployeeContext,
OrganisationContext OrganisationContext,
IMapper mapper,
IUnitOfWork<OrderContext> unitOfWork,
IDapper dapper,
IConfiguration configuration,
UserResolverService userService,
ILogger<OrderService> logger) : BaseService<InterdictOrder, OrderContext, InterdictOrderDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private SystemSettingContext _systemSettingContext = SystemSettingContext;
    private OrganisationContext _organisationContext = OrganisationContext;
    private EmployeeContext _employeeContext = EmployeeContext;
    private OrganisationEmployeeTypeOrderTypeCanChangeService _organisationEmployeeTypeOrderTypeCanChangeService = OrganisationEmployeeTypeOrderTypeCanChangeService;
    private OrganisationEmployeeTypeOrderTypeDescriptionService _organisationEmployeeTypeOrderTypeDescriptionService = OrganisationEmployeeTypeOrderTypeDescriptionService;
    private OrganisationEmployeeTypeOrderTypeSummaryCalcService _organisationEmployeeTypeOrderTypeSummaryCalcService = OrganisationEmployeeTypeOrderTypeSummaryCalcService;
    private OrganisationEmployeeTypeOrderTypeCheckService _organisationEmployeeTypeOrderTypeCheckService = OrganisationEmployeeTypeOrderTypeCheckService;
    private OrganisationEmployeeTypeWageItemService _organisationEmployeeTypeWageItemService = OrganisationEmployeeTypeWageItemService;
    private OrganisationEmployeeTypeOrderTypeService _organisationEmployeeTypeOrderTypeService = OrganisationEmployeeTypeOrderTypeService;
    private OrganisationPositionService _organisationPositionService = OrganisationPositionService;
    private FormulaEngine.Infrastructure.Services.FormulaService _formulaService = FormulaService;
    private OrganizationJobService _organizationJobService = OrganizationJobService;
    private WageItemService _wageItemService = WageItemService;
    private BaseInfo.infrastructure.Services.CoefficientService _coefficientService = CoefficientService;
    private OrganisationEmployeeTypeMRTService _organisationEmployeeTypeMRTService = OrganisationEmployeeTypeMRTService;
    private OrganisationMRTService _organisationMRTService = OrganisationMRTService;
    private FormulaTableValueService _formulaTableValueService = FormulaTableValueService;
    private FormulaTableService _formulaTableService = FormulaTableService;
    private OrganisationOrderTypeService _organisationOrderTypeService = OrganisationOrderTypeService;
    private OrganisationCoefficientService _organisationCoefficientService = OrganisationCoefficientService;
    private EmployeeTypeService _employeeTypeService = EmployeeTypeService;
    private EmployeeStatusService _employeeStatusService = EmployeeStatusService;
    private FormulaOperandService _formulaOperandService = FormulaOperandService;
    private FormulaDatabaseFunctionDefinitionService _formulaDatabaseFunctionDefinitionService = FormulaDatabaseFunctionDefinitionService;
    private BaseTableValueService _baseTableValueService = BaseTableValueService;
    private OrganisationWageItemService _organisationWageItemService = OrganisationWageItemService;
    private OrganisationEmployeeTypeService _organisationEmployeeTypeService = OrganisationEmployeeTypeService;
    private EducationGradeService _educationGradeService = EducationGradeService;
    private OrganisationChartService _organisationChartService = OrganisationChartService;
    private ProjectService _projectService = ProjectService;
    private OrganisationFormulaService _organisationFormulaService = OrganisationFormulaService;
    private JobService _jobService = JobService;

    private OrderTypeService _orderTypeService = OrderTypeService;
    private OrganizationJobAbilityQualificationService _organizationJobAbilityQualificationService = OrganizationJobAbilityQualificationService;
    private OrganizationJobCompetencyQualificationService _organizationJobCompetencyQualificationService = OrganizationJobCompetencyQualificationService;
    private OrganizationJobEducationFieldQualificationService _organizationJobEducationFieldQualificationService = OrganizationJobEducationFieldQualificationService;
    private OrganizationJobEducationGradeQualificationService _organizationJobEducationGradeQualificationService = OrganizationJobEducationGradeQualificationService;
    private OrganizationJobForeignLanguageQualificationService _organizationJobForeignLanguageQualificationService = OrganizationJobForeignLanguageQualificationService;
    private OrganizationJobInitialCourseQualificationService _organizationJobInitialCourseQualificationService = OrganizationJobInitialCourseQualificationService;
    private OrganizationJobRequiredCharacterQualificationService _organizationJobRequiredCharacterQualificationService = OrganizationJobRequiredCharacterQualificationService;
    private OrganizationJobRequiredSoftwaresQualificationService _organizationJobRequiredSoftwaresQualificationService = OrganizationJobRequiredSoftwaresQualificationService;

    private AbilityService _abilityService = AbilityService;
    private CompetencyService _competencyService = CompetencyService;
    private ForignLanguageService _ForeignLanguageService = ForeignLanguageService;
    private CourseService _courseService = CourseService;
    private CharacterService _characterService = CharacterService;
    private EmployeeSoftwareService _employeeSoftwareService = EmployeeSoftwareService;

    private UserResolverService _userService = userService;
    private readonly ILogger<OrderService> _logger = logger;

    /// <summary>
    /// این متد بر اساس ورودی های فرم صدور گروهی احکام فهرست افراد را باز می گرداند
    /// </summary>
    /// <param name="filterDTO"></param>
    /// <returns></returns>
    public OperationResult GetBatchRequestFilteredPeople(FilterBatchDTO filterDTO)
    {


        if (filterDTO.EmployeeTypeId == null)
        {
            filterDTO.EmployeeTypeId = 0;
        }
        if (filterDTO.CostCenterId == null)
        {
            filterDTO.CostCenterId = 0;
        }
        if (filterDTO.OrganizationUnitId == null)
        {
            filterDTO.OrganizationUnitId = 0;
        }
        if (filterDTO.WorkPlaceId == null)
        {
            filterDTO.WorkPlaceId = 0;
        }
        if (filterDTO.EmployeeStatusId == null)
        {
            filterDTO.EmployeeStatusId = 0;
        }
        if (string.IsNullOrEmpty(filterDTO.NationalNos))
        {

        }
        else
        {
            filterDTO.NationalNos = Regex.Replace(filterDTO.NationalNos, @"\t|\n|\r", "|");
        }

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("[Order].[GetFilteredPeopleForBatch]", con);
            cmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = _currentUserDefaultOrganId;
            cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = filterDTO.EmployeeTypeId;
            cmd.Parameters.Add("@CostCenterId", SqlDbType.BigInt).Value = filterDTO.CostCenterId;
            cmd.Parameters.Add("@OrganizationUnitId", SqlDbType.BigInt).Value = filterDTO.OrganizationUnitId;
            cmd.Parameters.Add("@WorkPlaceId", SqlDbType.BigInt).Value = filterDTO.WorkPlaceId;
            cmd.Parameters.Add("@EmployeeStatusId", SqlDbType.BigInt).Value = filterDTO.EmployeeStatusId;
            cmd.Parameters.Add("@NationalNos", SqlDbType.NVarChar).Value = (object?)filterDTO.NationalNos ?? DBNull.Value;
            cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = filterDTO.ImpleDate.Date;
            cmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = filterDTO.PageNo < 1 ? 1 : filterDTO.PageNo;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = filterDTO.PageSize < 1 ? 25 : filterDTO.PageSize;
            cmd.Parameters.Add("@SortColumn", SqlDbType.NVarChar, 20).Value = NormalizeBatchPeopleSortColumn(filterDTO.SortColumn);
            cmd.Parameters.Add("@SortOrder", SqlDbType.NVarChar, 4).Value = NormalizeBatchPeopleSortOrder(filterDTO.SortOrder);
            cmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 450).Value = (object?)filterDTO.SearchText ?? DBNull.Value;
            cmd.Parameters.Add("@ReturnAll", SqlDbType.Bit).Value = filterDTO.ReturnAll;
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            List<BatchFilteredPeopleRow> ret = new();
            int? totalCount = null;
            while (rdr.Read())
            {
                var row = rdr.ConvertToObject<BatchFilteredPeopleRow>();
                totalCount ??= row.TotalCount;
                ret.Add(row);
            }
            con.Close();
            return OperationResult.Succeeded(payload: ret, rowCount: totalCount ?? ret.Count);
        }


    }

    private static string NormalizeBatchPeopleSortColumn(string? sortColumn)
    {
        return sortColumn?.Trim() switch
        {
            "FirstName" or "firstName" => "FirstName",
            "LastName" or "lastName" => "LastName",
            "NationalNo" or "nationalNo" => "NationalNo",
            "PersonelCode" or "personelCode" => "PersonelCode",
            _ => "PersonelCode",
        };
    }

    private static string NormalizeBatchPeopleSortOrder(string? sortOrder)
    {
        return string.Equals(sortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
    }


    /// <summary>
    /// تایید نهایی همه احکام در انتظار کارگزینی برای محل پرداخت جاری کاربر.
    /// </summary>
    public OperationResult FinalApproveOrderAll()
    {
        var pendingOrderIds = GetPendingInterdictOrderIdsForCurrentOrgan();

        if (pendingOrderIds.Count == 0)
        {
            _logger.LogInformation(
                "تایید گروهی احکام: حکمی در انتظار یافت نشد. PayLocationId={PayLocationId}",
                _currentUserDefaultOrganId);
            return OperationResult.Succeeded("هیچ حکمی در انتظار تایید یافت نشد");
        }

        _logger.LogInformation(
            "شروع تایید گروهی احکام. PayLocationId={PayLocationId}, PendingCount={PendingCount}",
            _currentUserDefaultOrganId,
            pendingOrderIds.Count);

        var items = new List<OrderBatchActionItemResultDto>();

        foreach (var orderId in pendingOrderIds)
        {
            try
            {
                var singleResult = FinalApproveOrder(orderId);

                items.Add(new OrderBatchActionItemResultDto
                {
                    OrderId = orderId,
                    Success = singleResult.Success,
                    Message = string.IsNullOrWhiteSpace(singleResult.Message)
                        ? (singleResult.Success ? "عملیات با موفقیت انجام شد" : "عملیات ناموفق بود")
                        : singleResult.Message
                });

                if (!singleResult.Success)
                {
                    _logger.LogWarning(
                        "تایید حکم در عملیات گروهی ناموفق بود. OrderId={OrderId}, Message={Message}",
                        orderId,
                        singleResult.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای غیرمنتظره در تایید گروهی حکم. OrderId={OrderId}", orderId);

                items.Add(new OrderBatchActionItemResultDto
                {
                    OrderId = orderId,
                    Success = false,
                    Message = "خطای غیرمنتظره در پردازش این حکم"
                });
            }
        }

        var summary = new OrderBatchActionResultDto
        {
            TotalCount = items.Count,
            SuccessCount = items.Count(i => i.Success),
            FailedCount = items.Count(i => !i.Success),
            Items = items
        };

        var message = summary.FailedCount == 0
            ? $"تایید گروهی برای {summary.SuccessCount} حکم با موفقیت انجام شد."
            : $"از {summary.TotalCount} حکم، {summary.SuccessCount} مورد تایید و {summary.FailedCount} مورد ناموفق بود.";

        _logger.LogInformation(
            "پایان تایید گروهی احکام. PayLocationId={PayLocationId}, TotalCount={TotalCount}, SuccessCount={SuccessCount}, FailedCount={FailedCount}",
            _currentUserDefaultOrganId,
            summary.TotalCount,
            summary.SuccessCount,
            summary.FailedCount);

        return summary.FailedCount == 0
            ? OperationResult.Succeeded(message, summary, summary.SuccessCount)
            : OperationResult.Failed(message, summary);
    }

    /// <summary>
    /// محل‌های پرداختی که کاربر جاری به آن‌ها دسترسی دارد (پیش‌فرض + User_PayLocation).
    /// </summary>
    private List<long> GetAccessiblePayLocationIdsForCurrentUser()
    {
        var payLocationIds = new HashSet<long>();
        if (_currentUserDefaultOrganId > 0)
        {
            payLocationIds.Add(_currentUserDefaultOrganId);
        }

        var currentUserId = _userService.GetUserId();
        if (currentUserId <= 0)
        {
            return payLocationIds.ToList();
        }

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"
SELECT DISTINCT upl.PayLocationId
FROM [Identity].[User_PayLocation] upl
WHERE upl.UserId = @UserId
  AND [dbo].[Fn_isValidDateRange](upl.StartDate, upl.EndDate, GETDATE()) = 1", con);
        cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = currentUserId;
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (!reader.IsDBNull(0))
            {
                var payLocationId = reader.GetInt64(0);
                if (payLocationId > 0)
                {
                    payLocationIds.Add(payLocationId);
                }
            }
        }

        return payLocationIds.ToList();
    }

    private static InterdictOrder? GetInterdictOrderWithRecruit(long id, DbContext db)
    {
        return db.Set<InterdictOrder>()
            .Include(i => i.RecruitOrder)
            .SingleOrDefault(i => i.Id == id && !i.IsDeleted);
    }

    /// <summary>
    /// آخرین حکم فرد (وضعیت ۹) بدون محدودیت محل پرداخت.
    /// </summary>
    private static InterdictOrder? GetEmployeeCurrentFinalOrder(long employeeId, DbContext db)
    {
        return db.Set<InterdictOrder>()
            .Include(i => i.RecruitOrder)
            .Where(i => !i.IsDeleted
                        && i.StatusId == (long)Enums.OrderStatus.FinalOrder
                        && i.RecruitOrder != null
                        && i.RecruitOrder.EmployeeId == employeeId)
            .SingleOrDefault();
    }

    private static bool HasPayLocationChanged(InterdictOrder approvingOrder, InterdictOrder? currentFinalOrder)
    {
        if (currentFinalOrder?.RecruitOrder == null || approvingOrder.RecruitOrder == null)
        {
            return false;
        }

        if (currentFinalOrder.Id == approvingOrder.Id)
        {
            return false;
        }

        return currentFinalOrder.RecruitOrder.PayLocationId != approvingOrder.RecruitOrder.PayLocationId;
    }

    /// <summary>
    /// تایید حکم هنگام تغییر محل پرداخت — بدون مدیریت تراکنش؛ caller باید SaveChanges/Commit انجام دهد.
    /// </summary>
    private static OperationResult ApplyFinalApproveOnPayLocationChange(
        long id,
        long approvingPayLocationId,
        List<SP_GetOrderListByEmployeeID_Result> orderList,
        InterdictOrder currentFinalOrder,
        DbContext db)
    {
        var toApproveinterdict = db.Set<InterdictOrder>().Find(id);
        if (toApproveinterdict == null)
        {
            return OperationResult.NotFound("حکم مورد نظر یافت نشد");
        }

        if (orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.Draft))
        {
            return OperationResult.Failed("فرد مورد نظر حکم پیش نویس دارد ابتدا آن را تعیین وضعیت بفرمایید");
        }

        var previousFinalOrder = db.Set<InterdictOrder>().Find(currentFinalOrder.Id);
        if (previousFinalOrder == null)
        {
            return OperationResult.Failed("آخرین حکم فرد یافت نشد");
        }

        toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalOrder;

        if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Cancelation
            && toApproveinterdict.CorrectedInterdictOrderId.HasValue)
        {
            var toCancelInterdict = db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId.Value);
            if (toCancelInterdict != null)
            {
                if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date
                                       && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                {
                    foreach (var finalApproveOrder in orderList.Where(i =>
                                 i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date
                                 && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                    {
                        var finalAppOrder = db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                        if (finalAppOrder != null)
                        {
                            finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                            db.Set<InterdictOrder>().Update(finalAppOrder);
                        }
                    }
                }

                toCancelInterdict.StatusId = (long)Enums.OrderStatus.CanceledOrder;
                db.Set<InterdictOrder>().Update(toCancelInterdict);
            }
        }
        else if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Correction
                 && toApproveinterdict.CorrectedInterdictOrderId.HasValue)
        {
            var toCorrectInterdict = db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId.Value);
            if (toCorrectInterdict != null)
            {
                if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date
                                       && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                {
                    foreach (var finalApproveOrder in orderList.Where(i =>
                                 i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date
                                 && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                    {
                        var finalAppOrder = db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                        if (finalAppOrder != null)
                        {
                            finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                            db.Set<InterdictOrder>().Update(finalAppOrder);
                        }
                    }
                }

                toCorrectInterdict.StatusId = (long)Enums.OrderStatus.CorrectedOrder;
                db.Set<InterdictOrder>().Update(toCorrectInterdict);
            }
        }

        if (previousFinalOrder.StatusId == (long)Enums.OrderStatus.FinalOrder)
        {
            previousFinalOrder.StatusId = (long)Enums.OrderStatus.LastInPayLocation;
            db.Set<InterdictOrder>().Update(previousFinalOrder);
        }

        db.Set<InterdictOrder>().Update(toApproveinterdict);
        return OperationResult.Succeeded();
    }

    /// <summary>
    /// اعمال منطق تایید حکم بدون مدیریت تراکنش — برای استفاده در گردش کار یا API مستقل.
    /// </summary>
    public OperationResult ApplyFinalApproveToPendingOrder(long orderId, long currentUserId, DbContext db)
    {
        var interdictOrder = GetInterdictOrderWithRecruit(orderId, db);
        if (interdictOrder == null)
        {
            return OperationResult.NotFound("حکم مورد نظر یافت نشد");
        }

        if (interdictOrder.RecruitOrder == null)
        {
            return OperationResult.Failed("اطلاعات استخدامی حکم مورد نظر یافت نشد");
        }

        if (interdictOrder.StatusId != (long)Enums.OrderStatus.Pending)
        {
            return OperationResult.Failed("وضعیت حکم مورد نظر در حال بررسی کارگزینی نیست");
        }

        var employeeId = interdictOrder.RecruitOrder.EmployeeId;
        if (OrderSerialSequenceHelper.HasSmallerPendingOrder(employeeId, (short)(interdictOrder.Serial ?? 0), orderId, db))
        {
            return OperationResult.Failed(OrderSerialSequenceHelper.ApproveOutOfOrderMessage);
        }

        var approvingPayLocationId = interdictOrder.RecruitOrder.PayLocationId;
        var orderList = GetOrderList(new GetOrderListByEmployeeIdRequest()
        {
            EmployeeId = employeeId,
            PayLocationId = approvingPayLocationId,
            CurrentUserId = currentUserId,
            PageNo = 0,
            PageSize = 1000,
            SortColumn = "",
            SortOrder = ""
        });

        var currentFinalOrder = GetEmployeeCurrentFinalOrder(employeeId, db);
        if (HasPayLocationChanged(interdictOrder, currentFinalOrder))
        {
            return ApplyFinalApproveOnPayLocationChange(
                orderId,
                approvingPayLocationId,
                orderList,
                currentFinalOrder!,
                db);
        }

        if (orderList.Count == 1)
        {
            if (orderList.SingleOrDefault()?.StatusId == (long)Enums.OrderStatus.Pending)
            {
                var toApproveinterdict = db.Set<InterdictOrder>().Find(orderId);
                if (toApproveinterdict == null)
                {
                    return OperationResult.NotFound("حکم مورد نظر یافت نشد");
                }

                toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalOrder;
                db.Set<InterdictOrder>().Update(toApproveinterdict);
            }
        }
        else if (orderList.Count > 1 && orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder))
        {
            var toApproveinterdict = db.Set<InterdictOrder>().Find(orderId);
            if (toApproveinterdict == null)
            {
                return OperationResult.NotFound("حکم مورد نظر یافت نشد");
            }

            if (orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.Draft))
            {
                return OperationResult.Failed("فرد مورد نظر حکم پیش نویس دارد ابتدا آن را تعیین وضعیت بفرمایید");
            }

            if (orderList.Any(i => i.OrderSerial < toApproveinterdict.Serial && i.StartDate.Value.Date > toApproveinterdict.StartDate.Value.Date))
            {
                toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalAprove;
            }
            else
            {
                toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalOrder;
            }

            var previewsFinalOrder = db.Set<InterdictOrder>().Find(orderList.Single(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder).Id);
            if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Cancelation)
            {
                var toCancelInterdict = db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId);
                if (toCancelInterdict?.StatusId == (long)Enums.OrderStatus.FinalOrder)
                {
                    if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                    {
                        foreach (var finalApproveOrder in orderList.Where(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                        {
                            var finalAppOrder = db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                            if (finalAppOrder != null)
                            {
                                finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                db.Set<InterdictOrder>().Update(finalAppOrder);
                            }
                        }
                    }
                }

                if (toCancelInterdict != null)
                {
                    toCancelInterdict.StatusId = (long)Enums.OrderStatus.CanceledOrder;
                    db.Set<InterdictOrder>().Update(toCancelInterdict);
                }
            }

            if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Correction)
            {
                var toCorrectInterdict = db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId);
                if (toCorrectInterdict?.StatusId == (long)Enums.OrderStatus.FinalOrder)
                {
                    if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                    {
                        foreach (var finalApproveOrder in orderList.Where(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                        {
                            var finalAppOrder = db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                            if (finalAppOrder != null)
                            {
                                finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                db.Set<InterdictOrder>().Update(finalAppOrder);
                            }
                        }
                    }
                }

                if (toCorrectInterdict != null)
                {
                    toCorrectInterdict.StatusId = (long)Enums.OrderStatus.CorrectedOrder;
                    db.Set<InterdictOrder>().Update(toCorrectInterdict);
                }
            }

            if (previewsFinalOrder != null)
            {
                if (toApproveinterdict.CorrectedInterdictOrderId != previewsFinalOrder.Id)
                {
                    if (!orderList.Any(i => i.OrderSerial < toApproveinterdict.Serial && i.StartDate > toApproveinterdict.StartDate))
                    {
                        previewsFinalOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                        db.Set<InterdictOrder>().Update(previewsFinalOrder);
                    }
                }

                if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Normal
                    && toApproveinterdict.StatusId == (long)Enums.OrderStatus.FinalOrder)
                {
                    previewsFinalOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                    db.Set<InterdictOrder>().Update(previewsFinalOrder);
                }
            }

            db.Set<InterdictOrder>().Update(toApproveinterdict);
        }

        return OperationResult.Succeeded();
    }

    /// <summary>
    /// شناسه احکام در انتظار کارگزینی برای محل‌های پرداخت قابل‌دسترس کاربر، مرتب‌شده بر اساس پرسنل و سریال.
    /// </summary>
    private List<long> GetPendingInterdictOrderIdsForCurrentOrgan()
    {
        const long pendingStatus = (long)Enums.OrderStatus.Pending;
        var accessiblePayLocationIds = GetAccessiblePayLocationIdsForCurrentUser();
        if (accessiblePayLocationIds.Count == 0)
        {
            return [];
        }

        return (
            from interdict in _db.Set<InterdictOrder>()
            join recruit in _db.Set<RecruitOrder>() on interdict.RecruitOrderId equals recruit.Id
            where !interdict.IsDeleted
                  && interdict.StatusId == pendingStatus
                  && accessiblePayLocationIds.Contains(recruit.PayLocationId)
            orderby recruit.EmployeeId, interdict.Serial, interdict.StartDate, interdict.Id
            select interdict.Id
        ).ToList();
    }
    /// <summary>
    /// تایید حکم بدون گردش کار از فرم مدیریتی
    /// </summary>
    public OperationResult FinalApproveOrder(long Id)
    {
        try
        {
            _unitOfWork.CreateTransaction();
            var result = ApplyFinalApproveToPendingOrder(Id, _userService.GetUserId(), _unitOfWork.Context);
            if (!result.Success)
            {
                _unitOfWork.Rollback();
                return result;
            }

            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
            return OperationResult.Succeeded();
        }
        catch (Exception)
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
    public RecruitOrder GetRelatedRecruit(long RecruitOrderId)
    {

        return _unitOfWork.Context.RecruitOrders.Find(RecruitOrderId);
    }
    public OperationResult GetOrderForPrint(long Id)
    {
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("[Order].[GetOrderForPrint]", con))
            {
                cmd.Parameters.AddWithValue("@InterdictId", Id);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
                {
                    GetOrderForPrint_Result ret = new GetOrderForPrint_Result();
                    if (rdr.Read())
                    {
                        ret = rdr.ConvertToObject<GetOrderForPrint_Result>();
                        EnrichOrderForPrintJobInfo(ret);
                    }
                    return OperationResult.Succeeded(payload: ret);
                }
            }
        }

    }

    private void EnrichOrderForPrintJobInfo(GetOrderForPrint_Result ret)
    {
        if (ret.OrganizationJobId is not > 0)
        {
            return;
        }

        var organizationJob = _db.Set<OrganizationJob>()
            .AsNoTracking()
            .Include(i => i.Job)
            .SingleOrDefault(i => i.Id == ret.OrganizationJobId);

        if (organizationJob?.Job == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ret.JobTitle))
        {
            ret.JobTitle = organizationJob.Job.title;
        }

        if (!ret.JobDegree.HasValue)
        {
            if (organizationJob.JobDegree > 0)
            {
                ret.JobDegree = organizationJob.JobDegree;
            }
            else if (ret.SummaryJobDegree.HasValue)
            {
                ret.JobDegree = ret.SummaryJobDegree;
            }
        }
    }
    public OperationResult SetInterdictFieldValue(AdminFormOrderFieldDTO req)
    {
        return SetAdminEntityFieldValue<InterdictOrder>(req?.interdictid, req?.Property, req?.Value, "حکم حقوقی");
    }

    public OperationResult SetRecruitFieldValue(AdminFormOrderFieldDTO req)
    {
        return SetAdminEntityFieldValue<RecruitOrder>(req?.recruitId, req?.Property, req?.Value, "حکم استخدامی");
    }

    public OperationResult GetInterdictOrderProperties()
    {
        return OperationResult.Succeeded(payload: BuildAdminEditablePropertyList(typeof(InterdictOrder)));
    }

    public OperationResult GetRecruitOrderProperties()
    {
        return OperationResult.Succeeded(payload: BuildAdminEditablePropertyList(typeof(RecruitOrder)));
    }

    private OperationResult SetAdminEntityFieldValue<TEntity>(string? entityId, string? propertyName, object? value, string entityTitle)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(entityId) || !long.TryParse(entityId.Trim(), out var id) || id <= 0)
        {
            return OperationResult.Failed($"شناسه {entityTitle} نامعتبر است");
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return OperationResult.Failed("نام ستون مشخص نشده است");
        }

        var prop = FindAdminEditableProperty(typeof(TEntity), propertyName);
        if (prop == null)
        {
            return OperationResult.Failed($"ستون «{propertyName.Trim()}» قابل ویرایش نیست یا وجود ندارد");
        }

        if (string.Equals(prop.Name, nameof(BaseEntity.Id), StringComparison.OrdinalIgnoreCase))
        {
            return OperationResult.Failed("ویرایش شناسه مجاز نیست");
        }

        try
        {
            _unitOfWork.CreateTransaction();
            var entity = _unitOfWork.Context.Set<TEntity>().Find(id);
            if (entity == null)
            {
                _unitOfWork.Rollback();
                return OperationResult.NotFound($"{entityTitle} یافت نشد");
            }

            object? converted;
            try
            {
                converted = ConvertAdminFieldValue(value, prop.PropertyType);
            }
            catch (Exception convertEx)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed($"مقدار برای ستون «{prop.Name}» قابل تبدیل نیست: {convertEx.Message}");
            }

            prop.SetValue(entity, converted);
            _unitOfWork.Context.Update(entity);
            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
            return OperationResult.Succeeded(msg: $"ستون «{prop.Name}» با موفقیت بروزرسانی شد");
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    private static List<SharedKernel.Data.KeyValuePair> BuildAdminEditablePropertyList(Type entityType)
    {
        var props = GetAdminEditableProperties(entityType)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var list = new List<SharedKernel.Data.KeyValuePair>();
        var index = 1;
        foreach (var name in props)
        {
            list.Add(new SharedKernel.Data.KeyValuePair
            {
                id = index,
                key = index,
                value = name
            });
            index++;
        }

        return list;
    }

    private static PropertyInfo? FindAdminEditableProperty(Type entityType, string propertyName)
    {
        var target = propertyName.Trim();
        return GetAdminEditableProperties(entityType)
            .FirstOrDefault(p => string.Equals(p.Name, target, StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<PropertyInfo> GetAdminEditableProperties(Type entityType)
    {
        return entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
                p.CanRead &&
                p.CanWrite &&
                p.GetIndexParameters().Length == 0 &&
                p.GetCustomAttribute<NotMappedAttribute>() == null &&
                IsAdminEditableScalarType(p.PropertyType));
    }

    private static bool IsAdminEditableScalarType(Type type)
    {
        var t = Nullable.GetUnderlyingType(type) ?? type;
        if (t.IsEnum || t.IsPrimitive)
        {
            return true;
        }

        return t == typeof(string)
            || t == typeof(decimal)
            || t == typeof(DateTime)
            || t == typeof(Guid)
            || t == typeof(float)
            || t == typeof(double);
    }

    private static object? ConvertAdminFieldValue(object? value, Type targetType)
    {
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var isNullable = Nullable.GetUnderlyingType(targetType) != null || !underlying.IsValueType;

        if (value == null || value is DBNull)
        {
            return isNullable ? null : Activator.CreateInstance(underlying);
        }

        if (value is JsonElement jsonElement)
        {
            value = jsonElement.ValueKind switch
            {
                JsonValueKind.Null or JsonValueKind.Undefined => null,
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => jsonElement.ToString(),
                _ => jsonElement.ToString()
            };

            if (value == null)
            {
                return isNullable ? null : Activator.CreateInstance(underlying);
            }
        }

        if (underlying.IsInstanceOfType(value))
        {
            return value;
        }

        var raw = value is string s ? s.Trim() : Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return isNullable ? null : Activator.CreateInstance(underlying);
        }

        if (underlying == typeof(string))
        {
            return raw;
        }

        if (underlying == typeof(bool))
        {
            if (bool.TryParse(raw, out var boolValue))
            {
                return boolValue;
            }

            if (raw.Equals("1", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("y", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("بله", StringComparison.Ordinal)
                || raw.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (raw.Equals("0", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("no", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("n", StringComparison.OrdinalIgnoreCase)
                || raw.Equals("خیر", StringComparison.Ordinal)
                || raw.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new FormatException("مقدار بولین نامعتبر است (true/false یا 1/0)");
        }

        if (underlying == typeof(DateTime))
        {
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt) ||
                DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
            {
                return dt;
            }

            throw new FormatException("فرمت تاریخ نامعتبر است");
        }

        if (underlying == typeof(Guid))
        {
            return Guid.Parse(raw);
        }

        if (underlying == typeof(decimal))
        {
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var decInv) ||
                decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out decInv))
            {
                return decInv;
            }

            throw new FormatException("عدد اعشاری نامعتبر است");
        }

        if (underlying == typeof(float))
        {
            if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var fInv) ||
                float.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out fInv))
            {
                return fInv;
            }

            throw new FormatException("عدد اعشاری نامعتبر است");
        }

        if (underlying == typeof(double))
        {
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var dInv) ||
                double.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out dInv))
            {
                return dInv;
            }

            throw new FormatException("عدد اعشاری نامعتبر است");
        }

        if (underlying.IsEnum)
        {
            return Enum.Parse(underlying, raw, ignoreCase: true);
        }

        return Convert.ChangeType(raw, underlying, CultureInfo.InvariantCulture);
    }

    public InterdictOrder? GetCurrentEmployeeLastOrder(long Id)
    {
        var employeeOrders = All(false)
            .Include(i => i.RecruitOrder)
            .Where(i => i.RecruitOrder != null && i.RecruitOrder.EmployeeId == Id)
            .ToList();

        if (!employeeOrders.Any())
        {
            return null;
        }

        var finalOrder = employeeOrders
            .Where(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder)
            .SingleOrDefault();

        if (finalOrder != null)
        {
            return finalOrder;
        }

        // For newly hired employees without a finalized order yet, use the latest active order.
        long[] activeSummaryStatuses =
        [
            (long)Enums.OrderStatus.Pending,
            (long)Enums.OrderStatus.FinalAprove,
            (long)Enums.OrderStatus.Draft,
        ];

        return employeeOrders
            .Where(i => activeSummaryStatuses.Contains(i.StatusId))
            .OrderByDescending(i => i.Serial ?? 0)
            .ThenByDescending(i => i.Id)
            .FirstOrDefault();
    }
    /// <summary>
    /// گرفتن آخرین حکم بر اساس شناسه کارمندی
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult GetLastOrderDetailByEmployeeId(long Id)
    {
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            var lastOrder = GetCurrentEmployeeLastOrder(Id);
            if (lastOrder != null)
            {
                using (SqlCommand cmd = new SqlCommand("[Order].[GetOrderForPrint]", con))
                {
                    cmd.Parameters.AddWithValue("@InterdictId", lastOrder.Id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
                    {
                        GetOrderForPrint_Result ret = new GetOrderForPrint_Result();
                        if (rdr.Read())
                        {
                            ret = rdr.ConvertToObject<GetOrderForPrint_Result>();
                            EnrichOrderForPrintJobInfo(ret);
                        }
                        return OperationResult.Succeeded(payload: ret);
                    }
                }
            }

        }
        return OperationResult.NotFound("حکم آخری یافت نشد");
    }


    /// <summary>
    /// گرفتن حکم‌های در حال بررسی کارگزینی فقط برای محل پرداخت (واحد جاری) پیش‌فرض کاربر.
    /// </summary>
    public OperationResult GetAllPendingOrders()
    {
        var ret = new List<GetAllPendingOrders_Result>();

        if (_currentUserDefaultOrganId <= 0)
        {
            return OperationResult.Succeeded(payload: ret);
        }

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("[Order].[GetAllPendingOrders]", con);
            cmd.Parameters.AddWithValue("@currentUserDefaultOrganId", _currentUserDefaultOrganId);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ret.Add(rdr.ConvertToObject<GetAllPendingOrders_Result>());
            }
            rdr.Close();
            con.Close();
        }

        ret = ret
            .OrderBy(i => i.NationalNo)
            .ThenBy(i => i.Serial)
            .ToList();

        return OperationResult.Succeeded(payload: ret);
    }


    /// <summary>
    /// ارسال حکم مورد نظر به کارتابل تایید کننده حکم
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult SendOrderToCartable(long Id)
    {
        var currentOrder = All(false).SingleOrDefault(i => i.Id == Id);
        if (currentOrder.StatusId == (long)Enums.OrderStatus.Draft)
        {
            currentOrder.StatusId = (long)Enums.OrderStatus.Pending;
            Update(currentOrder);
            if (_unitOfWork.Save().Result > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            else
            {
                return OperationResult.Failed();
            }
        }
        else
        {
            return OperationResult.Failed("وضعیت حکم جهت ارسال به گردش کار معتبر نیست");
        }
    }

    public OperationResult UpdateDraftOrderDates(UpdateDraftOrderDatesDTO req)
    {
        var currentOrder = All(IgnoreExpired: false).SingleOrDefault(i => i.Id == req.Id);
        if (currentOrder == null)
        {
            return OperationResult.Failed("حکم یافت نشد");
        }

        if (currentOrder.StatusId != (long)Enums.OrderStatus.Draft)
        {
            return OperationResult.Failed("فقط احکام پیش‌نویس قابل ویرایش تاریخ هستند");
        }

        if (req.EndDate.HasValue && req.EndDate.Value.Date < req.StartDate.Date)
        {
            return OperationResult.Failed("تاریخ پایان نمی‌تواند قبل از تاریخ آغاز باشد");
        }

        currentOrder.StartDate = req.StartDate.Date;
        currentOrder.EndDate = req.EndDate?.Date;
        currentOrder.LastModifiedDate = DateTime.Now;
        Update(currentOrder);

        if (_unitOfWork.Save().Result > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }
    /// <summary>
    /// گرفتن حکم جهت نمایش
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult GetOrderFlat(long Id)
    {
        var currentOrder = All(false)
               .AsNoTracking()
               .Include(i => i.RecruitOrder)
               .Include(i => i.OrderType)
               .Include(i => i.RecruitOrder.EmployeeType)
               .Include(i => i.RecruitOrder.OrganizationUnit)
               .Include(i => i.RecruitOrder.WorkPlace)

               .SingleOrDefault(i => i.Id == Id);
        var ret = _mapper.Map<InterdictOrderFlatDTO>(currentOrder);


        ret.OrderType = currentOrder.OrderType.title;

        var employeeTypeSetting = _organisationEmployeeTypeWageItemService.All(ImpleDate: currentOrder.StartDate)

            .Where(i => i.OrganisationChartId == currentOrder.RecruitOrder.PayLocationId
            && i.EmployeeTypeId == currentOrder.RecruitOrder.EmployeeTypeId

            ).OrderBy(i => i.Priority);

        var employeeTypeSettingList = employeeTypeSetting.ToList();

        if (ret.OrganisationPositionId > 0)
        {
            var position = _db.Set<OrganisationPosition>().Include(i => i.Position).Single(i => i.Id == ret.OrganisationPositionId);
            ret.OrganisationPosition = position.Position.title + " ( " + (position.PositionCode == null ? "" : position.PositionCode) + " ) ";
        }

        if (ret.OrganizationJobId > 0)
        {
            var oj = _db.Set<OrganizationJob>().Include(i => i.Job).Single(i => i.Id == ret.OrganizationJobId);
            ret.OrganizationJob = oj.Job.title + " ( " + (oj.Code == null ? "" : oj.Code) + " ) ";
        }

        var wageSettingByWageItemId = employeeTypeSettingList.ToDictionary(e => e.WageItemId);
        var wageItems = _db.Set<InterdictOrderWageItem>().AsNoTracking().Include(i => i.WageItem).Where(i => i.InterdictOrderId == Id).ToList();
        if (wageItems != null && wageItems.Any())
        {
            var orderIndex = employeeTypeSettingList
                .Select((e, idx) => new { e.WageItemId, idx })
                .ToDictionary(x => x.WageItemId, x => x.idx);

            wageItems = wageItems
                .OrderBy(w => orderIndex.ContainsKey(w.WageItemId) ? orderIndex[w.WageItemId] : int.MaxValue)
                .ToList();
        }

        int index = 1;
        if (wageItems != null)
        {
            if (wageItems.Any())
            {
                ret.WageItemList = new List<InterdictOrderWageItemDTO>();
                foreach (var item in wageItems)
                {
                    bool IsDaily = wageSettingByWageItemId.TryGetValue(item.WageItemId, out var wageSetting)
                        && wageSetting.IsDaily;
                    ret.WageItemList.Add(new InterdictOrderWageItemDTO()
                    {
                        InterdictOrderId = Id,
                        WageItemId = item.WageItemId,
                        WageItem = item.WageItem.title,
                        Value = item.Value,
                        IsDaily = IsDaily,
                        Id = index
                    });
                    index++;
                }
            }
        }
        var CoefficientItems = _db.Set<InterdictOrderCoefficientItem>().AsNoTracking().Include(i => i.Coefficient).Where(i => i.InterdictOrderId == Id).ToList();

        if (CoefficientItems != null)
        {
            if (CoefficientItems.Any())
            {
                ret.CoefficientItemList = new List<InterdictOrderCoefficientItemDTO>();
                index = 1;
                foreach (var item in CoefficientItems)
                {
                    ret.CoefficientItemList.Add(new InterdictOrderCoefficientItemDTO()
                    {
                        InterdictOrderId = Id,
                        CoefficientId = item.CoefficientId,
                        Coefficient = item.Coefficient.title,
                        OutPutFactValue = item.OutPutFactValue,

                        Id = index,
                    });
                    index++;
                }
            }
        }
        return OperationResult.Succeeded(payload: ret);
    }

    /// <summary>
    /// حذف منطقی
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public new OperationResult DeleteRecord(long Id)
    {
        var toDeleteOrder = All(IgnoreExpired: false).SingleOrDefault(i => i.Id == Id);
        if (toDeleteOrder.StatusId == (long)Enums.OrderStatus.Draft || toDeleteOrder.StatusId == (long)Enums.OrderStatus.RejectedOrder)
        {
            toDeleteOrder.StatusId = (long)Enums.OrderStatus.LogicalDeleted;
            toDeleteOrder.IsDeleted = true;
            Update(toDeleteOrder);
            if (_unitOfWork.Save().Result > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            else { return OperationResult.Failed(); }
        }
        else
        {
            return OperationResult.Failed("وضعیت حکم برای حذف مجاز نیست");
        }

    }
    private static readonly HttpClient client = new HttpClient();
    public OperationResult DownloadOrderPDF(long OrderId, bool isRaw)
    {

        GetOrderForPrint_Result Order = (GetOrderForPrint_Result)GetOrderForPrint(OrderId).Payload;
        var res = _organisationEmployeeTypeMRTService.All().Where(i => i.EmployeeTypeId == Order.EmployeeTypeId && i.OrganisationChartId == Order.PayLocationId
             && i.IsRaw == isRaw && i.IsManager == false && i.SettingTypeId == (long)Enums.MRTtype.Order
             );
        OrganisationMRT MRT = null;
        if (res == null)
        {
            return OperationResult.NotFound("تنظیمات گزارش چاپی یافت نشد");
        }
        else
        {
            if (res.Any())
            {
                if (res.Count() == 1)
                {
                    MRT = _organisationMRTService.GetIdAsync(res.Single().OrganisationMRTId).Result;
                    var order = unitOfWork.Context.InterdictOrders.Include(i => i.RecruitOrder).Single(i => i.Id == OrderId);

                    OrganisationChartImage OrgImg = null;
                    var imgresp = _organisationContext.OrganisationChartImages.Where(i => i.OrganisationChartId == order.RecruitOrder.PayLocationId);
                    if (imgresp != null)
                    {
                        if (imgresp.Any())
                        {
                            OrgImg = imgresp.First();
                        }
                    }

                    HR.Employee.Core.Entities.Image EmployeeIMG = null;
                    var employeeResp = _employeeContext.Images.Where(i => i.OrganisationChartId == order.RecruitOrder.PayLocationId && i.EmployeeId == order.RecruitOrder.EmployeeId).OrderBy(i => i.Id);
                    if (employeeResp != null)
                    {
                        if (employeeResp.Any())
                        {
                            EmployeeIMG = employeeResp.Last();
                        }
                    }

                    try
                    {
                        string absolutePath = null;
                        var baseDirs = new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory };
                        foreach (var baseDir in baseDirs)
                        {
                            var probe = baseDir;
                            for (int up = 0; up <= 5 && !string.IsNullOrEmpty(probe); up++)
                            {
                                var candidate = Path.Combine(probe, "Assets", "Fonts");
                                if (Directory.Exists(candidate))
                                {
                                    absolutePath = candidate;
                                    break;
                                }
                                probe = Path.GetDirectoryName(probe);
                            }
                            if (absolutePath != null) break;
                        }
                        if (string.IsNullOrWhiteSpace(absolutePath))
                        {
                            return OperationResult.Failed("مسیر فونت‌ها یافت نشد: Assets\\Fonts");
                        }
                        var OrderPDF = new OrderPrint().GetOrderPrint(order, MRT, OrgImg, EmployeeIMG, absolutePath, _connectionString);
                        return OperationResult.Succeeded(payload: OrderPDF);
                    }
                    catch (Exception ex)
                    {
                        return OperationResult.Failed("خطا در تولید PDF: " + ex.Message);
                    }

                }
                if (res.Count() > 1)
                {
                    return OperationResult.NotFound("بیش از یک رکورد تنظیمات چاپ یافت شد لطفا تاریخ تنظیمات را بررسی بفرمایید");
                }
            }
            else
            {
                return OperationResult.NotFound("تنظیمات گزارش چاپی یافت نشد");
            }
        }

        return OperationResult.NotFound("تنظیمات گزارش چاپی یافت نشد");
    }

    /// <summary>
    /// گرفتن فهرست احکام کارکنان
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public List<SP_GetOrderListByEmployeeID_Result> GetOrderList(GetOrderListByEmployeeIdRequest req, bool isFromProfile = false)
    {
        List<SP_GetOrderListByEmployeeID_Result> ret = new List<SP_GetOrderListByEmployeeID_Result>();
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            req.PageNo = req.PageNo + 1;
            SqlCommand cmd = new SqlCommand("[Order].[SP_GetOrderListByEmployeeID]", con);
            cmd.Parameters.AddWithValue("@employeeID", req.EmployeeId);
            cmd.Parameters.AddWithValue("@CurrentUserId", req.CurrentUserId);
            cmd.Parameters.AddWithValue("@PageNo", req.PageNo);
            cmd.Parameters.AddWithValue("@PageSize", req.PageSize);
            cmd.Parameters.AddWithValue("@PaylocationList", "");
            cmd.Parameters.AddWithValue("@SortColumn", req.SortColumn);
            cmd.Parameters.AddWithValue("@SortOrder", req.SortOrder);
            cmd.Parameters.AddWithValue("@isFromProfile", isFromProfile);

            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ret.Add(rdr.ConvertToObject<SP_GetOrderListByEmployeeID_Result>());

            }
            con.Close();
        }
        return ret;
    }
    /// <summary>
    /// بررسی شرایط فردا جهت صدور حکم جدید
    /// </summary>
    /// <returns></returns>
    public Tuple<bool, string> GetIssuePermission(GetOrderListByEmployeeIdRequest req, List<SP_GetOrderListByEmployeeID_Result>? orderList = null)
    {
        req.PageSize = 2500;
        var OrderList = orderList ?? GetOrderList(req);
        if (OrderList == null)
        {

        }
        else
        {
            List<long> validStateList = new List<long>() {
                (long)Enums.OrderStatus.Draft,
                (long)Enums.OrderStatus.RejectedOrder,

                };
            if (OrderList.Any(i => validStateList.Contains(i.StatusId)))
            {
                return new Tuple<bool, string>(false, "لطفا احکام قبلی را تعیین وضعیت بفرمایید");

            }

            if (OrderList.Any(i => (i.StatusId == (long)Enums.OrderStatus.Pending || i.StatusId == (long)Enums.OrderStatus.FinalAprove) && i.StartDate > req.StartDate))
            {
                return new Tuple<bool, string>(false, "لطفا احکام قبلی را تعیین وضعیت بفرمایید");
            }

        }

        var summarySetting = _organisationEmployeeTypeOrderTypeSummaryCalcService.All(ImpleDate: req.StartDate)

.Where(i => i.OrganisationChartId == req.PayLocationId &&
i.EmployeeTypeId == req.EmployeeTypeId &&
i.OrderTypeId == req.OrderTypeId
);
        if (summarySetting == null)
        {
            return new Tuple<bool, string>(false, "تنظیمات خلاصه حکم یافت نشد");
        }
        else
        {
            if (summarySetting.Any())
            {
                        if (summarySetting.Count() > 1)
                {
                    return new Tuple<bool, string>(false, "بیش از یک تنظیمات برای خلاصه حکم یافت شد !");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "تنظیمات خلاصه حکم یافت نشد");
            }
        }


        return new Tuple<bool, string>(true, "");

    }
    /// <summary>
    /// گرفتن فهرست احکام بر اساس شناسه کارمندی
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public OperationResult GetOrderListByEmployeeID(GetOrderListByEmployeeIdRequest req, bool isFromProfile = false)
    {
        var ret = GetOrderList(req, isFromProfile);
        List<SqlParameter> list = new List<SqlParameter>();
        list.Add(new SqlParameter("@EmployeeID", req.EmployeeId));
        var orderCount = HR.SharedKernel.Data.DbFunction.CallDBFunction(_connectionString, "Order", "Fn_GetOrderCount", list.ToArray());
        return OperationResult.Succeeded(payload: ret, rowCount: Convert.ToInt32(orderCount));
    }

    public OperationResult GetSelectedOrderLastOrderForCorrectAndCancellation(long InterdictId)
    {
        var selectedOrder = All().Include(i => i.RecruitOrder).SingleOrDefault(i => i.Id == InterdictId);
        return OperationResult.Succeeded(payload: GetLastOrderByImpleDate(selectedOrder.RecruitOrder.EmployeeId, selectedOrder.StartDate.Value, InterdictId, selectedOrder.RecruitOrder.PayLocationId));
    }
    public long GetLastOrderByImpleDate(long EmployeeId, DateTime ImpleDate, long? CorrectionOrderId, long PayLocationId, long? paymentPeriodId = null, bool requireNotEmployed = false)
    {
        if (CorrectionOrderId > 0)
        {

        }
        else
        {
            CorrectionOrderId = 0;
        }
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (var command = con.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[Order].[GetLastOrderByImpleDate]";
                command.Parameters.Add("@EmployeeId", SqlDbType.BigInt).Value = EmployeeId;
                command.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = ImpleDate.Date;
                command.Parameters.Add("@IsForFiche", SqlDbType.Bit).Value = paymentPeriodId is > 0;
                command.Parameters.Add("@CorrectionOrderId", SqlDbType.BigInt).Value = CorrectionOrderId;
                command.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = PayLocationId;
                command.Parameters.Add("@PaymentPeriodId", SqlDbType.BigInt).Value = (object?)paymentPeriodId ?? DBNull.Value;
                command.Parameters.Add("@RequireNotEmployed", SqlDbType.Bit).Value = requireNotEmployed;
                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue;
                con.Open();
                command.ExecuteNonQuery();
                return Convert.ToInt32(returnValue.Value);
            }
        }
    }

    private static double CalculateOrderItemAdjustedValue(double baseValue, long enterTypeId, int? fixValue)
    {
        var adjustment = fixValue ?? 0;
        return enterTypeId switch
        {
            (long)Enums.EnterTypeId.PercentIncrease => Math.Max(0, baseValue + (baseValue * adjustment / 100.0)),
            (long)Enums.EnterTypeId.PercentDecrease => Math.Max(0, baseValue - (baseValue * adjustment / 100.0)),
            (long)Enums.EnterTypeId.AmountIncrease => Math.Max(0, baseValue + adjustment),
            (long)Enums.EnterTypeId.AmountDecrease => Math.Max(0, baseValue - adjustment),
            (long)Enums.EnterTypeId.fixValue => Math.Max(0, adjustment),
            (long)Enums.EnterTypeId.boolean => adjustment != 0 ? 1 : 0,
            _ => baseValue
        };
    }

    private static double ResolveFinalOrderItemValue(
        double calculatedValue,
        long enterTypeId,
        int? fixValue,
        int? min,
        int? max)
    {
        var value = enterTypeId switch
        {
            (long)Enums.EnterTypeId.fixValue => Math.Max(0, fixValue ?? 0),
            (long)Enums.EnterTypeId.boolean => fixValue.HasValue && fixValue.Value != 0 ? 1 : 0,
            _ => calculatedValue
        };

        if (min.HasValue)
        {
            value = Math.Max(value, min.Value);
        }

        if (max.HasValue)
        {
            value = Math.Min(value, max.Value);
        }

        return value;
    }

    private static double ResolveEqualToInputWageValue(
        IReadOnlyDictionary<long, int> submittedWageValuesById,
        OrganisationEmployeeTypeOrderTypeWageItemDTO wageItem,
        IReadOnlyDictionary<long, InterdictOrderWageItemDTO>? interdictWagesById)
    {
        if (submittedWageValuesById.TryGetValue(wageItem.WageItemId, out var submittedValue))
        {
            return submittedValue;
        }

        if (interdictWagesById != null && interdictWagesById.TryGetValue(wageItem.WageItemId, out var interdictWage))
        {
            return interdictWage.Value;
        }

        return 0;
    }

    private static double ResolveEqualToInputCoefficientValue(
        IReadOnlyDictionary<long, double> submittedCoefValuesById,
        OrganisationEmployeeTypeOrderTypeCoefficientDTO coefficientItem,
        IReadOnlyDictionary<long, InterdictOrderCoefficientItemDTO>? interdictCoeffsById)
    {
        if (submittedCoefValuesById.TryGetValue(coefficientItem.CoefficientId, out var submittedValue))
        {
            return submittedValue;
        }

        if (interdictCoeffsById != null && interdictCoeffsById.TryGetValue(coefficientItem.CoefficientId, out var interdictCoeff))
        {
            return interdictCoeff.OutPutFactValue ?? 0;
        }

        return 0;
    }

    private static double ResolveWageValueWhenCheckFormulaSkipsMainCalculation(
        IReadOnlyDictionary<long, int> submittedWageValuesById,
        OrganisationEmployeeTypeOrderTypeWageItemDTO wageItem,
        IReadOnlyDictionary<long, InterdictOrderWageItemDTO>? interdictWagesById)
    {
        if (wageItem.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput)
        {
            return ResolveEqualToInputWageValue(submittedWageValuesById, wageItem, interdictWagesById);
        }

        if (submittedWageValuesById.TryGetValue(wageItem.WageItemId, out var submittedValue))
        {
            return submittedValue;
        }

        return 0;
    }

    private static double ResolveCoefficientValueWhenCheckFormulaSkipsMainCalculation(
        IReadOnlyDictionary<long, double> submittedCoefValuesById,
        OrganisationEmployeeTypeOrderTypeCoefficientDTO coefficientItem,
        IReadOnlyDictionary<long, InterdictOrderCoefficientItemDTO>? interdictCoeffsById)
    {
        if (coefficientItem.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput)
        {
            return ResolveEqualToInputCoefficientValue(submittedCoefValuesById, coefficientItem, interdictCoeffsById);
        }

        if (submittedCoefValuesById.TryGetValue(coefficientItem.CoefficientId, out var submittedValue))
        {
            return submittedValue;
        }

        return 0;
    }

    private static void ApplyEqualToInputSubmittedWageValue(
        IReadOnlyDictionary<long, int> submittedWageValuesById,
        OrganisationEmployeeTypeOrderTypeWageItemDTO wageItem)
    {
        if (wageItem.EnterTypeId != (long)Enums.EnterTypeId.EqualToinput)
        {
            return;
        }

        if (submittedWageValuesById.TryGetValue(wageItem.WageItemId, out var submittedValue))
        {
            wageItem.NewValue = submittedValue;
        }
    }

    private static void ApplyEqualToInputSubmittedCoefficientValue(
        IReadOnlyDictionary<long, double> submittedCoefValuesById,
        OrganisationEmployeeTypeOrderTypeCoefficientDTO coefficientItem)
    {
        if (coefficientItem.EnterTypeId != (long)Enums.EnterTypeId.EqualToinput)
        {
            return;
        }

        if (submittedCoefValuesById.TryGetValue(coefficientItem.CoefficientId, out var submittedValue))
        {
            coefficientItem.NewOutPutFactValue = submittedValue;
        }
    }

    private static bool BatchExcelRowBelongsToEmployee(
        BatchGridModelForExcel row,
        long employeeId,
        string? employeeNationalNo)
    {
        if (row.EmployeeId.HasValue && row.EmployeeId.Value == employeeId)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(employeeNationalNo) || string.IsNullOrWhiteSpace(row.NationalNo))
        {
            return false;
        }

        return string.Equals(row.NationalNo.Trim(), employeeNationalNo.Trim(), StringComparison.Ordinal);
    }

    private static Dictionary<long, BatchGridModelForExcel>? BuildBatchExcelOverrideMap(
        List<BatchGridModelForExcel>? excelRows,
        long employeeId,
        string? employeeNationalNo = null)
    {
        if (excelRows == null || excelRows.Count == 0)
        {
            return null;
        }

        var matchedRows = excelRows
            .Where(i => BatchExcelRowBelongsToEmployee(i, employeeId, employeeNationalNo))
            .ToList();

        if (matchedRows.Count == 0)
        {
            return null;
        }

        return matchedRows
            .GroupBy(i => i.ItemId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    private static void SeedBatchExcelOverrideItems(
        BaseOrderRequest req,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? wageExcelByItemId,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? coefExcelByItemId)
    {
        if (wageExcelByItemId != null && wageExcelByItemId.Count > 0)
        {
            req.WageItems ??= new List<Core.DTOs.WageItem>();
            var wageItemsById = req.WageItems.ToDictionary(w => w.WageItemId);
            foreach (var excelRow in wageExcelByItemId.Values)
            {
                var value = Convert.ToInt32(excelRow.Value);
                if (wageItemsById.TryGetValue(excelRow.ItemId, out var existingWageItem))
                {
                    existingWageItem.Value = value;
                    continue;
                }

                var newWageItem = new Core.DTOs.WageItem()
                {
                    WageItemId = excelRow.ItemId,
                    EnterTypeId = (long)Enums.EnterTypeId.fixValue,
                    Value = value,
                };
                req.WageItems.Add(newWageItem);
                wageItemsById[excelRow.ItemId] = newWageItem;
            }
        }

        if (coefExcelByItemId != null && coefExcelByItemId.Count > 0)
        {
            req.CoeficentItems ??= new List<coeficentItem>();
            var coefItemsById = req.CoeficentItems.ToDictionary(c => c.CoefficientId);
            foreach (var excelRow in coefExcelByItemId.Values)
            {
                var value = Convert.ToDouble(excelRow.Value);
                if (coefItemsById.TryGetValue(excelRow.ItemId, out var existingCoefItem))
                {
                    existingCoefItem.Value = value;
                    continue;
                }

                var newCoefItem = new coeficentItem()
                {
                    CoefficientId = excelRow.ItemId,
                    EnterTypeId = (long)Enums.EnterTypeId.fixValue,
                    Value = value,
                };
                req.CoeficentItems.Add(newCoefItem);
                coefItemsById[excelRow.ItemId] = newCoefItem;
            }
        }
    }

    private static bool TryApplyBatchExcelCoefficientOverride(
        OrganisationEmployeeTypeOrderTypeCoefficientDTO coefficientItem,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? coefExcelByItemId,
        Dictionary<long, coeficentItem> coeficentItemsById,
        List<coeficentItem> coeficentItems)
    {
        if (coefExcelByItemId == null || !coefExcelByItemId.TryGetValue(coefficientItem.CoefficientId, out var coefExcelOverride))
        {
            return false;
        }

        coefficientItem.NewOutPutFactValue = Convert.ToDouble(coefExcelOverride.Value);
        if (coeficentItemsById.TryGetValue(coefficientItem.CoefficientId, out var existingCoefItem))
        {
            existingCoefItem.EnterTypeId = coefficientItem.EnterTypeId;
            existingCoefItem.Value = Convert.ToDouble(coefficientItem.NewOutPutFactValue);
        }
        else
        {
            var newCoefItem = new coeficentItem()
            {
                CoefficientId = coefficientItem.CoefficientId,
                EnterTypeId = coefficientItem.EnterTypeId,
                Value = coefficientItem.NewOutPutFactValue == null ? 0 : Convert.ToDouble(coefficientItem.NewOutPutFactValue),
            };
            coeficentItems.Add(newCoefItem);
            coeficentItemsById[coefficientItem.CoefficientId] = newCoefItem;
        }

        coefficientItem.IsRowSuccess = true;
        coefficientItem.formularowmessage = "Ok - از اکسل";
        return true;
    }

    private static bool TryApplyBatchExcelWageOverride(
        OrganisationEmployeeTypeOrderTypeWageItemDTO wageItem,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? wageExcelByItemId,
        Dictionary<long, Core.DTOs.WageItem> wageItemsById,
        List<Core.DTOs.WageItem> wageItems)
    {
        if (wageExcelByItemId == null || !wageExcelByItemId.TryGetValue(wageItem.WageItemId, out var wageExcelOverride))
        {
            return false;
        }

        wageItem.NewValue = Convert.ToDouble(wageExcelOverride.Value);
        if (wageItemsById.TryGetValue(wageItem.WageItemId, out var existingWageItem))
        {
            existingWageItem.EnterTypeId = wageItem.EnterTypeId;
            existingWageItem.Value = Convert.ToInt32(wageItem.NewValue);
        }
        else
        {
            var newWageItem = new Core.DTOs.WageItem()
            {
                WageItemId = wageItem.WageItemId,
                EnterTypeId = wageItem.EnterTypeId,
                Value = Convert.ToInt32(wageItem.NewValue),
            };
            wageItems.Add(newWageItem);
            wageItemsById[wageItem.WageItemId] = newWageItem;
        }

        wageItem.IsRowSuccess = true;
        wageItem.formularowmessage = "Ok - از اکسل";
        return true;
    }

    private static void RestoreEqualToInputValuesFromSubmitted(
        ConsequenceRespone resp,
        IReadOnlyDictionary<long, int> submittedWageValuesById,
        IReadOnlyDictionary<long, double> submittedCoefValuesById,
        Dictionary<long, Core.DTOs.WageItem> wageItemsById,
        Dictionary<long, coeficentItem> coeficentItemsById,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? wageExcelByItemId = null,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? coefExcelByItemId = null)
    {
        if (submittedWageValuesById.Count == 0 && submittedCoefValuesById.Count == 0)
        {
            return;
        }

        foreach (var wageItem in resp.OrderWageSettingList)
        {
            if (wageExcelByItemId != null && wageExcelByItemId.ContainsKey(wageItem.WageItemId))
            {
                continue;
            }

            if (wageItem.EnterTypeId != (long)Enums.EnterTypeId.EqualToinput)
            {
                continue;
            }

            if (!submittedWageValuesById.TryGetValue(wageItem.WageItemId, out var submittedWageValue))
            {
                continue;
            }

            wageItem.NewValue = submittedWageValue;
            if (wageItemsById.TryGetValue(wageItem.WageItemId, out var reqWageItem))
            {
                reqWageItem.Value = submittedWageValue;
            }
        }

        foreach (var coefficientItem in resp.OrderCoefficientSettingList)
        {
            if (coefExcelByItemId != null && coefExcelByItemId.ContainsKey(coefficientItem.CoefficientId))
            {
                continue;
            }

            if (coefficientItem.EnterTypeId != (long)Enums.EnterTypeId.EqualToinput)
            {
                continue;
            }

            if (!submittedCoefValuesById.TryGetValue(coefficientItem.CoefficientId, out var submittedCoefValue))
            {
                continue;
            }

            coefficientItem.NewOutPutFactValue = submittedCoefValue;
            if (coeficentItemsById.TryGetValue(coefficientItem.CoefficientId, out var reqCoefItem))
            {
                reqCoefItem.Value = submittedCoefValue;
            }
        }
    }

    private static int ResolveIssueOrderPersistedWageValue(
        OrganisationEmployeeTypeOrderTypeWageItemDTO item,
        IReadOnlyDictionary<long, Core.DTOs.WageItem> clientSubmittedWagesById,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? wageExcelByItemId = null)
    {
        if (wageExcelByItemId != null
            && wageExcelByItemId.TryGetValue(item.WageItemId, out var wageExcelOverride))
        {
            return Convert.ToInt32(wageExcelOverride.Value);
        }

        if (item.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput
            && clientSubmittedWagesById.TryGetValue(item.WageItemId, out var equalToInputWage))
        {
            return equalToInputWage.Value;
        }

        if (clientSubmittedWagesById.TryGetValue(item.WageItemId, out var clientWage))
        {
            return clientWage.Value;
        }

        return Convert.ToInt32(item.NewValue);
    }

    private static double ResolveIssueOrderPersistedCoefficientValue(
        OrganisationEmployeeTypeOrderTypeCoefficientDTO item,
        IReadOnlyDictionary<long, coeficentItem> clientSubmittedCoefsById,
        IReadOnlyDictionary<long, BatchGridModelForExcel>? coefExcelByItemId = null)
    {
        if (coefExcelByItemId != null
            && coefExcelByItemId.TryGetValue(item.CoefficientId, out var coefExcelOverride))
        {
            return Convert.ToDouble(coefExcelOverride.Value);
        }

        if (item.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput
            && clientSubmittedCoefsById.TryGetValue(item.CoefficientId, out var equalToInputCoef))
        {
            return equalToInputCoef.Value;
        }

        if (clientSubmittedCoefsById.TryGetValue(item.CoefficientId, out var clientCoef))
        {
            return clientCoef.Value;
        }

        return Convert.ToDouble(item.NewOutPutFactValue);
    }

    private static void SyncCalculatedWageItemToRequest(
        OrganisationEmployeeTypeOrderTypeWageItemDTO wageItem,
        Dictionary<long, Core.DTOs.WageItem> wageItemsById,
        List<Core.DTOs.WageItem> wageItems)
    {
        wageItem.NewValue = ResolveFinalOrderItemValue(
            wageItem.NewValue,
            wageItem.EnterTypeId,
            wageItem.FixValue,
            wageItem.Min,
            wageItem.Max);

        if (wageItemsById.TryGetValue(wageItem.WageItemId, out var updatedWageItem))
        {
            updatedWageItem.EnterTypeId = wageItem.EnterTypeId;
            updatedWageItem.Value = Convert.ToInt32(wageItem.NewValue);
            return;
        }

        var newWageItem = new Core.DTOs.WageItem()
        {
            WageItemId = wageItem.WageItemId,
            EnterTypeId = wageItem.EnterTypeId,
            Value = Convert.ToInt32(wageItem.NewValue),
        };
        wageItems.Add(newWageItem);
        wageItemsById[wageItem.WageItemId] = newWageItem;
    }

    private static void SyncCalculatedCoefficientItemToRequest(
        OrganisationEmployeeTypeOrderTypeCoefficientDTO coefficientItem,
        Dictionary<long, coeficentItem> coeficentItemsById,
        List<coeficentItem> coeficentItems)
    {
        coefficientItem.NewOutPutFactValue = ResolveFinalOrderItemValue(
            coefficientItem.NewOutPutFactValue ?? 0,
            coefficientItem.EnterTypeId,
            coefficientItem.FixValue,
            coefficientItem.Min,
            coefficientItem.Max);

        if (coeficentItemsById.TryGetValue(coefficientItem.CoefficientId, out var updatedCoefItem))
        {
            updatedCoefItem.EnterTypeId = coefficientItem.EnterTypeId;
            updatedCoefItem.Value = Convert.ToDouble(coefficientItem.NewOutPutFactValue);
            return;
        }

        var newCoefItem = new coeficentItem()
        {
            CoefficientId = coefficientItem.CoefficientId,
            EnterTypeId = coefficientItem.EnterTypeId,
            Value = coefficientItem.NewOutPutFactValue == null ? 0 : Convert.ToDouble(coefficientItem.NewOutPutFactValue),
        };
        coeficentItems.Add(newCoefItem);
        coeficentItemsById[coefficientItem.CoefficientId] = newCoefItem;
    }

    private OperationResult SetOrderItemsGrid(BaseOrderRequest req)
    {
        ConsequenceRespone resp = new ConsequenceRespone();

        resp.InterdictOrderDTO = req.InterdictOrderDTO;
        #region GetConsequences

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();

            using (SqlCommand wageCmd = new SqlCommand("[Order].[GetCurrentPayLocationEmployeeTypeOrderTypeWageItems]", con))
            {
                wageCmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = req.PayLocationId;
                wageCmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = req.EmployeeTypeId;
                wageCmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = req.OrderTypeId;
                wageCmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = (object?)req.ImpleDate?.Date ?? DBNull.Value;
                wageCmd.CommandType = CommandType.StoredProcedure;
                using SqlDataReader rdr = wageCmd.ExecuteReader();
                resp.OrderWageSettingList = new List<OrganisationEmployeeTypeOrderTypeWageItemDTO>();
                int Windex = 1;
                while (rdr.Read())
                {
                    var row = rdr.ConvertToObject<OrganisationEmployeeTypeOrderTypeWageItemDTO>();
                    row.Id = Windex;
                    resp.OrderWageSettingList.Add(row);
                    Windex++;
                }
            }

            using (SqlCommand coefCmd = new SqlCommand("[Order].[GetCurrentPayLocationEmployeeTypeOrderTypeCoefficients]", con))
            {
                coefCmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = req.PayLocationId;
                coefCmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = req.EmployeeTypeId;
                coefCmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = req.OrderTypeId;
                coefCmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = (object?)req.ImpleDate?.Date ?? DBNull.Value;
                coefCmd.CommandType = CommandType.StoredProcedure;
                using SqlDataReader rdr = coefCmd.ExecuteReader();
                resp.OrderCoefficientSettingList = new List<OrganisationEmployeeTypeOrderTypeCoefficientDTO>();
                int index = 1;
                while (rdr.Read())
                {
                    var row = rdr.ConvertToObject<OrganisationEmployeeTypeOrderTypeCoefficientDTO>();
                    row.Id = index;
                    resp.OrderCoefficientSettingList.Add(row);
                    index++;
                }
            }
        }
        #endregion GetConsequences END

        if (req.IssueTypeId == (long)Enums.IssueType.Cancelation)
        {
            foreach (var item in resp.OrderWageSettingList)
            {
                item.EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec;
                item.OrganisationCheckFormulaId = null;
                item.OrganisationFormulaId = null;
            }
            foreach (var item in resp.OrderCoefficientSettingList)
            {
                item.EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec;
                item.OrganisationCheckFormulaId = null;
                item.OrganisationFormulaId = null;
            }
        }



        if (resp.OrderWageSettingList.Any(i => i.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput))
        {
            resp.SettingHasEqulToInput = true;
        }
        if (resp.OrderCoefficientSettingList.Any(i => i.EnterTypeId == (long)Enums.EnterTypeId.EqualToinput))
        {
            resp.SettingHasEqulToInput = true;
        }

        if (req.CoeficentItems == null)
        {
            req.CoeficentItems = new List<coeficentItem>();
        }

        var submittedWageValuesById = (req.WageItems ?? new List<Core.DTOs.WageItem>())
            .GroupBy(w => w.WageItemId)
            .ToDictionary(g => g.Key, g => g.First().Value);
        var submittedCoefValuesById = req.CoeficentItems
            .GroupBy(c => c.CoefficientId)
            .ToDictionary(g => g.Key, g => g.First().Value);

        var coefExcelByItemId = BuildBatchExcelOverrideMap(
            req.coefOverRideExcel,
            req.EmployeeId,
            req.InterdictOrderDTO?.NationalNo);
        var wageExcelByItemId = BuildBatchExcelOverrideMap(
            req.wageOverRideExcel,
            req.EmployeeId,
            req.InterdictOrderDTO?.NationalNo);
        SeedBatchExcelOverrideItems(req, wageExcelByItemId, coefExcelByItemId);

        var coeficentItemsById = req.CoeficentItems.ToDictionary(c => c.CoefficientId);
        var interdictCoeffsById = resp.InterdictOrderDTO?.CoefficientItemList?
            .ToDictionary(c => c.CoefficientId);

        foreach (var Coefficientitem in resp.OrderCoefficientSettingList)
        {
            try
            {

                #region Coef_Items_Excel

                if (TryApplyBatchExcelCoefficientOverride(
                        Coefficientitem,
                        coefExcelByItemId,
                        coeficentItemsById,
                        req.CoeficentItems))
                {
                    continue;
                }

                #endregion Coef_Items_Excel

                Coefficientitem.BuildTreeTrace = req.BuildTreeTrace;
                if (resp.InterdictOrderDTO == null)
                {
                    Coefficientitem.OutPutFactValue = 0;
                }
                else
                {
                    if (resp.InterdictOrderDTO.CoefficientItemList == null)
                    {
                        Coefficientitem.OutPutFactValue = 0;
                    }
                    else if (interdictCoeffsById != null && interdictCoeffsById.TryGetValue(Coefficientitem.CoefficientId, out var interdictCoeff))
                    {
                        Coefficientitem.OutPutFactValue = interdictCoeff.OutPutFactValue;
                        Coefficientitem.LastInterdictId = resp.InterdictOrderDTO.Id;
                    }
                    else
                    {
                        Coefficientitem.OutPutFactValue = 0;
                    }
                }
                if (Coefficientitem.OrganisationCheckFormulaId > 0 && Coefficientitem.CheckingTimeId == (long)Enums.CheckingTime.BeforeMainCalculation)
                {
                    var formulaRespone = CommunicateWithFormula(Coefficientitem.OrganisationCheckFormulaId.Value, req);
                    Coefficientitem.IsRowSuccess = formulaRespone.Succees;
                    if (req.BuildTreeTrace == true)
                    {
                        Coefficientitem.CheckFormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                        Coefficientitem.CheckFormulaVariableFriendlyList = formulaRespone.VariableFriendlyList;
                        Coefficientitem.CheckFormulaText = formulaRespone.FormulaText;
                        Coefficientitem.CheckFormulaTreeParser = formulaRespone.FormulaTreeParser;
                        Coefficientitem.CheckFormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                        // Add debug log to CheckFormulaTreeParser
                        if (Coefficientitem.CheckFormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                        {
                            Coefficientitem.CheckFormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                        }
                    }

                    if (formulaRespone.Succees)
                    {
                        if (formulaRespone.Result > 0)
                        {

                        }
                        else
                        {
                            Coefficientitem.NewOutPutFactValue = ResolveCoefficientValueWhenCheckFormulaSkipsMainCalculation(
                                submittedCoefValuesById,
                                Coefficientitem,
                                interdictCoeffsById);
                            Coefficientitem.IsRowSuccess = true;
                            Coefficientitem.CheckFormula = Coefficientitem.CheckFormula + " - X قبل";
                            if (Coefficientitem != null)
                            {
                                Coefficientitem.formularowmessage = Coefficientitem.CheckFormulaErrorMessage;
                            }

                            ApplyEqualToInputSubmittedCoefficientValue(submittedCoefValuesById, Coefficientitem);
                            SyncCalculatedCoefficientItemToRequest(Coefficientitem, coeficentItemsById, req.CoeficentItems);
                            continue;
                        }
                    }
                    else
                    {
                        Coefficientitem.formularowmessage = " خطا در فرمول بررسی صحت " + formulaRespone.ResponseMessage;
                        if (!formulaRespone.Succees)
                        {
                            Coefficientitem.IsRowSuccess = false;
                            Coefficientitem.NewOutPutFactValue = ResolveFinalOrderItemValue(
                                Coefficientitem.NewOutPutFactValue ?? 0,
                                Coefficientitem.EnterTypeId,
                                Coefficientitem.FixValue,
                                Coefficientitem.Min,
                                Coefficientitem.Max);
                            return OperationResult.Succeeded(payload: resp);
                        }
                    }

                }

                switch (Coefficientitem.EnterTypeId)
                {
                    case (long)Enums.EnterTypeId.EqualToinput:
                        Coefficientitem.NewOutPutFactValue = ResolveEqualToInputCoefficientValue(
                            submittedCoefValuesById,
                            Coefficientitem,
                            interdictCoeffsById);
                        Coefficientitem.IsRowSuccess = true;
                        Coefficientitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.EqualTolastRec:
                        if (interdictCoeffsById != null && interdictCoeffsById.TryGetValue(Coefficientitem.CoefficientId, out var lastRecCoeff))
                        {
                            Coefficientitem.NewOutPutFactValue = lastRecCoeff.OutPutFactValue;
                        }
                        else
                        {
                            Coefficientitem.NewOutPutFactValue = 0;
                        }
                        Coefficientitem.IsRowSuccess = true;
                        Coefficientitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.PercentIncrease:
                    case (long)Enums.EnterTypeId.PercentDecrease:
                    case (long)Enums.EnterTypeId.AmountIncrease:
                    case (long)Enums.EnterTypeId.AmountDecrease:
                    case (long)Enums.EnterTypeId.fixValue:
                    case (long)Enums.EnterTypeId.boolean:
                        Coefficientitem.NewOutPutFactValue = CalculateOrderItemAdjustedValue(
                            Coefficientitem.OutPutFactValue ?? 0,
                            Coefficientitem.EnterTypeId,
                            Coefficientitem.FixValue);
                        Coefficientitem.IsRowSuccess = true;
                        Coefficientitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.UseFormula:

                        if (Coefficientitem.OrganisationFormulaId == null)
                        {
                            Coefficientitem.NewOutPutFactValue = 0;
                            Coefficientitem.formularowmessage = "شناسه فرمول انتخاب نشده است";
                            resp.Permission = false;
                            Coefficientitem.IsRowSuccess = false;
                        }
                        else
                        {
                            if (resp.SettingHasEqulToInput == false || (resp.SettingHasEqulToInput == true && req.DoFinalCalc == true))
                            {

                                resp.CalculateSummary = true;
                                var formulaRespone = CommunicateWithFormula(Coefficientitem.OrganisationFormulaId.Value, req);
                                if (req.BuildTreeTrace == true)
                                {
                                    Coefficientitem.FormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                                    Coefficientitem.VariableFriendlyList = formulaRespone.VariableFriendlyList;
                                    Coefficientitem.FormulaText = formulaRespone.FormulaText;
                                    Coefficientitem.FormulaTreeParser = formulaRespone.FormulaTreeParser;
                                    Coefficientitem.FormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                                    // Add debug log to FormulaTreeParser
                                    if (Coefficientitem.FormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                                    {
                                        Coefficientitem.FormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                                    }
                                }
                                Coefficientitem.NewOutPutFactValue = formulaRespone.Result;
                                Coefficientitem.IsRowSuccess = formulaRespone.Succees;
                                Coefficientitem.SuccessRunTimeInmilliseconds = formulaRespone.SuccessRunTimeInmilliseconds;
                                if (!formulaRespone.Succees)
                                {
                                    Coefficientitem.formularowmessage = formulaRespone.ResponseMessage + " " + Coefficientitem.FormulaErrorMessage;
                                    Coefficientitem.IsRowSuccess = false;
                                    return OperationResult.Succeeded(payload: resp);
                                }
                                else
                                {
                                    Coefficientitem.formularowmessage = "Ok";
                                }
                            }
                            else
                            {
                                Coefficientitem.NewOutPutFactValue = 0;
                                Coefficientitem.formularowmessage = "در انتظار ورودی...";
                            }
                        }
                        break;

                    default:
                        Coefficientitem.NewOutPutFactValue = 0;
                        Coefficientitem.IsRowSuccess = false;
                        Coefficientitem.formularowmessage = "نحوه محاسبه ضریب پشتیبانی نمی‌شود";
                        break;
                }


                if (Coefficientitem.OrganisationCheckFormulaId > 0 && Coefficientitem.CheckingTimeId == (long)Enums.CheckingTime.AfterMainCalculation)
                {
                    var formulaRespone = CommunicateWithFormula(Coefficientitem.OrganisationCheckFormulaId.Value, req);
                    if (req.BuildTreeTrace == true)
                    {
                        Coefficientitem.CheckFormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                        Coefficientitem.CheckFormulaVariableFriendlyList = formulaRespone.VariableFriendlyList;
                        Coefficientitem.CheckFormulaText = formulaRespone.FormulaText;
                        Coefficientitem.CheckFormulaTreeParser = formulaRespone.FormulaTreeParser;
                        Coefficientitem.CheckFormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                        // Add debug log to CheckFormulaTreeParser
                        if (Coefficientitem.CheckFormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                        {
                            Coefficientitem.CheckFormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                        }
                    }
                    Coefficientitem.IsRowSuccess = formulaRespone.Succees;
                    if (formulaRespone.Succees)
                    {
                        if (formulaRespone.Result > 0)
                        {

                        }
                        else
                        {
                            Coefficientitem.NewOutPutFactValue = ResolveCoefficientValueWhenCheckFormulaSkipsMainCalculation(
                                submittedCoefValuesById,
                                Coefficientitem,
                                interdictCoeffsById);
                            Coefficientitem.IsRowSuccess = true;
                            Coefficientitem.CheckFormula = Coefficientitem.CheckFormula + " - X بعد";
                            ApplyEqualToInputSubmittedCoefficientValue(submittedCoefValuesById, Coefficientitem);
                            Coefficientitem.formularowmessage = Coefficientitem.CheckFormulaErrorMessage;
                            SyncCalculatedCoefficientItemToRequest(Coefficientitem, coeficentItemsById, req.CoeficentItems);
                            continue;
                        }
                    }
                    else
                    {
                        Coefficientitem.formularowmessage = " خطا در فرمول بررسی صحت " + formulaRespone.ResponseMessage;
                        if (!formulaRespone.Succees)
                        {
                            Coefficientitem.IsRowSuccess = false;
                            Coefficientitem.NewOutPutFactValue = ResolveFinalOrderItemValue(
                                Coefficientitem.NewOutPutFactValue ?? 0,
                                Coefficientitem.EnterTypeId,
                                Coefficientitem.FixValue,
                                Coefficientitem.Min,
                                Coefficientitem.Max);
                            return OperationResult.Succeeded(payload: resp);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Coefficientitem.OutPutFactValue = 0;
                Coefficientitem.IsRowSuccess = false;
                Coefficientitem.formularowmessage = ex.Message;
                break;
            }
            ApplyEqualToInputSubmittedCoefficientValue(submittedCoefValuesById, Coefficientitem);
            SyncCalculatedCoefficientItemToRequest(Coefficientitem, coeficentItemsById, req.CoeficentItems);
        }


        if (req.WageItems == null)
        {
            req.WageItems = new List<Core.DTOs.WageItem>();
        }
        resp.OrderWageSettingList = resp.OrderWageSettingList.OrderBy(i => i.Priority).ToList();

        var wageItemsById = req.WageItems.ToDictionary(w => w.WageItemId);
        var interdictWagesById = resp.InterdictOrderDTO?.WageItemList?
            .ToDictionary(w => w.WageItemId);

        foreach (var Wageitem in resp.OrderWageSettingList)
        {
            try
            {
                #region Wage_Items_Excel
                if (TryApplyBatchExcelWageOverride(
                        Wageitem,
                        wageExcelByItemId,
                        wageItemsById,
                        req.WageItems))
                {
                    continue;
                }
                #endregion Wage_Items_Excel

                Wageitem.BuildTreeTrace = req.BuildTreeTrace;

                if (resp.InterdictOrderDTO == null)
                {
                    Wageitem.Value = 0;
                }
                else
                {
                    if (interdictWagesById != null && interdictWagesById.TryGetValue(Wageitem.WageItemId, out var interdictWage))
                    {
                        Wageitem.Value = interdictWage.Value;
                        Wageitem.LastInterdictId = resp.InterdictOrderDTO.Id;
                    }
                    else
                    {
                        Wageitem.Value = 0;
                    }
                }
                if (Wageitem.OrganisationCheckFormulaId > 0 && Wageitem.CheckingTimeId == (long)Enums.CheckingTime.BeforeMainCalculation)
                {
                    var formulaRespone = CommunicateWithFormula(Wageitem.OrganisationCheckFormulaId.Value, req);
                    Wageitem.CheckFormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                    if (req.BuildTreeTrace == true)
                    {
                        Wageitem.CheckFormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                        Wageitem.CheckFormulaVariableFriendlyList = formulaRespone.VariableFriendlyList;
                        Wageitem.CheckFormulaText = formulaRespone.FormulaText;
                        Wageitem.CheckFormulaTreeParser = formulaRespone.FormulaTreeParser;
                        Wageitem.CheckFormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                        // Add debug log to CheckFormulaTreeParser
                        if (Wageitem.CheckFormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                        {
                            Wageitem.CheckFormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                        }
                    }
                    Wageitem.IsRowSuccess = formulaRespone.Succees;
                    if (formulaRespone.Succees)
                    {
                        if (formulaRespone.Result > 0)
                        {

                        }
                        else
                        {
                            Wageitem.NewValue = ResolveWageValueWhenCheckFormulaSkipsMainCalculation(
                                submittedWageValuesById,
                                Wageitem,
                                interdictWagesById);
                            Wageitem.IsRowSuccess = true;
                            Wageitem.CheckFormula = Wageitem.CheckFormula + " - X قبل";
                            Wageitem.formularowmessage = Wageitem.CheckFormulaErrorMessage;
                            ApplyEqualToInputSubmittedWageValue(submittedWageValuesById, Wageitem);
                            SyncCalculatedWageItemToRequest(Wageitem, wageItemsById, req.WageItems);
                            continue;
                        }
                    }
                    else
                    {
                        Wageitem.formularowmessage = " خطا در فرمول بررسی صحت " + formulaRespone.ResponseMessage;
                        if (!formulaRespone.Succees)
                        {
                            Wageitem.IsRowSuccess = false;
                            Wageitem.NewValue = ResolveFinalOrderItemValue(
                                Wageitem.NewValue,
                                Wageitem.EnterTypeId,
                                Wageitem.FixValue,
                                Wageitem.Min,
                                Wageitem.Max);
                            return OperationResult.Succeeded(payload: resp);
                        }
                    }

                }

                switch (Wageitem.EnterTypeId)
                {
                    case (long)Enums.EnterTypeId.EqualToinput:
                        Wageitem.NewValue = ResolveEqualToInputWageValue(
                            submittedWageValuesById,
                            Wageitem,
                            interdictWagesById);
                        Wageitem.IsRowSuccess = true;
                        Wageitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.EqualTolastRec:
                        if (interdictWagesById != null && interdictWagesById.TryGetValue(Wageitem.WageItemId, out var lastRecWage))
                        {
                            Wageitem.NewValue = lastRecWage.Value;
                        }
                        else
                        {
                            Wageitem.NewValue = 0;
                        }
                        Wageitem.IsRowSuccess = true;
                        Wageitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.PercentIncrease:
                    case (long)Enums.EnterTypeId.PercentDecrease:
                    case (long)Enums.EnterTypeId.AmountIncrease:
                    case (long)Enums.EnterTypeId.AmountDecrease:
                    case (long)Enums.EnterTypeId.fixValue:
                    case (long)Enums.EnterTypeId.boolean:
                        Wageitem.NewValue = CalculateOrderItemAdjustedValue(
                            Wageitem.Value,
                            Wageitem.EnterTypeId,
                            Wageitem.FixValue);
                        Wageitem.IsRowSuccess = true;
                        Wageitem.formularowmessage = "Ok";
                        break;

                    case (long)Enums.EnterTypeId.UseFormula:
                        if (Wageitem.OrganisationFormulaId == null)
                        {
                            Wageitem.NewValue = 0;
                            Wageitem.formularowmessage = "شناسه فرمول انتخاب نشده است";
                            resp.Permission = false;
                            Wageitem.IsRowSuccess = false;
                        }
                        else
                        {
                            if (resp.SettingHasEqulToInput == false || (resp.SettingHasEqulToInput == true && req.DoFinalCalc == true))
                            {
                                var formulaRespone = CommunicateWithFormula(Wageitem.OrganisationFormulaId.Value, req);
                                Wageitem.FormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                                if (req.BuildTreeTrace == true)
                                {
                                    Wageitem.FormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                                    Wageitem.VariableFriendlyList = formulaRespone.VariableFriendlyList;
                                    Wageitem.FormulaText = formulaRespone.FormulaText;
                                    Wageitem.FormulaTreeParser = formulaRespone.FormulaTreeParser;
                                    Wageitem.FormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                                    // Add debug log to FormulaTreeParser
                                    if (Wageitem.FormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                                    {
                                        Wageitem.FormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                                    }
                                }
                                Wageitem.NewValue = formulaRespone.Result;
                                Wageitem.IsRowSuccess = formulaRespone.Succees;
                                Wageitem.SuccessRunTimeInmilliseconds = formulaRespone.SuccessRunTimeInmilliseconds;
                                if (!formulaRespone.Succees)
                                {
                                    Wageitem.formularowmessage = formulaRespone.ResponseMessage + " " + Wageitem.FormulaErrorMessage;
                                    Wageitem.IsRowSuccess = false;
                                    return OperationResult.Succeeded(payload: resp);
                                }
                                else
                                {
                                    Wageitem.formularowmessage = "Ok";
                                }
                            }
                            else
                            {
                                Wageitem.NewValue = 0;
                                Wageitem.formularowmessage = "در انتظار ورودی...";
                            }
                        }
                        break;

                    default:
                        Wageitem.NewValue = 0;
                        Wageitem.IsRowSuccess = false;
                        Wageitem.formularowmessage = "نحوه محاسبه عامل حقوقی پشتیبانی نمی‌شود";
                        break;
                }
                if (Wageitem.OrganisationCheckFormulaId > 0 && Wageitem.CheckingTimeId == (long)Enums.CheckingTime.AfterMainCalculation)
                {
                    var formulaRespone = CommunicateWithFormula(Wageitem.OrganisationCheckFormulaId.Value, req);
                    if (req.BuildTreeTrace == true)
                    {
                        Wageitem.CheckFormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                        Wageitem.CheckFormulaVariableFriendlyList = formulaRespone.VariableFriendlyList;
                        Wageitem.CheckFormulaText = formulaRespone.FormulaText;
                        Wageitem.CheckFormulaTreeParser = formulaRespone.FormulaTreeParser;
                        Wageitem.CheckFormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                        // Add debug log to CheckFormulaTreeParser
                        if (Wageitem.CheckFormulaTreeParser != null && !string.IsNullOrEmpty(formulaRespone.DebugLog))
                        {
                            Wageitem.CheckFormulaTreeParser.DebugLog = formulaRespone.DebugLog;
                        }
                    }
                    Wageitem.IsRowSuccess = formulaRespone.Succees;
                    if (formulaRespone.Succees)
                    {
                        if (formulaRespone.Result > 0)
                        {

                        }
                        else
                        {
                            Wageitem.NewValue = ResolveWageValueWhenCheckFormulaSkipsMainCalculation(
                                submittedWageValuesById,
                                Wageitem,
                                interdictWagesById);
                            Wageitem.IsRowSuccess = true;
                            Wageitem.CheckFormula = Wageitem.CheckFormula + " - X بعد";
                            Wageitem.formularowmessage = Wageitem.CheckFormulaErrorMessage;
                            ApplyEqualToInputSubmittedWageValue(submittedWageValuesById, Wageitem);
                            SyncCalculatedWageItemToRequest(Wageitem, wageItemsById, req.WageItems);
                            continue;
                        }
                    }
                    else
                    {
                        Wageitem.formularowmessage = " خطا در فرمول بررسی صحت " + formulaRespone.ResponseMessage;
                        if (!formulaRespone.Succees)
                        {
                            Wageitem.IsRowSuccess = false;
                            Wageitem.NewValue = ResolveFinalOrderItemValue(
                                Wageitem.NewValue,
                                Wageitem.EnterTypeId,
                                Wageitem.FixValue,
                                Wageitem.Min,
                                Wageitem.Max);
                            return OperationResult.Succeeded(payload: resp);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Wageitem.Value = 0;
                Wageitem.IsRowSuccess = false;
                Wageitem.formularowmessage = ex.Message;
                break;
            }
            ApplyEqualToInputSubmittedWageValue(submittedWageValuesById, Wageitem);
            SyncCalculatedWageItemToRequest(Wageitem, wageItemsById, req.WageItems);
        }

        RestoreEqualToInputValuesFromSubmitted(
            resp,
            submittedWageValuesById,
            submittedCoefValuesById,
            wageItemsById,
            coeficentItemsById,
            wageExcelByItemId,
            coefExcelByItemId);

        if (resp.SettingHasEqulToInput == false || (resp.SettingHasEqulToInput == true && req.DoFinalCalc == true))
        {
            resp.CalculateSummary = true;
        }
        System.Console.WriteLine("OrderWageSettingList end");
        return OperationResult.Succeeded(payload: resp);
    }


    public class GetOrderLandingPageSummary_Result
    {
        public string? OrderStatus { get; set; }
        public int? Count { get; set; }
        public long? OrderStatusId { get; set; }
    }

    public OperationResult GetOrderLandingPageSummary(long UserId)
    {
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("[Order].[SP_GetOrderLandingPageSummary]", con);

            DateTime dt = DateTime.Now;

            PersianCalendar PersianCalendar = new PersianCalendar();

            DateTime FromDate = PersianCalendar.ToDateTime(PersianCalendar.GetYear(dt), PersianCalendar.GetMonth(dt), 1, 0, 0, 0, 0);
            DateTime ToDate = DateTime.Now.AddMonths(1);

            //if (PersianCalendar.GetMonth(dt) <= 6)
            //{
            //    ToDate = PersianCalendar.ToDateTime(PersianCalendar.GetYear(dt), PersianCalendar.GetMonth(dt), 31, 0, 0, 0, 0);
            //}
            //if (PersianCalendar.GetMonth(dt) > 6 && PersianCalendar.GetMonth(dt) <= 11)
            //{
            //    ToDate = PersianCalendar.ToDateTime(PersianCalendar.GetYear(dt), PersianCalendar.GetMonth(dt), 30, 0, 0, 0, 0);
            //}
            //if (PersianCalendar.GetMonth(dt) == 12)
            //{
            //    if (PersianCalendar.IsLeapYear(PersianCalendar.GetYear(dt)) == true)
            //    {
            //        ToDate = PersianCalendar.ToDateTime(PersianCalendar.GetYear(dt), PersianCalendar.GetMonth(dt), 29, 0, 0, 0, 0);
            //    }
            //    else
            //    {
            //        ToDate = PersianCalendar.ToDateTime(PersianCalendar.GetYear(dt), PersianCalendar.GetMonth(dt), 30, 0, 0, 0, 0);
            //    }
            //}

            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            List<GetOrderLandingPageSummary_Result> ret = new();

            while (rdr.Read())
            {
                var row = rdr.ConvertToObject<GetOrderLandingPageSummary_Result>();
                ret.Add(row);
            }
            con.Close();
            return OperationResult.Succeeded(payload: ret);
        }
    }

    public OperationResult GetOrderYearlyLandingPageSummary(long userId)
    {
        var persianCalendar = new PersianCalendar();
        var now = DateTime.Now;
        var year = persianCalendar.GetYear(now);
        var fromDate = persianCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0);
        var toDate = persianCalendar.ToDateTime(year + 1, 1, 1, 0, 0, 0, 0);

        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.Status)
            .AsNoTracking()
            .Where(i => i.AspNetUsersId == userId
                && !i.IsDeleted
                && i.StatusId != (long)Enums.OrderStatus.LogicalDeleted
                && i.CreateDate >= fromDate
                && i.CreateDate < toDate)
            .GroupBy(i => new { i.StatusId, StatusTitle = i.Status != null ? i.Status.title : "" })
            .Select(g => new GetOrderLandingPageSummary_Result
            {
                OrderStatusId = g.Key.StatusId,
                OrderStatus = g.Key.StatusTitle,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return OperationResult.Succeeded(payload: query);
    }

    /// <summary>
    /// واکشی تنظیمات و محاسبه ردیف های حکم
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public OperationResult GetCurrentOrderConsequencs(BaseOrderRequest req)
    {
        bool isPayLocationChange = false;
        ConsequenceRespone resp = new ConsequenceRespone();
        var orderListRequest = new GetOrderListByEmployeeIdRequest()
        {
            EmployeeId = req.EmployeeId,
            CurrentUserId = _userService.GetUserId(),
            EmployeeTypeId = req.EmployeeTypeId,
            OrderTypeId = req.OrderTypeId,
            PayLocationId = req.PayLocationId,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            PageNo = 0,
            PageSize = 2500,
            SortColumn = "",
            SortOrder = ""
        };
        var OrderList = GetOrderList(orderListRequest);
        var permissionRespone = GetIssuePermission(orderListRequest, OrderList);

        if (permissionRespone.Item1)
        {
            resp.Permission = true;
        }
        else
        {

            resp.Permission = false;
            return OperationResult.Succeeded(permissionRespone.Item2, payload: resp);
        }

        var employeeTypeSetting = _organisationEmployeeTypeOrderTypeService.All(ImpleDate: req.StartDate).Where(i => i.OrganisationChartId == req.PayLocationId &&
        i.EmployeeTypeId == req.EmployeeTypeId &&
        i.OrderTypeId == req.OrderTypeId

        ).ToList();

        if (employeeTypeSetting != null)
        {
            if (employeeTypeSetting.Any())
            {
                var orderTypeSettingRow = employeeTypeSetting.Single();
                resp.OrderLevelId = orderTypeSettingRow.OrderLevelTypeId;
                resp.SelectPostFromChart = orderTypeSettingRow.SelectPostFromChart;
            }
            else
            {
                resp.Permission = false;
                return OperationResult.Succeeded("با توجه به تاریخ اجرای حکم تنظیمات احکام در نوع استخدام سازمان جاری یافت نشد", payload: resp);
            }
        }
        else
        {
            resp.Permission = false;
            return OperationResult.Succeeded("با توجه به تاریخ اجرای حکم تنظیمات احکام در نوع استخدام سازمان جاری یافت نشد", payload: resp);
        }



        resp.OrganisationEmployeeTypeOrderTypeDescriptionList = _mapper.Map<List<OrganisationEmployeeTypeOrderTypeDescriptionDTO>>(_organisationEmployeeTypeOrderTypeDescriptionService.All(ImpleDate: req.StartDate).Where(i => i.OrganisationChartId == req.PayLocationId &&
                           i.EmployeeTypeId == req.EmployeeTypeId &&
                           i.OrderTypeId == req.OrderTypeId

                           ).ToList());


        if (resp.OrderLevelId > 0)
        {
            if (req.IssueTypeId == (long)Enums.IssueType.Cancelation)
            {
                resp.CanChangeDTO = new Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypeOrderTypeCanChange();
                resp.OrderLevelId = (int)(Enums.OrderLevel.Recruit);
            }
            else
            {
                if (resp.OrderLevelId == (int)(Enums.OrderLevel.Recruit))
                {
                    var CanChange = _organisationEmployeeTypeOrderTypeCanChangeService.All(ImpleDate: req.StartDate)
                           .Include(i => i.DefaultEmpType)
                           .Include(i => i.DefaultEmpStatus)
                           .Where(i => i.OrganisationChartId == req.PayLocationId &&
                           i.EmployeeTypeId == req.EmployeeTypeId &&
                           i.OrderTypeId == req.OrderTypeId

                           ).ToList();
                    if (CanChange != null)
                    {
                        if (CanChange.Any())
                        {
                            if (CanChange.Count() == 1)
                            {
                                resp.CanChangeDTO = CanChange.SingleOrDefault();
                            }
                        }
                        else
                        {
                            resp.CanChangeDTO = new Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypeOrderTypeCanChange();
                        }
                    }
                }
                else
                {
                    resp.CanChangeDTO = new Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypeOrderTypeCanChange();
                }
            }

        }
        else
        {
            resp.Permission = false;
            return OperationResult.Succeeded("استخدامی یا حقوقی بودن حکم مشخص نیست لطفا از فرم تنظیمات احکام در سطح نوع استخدام اصلاح بفرمایید", payload: resp);
        }

        long lastorderId = GetLastOrderByImpleDate(req.EmployeeId, req.StartDate.Value, req.CorrectionOrderId, _currentUserDefaultOrganId);

        if (lastorderId > 0)
        {

            req.lastorderId = lastorderId;
            var finalOrder = OrderList.FirstOrDefault(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder);
            if (finalOrder == null)
            {
                resp.Permission = false;
                return OperationResult.Succeeded("حکم نهایی کارمند یافت نشد", payload: resp);
            }

            var lastOrderInList = OrderList.FirstOrDefault(i => i.Id == lastorderId);
            if (lastOrderInList == null)
            {
                resp.Permission = false;
                return OperationResult.Succeeded("حکم قبلی کارمند یافت نشد", payload: resp);
            }

            if (lastorderId == finalOrder.Id)
            {

            }
            else
            {
                if (req.ImpleDate.HasValue
                    && finalOrder.StartDate.HasValue
                    && req.ImpleDate.Value.Date < finalOrder.StartDate.Value.Date)
                {
                    resp.IsOutDate = true;
                    resp.OrderLevelId = (int)OrderLevel.Recruit;
                }
            }
            resp.lastorderId = lastorderId;
            resp.latorderSerial = lastOrderInList.OrderSerial;
            var lastOrderFlat = GetOrderFlat(lastorderId);
            if (lastOrderFlat?.Payload == null)
            {
                resp.Permission = false;
                return OperationResult.Succeeded("اطلاعات حکم قبلی کارمند قابل بازیابی نیست", payload: resp);
            }

            resp.InterdictOrderDTO = lastOrderFlat.Payload;


            #region بررسی تغییر نوع استخدام بودن و تغییر محل پرداخت


            //var CanChange = _organisationEmployeeTypeOrderTypeCanChangeService.All(ImpleDate: req.StartDate)
            //    .Where(i => i.OrganisationChartId == resp.InterdictOrderDTO.PayLocationId &&
            //    i.EmployeeTypeId == resp.InterdictOrderDTO.EmployeeTypeId &&
            //    i.OrderTypeId == req.OrderTypeId

            //    ).ToList();


            if (resp.InterdictOrderDTO.EmployeeTypeId != req.EmployeeTypeId)
            {
                if (resp.CanChangeDTO == null)
                {
                    resp.Permission = false;
                    return OperationResult.Succeeded("تنظیمات فیلد های استخدامی قابل تغییر برای حکم ما قبل جهت بررسی تغییر نوع استخدام یافت نشد", payload: resp);
                }
                else
                {
                    if (resp.CanChangeDTO.CanEmployeeTypeId == true)
                    {
                        if (resp.InterdictOrderDTO.EmployeeTypeId == req.EmployeeTypeId)
                        {

                        }
                        else
                        {
                            if (resp.InterdictOrderDTO.WageItemList != null)
                            {
                                foreach (var wageitem in resp.InterdictOrderDTO.WageItemList)
                                {
                                    wageitem.Value = 0;
                                }
                            }
                            if (resp.InterdictOrderDTO.CoefficientItemList != null)
                            {
                                foreach (var wageitem in resp.InterdictOrderDTO.CoefficientItemList)
                                {
                                    wageitem.OutPutFactValue = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        resp.Permission = false;
                        return OperationResult.Succeeded("خطا در حکم تغییر نوع استخدام !!!!!", payload: resp);
                    }

                }

            }
            else
            {
                if (req.IssueTypeId == (long)Enums.OrderLevel.Recruit)
                {
                    if (resp.CanChangeDTO == null)
                    {
                        resp.Permission = false;
                        return OperationResult.Succeeded("تنظیمات فیلد های استخدامی قابل تغییر برای حکم ما قبل جهت بررسی تغییر مجل پرداخت یافت نشد", payload: resp);
                    }
                    else
                    {
                        if (resp.CanChangeDTO.CanPayLocationId == true)
                        {
                            resp.InterdictOrderDTO.WageItemList = new List<InterdictOrderWageItemDTO>();
                            resp.InterdictOrderDTO.CoefficientItemList = new List<InterdictOrderCoefficientItemDTO>();
                            isPayLocationChange = true;
                        }
                    }
                }
            }

            #endregion بررسی تغییر نوع استخدام بودن

            if (resp.InterdictOrderDTO.WageItemList == null)
            {
                resp.InterdictOrderDTO.WageItemList = new List<InterdictOrderWageItemDTO>();
            }
            if (resp.InterdictOrderDTO.CoefficientItemList == null)
            {
                resp.InterdictOrderDTO.CoefficientItemList = new List<InterdictOrderCoefficientItemDTO>();
            }
        }
        else
        {
            var finalOrder = OrderList.Where(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder);
            if (finalOrder != null)
            {
                if (finalOrder.Any())
                {
                    resp.Permission = false;
                    return OperationResult.Succeeded("تاریخ اجرای حکم معتبر نمی باشد", payload: resp);
                }
            }
            resp.InterdictOrderDTO = new InterdictOrderFlatDTO();
            resp.InterdictOrderDTO.WageItemList = new List<InterdictOrderWageItemDTO>();
            resp.InterdictOrderDTO.CoefficientItemList = new List<InterdictOrderCoefficientItemDTO>();
        }



        #region CheckFormulas

        var checkformuls = new List<EmployeeTypeOrderTypeChecks_Result>();


        if (isPayLocationChange) // خکم تغییر محل پرداخت عامل ندارد
        {
            resp.OrderCoefficientSettingList = new List<OrganisationEmployeeTypeOrderTypeCoefficientDTO>();
            resp.OrderWageSettingList = new List<OrganisationEmployeeTypeOrderTypeWageItemDTO>();
        }
        else

        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[Order].[GetCurrentPayLocationEmployeeTypeOrderTypeChecks]", con);
                cmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = req.PayLocationId;
                cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = req.EmployeeTypeId;
                cmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = req.OrderTypeId;
                cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = (object?)req.ImpleDate?.Date ?? DBNull.Value;

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                int index = 1;
                while (rdr.Read())
                {
                    var checkformulasSetting = rdr.ConvertToObject<EmployeeTypeOrderTypeChecks_Result>();

                    checkformuls.Add(checkformulasSetting);
                    index++;
                }
                con.Close();
            }


            if (checkformuls != null)
            {
                if (checkformuls.Any())
                {
                    resp.CheckFormulas = new List<CheckFormula>();

                    var checkFormulaWageItems = new List<Core.DTOs.WageItem>();
                    var checkFormulaCoefItems = new List<coeficentItem>();
                    if (lastorderId > 0)
                    {
                        var currentOrderWages = _db.Set<InterdictOrderWageItem>()
                            .AsNoTracking()
                            .Where(i => i.InterdictOrderId == lastorderId)
                            .ToList();
                        foreach (var item in currentOrderWages)
                        {
                            checkFormulaWageItems.Add(new Core.DTOs.WageItem()
                            {
                                EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec,
                                Value = item.Value,
                                WageItemId = item.WageItemId
                            });
                        }

                        var currentOrderCoefficients = _db.Set<InterdictOrderCoefficientItem>()
                            .AsNoTracking()
                            .Where(i => i.InterdictOrderId == lastorderId)
                            .ToList();
                        foreach (var item in currentOrderCoefficients)
                        {
                            checkFormulaCoefItems.Add(new coeficentItem()
                            {
                                EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec,
                                Value = Convert.ToDouble(item.OutPutFactValue),
                                CoefficientId = item.CoefficientId
                            });
                        }
                    }

                    foreach (var checkformul in checkformuls)
                    {
                        CheckFormula checkFormula = new CheckFormula();
                        checkFormula.FormulaName = checkformul.FormulaTitle;
                        List<coeficentItem> CoeficentItems = checkFormulaCoefItems;
                        List<Core.DTOs.WageItem> WageItems = checkFormulaWageItems;
                        DateTime dt = DateTime.Now;
                        var formularespone = CommunicateWithFormula(checkformul.OrganisationFormulaId, new BaseOrderRequest()
                        {
                            BuildTreeTrace = req.BuildTreeTrace,
                            DoFinalCalc = true,
                            lastorderId = lastorderId,
                            EmployeeId = req.EmployeeId,
                            EmployeeTypeId = req.EmployeeTypeId,
                            OrderTypeId = req.OrderTypeId,
                            StartDate = req.StartDate,
                            EndDate = req.EndDate,
                            CorrectionOrderId = req.CorrectionOrderId,
                            EmployeeStatusId = req.EmployeeStatusId,
                            PayLocationId = _currentUserDefaultOrganId,
                            WorkPlaceId = req.WorkPlaceId,
                            OrganisationPositionId = req.OrganisationPositionId,
                            OrganizationJobId = req.OrganizationJobId,
                            ProjectId = req.ProjectId,
                            OrganizationUnitId = req.OrganizationUnitId,
                            CostCenterId = req.CostCenterId,
                            WageItems = WageItems,
                            CoeficentItems = CoeficentItems
                        });
                        checkFormula.ExecutionTimeInMs = (DateTime.Now - dt).Milliseconds;


                        if (req.BuildTreeTrace == true)
                        {
                            checkFormula.FormulaFriendlyText = formularespone.FormulaFriendlyText;
                            checkFormula.VariableFriendlyList = formularespone.VariableFriendlyList;
                            checkFormula.FormulaText = formularespone.FormulaText;
                            checkFormula.FormulaTreeParser = formularespone.FormulaTreeParser;
                            checkFormula.FormulaHelpDesc = formularespone.FormulaHelpDesc;
                            checkFormula.SuccessRunTimeInmilliseconds = (DateTime.Now - dt).Milliseconds;
                            // Add debug log to FormulaTreeParser
                            if (checkFormula.FormulaTreeParser != null && !string.IsNullOrEmpty(formularespone.DebugLog))
                            {
                                checkFormula.FormulaTreeParser.DebugLog = formularespone.DebugLog;
                            }
                            if (formularespone.Result > 0)
                            {
                                checkFormula.Result = true;
                            }
                            else
                            {
                                checkFormula.Result = false;
                            }

                        }


                        if (checkformul.CheckTypeId == (long)Enums.CheckFormulaType.Warning)
                        {
                            checkFormula.IsWarning = true;
                        }
                        if (formularespone.Succees)
                        {
                            if (formularespone.Result > 0)
                            {
                                checkFormula.Result = true;
                            }
                            else
                            {
                                checkFormula.Result = false;
                                if (checkformul.CheckTypeId == (long)Enums.CheckFormulaType.Warning)
                                {
                                    checkFormula.RelatedErrorMessage = checkformul.FailMessage;
                                }
                                else
                                {
                                    resp.Permission = false;
                                    return OperationResult.Succeeded(checkformul.FailMessage, payload: resp);
                                }
                            }
                        }
                        else
                        {
                            checkFormula.Result = formularespone.Succees;
                            checkFormula.IsWarning = false;
                            resp.Permission = false;
                            return OperationResult.Succeeded(formularespone.ResponseMessage + " خطا در فرمول چک قبل از صدور حکم ", payload: resp);
                        }

                        resp.CheckFormulas.Add(checkFormula);
                    }
                }
            }
        }
        #endregion CheckFormulas

        req.InterdictOrderDTO = resp.InterdictOrderDTO;
        var ItemsCalculation = SetOrderItemsGrid(req);


        if (ItemsCalculation.Success)
        {
            var ItemsPayLoad = (ConsequenceRespone)ItemsCalculation.Payload;

            if (ItemsPayLoad.OrderWageSettingList != null)
            {
                resp.OrderWageSettingList = [.. ItemsPayLoad.OrderWageSettingList.OrderBy(i => i.Priority)];
            }
            if (ItemsPayLoad.OrderCoefficientSettingList != null)
            {
                resp.OrderCoefficientSettingList = [.. ItemsPayLoad.OrderCoefficientSettingList.OrderBy(i => i.Priority)];
            }
            resp.SettingHasEqulToInput = ItemsPayLoad.SettingHasEqulToInput;
            #region CalculateSummary
            if (ItemsPayLoad.CalculateSummary)
            {

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[Order].[GetOrderViewSummary]", con);
                    cmd.Parameters.Add("@EmployeeID", SqlDbType.BigInt).Value = req.EmployeeId;
                    cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = req.EmployeeTypeId;
                    cmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = req.OrderTypeId;
                    cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = (object?)req.ImpleDate?.Date ?? DBNull.Value;
                    cmd.Parameters.Add("@LastInterdictID", SqlDbType.BigInt).Value = lastorderId;
                    cmd.Parameters.Add("@OrganisationChartId", SqlDbType.BigInt).Value = req.PayLocationId;
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        resp.VwInterdict_Order = rdr.ConvertToObject<VwInterdict_Order>();
                        break;
                    }
                    VwInterdict_Order lastInterdict;
                    if (lastorderId > 0)
                    {
                        lastInterdict = _dapper.Get<VwInterdict_Order>(
                            "SELECT * FROM [Order].[vw_Interdict_Order] WHERE Id = @Id",
                            new DynamicParameters(new { Id = lastorderId }),
                            CommandType.Text);
                    }
                    else
                    {
                        lastInterdict = new VwInterdict_Order();
                    }
                    resp.OrderComparitiveProperties = GetComparitiveFields(lastInterdict, resp.VwInterdict_Order);
                    con.Close();
                }

            }


            #endregion CalculateSummary
        }
        else
        {
            return ItemsCalculation;
        }

        return OperationResult.Succeeded(payload: resp);

    }

    private List<OrderComparitivePropertiesDTO> GetComparitiveFields(VwInterdict_Order LastOrder, VwInterdict_Order NewOrder)
    {

        List<OrderComparitivePropertiesDTO> ret = new List<OrderComparitivePropertiesDTO>();
        Type myType = NewOrder.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        PersianCalendar pc = new PersianCalendar();
        foreach (PropertyInfo prop in props)
        {
            object NewValue = prop.GetValue(NewOrder, null);
            object OldValue = prop.GetValue(LastOrder, null);
            try
            {


                if (NewValue == OldValue)
                {

                }
                else
                {
                    OrderComparitivePropertiesDTO toAdd = new OrderComparitivePropertiesDTO();

                    switch (prop.Name)
                    {
                        case "ExperienceRecorded":
                            toAdd.Property = "سابقه تجربی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "RetiredRecorded":

                            toAdd.Property = "سابقه بازنشستگی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "YearRecorded":

                            toAdd.Property = "سابقه سنواتی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;

                        case "HistoryOut":

                            toAdd.Property = "سابقه کار خارج از سازمان";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "HistoryStop":
                            toAdd.Property = "سابقه تجربی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "روز";
                            break;
                        case "RetiredFlagOk":

                            toAdd.Property = "سابقه تجربی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;

                        case "MarriageStatus":
                            toAdd.Property = "وضعیت تاهل";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;

                        case "YearCoefficient":
                            toAdd.Property = "ضریب افزایش سنواتی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "EducationGrade":

                            toAdd.Property = "مقطع تحصیلی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;

                        case "EffectiveEducationGrade":

                            toAdd.Property = "مقطع تحصیلی موثر";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IsWar":
                            toAdd.Property = "وضعیت رزمنده بودن";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IsCaptivity":
                            toAdd.Property = "وضعیت اسارت";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IsBasij":
                            toAdd.Property = "وضعیت بسیجی بودن";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IsIsar":

                            toAdd.Property = "وضعیت جانباز بودن";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;

                        case "IsarPercent":

                            toAdd.Property = "درصد جانبازی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "درصد";
                            break;

                        case "WarDuration":
                            toAdd.Property = "مدت رزمندگی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "روز";
                            break;
                        case "CaptivityDuration":
                            toAdd.Property = "مدت اسارت";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "روز";
                            break;
                        case "BasijDuration":

                            toAdd.Property = "مدت بسیج";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "روز";
                            break;

                        case "IsMartyrs":
                            toAdd.Property = "";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;
                        case "WifeCount":
                            toAdd.Property = "تعداد همسر";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "GradScore":
                            toAdd.Property = "امتیاز رتبه";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "EmployeeDate":
                            toAdd.Property = "تاریخ استخدام";

                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(OldValue);
                                toAdd.LastValue = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(NewValue);
                                toAdd.NewValue = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
                            }
                            toAdd.PostFix = "";
                            break;
                        case "InsuranceType":

                            toAdd.Property = "نوع بیمه";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;

                        case "AccountNumber":
                            toAdd.Property = "شماره حساب بانکی در حکم";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "OtherVeterans":

                            toAdd.Property = "سایر ایثارگری ها";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IsWomenHead":

                            toAdd.Property = "زن سرپرست خانوار";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(OldValue) == true)
                                {
                                    toAdd.LastValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.LastValue = "نمی باشد";
                                }

                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                if (Convert.ToBoolean(NewValue) == true)
                                {
                                    toAdd.NewValue = "می باشد";
                                }
                                else
                                {
                                    toAdd.NewValue = "نمی باشد";
                                }
                            }
                            toAdd.PostFix = "";
                            break;

                        case "FirstName":
                            toAdd.Property = "نام";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "LastName":
                            toAdd.Property = "نام خانوادگی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "FatherName":
                            toAdd.Property = "نام پدر";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "PersonelCode":
                            toAdd.Property = "کد کارمندی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IdentityNo":
                            toAdd.Property = "شماره شناسنامه";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "NationalNo":
                            toAdd.Property = "کد ملی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "DrivingLicenseType":
                            toAdd.Property = "نوع گواهینامه رانندگی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "ChildCount":
                            toAdd.Property = "تعداد اولاد";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "BirthDate":
                            toAdd.Property = "تاریخ تولد";

                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(OldValue);
                                toAdd.LastValue = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                var dt = Convert.ToDateTime(NewValue);
                                toAdd.NewValue = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
                            }
                            toAdd.PostFix = "";
                            break;
                        case "BirthPlace":
                            toAdd.Property = "محل تولد";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "DrivingLicenseNumber":
                            toAdd.Property = "شماره گواهینامه رانندگی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "EducatioField":
                            toAdd.Property = "رشته تحصیلی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "EducatioOrientation":
                            toAdd.Property = "گرایش تحصیلی";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;
                        case "IssuePlace":
                            toAdd.Property = "محل صدور";
                            if (OldValue == null)
                            {
                                toAdd.LastValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.LastValue = OldValue.ToString();
                            }
                            if (NewValue == null)
                            {
                                toAdd.NewValue = "نا مشخص";
                            }
                            else
                            {
                                toAdd.NewValue = NewValue.ToString();
                            }
                            toAdd.PostFix = "";
                            break;



                    }
                    if (string.IsNullOrEmpty(toAdd.Property))
                    {

                    }
                    else
                    {
                        if (OldValue == null && NewValue == null)
                        {

                        }
                        else
                        {
                            if (OldValue == null && NewValue != null)
                            {
                                ret.Add(toAdd);
                            }
                            if (NewValue == null && OldValue != null)
                            {
                                ret.Add(toAdd);
                            }

                            if (NewValue != null && OldValue != null)
                            {
                                if (NewValue.ToString().Trim() == OldValue.ToString().Trim())
                                {

                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(toAdd.NewValue))
                                    {
                                        toAdd.NewValue = "نا مشخص";
                                    }
                                    ret.Add(toAdd);
                                }
                            }
                        }


                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        return ret;
    }

    /// <summary>
    /// ارتباط با سرویس محاسباتی فرمول ها 
    /// </summary>
    /// <param name="OrganisationFormulaId"></param>
    /// <param name="req"></param>
    /// <returns></returns>
    public FormulaCalculateResponseDTO CommunicateWithFormula(long OrganisationFormulaId, BaseOrderRequest req)
    {
        var logBuilder = new StringBuilder();

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 1: Method started - OrganisationFormulaId: {OrganisationFormulaId}, EmployeeId: {req.EmployeeId}");
        }

        #region FillFrienlytextSources

        //List<EmployeeType> employeeTypes = new List<EmployeeType>();
        //List<OrganisationFormula> sorganisationFormulas = new List<OrganisationFormula>();
        //List<FormulaTable> formulatables = new List<FormulaTable>();
        //List<OrganisationOrderType> organisationOrderTypes = new List<OrganisationOrderType>();
        //List<OrganisationCoefficient> organisationCoefficients = new List<OrganisationCoefficient>();
        //List<FormulaDatabaseFunctionDefinition> formulaDatabaseFunctionDefinitions = new List<FormulaDatabaseFunctionDefinition>();
        //List<FormulaOperand> formulaOperands = new List<FormulaOperand>();
        //List<BaseTableValue> baseTableValues = new List<BaseTableValue>();
        //List<OrganisationWageItem> organisationWageItems = new List<OrganisationWageItem>();
        //List<OrganisationEmployeeType> organisationEmployeeTypes = new List<OrganisationEmployeeType>();
        //List<EducationGrade> educationGrades = new List<EducationGrade>();
        //List<OrganisationChart> organisationChart = new List<OrganisationChart>();
        //req.BuildTreeTrace = true;

        //if (req.BuildTreeTrace == true)
        //{
        //    //employeeTypes = _employeeTypeService.All(ImpleDate: req.StartDate).ToList();
        //    //sorganisationFormulas = _organisationFormulaService.All(ImpleDate: req.StartDate).ToList();
        //    //formulatables = _formulaTableService.All(ImpleDate: req.StartDate).ToList();
        //    //organisationOrderTypes = _organisationOrderTypeService.All(ImpleDate: req.StartDate).ToList();
        //    //organisationCoefficients = _organisationCoefficientService.All(ImpleDate: req.StartDate).ToList();
        //    //formulaDatabaseFunctionDefinitions = _formulaDatabaseFunctionDefinitionService.All(ImpleDate: req.StartDate).ToList();
        //    //formulaOperands = _formulaOperandService.All(ImpleDate: req.StartDate).ToList();
        //    //baseTableValues = _baseTableValueService.All(ImpleDate: req.StartDate).ToList();
        //    //organisationWageItems = _organisationWageItemService.All(ImpleDate: req.StartDate).ToList();
        //    //organisationEmployeeTypes = _organisationEmployeeTypeService.All(ImpleDate: req.StartDate).ToList();
        //    //educationGrades = _educationGradeService.All(ImpleDate: req.StartDate).ToList();
        //    //organisationChart = _organisationChartService.All(ImpleDate: req.StartDate).ToList();
        //}


        #endregion FillFrienlytextSources

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 2: Creating FormulaCalculationRequestDTO");
        }

        FormulaCalculationRequestDTO formulareq = new FormulaCalculationRequestDTO();

        if (req.IsBatch == true)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 3: Batch mode detected - disabling BuildTreeTrace");
            }
            formulareq.BuildTreeTrace = false;
            req.BuildTreeTrace = false;
        }

        formulareq.OrganisationFormulaId = OrganisationFormulaId;
        formulareq.VariableList = new Dictionary<string, double>
                                {
                                    { "EmployeeId", Convert.ToDouble(req.EmployeeId) },
                                    { "PayLocationId", Convert.ToDouble(req.PayLocationId) },
                                    { "NumericalDate", Convert.ToDouble(Utilities.ConvertDateToNumber(req.StartDate.Value)) },
                                    { "EmployeeTypeId", Convert.ToDouble(req.EmployeeTypeId) }
                                };

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 4: Base variables initialized - EmployeeId: {req.EmployeeId}, PayLocationId: {req.PayLocationId}, EmployeeTypeId: {req.EmployeeTypeId}");
        }

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 5: Building friendly variable list");

            formulareq.VariableFriendlyList = new Dictionary<string, string?>
                                {
                                    { "شناسه کارمندی", Convert.ToString(req.EmployeeId) },
                                    //{ "محل پرداخت", Convert.ToDouble(req.PayLocationId) },
                                    { "تاریخ عددی", Convert.ToString(Utilities.ConvertDateToNumber(req.StartDate.Value)) },
                                    //{ "نوع استخدام", Convert.ToDouble(req.EmployeeTypeId) }
                                };

            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 6: Querying EmployeeType table for EmployeeTypeId: {req.EmployeeTypeId}");
            var employeeType = _employeeTypeService.GetIdAsync(req.EmployeeTypeId).Result.title;
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 6: Retrieved EmployeeType: {employeeType}");

            formulareq.VariableFriendlyList.Add("نوع استخدام", employeeType);

            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 7: Querying OrganisationChart table for PayLocationId: {req.PayLocationId}");
            var PayLocation = _organisationChartService.GetIdAsync(req.PayLocationId).Result.title;
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 7: Retrieved PayLocation: {PayLocation}");

            formulareq.VariableFriendlyList.Add("محل پرداخت", PayLocation);

        }



        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 8: Calculating Persian calendar year");
        }

        var pc = new PersianCalendar();
        int shamsiYear = pc.GetYear(req.StartDate.Value);
        formulareq.VariableList.Add("shamsiYear", Convert.ToDouble(shamsiYear));

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 8: Persian year calculated: {shamsiYear}");
            formulareq.VariableFriendlyList.Add("سال شمسی", Convert.ToString(shamsiYear));
        }

        var employeeDateVars = _employeeContext.Employees
            .AsNoTracking()
            .Where(e => e.Id == req.EmployeeId)
            .Select(e => new { e.DeathDate, e.ReleaseDate })
            .FirstOrDefault();

        formulareq.VariableList.Add(
            "DeathDate",
            employeeDateVars?.DeathDate != null
                ? Convert.ToDouble(Utilities.ConvertDateToNumber(employeeDateVars.DeathDate.Value))
                : 0);
        formulareq.VariableList.Add(
            "ReleaseDate",
            employeeDateVars?.ReleaseDate != null
                ? Convert.ToDouble(Utilities.ConvertDateToNumber(employeeDateVars.ReleaseDate.Value))
                : 0);

        if (req.BuildTreeTrace == true)
        {
            formulareq.VariableFriendlyList.Add(
                "تاریخ فوت",
                employeeDateVars?.DeathDate != null
                    ? Convert.ToString(Utilities.ConvertDateToNumber(employeeDateVars.DeathDate.Value))
                    : "0");
            formulareq.VariableFriendlyList.Add(
                "تاریخ پایان همکاری",
                employeeDateVars?.ReleaseDate != null
                    ? Convert.ToString(Utilities.ConvertDateToNumber(employeeDateVars.ReleaseDate.Value))
                    : "0");
        }

        if (req.OrganizationJobId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 9: Processing OrganizationJob - OrganizationJobId: {req.OrganizationJobId}");
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 9: Querying OrganizationJob table for OrganizationJobId: {req.OrganizationJobId}");
            }

            var OrganizationJob = _organizationJobService.GetIdAsync(req.OrganizationJobId.Value).Result;

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 9: Retrieved OrganizationJob - JobDegree: {OrganizationJob.JobDegree}, Code: {OrganizationJob.Code}");
            }

            formulareq.VariableList.Add("JobDegree", Convert.ToDouble(OrganizationJob.JobDegree));

            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("گروه شغلی", Convert.ToString(OrganizationJob.JobDegree));
            }

            if (!string.IsNullOrEmpty(OrganizationJob.Code))
            {
                formulareq.VariableList.Add("JobCode", Convert.ToDouble(OrganizationJob.Code));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("کد شغل", Convert.ToString(OrganizationJob.Code));
                }

            }
            formulareq.VariableList.Add("OrganizationJobId", Convert.ToDouble(req.OrganizationJobId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه شغل", Convert.ToString(req.OrganizationJobId));
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 9: Querying Job table for JobId: {OrganizationJob.JobId}");
                var JobId = OrganizationJob.JobId;
                var jobTitle = _jobService.GetIdAsync(JobId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 9: Retrieved Job title: {jobTitle}");
                formulareq.VariableFriendlyList.Add("عنوان شغل", jobTitle);
            }

        }
        if (req.CostCenterId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 10: Processing CostCenter - CostCenterId: {req.CostCenterId}");
            }

            formulareq.VariableList.Add("CostCenterId", Convert.ToDouble(req.CostCenterId));

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 10: Querying OrganisationChart table for CostCenterId: {req.CostCenterId}");
                var CostCenter = _organisationChartService.GetIdAsync(req.CostCenterId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 10: Retrieved CostCenter: {CostCenter}");
                formulareq.VariableFriendlyList.Add("مرکز هزینه", CostCenter);
            }

        }
        if (req.WorkPlaceId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 11: Processing WorkPlace - WorkPlaceId: {req.WorkPlaceId}");
            }

            formulareq.VariableList.Add("WorkPlaceId", Convert.ToDouble(req.WorkPlaceId));
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 11: Querying OrganisationChart table for WorkPlaceId: {req.WorkPlaceId}");
                var WorkPlace = _organisationChartService.GetIdAsync(req.WorkPlaceId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 11: Retrieved WorkPlace: {WorkPlace}");
                formulareq.VariableFriendlyList.Add("محل خدمت", WorkPlace);
            }
        }


        if (req.OrganizationUnitId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 12: Processing OrganizationUnit - OrganizationUnitId: {req.OrganizationUnitId}");
            }

            formulareq.VariableList.Add("OrganizationUnitId", Convert.ToDouble(req.OrganizationUnitId));
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 12: Querying OrganisationChart table for OrganizationUnitId: {req.OrganizationUnitId}");
                var OrganizationUnit = _organisationChartService.GetIdAsync(req.OrganizationUnitId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 12: Retrieved OrganizationUnit: {OrganizationUnit}");
                formulareq.VariableFriendlyList.Add("واحد سازمانی", OrganizationUnit);
            }
        }
        if (req.ProjectId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 13: Processing Project - ProjectId: {req.ProjectId}");
            }

            formulareq.VariableList.Add("ProjectId", Convert.ToDouble(req.ProjectId));

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 13: Querying Project table for ProjectId: {req.ProjectId}");
                var Project = _projectService.GetIdAsync(req.ProjectId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 13: Retrieved Project: {Project}");
                formulareq.VariableFriendlyList.Add("پروژه", Project);
            }
        }
        if (req.lastorderId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 14: Processing LastOrder - lastorderId: {req.lastorderId}");
            }

            formulareq.VariableList.Add("lastorderId", Convert.ToDouble(req.lastorderId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه حکم ماقبل", req.lastorderId.ToString());
            }

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 14: Querying InterdictOrder table for lastorderId: {req.lastorderId}");
            }

            var lastInterdictObject = _db.Set<InterdictOrder>().Find(req.lastorderId);

            if (req.BuildTreeTrace == true)
            {
                if (lastInterdictObject != null)
                {
                    logBuilder.AppendLine($"[CommunicateWithFormula] STEP 14: Found InterdictOrder - EducationGradeId: {lastInterdictObject.EducationGradeId}, ChildCount: {lastInterdictObject.ChildCount}, WifeCount: {lastInterdictObject.WifeCount}, SponsorshipCount: {lastInterdictObject.SponsorshipCount}");
                }
                else
                {
                    logBuilder.AppendLine($"[CommunicateWithFormula] STEP 14: WARNING - InterdictOrder not found for lastorderId: {req.lastorderId}");
                }
            }

            if (lastInterdictObject.MarriageStatusId > 0)
            {
                formulareq.VariableList.Add("MarriageStatusId", Convert.ToDouble(lastInterdictObject.MarriageStatusId));
                if (req.BuildTreeTrace == true)
                {
                    var marriageStaus = _jobService._unitOfWork.Context.BaseTableValues.Find(lastInterdictObject.MarriageStatusId);

                    formulareq.VariableFriendlyList.Add("وضعیت تاهل در حکم ماقبل", marriageStaus.title);
                }
            }
            else
            {
                formulareq.VariableFriendlyList.Add("وضعیت تاهل در حکم ماقبل", "نا مشخص");
            }


            if (lastInterdictObject.EducationGradeId.HasValue)
            {
                formulareq.VariableList.Add("OrderEducationGradeId", Convert.ToDouble(lastInterdictObject.EducationGradeId));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add(" مقطع تحصیلی در حکم ", lastInterdictObject.EducationGradeId.ToString());
                }
            }

            formulareq.VariableList.Add("OrderChildCount", Convert.ToDouble(lastInterdictObject.ChildCount));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add(" تعداد اولاد در حکم ", lastInterdictObject.ChildCount.ToString());
            }


            formulareq.VariableList.Add("OrderWifeCount", Convert.ToDouble(lastInterdictObject.WifeCount));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add(" تعداد همسر در حکم ", lastInterdictObject.WifeCount.ToString());
            }


            formulareq.VariableList.Add("OrderSponsorshipCount", Convert.ToDouble(lastInterdictObject.SponsorshipCount));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add(" تعداد افراد تحت تکفل در حکم ", lastInterdictObject.WifeCount.ToString());
            }

        }

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: Querying Employee Education - EmployeeId: {req.EmployeeId}");
        }

        var currentEmployeeEducationInfos = _employeeContext.Educations.Where(i => i.EmployeeId == req.EmployeeId && i.IsDeleted != true);

        if (req.BuildTreeTrace == true)
        {
            var educationCount = currentEmployeeEducationInfos.Count();
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: Found {educationCount} education records in Educations table");
        }

        if (currentEmployeeEducationInfos != null)
        {
            if (currentEmployeeEducationInfos.Any())
            {
                if (currentEmployeeEducationInfos.Any(i => i.IsDefaultEducation == true))
                {
                    var defaultEducationCount = currentEmployeeEducationInfos.Count(i => i.IsDefaultEducation == true);
                    if (req.BuildTreeTrace == true)
                    {
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: Found {defaultEducationCount} default education records");
                    }

                    if (defaultEducationCount == 1)
                    {
                        var defultEdu = currentEmployeeEducationInfos.Single(i => i.IsDefaultEducation == true);

                        if (req.BuildTreeTrace == true)
                        {
                            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: Retrieved default education - EducationGradeId: {defultEdu.EducationGradeId}");
                        }

                        formulareq.VariableList.Add("EmployeeEducationGradeId", Convert.ToDouble(defultEdu.EducationGradeId));
                        if (req.BuildTreeTrace == true)
                        {
                            formulareq.VariableFriendlyList.Add(" مقطع تحصیلی در پرونده ", defultEdu.EducationGradeId.ToString());
                        }
                    }
                    else if (req.BuildTreeTrace == true)
                    {
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: WARNING - Multiple default education records found: {defaultEducationCount}");
                    }
                }
                else if (req.BuildTreeTrace == true)
                {
                    logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: No default education record found");
                }
            }
            else if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 15: No education records found for employee");
            }
        }

        if (req.OrderTypeId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 16: Processing OrderType - OrderTypeId: {req.OrderTypeId}");
            }

            formulareq.VariableList.Add("OrderTypeId", Convert.ToDouble(req.OrderTypeId));

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 16: Querying OrderType table for OrderTypeId: {req.OrderTypeId}");
                var OrderType = _orderTypeService.GetIdAsync(req.OrderTypeId).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 16: Retrieved OrderType: {OrderType}");
                formulareq.VariableFriendlyList.Add("نوع حکم", OrderType);
            }

        }
        if (req.EmployeeStatusId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 17: Processing EmployeeStatus - EmployeeStatusId: {req.EmployeeStatusId}");
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 17: Querying EmployeeStatus table for EmployeeStatusId: {req.EmployeeStatusId}");
                var EmployeeStatus = _employeeStatusService.GetIdAsync(req.EmployeeStatusId.Value).Result.title;
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 17: Retrieved EmployeeStatus: {EmployeeStatus}");
                formulareq.VariableFriendlyList.Add("وضعیت استخدامی", EmployeeStatus);
            }
            formulareq.VariableList.Add("EmployeeStatusId", Convert.ToDouble(req.EmployeeStatusId));
        }
        if (req.OrganisationPositionId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 18: Processing OrganisationPosition - OrganisationPositionId: {req.OrganisationPositionId}");
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 18: Querying OrganisationPosition table");
            }

            var organisationPosition = _organisationPositionService.All().Single(i => i.Id == req.OrganisationPositionId);

            if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 18: Retrieved OrganisationPosition - PositionCode: {organisationPosition.PositionCode}");
            }

            if (!string.IsNullOrEmpty(organisationPosition.PositionCode))
            {
                formulareq.VariableList.Add("PositionCode", Convert.ToDouble(organisationPosition.PositionCode));

                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("کد پست سازمانی", organisationPosition.PositionCode);
                }
            }
            formulareq.VariableList.Add("OrganisationPositionId", Convert.ToDouble(req.OrganisationPositionId));
            formulareq.VariableList.Add("OrganisationPositionDetailId", Convert.ToDouble(req.OrganisationPositionId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه پست سازمانی ", req.OrganisationPositionId.ToString());
            }
        }

        formulareq.StartDate = req.StartDate;

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 19: Setting StartDate: {req.StartDate}");
            formulareq.VariableFriendlyList.Add("تاریخ اجرا", req.StartDate.ToString());
        }

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: CRITICAL CHECK - Processing WageItems");
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: req.WageItems is null: {req.WageItems == null}");
            if (req.WageItems != null)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: WageItems count: {req.WageItems.Count()}");
            }
        }

        if (req.WageItems != null)
        {
            if (req.WageItems.Any())
            {
                if (req.BuildTreeTrace == true)
                {
                    logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: Processing {req.WageItems.Count()} WageItems");
                }

                Dictionary<long, string>? wageTitleCache = req.BuildTreeTrace == true ? new Dictionary<long, string>() : null;
                var wageSeparator = new System.Globalization.NumberFormatInfo()
                {
                    NumberDecimalDigits = 0,
                    NumberGroupSeparator = ","
                };

                foreach (var item in req.WageItems)
                {
                    formulareq.VariableList.Add("wf_" + item.WageItemId, Convert.ToDouble(item.Value));

                    if (req.BuildTreeTrace == true)
                    {
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: Processing WageItem - WageItemId: {item.WageItemId}, Value: {item.Value}");
                        if (!wageTitleCache!.TryGetValue(item.WageItemId, out var wage))
                        {
                            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: Querying WageItem table for WageItemId: {item.WageItemId}");
                            wage = _wageItemService.GetIdAsync(item.WageItemId).Result.title;
                            wageTitleCache[item.WageItemId] = wage;
                        }
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: Retrieved WageItem title: {wage}");
                        formulareq.VariableFriendlyList.Add(wage, item.Value.ToString("N", wageSeparator));
                    }
                }
            }
            else if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: WageItems is not null but has no items (empty collection)");
            }
        }
        else if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 20: CRITICAL ISSUE - WageItems is NULL - This should not happen!");
        }

        if (req.CoeficentItems != null)
        {
            if (req.CoeficentItems.Any())
            {
                if (req.BuildTreeTrace == true)
                {
                    logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: Processing {req.CoeficentItems.Count()} CoeficentItems");
                }

                Dictionary<long, string>? coefficientTitleCache = req.BuildTreeTrace == true ? new Dictionary<long, string>() : null;
                var coefSeparator = new System.Globalization.NumberFormatInfo()
                {
                    NumberDecimalDigits = 3,
                    NumberGroupSeparator = ","
                };

                foreach (var item in req.CoeficentItems)
                {
                    formulareq.VariableList.Add("cof_" + item.CoefficientId, Convert.ToDouble(item.Value));
                    if (req.BuildTreeTrace == true)
                    {
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: Processing CoeficentItem - CoefficientId: {item.CoefficientId}, Value: {item.Value}");
                        if (!coefficientTitleCache!.TryGetValue(item.CoefficientId, out var coefficientTitle))
                        {
                            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: Querying Coefficient table for CoefficientId: {item.CoefficientId}");
                            coefficientTitle = _coefficientService.GetIdAsync(item.CoefficientId).Result.title;
                            coefficientTitleCache[item.CoefficientId] = coefficientTitle;
                        }
                        logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: Retrieved Coefficient title: {coefficientTitle}");
                        formulareq.VariableFriendlyList.Add(coefficientTitle, item.Value.ToString("N", coefSeparator));
                    }

                }
            }
            else if (req.BuildTreeTrace == true)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: CoeficentItems is not null but has no items (empty collection)");
            }
        }
        else if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 21: CoeficentItems is NULL");
        }

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 22: Final preparation - Setting BuildTreeTrace: {req.BuildTreeTrace}");
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 22: Total variables in VariableList: {formulareq.VariableList.Count}");
            if (formulareq.VariableFriendlyList != null)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 22: Total variables in VariableFriendlyList: {formulareq.VariableFriendlyList.Count}");
            }
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 23: Calling FormulaService.Calculate with OrganisationFormulaId: {OrganisationFormulaId}");
        }

        formulareq.BuildTreeTrace = req.BuildTreeTrace;
        var result = _formulaService.Calculate(formulareq);

        if (req.BuildTreeTrace == true)
        {
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 24: Formula calculation completed - Success: {result.Succees}, Result: {result.Result}");
            if (!result.Succees)
            {
                logBuilder.AppendLine($"[CommunicateWithFormula] STEP 24: Formula calculation failed - Error: {result.ResponseMessage}");
            }
            logBuilder.AppendLine($"[CommunicateWithFormula] STEP 25: Method completed successfully");

            // Add debug log to result
            result.DebugLog = logBuilder.ToString();
        }

        return result;

    }
    /// <summary>
    /// بروز رسانی حکم از فرم مدیریتی
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult UpdateOrderStatus(long OrderId, long NewStatusId)
    {
        if (OrderId <= 0)
        {
            return OperationResult.Failed("شناسه حکم نامعتبر است");
        }

        if (NewStatusId <= 0)
        {
            return OperationResult.Failed("وضعیت انتخاب‌شده نامعتبر است");
        }

        try
        {
            _unitOfWork.CreateTransaction();
            var interdict = _unitOfWork.Context.Set<InterdictOrder>().Find(OrderId);
            if (interdict == null)
            {
                _unitOfWork.Rollback();
                return OperationResult.NotFound("حکم حقوقی یافت نشد");
            }

            interdict.StatusId = NewStatusId;
            _unitOfWork.Context.Update(interdict);
            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
            return OperationResult.Succeeded(msg: "وضعیت حکم با موفقیت بروزرسانی شد");
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public OperationResult RejectOrderAll()
    {
        try
        {
            IList<InterdictOrder> allPendingOrders = All()
                .Include(i => i.RecruitOrder)
                .Where(i => i.StatusId == (long)Enums.OrderStatus.Pending)
                .OrderBy(i => i.RecruitOrder!.EmployeeId)
                .ThenByDescending(i => i.Serial)
                .ToList();
            foreach (var item in allPendingOrders)
            {
                try
                {
                    var resp = RejectOrderSingle(item.Id);
                }
                catch (Exception ex)
                {
                }
            }
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطای سیستمی");
        }
    }
    public OperationResult GetOrderCartableDetail(long OrderId)
    {
        try
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[wf].[GetinterdictWorkFlowDetails]", con);
                cmd.Parameters.AddWithValue("@InterdictOrderId", OrderId);
                cmd.Parameters.AddWithValue("@UserId", _userService.GetUserId());
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                List<GetinterdictWorkFlowDetails_Result> ret = new();
                int index = 1;
                while (rdr.Read())
                {
                    var row = rdr.ConvertToObject<GetinterdictWorkFlowDetails_Result>();
                    row.Index = index;
                    ret.Add(row);
                    index++;
                }
                con.Close();
                return OperationResult.Succeeded(payload: ret);
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "GetOrderCartableDetail failed for OrderId={OrderId}", OrderId);
            return OperationResult.Failed($"خطا در دریافت جزئیات گردش کار: {ex.Message}");
        }
    }

    public OperationResult ApplyRejectToPendingOrder(InterdictOrder? interdict, DbContext db)
    {
        if (interdict == null)
        {
            return OperationResult.NotFound("حکم مورد نظر یافت نشد");
        }

        var fullInterdict = GetInterdictOrderWithRecruit(interdict.Id, db);
        if (fullInterdict == null)
        {
            return OperationResult.NotFound("حکم مورد نظر یافت نشد");
        }

        if (fullInterdict.StatusId != (long)Enums.OrderStatus.Pending)
        {
            return OperationResult.Failed("وضعیت حکم نا معتبر است");
        }

        if (fullInterdict.RecruitOrder == null)
        {
            return OperationResult.Failed("اطلاعات استخدامی حکم مورد نظر یافت نشد");
        }

        var employeeId = fullInterdict.RecruitOrder.EmployeeId;
        if (OrderSerialSequenceHelper.HasLargerPendingOrder(employeeId, (short)(fullInterdict.Serial ?? 0), fullInterdict.Id, db))
        {
            return OperationResult.Failed(OrderSerialSequenceHelper.RejectOutOfOrderMessage);
        }

        fullInterdict.StatusId = (long)Enums.OrderStatus.RejectedOrder;
        return OperationResult.Succeeded();
    }

    public OperationResult RejectOrderSingle(long OrderId)
    {
        try
        {
            _unitOfWork.CreateTransaction();
            var interdict = GetInterdictOrderWithRecruit(OrderId, _unitOfWork.Context);
            var rejectResult = ApplyRejectToPendingOrder(interdict, _unitOfWork.Context);
            if (!rejectResult.Success)
            {
                _unitOfWork.Rollback();
                return rejectResult;
            }

            _db.Set<InterdictOrder>().Update(interdict!);
            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed("خطای سیستمی");
        }
        return OperationResult.Succeeded();

    }


    #region IssueOrder

    /// <summary>
    /// در صدور گروهی (Batch) مقادیر نوع/وضعیت استخدام از حکم قبلی کپی می‌شوند؛
    /// مشابه app-issue-order پیش‌فرض‌های CanChange را روی درخواست اعمال می‌کند.
    /// </summary>
    private void ApplyBatchCanChangeDefaults(IssueOrderRequest req)
    {
        if (req.IsBatch != true || !req.StartDate.HasValue)
        {
            return;
        }

        var sourceEmployeeTypeId = req.EmployeeTypeId;
        var canChangeSettings = _organisationEmployeeTypeOrderTypeCanChangeService
            .All(ImpleDate: req.StartDate)
            .Where(i => i.OrganisationChartId == req.PayLocationId
                        && i.EmployeeTypeId == sourceEmployeeTypeId
                        && i.OrderTypeId == req.OrderTypeId)
            .ToList();

        if (canChangeSettings.Count != 1)
        {
            return;
        }

        var canChange = canChangeSettings[0];

        if (canChange.CanEmployeeTypeId
            && canChange.DefaultEmpTypeId.HasValue
            && canChange.DefaultEmpTypeId.Value > 0)
        {
            req.EmployeeTypeId = canChange.DefaultEmpTypeId.Value;
            if (req.InterdictOrderDTO != null)
            {
                req.InterdictOrderDTO.EmployeeTypeId = canChange.DefaultEmpTypeId.Value;
            }

            _logger.LogInformation(
                "صدور گروهی: اعمال نوع استخدام پیش‌فرض {DefaultEmpTypeId} به‌جای {PreviousEmployeeTypeId} برای EmployeeId={EmployeeId}",
                canChange.DefaultEmpTypeId.Value, sourceEmployeeTypeId, req.EmployeeId);
        }

        if (canChange.CanEmployeeStatusId
            && canChange.DefaultEmpStatusId.HasValue
            && canChange.DefaultEmpStatusId.Value > 0)
        {
            req.EmployeeStatusId = canChange.DefaultEmpStatusId.Value;
            if (req.InterdictOrderDTO != null)
            {
                req.InterdictOrderDTO.EmployeeStatusId = canChange.DefaultEmpStatusId.Value;
            }

            _logger.LogInformation(
                "صدور گروهی: اعمال وضعیت استخدام پیش‌فرض {DefaultEmpStatusId} برای EmployeeId={EmployeeId}",
                canChange.DefaultEmpStatusId.Value, req.EmployeeId);
        }
    }

    public OperationResult IssueOrder(IssueOrderRequest req)
    {
        #region Validation & Initial Setup

        _logger.LogInformation("شروع صدور حکم جدید. EmployeeId={EmployeeId}, EmployeeTypeId={EmployeeTypeId}, OrderTypeId={OrderTypeId}, IssueTypeId={IssueTypeId}",
            req.EmployeeId, req.EmployeeTypeId, req.OrderTypeId, req.IssueTypeId);

        req.StartDate = req.ImpleDate;
        var validationresponse = ValidateIssue(req);

        if (!validationresponse.Item1)
        {
            _logger.LogWarning("اعتبارسنجی صدور حکم ناموفق بود. EmployeeId={EmployeeId}, Message={Message}", req.EmployeeId, validationresponse.Item2);
            return OperationResult.Failed(validationresponse.Item2);
        }

        _logger.LogInformation("اعتبارسنجی صدور حکم با موفقیت انجام شد. EmployeeId={EmployeeId}", req.EmployeeId);

        ApplyBatchCanChangeDefaults(req);

        #endregion Validation & Initial Setup

        try
        {
            #region Recalculate Wage & Coefficient Items

            var LastInterdictID = GetLastOrderByImpleDate(req.EmployeeId, req.StartDate.Value, req.CorrectionOrderId, req.PayLocationId);

            InterdictOrder newinterdict = new();
            InterdictOrder LastInterdict = new();
            if (LastInterdictID > 0)
            {
                LastInterdict = All(false).Include(i => i.RecruitOrder).Single(i => i.Id == LastInterdictID);
                if (req.InterdictOrderDTO == null)
                {
                    req.InterdictOrderDTO = GetOrderFlat(LastInterdictID).Payload;
                }
                req.lastorderId = LastInterdictID;
            }

            var clientSubmittedWagesById = (req.WageItems ?? new List<Core.DTOs.WageItem>())
                .GroupBy(w => w.WageItemId)
                .ToDictionary(g => g.Key, g => g.First());
            var clientSubmittedCoefsById = (req.CoeficentItems ?? new List<coeficentItem>())
                .GroupBy(c => c.CoefficientId)
                .ToDictionary(g => g.Key, g => g.First());

            var wageExcelByItemId = BuildBatchExcelOverrideMap(
                req.wageOverRideExcel,
                req.EmployeeId,
                req.InterdictOrderDTO?.NationalNo);
            var coefExcelByItemId = BuildBatchExcelOverrideMap(
                req.coefOverRideExcel,
                req.EmployeeId,
                req.InterdictOrderDTO?.NationalNo);

            req.DoFinalCalc = true;

            var typesResp = SetOrderItemsGrid(req);

            if (typesResp.Success)
            {
                req.WageItems = new List<Core.DTOs.WageItem>();
                req.CoeficentItems = new List<coeficentItem>();
                var orderitemsGridResp = (ConsequenceRespone)typesResp.Payload;
                if (orderitemsGridResp.OrderWageSettingList != null)
                {
                    if (orderitemsGridResp.OrderWageSettingList.Any())
                    {
                        foreach (var item in orderitemsGridResp.OrderWageSettingList)
                        {
                            if (item.IsRowSuccess)
                            {

                            }
                            else
                            {
                                return OperationResult.Failed((item.FormulaErrorMessage == null ? "" : item.FormulaErrorMessage) + (item.CheckFormulaErrorMessage == null ? "" : item.CheckFormulaErrorMessage) + "Wage IsRowSuccess Fail for Id :" + item.WageItemId);
                            }
                            var wageValue = ResolveIssueOrderPersistedWageValue(item, clientSubmittedWagesById, wageExcelByItemId);
                            req.WageItems.Add(new Core.DTOs.WageItem()
                            {
                                EnterTypeId = item.EnterTypeId,
                                Value = wageValue,
                                WageItemId = item.WageItemId,
                            });
                        }
                    }
                }

                if (orderitemsGridResp.OrderCoefficientSettingList != null)
                {
                    if (orderitemsGridResp.OrderCoefficientSettingList.Any())
                    {
                        foreach (var item in orderitemsGridResp.OrderCoefficientSettingList)
                        {
                            if (item.IsRowSuccess)
                            {

                            }
                            else
                            {
                                return OperationResult.Failed((item.FormulaErrorMessage == null ? "" : item.FormulaErrorMessage) + (item.CheckFormulaErrorMessage == null ? "" : item.CheckFormulaErrorMessage));
                            }

                            var coefValue = ResolveIssueOrderPersistedCoefficientValue(item, clientSubmittedCoefsById, coefExcelByItemId);
                            req.CoeficentItems.Add(new coeficentItem()
                            {
                                EnterTypeId = item.EnterTypeId,
                                Value = coefValue,
                                CoefficientId = item.CoefficientId,
                            });
                        }
                    }
                }
            }
            else
            {
                return OperationResult.Failed(typesResp.Message);
            }

            #endregion Recalculate Wage & Coefficient Items

            _unitOfWork.CreateTransaction();

            try
            {
                #region Transaction & Pre-Issue Validation

                // در حالت Batch یا پس‌زمینه HttpContext وجود ندارد و GetUserId() برابر -1 برمی‌گرداند؛ از req.UserId استفاده می‌کنیم.
                var currentUserIdForList = req.UserId > 0 ? req.UserId : _userService.GetUserId();
                var orderList = GetOrderList(new GetOrderListByEmployeeIdRequest()
                {
                    EmployeeId = req.EmployeeId,
                    EmployeeTypeId = req.EmployeeTypeId,
                    CurrentUserId = currentUserIdForList,
                    PageNo = 0,
                    PageSize = 1500,
                    PayLocationId = _currentUserDefaultOrganId,
                    SortColumn = "",
                    SortOrder = "",
                });

                if (orderList != null)
                {
                    if (orderList.Any())
                    {
                        List<long> validstatesforDuplicateCheck = [
                                 (long)Enums.OrderStatus.Pending,
                            (long)Enums.OrderStatus.FinalAprove,
                            (long)Enums.OrderStatus.Draft,
                            (long)Enums.OrderStatus.RejectedOrder,
                        ];

                        if (orderList.Any(i => i.CorrigendumRecInterdictID == req.CorrectionOrderId && req.CorrectionOrderId.HasValue && validstatesforDuplicateCheck.Contains(i.StatusId)))
                        {
                            _unitOfWork.Rollback();
                            _logger.LogWarning("اصلاحیه تعیین‌تکلیف‌نشده برای حکم وجود دارد. EmployeeId={EmployeeId}, CorrectionOrderId={CorrectionOrderId}",
                                req.EmployeeId, req.CorrectionOrderId);
                            return OperationResult.Failed("برای حکم مورد نظر یک اصلاحیه تعیین تکلیف نشده وجود دارد، لطفا ابتدا آن را تعیین تکلیف بفرمایید.");
                        }

                        if (orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.Draft) || orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.RejectedOrder))
                        {
                            _unitOfWork.Rollback();
                            _logger.LogWarning("وجود احکام در وضعیت پیش‌نویس یا ردشده قبل از صدور حکم جدید. EmployeeId={EmployeeId}", req.EmployeeId);
                            return OperationResult.Failed("برخی از احکام قبلی هنوز در وضعیت پیش‌نویس یا رد شده هستند، لطفا ابتدا وضعیت آن‌ها را تعیین کنید.");
                        }
                    }
                }
                if (req.IssueTypeId == (long)Enums.IssueType.Correction)
                {
                    if (req.CorrectionOrderId > 0)
                    {

                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        _logger.LogWarning("درخواست اصلاح حکم بدون تعیین CorrectionOrderId. EmployeeId={EmployeeId}", req.EmployeeId);
                        return OperationResult.Failed("حکمی که قصد اصلاح آن را دارید مشخص نشده است.");
                    }
                }

                #endregion Transaction & Pre-Issue Validation

                #region Build InterdictOrder Entity

                if (req.IssueTypeId == (long)Enums.IssueType.Cancelation)
                {
                    if (req.CorrectionOrderId > 0)
                    {
                        #region CopyOrder
                        newinterdict.OrderTypeId = LastInterdict.OrderTypeId;
                        newinterdict.Code = LastInterdict.Code;
                        newinterdict.SumWageFactors = LastInterdict.SumWageFactors;
                        newinterdict.Serial = LastInterdict.Serial;
                        newinterdict.CreatorUserName = LastInterdict.CreatorUserName;
                        newinterdict.OrderTypeId = LastInterdict.OrderTypeId;
                        newinterdict.StatusId = LastInterdict.StatusId;
                        newinterdict.AspNetUsersId = LastInterdict.AspNetUsersId;
                        newinterdict.Description = LastInterdict.Description;
                        newinterdict.LastInterdictOrderId = LastInterdict.LastInterdictOrderId;
                        newinterdict.CorrectedInterdictOrderId = LastInterdict.CorrectedInterdictOrderId;
                        _db.Entry(newinterdict).Property("IssueTypeId").CurrentValue = LastInterdict.IssueTypeId;
                        newinterdict.EducatioFieldId = LastInterdict.EducatioFieldId;
                        newinterdict.EducatioOrientationId = LastInterdict.EducatioOrientationId;
                        newinterdict.BirthDate = LastInterdict.BirthDate;
                        newinterdict.EmpEduID = LastInterdict.EmpEduID;
                        newinterdict.BirthPlaceId = LastInterdict.BirthPlaceId;
                        newinterdict.DrivingLicenseNumber = LastInterdict.DrivingLicenseNumber;
                        newinterdict.IssuePlaceId = LastInterdict.IssuePlaceId;
                        newinterdict.ExperienceRecorded = LastInterdict.ExperienceRecorded;
                        newinterdict.RetiredRecorded = LastInterdict.RetiredRecorded;
                        newinterdict.YearRecorded = LastInterdict.YearRecorded;
                        newinterdict.HistoryOut = LastInterdict.HistoryOut;
                        newinterdict.HistoryStop = LastInterdict.HistoryStop;
                        newinterdict.RetiredFlagOk = LastInterdict.RetiredFlagOk;
                        _db.Entry(newinterdict).Property("MarriageStatusId").CurrentValue = LastInterdict.MarriageStatusId;
                        newinterdict.SponsorshipCount = LastInterdict.SponsorshipCount;
                        newinterdict.YearCoefficient = LastInterdict.YearCoefficient;
                        newinterdict.EducationGradeId = LastInterdict.EducationGradeId;
                        newinterdict.EffectiveEducationGradeId = LastInterdict.EffectiveEducationGradeId;
                        newinterdict.IsWar = LastInterdict.IsWar;
                        newinterdict.IsCaptivity = LastInterdict.IsCaptivity;
                        newinterdict.IsBasij = LastInterdict.IsBasij;
                        newinterdict.IsIsar = LastInterdict.IsIsar;
                        newinterdict.IsarPercent = LastInterdict.IsarPercent;
                        newinterdict.WarDuration = LastInterdict.WarDuration;
                        newinterdict.CaptivityDuration = LastInterdict.CaptivityDuration;
                        newinterdict.BasijDuration = LastInterdict.BasijDuration;
                        newinterdict.JobDegree = LastInterdict.JobDegree;
                        newinterdict.IsMartyrs = LastInterdict.IsMartyrs;
                        newinterdict.WifeCount = LastInterdict.WifeCount;
                        newinterdict.GradScore = LastInterdict.GradScore;
                        newinterdict.EmployeeDate = LastInterdict.EmployeeDate;
                        newinterdict.ApproverSignatureGuid = LastInterdict.ApproverSignatureGuid;
                        _db.Entry(newinterdict).Property("InsuranceTypeId").CurrentValue = LastInterdict.InsuranceTypeId;
                        newinterdict.AccountNumber = LastInterdict.AccountNumber;
                        newinterdict.OtherVeterans = LastInterdict.OtherVeterans;
                        newinterdict.ApproverSignatureDate = LastInterdict.ApproverSignatureDate;
                        newinterdict.IsWomenHead = LastInterdict.IsWomenHead;
                        newinterdict.FirstName = LastInterdict.FirstName;
                        newinterdict.LastName = LastInterdict.LastName;
                        newinterdict.FatherName = LastInterdict.FatherName;
                        newinterdict.PersonelCode = LastInterdict.PersonelCode;
                        newinterdict.IdentityNo = LastInterdict.IdentityNo;
                        newinterdict.NationalNo = LastInterdict.NationalNo;
                        newinterdict.DrivingLicenseTypeId = LastInterdict.DrivingLicenseTypeId;
                        newinterdict.ChildCount = LastInterdict.ChildCount;
                        #endregion CopyOrder
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        _logger.LogWarning("درخواست ابطال حکم بدون تعیین CorrectionOrderId. EmployeeId={EmployeeId}", req.EmployeeId);
                        return OperationResult.Failed("حکمی که قصد ابطال آن را دارید مشخص نشده است.");
                    }
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("[Order].[GetOrderSummary]", con);
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.BigInt).Value = req.EmployeeId;
                        cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = req.EmployeeTypeId;
                        cmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = req.OrderTypeId;
                        cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = (object?)req.ImpleDate?.Date ?? DBNull.Value;
                        cmd.Parameters.Add("@LastInterdictID", SqlDbType.BigInt).Value = LastInterdictID;
                        cmd.Parameters.Add("@OrganisationChartId", SqlDbType.BigInt).Value = req.PayLocationId;
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            newinterdict = rdr.ConvertToObject<InterdictOrder>();
                            break;

                        }
                        con.Close();
                    }
                }

                if (LastInterdictID > 0)
                {
                    newinterdict.LastInterdictOrderId = LastInterdictID;
                }
                if (req.CorrectionOrderId > 0)
                {
                    newinterdict.CorrectedInterdictOrderId = req.CorrectionOrderId;
                }

                #endregion Build InterdictOrder Entity

                #region Resolve Order Level

                var OrderTypeSetting = _organisationEmployeeTypeOrderTypeService.All(ImpleDate: req.StartDate).Where(i => i.OrganisationChartId == req.PayLocationId
                && i.EmployeeTypeId == req.EmployeeTypeId &&
                i.OrderTypeId == req.OrderTypeId

                ).ToList();


                if (OrderTypeSetting == null)
                {
                    _unitOfWork.Rollback();
                    _logger.LogWarning("تنظیمات نوع حکم برای این نوع استخدام یافت نشد. EmployeeId={EmployeeId}, EmployeeTypeId={EmployeeTypeId}, OrderTypeId={OrderTypeId}, PayLocationId={PayLocationId}",
                        req.EmployeeId, req.EmployeeTypeId, req.OrderTypeId, req.PayLocationId);
                    return OperationResult.Failed("تنظیمات مربوط به نوع حکم برای این نوع استخدام یافت نشد.");
                }
                else
                {
                    if (OrderTypeSetting.Any())
                    {
                        var singleRecord = OrderTypeSetting.Single();
                        if (singleRecord.OrderLevelTypeId == (long)Enums.OrderLevel.Recruit)
                        {
                            req.OrderLevelId = (long)Enums.OrderLevel.Recruit;
                        }
                        else
                        {
                            if (req.IsOutDate || req.IssueTypeId == (int)IssueType.Cancelation || req.IssueTypeId == (int)IssueType.Correction)
                            {
                                req.OrderLevelId = (long)Enums.OrderLevel.Recruit;
                            }
                            if (orderList != null)
                            {
                                if (orderList.Any())
                                {
                                    if (orderList.Any(i => i.StartDate > req.StartDate))
                                    {
                                        req.OrderLevelId = (long)Enums.OrderLevel.Recruit;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("تنظیمات حکم در این نوع استخدام یافت نشد");
                    }
                }

                #endregion Resolve Order Level

                #region Create RecruitOrder

                var newrecruit = new RecruitOrder();
                if (req.IssueTypeId == (int)IssueType.Cancelation)
                {
                    if (LastInterdict?.RecruitOrder == null)
                    {
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("اطلاعات استخدامی حکم ماقبل برای ابطال یافت نشد.");
                    }
                    newrecruit.CostCenterId = LastInterdict.RecruitOrder.CostCenterId;
                    newrecruit.EmployeeId = LastInterdict.RecruitOrder.EmployeeId;
                    newrecruit.EmployeeStatusId = LastInterdict.RecruitOrder.EmployeeStatusId;
                    newrecruit.EmployeeTypeId = LastInterdict.RecruitOrder.EmployeeTypeId;
                    newrecruit.OrganizationUnitId = LastInterdict.RecruitOrder.OrganizationUnitId;
                    newrecruit.OrganizationJobId = LastInterdict.RecruitOrder.OrganizationJobId;
                    newrecruit.OrganisationPositionId = LastInterdict.RecruitOrder.OrganisationPositionId;
                    newrecruit.PayLocationId = LastInterdict.RecruitOrder.PayLocationId;
                    newrecruit.WorkPlaceId = LastInterdict.RecruitOrder.WorkPlaceId;
                    newrecruit.ProjectId = LastInterdict.RecruitOrder.ProjectId;
                    newrecruit.IPAddress = "";
                    newrecruit.CreateDate = DateTime.Now;
                    _unitOfWork.Context.Set<RecruitOrder>().Add(newrecruit);
                    _unitOfWork.Context.SaveChanges();
                    newinterdict.RecruitOrderId = newrecruit.Id;
                }
                else
                {
                    if (req.OrderLevelId == (long)Enums.OrderLevel.Recruit)
                    {
                        // تنظیمات CanChange بر اساس نوع استخدام حکم قبلی است؛ نه نوع پیش‌فرض اعمال‌شده روی درخواست.
                        var canChangeLookupEmployeeTypeId = LastInterdictID > 0 && LastInterdict?.RecruitOrder != null
                            ? LastInterdict.RecruitOrder.EmployeeTypeId
                            : req.EmployeeTypeId;

                        var CanChange = _organisationEmployeeTypeOrderTypeCanChangeService.All(ImpleDate: req.StartDate)
                              .Where(i => i.OrganisationChartId == req.PayLocationId &&
                              i.EmployeeTypeId == canChangeLookupEmployeeTypeId &&
                              i.OrderTypeId == req.OrderTypeId

                              ).ToList();
                        if (CanChange != null)
                        {
                            if (CanChange.Any())
                            {
                                if (CanChange.Count() == 1)
                                {
                                    var canChangeSetting = CanChange.SingleOrDefault();
                                    newrecruit.EmployeeId = req.EmployeeId;


                                    if (LastInterdictID > 0)
                                    {
                                        if (canChangeSetting.CanEmployeeTypeId)
                                        {
                                            newrecruit.EmployeeTypeId = req.EmployeeTypeId;
                                        }
                                        else
                                        {
                                            if (LastInterdict.RecruitOrder == null)
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("نوع استخدام بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                            }
                                            else
                                            {
                                                newrecruit.EmployeeTypeId = LastInterdict.RecruitOrder.EmployeeTypeId;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        newrecruit.EmployeeTypeId = req.EmployeeTypeId;
                                    }


                                    if (LastInterdictID > 0)
                                    {
                                        if (canChangeSetting.CanCostCenterId)
                                        {
                                            if (req.CostCenterId.HasValue)
                                            {
                                                newrecruit.CostCenterId = req.CostCenterId.Value;
                                            }
                                            else
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("مرکز هزینه انتخاب نشده است");
                                            }
                                        }
                                        else
                                        {
                                            if (LastInterdict.RecruitOrder == null)
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("مرکز هزینه بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                            }
                                            else
                                            {
                                                newrecruit.CostCenterId = LastInterdict.RecruitOrder.CostCenterId;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (req.CostCenterId.HasValue)
                                        {
                                            newrecruit.CostCenterId = req.CostCenterId.Value;
                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed("مرکز هزینه انتخاب نشده است");
                                        }
                                    }



                                    if (LastInterdictID > 0)
                                    {

                                        if (canChangeSetting.CanEmployeeStatusId)
                                        {
                                            if (!req.EmployeeStatusId.HasValue)
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("وضعیت استخدام مشخص نشده است !");
                                            }
                                            newrecruit.EmployeeStatusId = req.EmployeeStatusId.Value;
                                        }
                                        else
                                        {
                                            if (LastInterdict.RecruitOrder == null)
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("وضعیت استخدام بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                            }
                                            else
                                            {
                                                newrecruit.EmployeeStatusId = LastInterdict.RecruitOrder.EmployeeStatusId;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (req.EmployeeStatusId.HasValue)
                                        {
                                            newrecruit.EmployeeStatusId = req.EmployeeStatusId.Value;
                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed("وضعیت استخدام مشخص نشده است !");
                                        }
                                    }

                                    if (canChangeSetting.CanOrganizationUnitId)
                                    {
                                        newrecruit.OrganizationUnitId = req.OrganizationUnitId;
                                    }
                                    else
                                    {
                                        if (LastInterdict.RecruitOrder == null)
                                        {
                                            //_unitOfWork.Rollback();
                                            //return OperationResult.Failed("واحد سازمانی بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                        }
                                        else
                                        {
                                            newrecruit.OrganizationUnitId = LastInterdict.RecruitOrder.OrganizationUnitId;
                                        }
                                    }

                                    if (canChangeSetting.CanJobID)
                                    {
                                        newrecruit.OrganizationJobId = req.OrganizationJobId;
                                    }
                                    else
                                    {
                                        if (LastInterdict.RecruitOrder == null)
                                        {
                                            //_unitOfWork.Rollback();
                                            //return OperationResult.Failed("شغل بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                        }
                                        else
                                        {
                                            newrecruit.OrganizationJobId = LastInterdict.RecruitOrder.OrganizationJobId;
                                        }
                                    }


                                    if (canChangeSetting.CanPositionId)
                                    {
                                        newrecruit.OrganisationPositionId = req.OrganisationPositionId;
                                    }
                                    else
                                    {
                                        if (LastInterdict.RecruitOrder == null)
                                        {
                                            //    _unitOfWork.Rollback();
                                            //    return OperationResult.Failed("پست سازمانی بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                        }
                                        else
                                        {
                                            newrecruit.OrganisationPositionId = LastInterdict.RecruitOrder.OrganisationPositionId;
                                        }
                                    }

                                    if (canChangeSetting.CanPayLocationId)
                                    {
                                        newrecruit.PayLocationId = req.PayLocationId;
                                    }
                                    else
                                    {
                                        if (LastInterdict.RecruitOrder == null)
                                        {

                                        }
                                        else
                                        {
                                            newrecruit.PayLocationId = LastInterdict.RecruitOrder.PayLocationId;
                                        }
                                    }
                                    if (LastInterdictID > 0)
                                    {
                                        if (canChangeSetting.CanWorkPlaceId)
                                        {
                                            newrecruit.WorkPlaceId = req.WorkPlaceId;
                                        }
                                        else
                                        {
                                            if (LastInterdict.RecruitOrder == null)
                                            {
                                                _unitOfWork.Rollback();
                                                return OperationResult.Failed("محل خدمت بر اساس حکم ما قبل است و حکم ما قبلی وجود ندارد");
                                            }
                                            else
                                            {
                                                newrecruit.WorkPlaceId = LastInterdict.RecruitOrder.WorkPlaceId;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        newrecruit.WorkPlaceId = req.WorkPlaceId;
                                    }



                                    if (canChangeSetting.CanProjectId)
                                    {
                                        newrecruit.ProjectId = req.ProjectId;
                                    }
                                    else
                                    {
                                        if (LastInterdict.RecruitOrder == null)
                                        {
                                            //_unitOfWork.Rollback();
                                            //return OperationResult.Failed("پروژه بر اساس حکم ماقل است و حکم ما قبلی وجود ندارد");
                                        }
                                        else
                                        {
                                            newrecruit.ProjectId = LastInterdict.RecruitOrder.ProjectId;
                                        }
                                    }

                                    newrecruit.CreateDate = DateTime.Now;
                                    newrecruit.IPAddress = "";

                                    if (newrecruit.PayLocationId > 0 )
                                    {
                                        
                                    }
                                    else
                                    {
                                        _unitOfWork.Rollback();
                                        return OperationResult.Failed("محل پرداخت معتبر ارسال نشده است");

                                    }

                                    _unitOfWork.Context.Set<RecruitOrder>().Add(newrecruit);
                                    _unitOfWork.Context.SaveChanges();
                                    newinterdict.RecruitOrderId = newrecruit.Id;
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed("تنظیمات مشخصات استخدامی قابل تغییر یافت نشد");
                                }
                            }
                        }
                        else
                        {
                            if (orderList != null)
                            {
                                if (orderList.Any())
                                {
                                    newrecruit.Id = orderList.Single(i => i.Id == LastInterdictID).RecruitOrderId;
                                    newinterdict.RecruitOrderId = newrecruit.Id;
                                }
                            }
                        }
                    }

                }

                #endregion Create RecruitOrder

                #region Wage Settings & Sum Calculation

                // یک‌بار از دیتابیس خوانده می‌شود (قبلاً داخل حلقه WageItems به ازای هر آیتم یک کوئری جداگانه اجرا می‌شد -> مشکل N+1).
                var currentEmpTypeDailySetting = _organisationEmployeeTypeWageItemService.All(ImpleDate: req.StartDate)
                    .Where(i => i.OrganisationChartId == req.PayLocationId
                        && i.EmployeeTypeId == req.EmployeeTypeId)
                    .ToList();
                if (!currentEmpTypeDailySetting.Any())
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("بر اساس تاریخ اجرای انتخابی تنظیمات عامل حقوقی برای این نوع استخدام یافت نشد");
                }
                decimal? SumWageFactors = 0;

                if (req.IssueTypeId == (int)IssueType.Cancelation)
                {
                    req.WageItems = new List<Core.DTOs.WageItem>();
                    req.CoeficentItems = new List<coeficentItem>();
                    var wageItems = _db.Set<InterdictOrderWageItem>().Where(i => i.InterdictOrderId == LastInterdictID);
                    if (wageItems != null)
                    {
                        if (wageItems.Any())
                        {
                            foreach (var wage in wageItems)
                            {
                                req.WageItems.Add(new Core.DTOs.WageItem()
                                {
                                    WageItemId = wage.WageItemId,
                                    Value = wage.Value,
                                    EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec
                                });
                            }
                        }
                    }
                    var coefficientItems = _db.Set<InterdictOrderCoefficientItem>().Where(i => i.InterdictOrderId == LastInterdictID);
                    if (coefficientItems != null)
                    {
                        if (coefficientItems.Any())
                        {
                            foreach (var Coefficient in coefficientItems)
                            {
                                req.CoeficentItems.Add(new coeficentItem()
                                {
                                    CoefficientId = Coefficient.CoefficientId,
                                    Value = Coefficient.OutPutFactValue.Value,
                                    EnterTypeId = (long)Enums.EnterTypeId.EqualTolastRec
                                });
                            }
                        }
                    }

                }


                if (req.WageItems != null)
                {
                    foreach (var WageItem in req.WageItems)
                    {
                        var relatedEmployeeTypeSetting = currentEmpTypeDailySetting.Single(i => i.OrganisationChartId == req.PayLocationId
                        && i.EmployeeTypeId == req.EmployeeTypeId && i.WageItemId == WageItem.WageItemId
                        );

                        if (relatedEmployeeTypeSetting.IsDaily == true)
                        {
                            SumWageFactors = SumWageFactors + (Math.Abs(WageItem.Value) * 30);
                        }
                        else
                        {
                            SumWageFactors = SumWageFactors + WageItem.Value;
                        }
                    }
                }
                //if (SumWageFactors < 0)
                //{
                //    SumWageFactors = SumWageFactors * -1;
                //}
                newinterdict.SumWageFactors = SumWageFactors;

                #endregion Wage Settings & Sum Calculation

                #region Set InterdictOrder Properties

                if (orderList == null)
                {
                    newinterdict.Serial = 1;
                }
                else
                {
                    if (orderList.Any(i => i.PaylocationID == req.PayLocationId))
                    {
                        newinterdict.Serial = Convert.ToInt16(orderList.Where(i => i.PaylocationID == req.PayLocationId).OrderBy(j => j.OrderSerial).Select(s => s.OrderSerial).Last().Value + 1);
                    }
                    else
                    {
                        newinterdict.Serial = 1;
                    }
                }
                newinterdict.Id = 0;
                _db.Entry(newinterdict).Property("IssueTypeId").CurrentValue = req.IssueTypeId;
                newinterdict.StatusId = req.StatusId;// (int)OrderStatus.Draft;
                newinterdict.OrderTypeId = req.OrderTypeId;
                newinterdict.CreatorUserName = "";
                newinterdict.IPAddress = "";
                newinterdict.AspNetUsersId = req.UserId;
                newinterdict.StartDate = req.StartDate.Value.Date;
                newinterdict.EndDate = req.EndDate;
                newinterdict.CreateDate = DateTime.Now;
                newinterdict.IsDeleted = false;
                newinterdict.OrderReason = req.OrderReason;
                // شرح حکم: در حالت Batch مشابه کامپوننت app-issue-order از توضیح پیش‌فرض نوع حکم (OrganisationEmployeeTypeOrderTypeDescription) استفاده می‌شود.
                newinterdict.Description = req.Description;
                if (req.IsBatch == true && string.IsNullOrWhiteSpace(newinterdict.Description))
                {
                    var descriptionList = _organisationEmployeeTypeOrderTypeDescriptionService.All(ImpleDate: req.StartDate)
                        .Where(i => i.OrganisationChartId == req.PayLocationId && i.EmployeeTypeId == req.EmployeeTypeId && i.OrderTypeId == req.OrderTypeId)
                        .ToList();
                    var defaultDesc = descriptionList.FirstOrDefault(i => i.IsDefault);
                    newinterdict.Description = defaultDesc?.Description;
                }
                newinterdict.UniqueId = Guid.NewGuid();

                if (req.IssueTypeId == (long)Enums.IssueType.Normal)
                {
                    newinterdict.CorrectedInterdictOrderId = null;
                }

                if (req.WageItems != null)
                {
                    if (req.WageItems.Any())
                    {
                        newinterdict.ItemCount = req.WageItems.Count();
                    }
                }
                newinterdict.PayRollApproveUser = null;
                newinterdict.PayRollAprove = false;
                newinterdict.PayRollAproveDate = null;
                newinterdict.PayRollRealExecuteDate = null;

                #endregion Set InterdictOrder Properties

                #region Job Qualification Checks
                // بررسی توانایی های فردی در شرایط احراز شغل

                #region Ability Qualification

                bool JobAbilityQualification = false;
                var JobAbilityQualificationSetting = GetSettingById(10010);

                if (JobAbilityQualificationSetting == "-1")
                {
                    JobAbilityQualification = Constants.JobAbilityQualification;
                }
                else
                {
                    if (JobAbilityQualificationSetting == "0")
                    {
                        JobAbilityQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobAbilityQualificationSetting))
                    {
                        JobAbilityQualification = false;
                    }
                    else
                    {
                        JobAbilityQualification = bool.Parse(JobAbilityQualificationSetting);
                    }
                }

                if (JobAbilityQualification)
                {
                    var JobAbilityQualificationList = _organizationJobAbilityQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobAbilityQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobAbilityQualificationList.Any())
                        {
                            var employeeAbilities = _abilityService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId).ToList();
                            if (employeeAbilities == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("توانایی های فردی با شخص متناظر مطابقت ندارد");
                            }
                            else
                            {
                                if (employeeAbilities.Any())
                                {
                                    foreach (var item in JobAbilityQualificationList)
                                    {
                                        if (employeeAbilities.Any(i => i.AbilityTypeId == item.AbilityTypeId && i.LevelTypeId >= item.LevelTypeId))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed("توانایی های فردی با شخص متناظر مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed("توانایی های فردی با شخص متناظر مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Ability Qualification

                /// بررسی شایستگی های فردی در شرایط احراز شغل

                #region Competency Qualification

                bool JobCompetencyQualification = false;
                var JobCompetencyQualificationSetting = GetSettingById(10011);

                if (JobCompetencyQualificationSetting == "-1")
                {
                    JobCompetencyQualification = Constants.JobCompetencyQualification;
                }
                else
                {
                    if (JobCompetencyQualificationSetting == "0")
                    {
                        JobCompetencyQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobCompetencyQualificationSetting))
                    {
                        JobCompetencyQualification = false;
                    }
                    else
                    {
                        JobCompetencyQualification = bool.Parse(JobCompetencyQualificationSetting);
                    }
                }

                if (JobCompetencyQualification)
                {
                    var JobCompetencyQualificationList = _organizationJobCompetencyQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobCompetencyQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobCompetencyQualificationList.Any())
                        {
                            var employeeCompetencies = _competencyService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId).ToList();
                            if (employeeCompetencies == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("شایستگی های فردی با شخص متناظر مطابقت ندارد");
                            }
                            else
                            {
                                if (employeeCompetencies.Any())
                                {
                                    foreach (var item in JobCompetencyQualificationList)
                                    {
                                        if (employeeCompetencies.Any(i => i.CompetencyTypeId == item.CompetencyTypeId && i.CompetencyLevelId >= item.CompetencyLevelId && i.Acceptable == true))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed("شایستگی های فردی با شخص متناظر مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed("شایستگی های فردی با شخص متناظر مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Competency Qualification

                ///بررسی رشته های تحصیلی در شرایط احراز شغل

                #region Education Field Qualification

                bool JobEducationFieldQualification = false;
                var JobEducationFieldQualificationSetting = GetSettingById(10012);

                if (JobEducationFieldQualificationSetting == "-1")
                {
                    JobEducationFieldQualification = Constants.JobEducationFieldQualification;
                }
                else
                {
                    if (JobEducationFieldQualificationSetting == "0")
                    {
                        JobEducationFieldQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobEducationFieldQualificationSetting))
                    {
                        JobEducationFieldQualification = false;
                    }
                    else
                    {
                        JobEducationFieldQualification = bool.Parse(JobEducationFieldQualificationSetting);
                    }
                }

                if (JobEducationFieldQualification)
                {
                    var JobEducationFieldQualificationList = _organizationJobEducationFieldQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobEducationFieldQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobEducationFieldQualificationList.Any())
                        {
                            if (JobEducationFieldQualificationList.Any(i => i.EducationFieldId == newinterdict.EducatioFieldId))
                            {

                            }
                            else
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed(" رشته تحصیلی حکم با شرایط احراز شغلی مطابقت ندارد");
                            }
                        }

                    }
                }
                #endregion Education Field Qualification

                /// بررسی مقطع تحصیلی در شرایط احراز شغل

                #region Education Grade Qualification

                bool JobEducationGradeQualification = false;
                var JobEducationGradeQualificationSetting = GetSettingById(10013);

                if (JobEducationGradeQualificationSetting == "-1")
                {
                    JobEducationGradeQualification = Constants.JobEducationGradeQualification;
                }
                else
                {
                    if (JobEducationGradeQualificationSetting == "0")
                    {
                        JobEducationGradeQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobEducationGradeQualificationSetting))
                    {
                        JobEducationGradeQualification = false;
                    }
                    else
                    {
                        JobEducationGradeQualification = bool.Parse(JobEducationGradeQualificationSetting);
                    }
                }

                if (JobEducationGradeQualification)
                {
                    var JobEducationGradeQualificationList = _organizationJobEducationGradeQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();
                    if (JobEducationGradeQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobEducationGradeQualificationList.Any())
                        {
                            if (JobEducationGradeQualificationList.Any(i => i.EducationGradeId == newinterdict.EducationGradeId))
                            {

                            }
                            else
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed(" مقطع تحصیلی حکم با شرایط احراز شغلی مطابقت ندارد");
                            }
                        }
                    }
                }
                #endregion Education Grade Qualification

                ///بررسی زبان های خارجی در شرایط احراز شغل

                #region Foreign Language Qualification

                bool JobForeignLanguageQualification = false;
                var JobForeignLanguageQualificationSetting = GetSettingById(10014);

                if (JobForeignLanguageQualificationSetting == "-1")
                {
                    JobForeignLanguageQualification = Constants.JobForeignLanguageQualification;
                }
                else
                {
                    if (JobForeignLanguageQualificationSetting == "0")
                    {
                        JobForeignLanguageQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobForeignLanguageQualificationSetting))
                    {
                        JobForeignLanguageQualification = false;
                    }
                    else
                    {
                        JobForeignLanguageQualification = bool.Parse(JobForeignLanguageQualificationSetting);
                    }
                }

                if (JobForeignLanguageQualification)
                {
                    var JobForeignLanguageQualificationList = _organizationJobForeignLanguageQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobForeignLanguageQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobForeignLanguageQualificationList.Any())
                        {
                            var employeeForeignLanguages = _ForeignLanguageService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId).ToList();
                            if (employeeForeignLanguages == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("زبان های خارجی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                            }
                            else
                            {
                                if (employeeForeignLanguages.Any())
                                {
                                    foreach (var item in JobForeignLanguageQualificationList)
                                    {
                                        if (employeeForeignLanguages.Any(i => i.LanguageskillId == item.LanguageSkillTypeId && i.LevelId >= item.LanguageLevelTypeId && i.LanguageId == item.LanguageTypeId && i.Acceptable == true))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed("زبان های خارجی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed("زبان های خارجی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Foreign Language Qualification

                /// بررسی دوره های آموزشی در شرایط احراز شغل

                #region Initial Course Qualification

                bool JobInitialCourseQualification = false;
                var JobInitialCourseQualificationSetting = GetSettingById(10015);

                if (JobInitialCourseQualificationSetting == "-1")
                {
                    JobInitialCourseQualification = Constants.JobInitialCourseQualification;
                }
                else
                {
                    if (JobInitialCourseQualificationSetting == "0")
                    {
                        JobInitialCourseQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobInitialCourseQualificationSetting))
                    {
                        JobInitialCourseQualification = false;
                    }
                    else
                    {
                        JobInitialCourseQualification = bool.Parse(JobInitialCourseQualificationSetting);
                    }
                }

                if (JobInitialCourseQualification)
                {
                    var JobInitialCourseQualificationList = _organizationJobInitialCourseQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobInitialCourseQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobInitialCourseQualificationList.Any())
                        {
                            var employeecourses = _courseService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId && i.CourseStatusId == 225).ToList(); // وضعیت دوره باید گذرانده شده باشد
                            if (employeecourses == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed(" دوره های آموزشی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                            }
                            else
                            {
                                if (employeecourses.Any())
                                {
                                    foreach (var item in JobInitialCourseQualificationList)
                                    {
                                        if (employeecourses.Any(i => i.CourseLicenseId >= item.CourseLevelId && i.CourseRegTypeId == item.CourseTypeId))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed(" دوره های آموزشی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed(" دوره های آموزشی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Initial Course Qualification

                /// بررسی ویژگی های شخصیتی در شرایط احراز شغل

                #region Character Qualification

                bool JobRequiredCharacterQualification = false;
                var JobRequiredCharacterQualificationSetting = GetSettingById(10016);

                if (JobRequiredCharacterQualificationSetting == "-1")
                {
                    JobRequiredCharacterQualification = Constants.JobRequiredCharacterQualification;
                }
                else
                {
                    if (JobRequiredCharacterQualificationSetting == "0")
                    {
                        JobRequiredCharacterQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobRequiredCharacterQualificationSetting))
                    {
                        JobRequiredCharacterQualification = false;
                    }
                    else
                    {
                        JobRequiredCharacterQualification = bool.Parse(JobRequiredCharacterQualificationSetting);
                    }
                }

                if (JobRequiredCharacterQualification)
                {
                    var JobRequiredCharacterQualificationList = _organizationJobRequiredCharacterQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobRequiredCharacterQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobRequiredCharacterQualificationList.Any())
                        {
                            var employeecharacter = _characterService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId).ToList(); // وضعیت دوره باید گذرانده شده باشد
                            if (employeecharacter == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed(" ویژگی های شخصیتی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                            }
                            else
                            {
                                if (employeecharacter.Any())
                                {
                                    foreach (var item in JobRequiredCharacterQualificationList)
                                    {
                                        if (employeecharacter.Any(i => i.RequiredLevelId >= item.RequiredLevelId && i.CharacterTypeId == item.CharacterTypeId))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed(" ویژگی های شخصیتی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed(" ویژگی های شخصیتی در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Character Qualification

                /// بررسی نرم افزار های مورد نیاز در شرایط احراز شغل

                #region Software Qualification

                bool JobRequiredSoftwaresQualification = false;
                var JobRequiredSoftwaresQualificationSetting = GetSettingById(10017);

                if (JobRequiredSoftwaresQualificationSetting == "-1")
                {
                    JobRequiredSoftwaresQualification = Constants.JobRequiredSoftwaresQualification;
                }
                else
                {
                    if (JobRequiredSoftwaresQualificationSetting == "0")
                    {
                        JobRequiredSoftwaresQualificationSetting = "false";
                    }
                    if (string.IsNullOrEmpty(JobRequiredSoftwaresQualificationSetting))
                    {
                        JobRequiredSoftwaresQualification = false;
                    }
                    else
                    {
                        JobRequiredSoftwaresQualification = bool.Parse(JobRequiredSoftwaresQualificationSetting);
                    }
                }

                if (JobRequiredSoftwaresQualification)
                {
                    var JobRequiredSoftwaresQualificationList = _organizationJobRequiredSoftwaresQualificationService.All().Where(i => i.OrganizationJobId == newrecruit.OrganizationJobId).ToList();

                    if (JobRequiredSoftwaresQualificationList == null)
                    {

                    }
                    else
                    {
                        if (JobRequiredSoftwaresQualificationList.Any())
                        {
                            var employeeSoftwares = _employeeSoftwareService.All().Where(i => i.EmployeeId == newrecruit.EmployeeId).ToList(); // وضعیت دوره باید گذرانده شده باشد
                            if (employeeSoftwares == null)
                            {
                                _unitOfWork.Rollback();
                                return OperationResult.Failed(" نرم افزار های مورد نیاز در پرونده با شرایط احراز شغلی مطابقت ندارد");
                            }
                            else
                            {
                                if (employeeSoftwares.Any())
                                {
                                    foreach (var item in JobRequiredSoftwaresQualificationList)
                                    {
                                        if (employeeSoftwares.Any(i => i.MasteryLevelTypeId >= item.MasteryLevelTypeId && i.SoftwareId == item.SoftwareId && i.SoftwareTypeId == item.SoftwareTypeId))
                                        {

                                        }
                                        else
                                        {
                                            _unitOfWork.Rollback();
                                            return OperationResult.Failed(" نرم افزار های مورد نیاز در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                        }
                                    }
                                }
                                else
                                {
                                    _unitOfWork.Rollback();
                                    return OperationResult.Failed(" نرم افزار های مورد نیاز در پرونده با شرایط احراز شغلی مطابقت ندارد");
                                }
                            }
                        }
                    }
                }
                #endregion Software Qualification

                #endregion Job Qualification Checks

                #region Persist InterdictOrder

                newinterdict.IsArrears = false;
                newinterdict.ArearsStatusId = 3;

                // تحت هر شرایطی (موردی یا گروهی) CreatorUserName و CreatedBy و AspNetUsersId بر اساس کاربر جاری مقداردهی می‌شوند.
                var effectiveAspNetUsersId = currentUserIdForList > 0 ? currentUserIdForList : (long?)null;
                newinterdict.AspNetUsersId = effectiveAspNetUsersId;
                var (creatorUserName, createdBy) = GetCreatorUserNameAndCreatedBy(effectiveAspNetUsersId);
                newinterdict.CreatorUserName = creatorUserName;
                newinterdict.CreatedBy = createdBy;

                _unitOfWork.Context.Set<InterdictOrder>().Add(newinterdict);
                _unitOfWork.Context.SaveChanges();

                #endregion Persist InterdictOrder

                #region WorkFlow

                if (newinterdict.StatusId == (long)Enums.OrderStatus.Pending)
                {
                    var definitions = _unitOfWork.Context.Set<HR.WorkFlow.Core.Data.Definition>().Where(DateValidityExtension<HR.WorkFlow.Core.Data.Definition>.GetDateValidationPredicate().And(i => i.WorkFlowId == 1));


                    if (definitions != null)
                    {
                        if (definitions.Any())
                        {
                            HR.WorkFlow.Core.Data.WorkFlowInstance instance = new HR.WorkFlow.Core.Data.WorkFlowInstance()
                            {
                                CreateDate = DateTime.Now,
                                InterdictOrderId = newinterdict.Id,
                                CreateBy = newinterdict.CreatorUserName,
                                WorkFlowId = 1,
                                IPAddress = "",
                                IsDeleted = false,
                            };


                            _unitOfWork.Context.Set<HR.WorkFlow.Core.Data.WorkFlowInstance>().Add(instance);
                            _unitOfWork.Context.SaveChanges();

                            var starterNode = definitions.Single(i => i.FromNodeId == null);

                            HR.WorkFlow.Core.Data.ActivityTemplate initial = new WorkFlow.Core.Data.ActivityTemplate()
                            {
                                WorkFlowInstanceId = instance.Id,
                                FromNodeId = null,
                                ToNodeId = starterNode.ToNodeId,
                                ActionId = (long)Enums.WorkFlowActions.Approve,
                                CreateDate = DateTime.Now,
                                IPAddress = "",
                                IsDeleted = false,
                                Pending = false,
                                LastModifiedDate = DateTime.Now,
                                title = "گره شروع",
                                DoDate = DateTime.Now,
                            };

                            _unitOfWork.Context.Set<HR.WorkFlow.Core.Data.ActivityTemplate>().Add(initial);
                            _unitOfWork.Context.SaveChanges();

                            instance.LastActivityId = initial.Id;
                            _unitOfWork.Context.Update(instance);
                            _unitOfWork.Context.SaveChanges();

                            HR.WorkFlow.Core.Data.Definition Definition = new HR.WorkFlow.Core.Data.Definition();
                            Definition.Id = starterNode.Id;

                            long? ToNodeId = starterNode.ToNodeId;
                            long? FromNodeId = null;
                            long ActionId = (long)Enums.WorkFlowActions.Approve;
                            bool init = true;
                            while (Definition.ToNodeId != null || init)
                            {
                                HR.WorkFlow.Core.Data.ActivityTemplate Step = new WorkFlow.Core.Data.ActivityTemplate()
                                {
                                    WorkFlowInstanceId = instance.Id,
                                    FromNodeId = FromNodeId,
                                    ToNodeId = ToNodeId,
                                    ActionId = ActionId,
                                    CreateDate = DateTime.Now,
                                    IPAddress = "",
                                    IsDeleted = false,
                                    Pending = init,
                                    LastModifiedDate = DateTime.Now,
                                    title = Definition.Id.ToString()
                                };

                                if (Definition.Id != starterNode.Id)
                                {
                                    _unitOfWork.Context.Set<HR.WorkFlow.Core.Data.ActivityTemplate>().Add(Step);
                                    _unitOfWork.Context.SaveChanges();
                                    init = false;
                                }

                                if (definitions.Any(i => i.FromNodeId == ToNodeId))
                                {
                                    Definition = definitions.Single(i => i.FromNodeId == ToNodeId && i.ActionId == 1);
                                    ActionId = Definition.ActionId;
                                    FromNodeId = Definition.FromNodeId;
                                    ToNodeId = Definition.ToNodeId;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var lastNode = definitions.Single(i => i.ToNodeId == null && i.ActionId == 1);


                            HR.WorkFlow.Core.Data.ActivityTemplate lastActivityTemplate = new WorkFlow.Core.Data.ActivityTemplate()
                            {
                                WorkFlowInstanceId = instance.Id,
                                FromNodeId = lastNode.FromNodeId,
                                ToNodeId = lastNode.ToNodeId,
                                ActionId = (long)Enums.WorkFlowActions.Approve,
                                CreateDate = DateTime.Now,
                                IPAddress = "",
                                IsDeleted = false,
                                Pending = false,
                                LastModifiedDate = DateTime.Now,
                                title = "گره پایان",
                                IsFinalTransition = true
                            };

                            _unitOfWork.Context.Set<HR.WorkFlow.Core.Data.ActivityTemplate>().Add(lastActivityTemplate);
                            _unitOfWork.Context.SaveChanges();
                        }
                    }
                }

                #endregion WorkFlow

                #region Save Related Entities

                if (req.OrderCopyList != null)
                {
                    if (req.OrderCopyList.Any())
                    {
                        foreach (var item in req.OrderCopyList)
                        {
                            InterdictOrderCopy toadd = new InterdictOrderCopy()
                            {
                                InterdictOrderId = newinterdict.Id,
                                CreateDate = DateTime.Now,
                                IsDeleted = false,
                                OrganisationChartId = item,
                                IPAddress = "",
                            };
                            _unitOfWork.Context.Set<InterdictOrderCopy>().Add(toadd);
                        }
                    }
                }

                if (req.InterdictOrderPromissories != null)
                {
                    if (req.InterdictOrderPromissories.Any())
                    {
                        foreach (var item in req.InterdictOrderPromissories)
                        {
                            InterdictOrderPromissory toadd = new InterdictOrderPromissory()
                            {
                                InterdictOrderId = newinterdict.Id,
                                CreateDate = DateTime.Now,
                                IsDeleted = false,
                                PromissoryNote = item.PromissoryNote,
                                PromissoryNumber = item.PromissoryNumber,
                                PromissoryValue = item.PromissoryValue,
                                IPAddress = "",
                            };
                            _unitOfWork.Context.Set<InterdictOrderPromissory>().Add(toadd);
                        }
                    }
                }

                if (req.WageItems != null)
                {
                    foreach (var WageItem in req.WageItems)
                    {

                        if (WageItem.Value > 0) {
                            InterdictOrderWageItem toadd = new InterdictOrderWageItem()
                            {
                                InterdictOrderId = newinterdict.Id,
                                CreateDate = DateTime.Now,
                                Value = WageItem.Value,
                                IsDeleted = false,
                                WageItemId = WageItem.WageItemId,
                                IPAddress = "",
                            };
                            _unitOfWork.Context.Set<InterdictOrderWageItem>().Add(toadd);
                        }
                     

                    }
                }
                if (req.CoeficentItems != null)
                {
                    foreach (var WageItem in req.CoeficentItems)
                    {
                        if (WageItem.Value > 0 )
                        {
                            InterdictOrderCoefficientItem toadd = new InterdictOrderCoefficientItem()
                            {
                                InterdictOrderId = newinterdict.Id,
                                CreateDate = DateTime.Now,
                                OutPutFactValue = WageItem.Value,
                                IsDeleted = false,
                                CoefficientId = WageItem.CoefficientId,
                                IPAddress = "",
                            };
                            _unitOfWork.Context.Set<InterdictOrderCoefficientItem>().Add(toadd);
                        }

                   
                    }
                }

                #endregion Save Related Entities

                #region Commit Transaction

                _unitOfWork.Context.SaveChanges();
                _unitOfWork.Commit();

                _logger.LogInformation("صدور حکم با موفقیت انجام شد. EmployeeId={EmployeeId}, InterdictOrderId={InterdictOrderId}",
                    req.EmployeeId, newinterdict.Id);

                return OperationResult.Succeeded(payload: newinterdict.Id);

                #endregion Commit Transaction
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                var fullMessage = GetFullExceptionMessage(ex);
                _logger.LogError(ex, "خطا در تراکنش صدور حکم. EmployeeId={EmployeeId}, OrderTypeId={OrderTypeId}, IssueTypeId={IssueTypeId}. {FullMessage}",
                    req.EmployeeId, req.OrderTypeId, req.IssueTypeId, fullMessage);
                return OperationResult.Failed("در فرایند صدور حکم خطای سیستمی رخ داد: " + fullMessage);
            }
        }
        catch (Exception ex)
        {
            var fullMessage = GetFullExceptionMessage(ex);
            _logger.LogError(ex, "خطای غیرمنتظره در متد IssueOrder. EmployeeId={EmployeeId}, OrderTypeId={OrderTypeId}, IssueTypeId={IssueTypeId}. {FullMessage}",
                req.EmployeeId, req.OrderTypeId, req.IssueTypeId, fullMessage);
            _unitOfWork.Rollback();
            return OperationResult.Failed("خطای غیرمنتظره در صدور حکم: " + fullMessage);
        }
    }

    /// <summary>
    /// متن کامل خطا شامل InnerException را برای لاگ و نمایش به فراخوان‌کننده برمی‌گرداند.
    /// </summary>
    private static string GetFullExceptionMessage(Exception ex, int maxLength = 2000)
    {
        if (ex == null) return "";
        var sb = new StringBuilder();
        var current = ex;
        while (current != null)
        {
            if (sb.Length > 0) sb.Append(" | Inner: ");
            sb.Append(current.Message);
            current = current.InnerException;
        }
        var msg = sb.ToString();
        return msg.Length > maxLength ? msg.Substring(0, maxLength) + "..." : msg;
    }

    /// <summary>
    /// بر اساس AspNetUsersId نام کاربر و متن CreatedBy را از دیتابیس برمی‌گرداند (برای صدور موردی و گروهی).
    /// </summary>
    private (string creatorUserName, string createdBy) GetCreatorUserNameAndCreatedBy(long? aspNetUsersId)
    {
        if (aspNetUsersId == null || aspNetUsersId.Value <= 0)
        {
            return ("", "System");
        }
        try
        {
            var query = "SELECT [UserName] FROM [Identity].[AspNetUsers] WHERE [Id] = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", aspNetUsersId.Value, DbType.Int64);
            var userName = _dapper.Get<string>(query, parameters, CommandType.Text);
            if (string.IsNullOrEmpty(userName))
            {
                return ("", $"UserId:{aspNetUsersId.Value}");
            }
            var createdBy = $"{userName} | UserId:{aspNetUsersId.Value}";
            return (userName, createdBy);
        }
        catch
        {
            return ("", $"UserId:{aspNetUsersId.Value}");
        }
    }

    #endregion
    private bool EmployeeHasInsuranceRecord(long employeeId, long? organisationChartId)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            organisationChartId.HasValue
                ? "SELECT TOP 1 id FROM emp.Insurance WHERE IsLast = 1 AND EmployeeId = @EmployeeId AND OrganisationChartId = @OrganisationChartId"
                : "SELECT TOP 1 id FROM emp.Insurance WHERE IsLast = 1 AND EmployeeId = @EmployeeId",
            con);
        cmd.Parameters.Add("@EmployeeId", SqlDbType.BigInt).Value = employeeId;
        if (organisationChartId.HasValue)
        {
            cmd.Parameters.Add("@OrganisationChartId", SqlDbType.BigInt).Value = organisationChartId.Value;
        }
        con.Open();
        var scalar = cmd.ExecuteScalar();
        return scalar != null && scalar != DBNull.Value && Convert.ToInt32(scalar) > 0;
    }

    private long? GetPreviousOrderPayLocationId(long employeeId, DateTime? beforeStartDate)
    {
        var query = _unitOfWork.Context.InterdictOrders
            .AsNoTracking()
            .Include(i => i.RecruitOrder)
            .Where(i => i.RecruitOrder != null && i.RecruitOrder.EmployeeId == employeeId);

        if (beforeStartDate.HasValue)
        {
            query = query.Where(i => i.StartDate < beforeStartDate.Value);
        }

        return query
            .OrderByDescending(i => i.StartDate)
            .ThenByDescending(i => i.Id)
            .Select(i => (long?)i.RecruitOrder!.PayLocationId)
            .FirstOrDefault();
    }

    public Tuple<bool, string> ValidateIssue(IssueOrderRequest req)
    {
        List<long> validIssueStatuses = new List<long>() { (long)Enums.OrderStatus.Draft, (long)Enums.OrderStatus.Pending };
        if (validIssueStatuses.Contains(req.StatusId))
        {

        }
        else
        {
            return new Tuple<bool, string>(false, "وضعیت ارسال معتبر نمی باشد");
        }

        if (req.PayLocationId <= 0)
        {
            return new Tuple<bool, string>(false, "محل پرداخت معتبر ارسال نشده است");
        }
        var summarySetting = _organisationEmployeeTypeOrderTypeSummaryCalcService.All(ImpleDate: req.StartDate)

            .Where(i => i.OrganisationChartId == req.PayLocationId &&
                i.EmployeeTypeId == req.EmployeeTypeId &&
                i.OrderTypeId == req.OrderTypeId
            );
        if (summarySetting == null)
        {
            return new Tuple<bool, string>(false, "تنظیمات خلاصه حکم یافت نشد");
        }
        else
        {
            if (summarySetting.Any())
            {
                if (summarySetting.Count() > 1)
                {
                    return new Tuple<bool, string>(false, "بیش از یک تنظیمات برای خلاصه حکم یافت شد !");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "تنظیمات خلاصه حکم یافت نشد");
            }
        }

        if (!EmployeeHasInsuranceRecord(req.EmployeeId, req.PayLocationId))
        {
            var effectiveStartDate = req.StartDate ?? req.ImpleDate;
            var previousPayLocationId = GetPreviousOrderPayLocationId(req.EmployeeId, effectiveStartDate);
            var isPayLocationChange = previousPayLocationId.HasValue
                && previousPayLocationId.Value > 0
                && previousPayLocationId.Value != req.PayLocationId;

            if (!isPayLocationChange && !EmployeeHasInsuranceRecord(req.EmployeeId, null))
            {
                return new Tuple<bool, string>(false, "هیچ رکورد بیمه ای یافت نشد");
            }
        }

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            try
            {
                string SQL = "select [GenderId] from [emp].[Employee]   WHERE Id = " + req.EmployeeId;
                SqlCommand cmd = new SqlCommand(SQL, con);

                cmd.CommandType = CommandType.Text;
                con.Open();
                object genderScalar = cmd.ExecuteScalar();
                con.Close();
                if (genderScalar == null || genderScalar == DBNull.Value)
                {
                    return new Tuple<bool, string>(false, " جنسیت کارمند مورد نظر ثبت نشده است ");
                }
                int result = Convert.ToInt32(genderScalar);
                if (result > 0)
                {

                }
                else
                {
                    return new Tuple<bool, string>(false, " جنسیت کارمند مورد نظر ثبت نشده است ");
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }


        return new Tuple<bool, string>(true, "");
    }
    public bool Validate(InterdictOrder entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    public OperationResult GetInterdictCountsByStatus()
    {
        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.Status)
            .AsNoTracking()
            .GroupBy(i => new { i.StatusId, StatusTitle = i.Status != null ? i.Status.title : "" })
            .Select(g => new
            {
                OrderStatusId = g.Key.StatusId,
                OrderStatus = g.Key.StatusTitle,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return OperationResult.Succeeded(payload: query);
    }

    public OperationResult GetInterdictCountsByPayLocation()
    {
        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.RecruitOrder)
            .ThenInclude(r => r.PayLocation)
            .AsNoTracking()
            .GroupBy(i => new
            {
                PayLocationId = i.RecruitOrder != null ? (long?)i.RecruitOrder.PayLocationId : null,
                PayLocationTitle = i.RecruitOrder != null && i.RecruitOrder.PayLocation != null ? i.RecruitOrder.PayLocation.title : ""
            })
            .Select(g => new
            {
                PayLocationId = g.Key.PayLocationId,
                PayLocation = g.Key.PayLocationTitle,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return OperationResult.Succeeded(payload: query);
    }

    /// <summary>
    /// مقایسه حکم جاری با حکم قبلی
    /// </summary>
    /// <param name="interdictOrderId">شناسه حکم جاری</param>
    /// <returns>نتیجه مقایسه شامل تفاوت‌های InterdictOrder، RecruitOrder، WageItems و CoefficientItems</returns>
    public OperationResult CompareInterdictOrders(long interdictOrderId)
    {
        try
        {
            // واکشی حکم جاری
            var currentOrder = InterdictOrdersWithComparisonIncludes()
                .FirstOrDefault(i => i.Id == interdictOrderId);

            if (currentOrder == null)
            {
                return OperationResult.Failed("حکم مورد نظر یافت نشد");
            }

            var result = new OrderComparisonResultDTO
            {
                HasPreviousOrder = currentOrder.LastInterdictOrderId.HasValue
            };

            // بررسی وجود حکم قبلی
            if (!currentOrder.LastInterdictOrderId.HasValue)
            {
                result.Message = "حکم ماقبل جهت مقایسه یافت نشد";
                return OperationResult.Succeeded(payload: result);
            }

            // واکشی حکم قبلی
            var previousOrder = InterdictOrdersWithComparisonIncludes()
                .FirstOrDefault(i => i.Id == currentOrder.LastInterdictOrderId.Value);

            if (previousOrder == null)
            {
                result.Message = "حکم ماقبل جهت مقایسه یافت نشد";
                return OperationResult.Succeeded(payload: result);
            }

            // مقایسه InterdictOrder
            result.InterdictOrderComparison = CompareInterdictOrderProperties(previousOrder, currentOrder);

            // مقایسه RecruitOrder
            if (currentOrder.RecruitOrder != null && previousOrder.RecruitOrder != null)
            {
                result.RecruitOrderComparison = CompareRecruitOrderProperties(previousOrder.RecruitOrder, currentOrder.RecruitOrder);
            }

            // مقایسه WageItems
            result.WageItemComparisons = CompareWageItems(previousOrder.Id, currentOrder.Id);

            // مقایسه CoefficientItems
            result.CoefficientItemComparisons = CompareCoefficientItems(previousOrder.Id, currentOrder.Id);

            return OperationResult.Succeeded(payload: result);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در مقایسه احکام: {ex.Message}");
        }
    }

    private IQueryable<InterdictOrder> InterdictOrdersWithComparisonIncludes()
    {
        return _unitOfWork.Context.InterdictOrders
            .AsNoTracking()
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.PayLocation)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.CostCenter)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.OrganizationUnit)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.WorkPlace)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.Project)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.EmployeeStatus)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.EmployeeType)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.OrganizationJob).ThenInclude(oj => oj!.Job)
            .Include(i => i.RecruitOrder).ThenInclude(r => r!.OrganisationPosition)
            .Include(i => i.Status)
            .Include(i => i.OrderType)
            .Include(i => i.IssueType)
            .Include(i => i.MarriageStatus)
            .Include(i => i.InsuranceType)
            .Include(i => i.EducationGrade)
            .Include(i => i.EffectiveEducationGrade);
    }

    private InterdictOrderComparisonDTO CompareInterdictOrderProperties(InterdictOrder previous, InterdictOrder current)
    {
        var comparison = new InterdictOrderComparisonDTO
        {
            CurrentOrderId = current.Id,
            PreviousOrderId = previous.Id
        };

        var propertyMappings = GetInterdictOrderPropertyMappings();

        foreach (var mapping in propertyMappings)
        {
            var oldValue = mapping.GetValue(previous);
            var newValue = mapping.GetValue(current);

            if (!AreValuesEqual(oldValue, newValue))
            {
                comparison.PropertyDifferences.Add(new PropertyComparisonDTO
                {
                    PropertyName = mapping.PropertyName,
                    PropertyNameFa = mapping.PropertyNameFa,
                    OldValue = FormatValue(oldValue),
                    NewValue = FormatValue(newValue),
                    ChangeType = "Modified"
                });
            }
        }

        return comparison;
    }

    private RecruitOrderComparisonDTO CompareRecruitOrderProperties(RecruitOrder previous, RecruitOrder current)
    {
        var comparison = new RecruitOrderComparisonDTO
        {
            CurrentRecruitOrderId = current.Id,
            PreviousRecruitOrderId = previous.Id
        };

        var propertyMappings = GetRecruitOrderPropertyMappings();

        foreach (var mapping in propertyMappings)
        {
            var oldValue = mapping.GetValue(previous);
            var newValue = mapping.GetValue(current);

            if (!AreValuesEqual(oldValue, newValue))
            {
                comparison.PropertyDifferences.Add(new PropertyComparisonDTO
                {
                    PropertyName = mapping.PropertyName,
                    PropertyNameFa = mapping.PropertyNameFa,
                    OldValue = FormatValue(oldValue),
                    NewValue = FormatValue(newValue),
                    ChangeType = "Modified"
                });
            }
        }

        return comparison;
    }

    private List<InterdictOrderWageItemComparisonDTO> CompareWageItems(long previousOrderId, long currentOrderId)
    {
        var previousItems = _unitOfWork.Context.InterdictOrderWageItems
            .Include(w => w.WageItem)
            .Where(w => w.InterdictOrderId == previousOrderId)
            .AsNoTracking()
            .ToList();

        var currentItems = _unitOfWork.Context.InterdictOrderWageItems
            .Include(w => w.WageItem)
            .Where(w => w.InterdictOrderId == currentOrderId)
            .AsNoTracking()
            .ToList();

        var comparisons = new List<InterdictOrderWageItemComparisonDTO>();
        var currentItemsByWageItemId = currentItems.ToDictionary(c => c.WageItemId);
        var previousItemsByWageItemId = previousItems.ToDictionary(p => p.WageItemId);

        foreach (var prevItem in previousItems)
        {
            if (!currentItemsByWageItemId.ContainsKey(prevItem.WageItemId))
            {
                comparisons.Add(new InterdictOrderWageItemComparisonDTO
                {
                    Id = prevItem.Id,
                    WageItemId = prevItem.WageItemId,
                    WageItemName = prevItem.WageItem?.title,
                    OldValue = prevItem.Value,
                    NewValue = null,
                    ChangeType = "Deleted"
                });
            }
        }

        foreach (var currItem in currentItems)
        {
            if (!previousItemsByWageItemId.TryGetValue(currItem.WageItemId, out var previousItem))
            {
                comparisons.Add(new InterdictOrderWageItemComparisonDTO
                {
                    Id = currItem.Id,
                    WageItemId = currItem.WageItemId,
                    WageItemName = currItem.WageItem?.title,
                    OldValue = null,
                    NewValue = currItem.Value,
                    ChangeType = "Added"
                });
            }
            else if (previousItem.Value != currItem.Value)
            {
                comparisons.Add(new InterdictOrderWageItemComparisonDTO
                {
                    Id = currItem.Id,
                    WageItemId = currItem.WageItemId,
                    WageItemName = currItem.WageItem?.title,
                    OldValue = previousItem.Value,
                    NewValue = currItem.Value,
                    ChangeType = "Modified"
                });
            }
        }

        return comparisons;
    }

    private List<InterdictOrderCoefficientItemComparisonDTO> CompareCoefficientItems(long previousOrderId, long currentOrderId)
    {
        var previousItems = _unitOfWork.Context.InterdictOrderCoefficientItems
            .Include(c => c.Coefficient)
            .Where(c => c.InterdictOrderId == previousOrderId)
            .AsNoTracking()
            .ToList();

        var currentItems = _unitOfWork.Context.InterdictOrderCoefficientItems
            .Include(c => c.Coefficient)
            .Where(c => c.InterdictOrderId == currentOrderId)
            .AsNoTracking()
            .ToList();

        var comparisons = new List<InterdictOrderCoefficientItemComparisonDTO>();
        var currentItemsByCoefficientId = currentItems.ToDictionary(c => c.CoefficientId);
        var previousItemsByCoefficientId = previousItems.ToDictionary(p => p.CoefficientId);

        foreach (var prevItem in previousItems)
        {
            if (!currentItemsByCoefficientId.ContainsKey(prevItem.CoefficientId))
            {
                comparisons.Add(new InterdictOrderCoefficientItemComparisonDTO
                {
                    Id = prevItem.Id,
                    CoefficientId = prevItem.CoefficientId,
                    CoefficientName = prevItem.Coefficient?.title,
                    OldValue = prevItem.OutPutFactValue,
                    NewValue = null,
                    ChangeType = "Deleted"
                });
            }
        }

        foreach (var currItem in currentItems)
        {
            if (!previousItemsByCoefficientId.TryGetValue(currItem.CoefficientId, out var previousItem))
            {
                comparisons.Add(new InterdictOrderCoefficientItemComparisonDTO
                {
                    Id = currItem.Id,
                    CoefficientId = currItem.CoefficientId,
                    CoefficientName = currItem.Coefficient?.title,
                    OldValue = null,
                    NewValue = currItem.OutPutFactValue,
                    ChangeType = "Added"
                });
            }
            else if (previousItem.OutPutFactValue != currItem.OutPutFactValue)
            {
                comparisons.Add(new InterdictOrderCoefficientItemComparisonDTO
                {
                    Id = currItem.Id,
                    CoefficientId = currItem.CoefficientId,
                    CoefficientName = currItem.Coefficient?.title,
                    OldValue = previousItem.OutPutFactValue,
                    NewValue = currItem.OutPutFactValue,
                    ChangeType = "Modified"
                });
            }
        }

        return comparisons;
    }

    private List<PropertyMapping<InterdictOrder>> GetInterdictOrderPropertyMappings()
    {
        return new List<PropertyMapping<InterdictOrder>>
        {
            new PropertyMapping<InterdictOrder>("Code", "کد حکم", o => o.Code),
            new PropertyMapping<InterdictOrder>("Serial", "شماره سریال", o => o.Serial?.ToString()),
            new PropertyMapping<InterdictOrder>("FirstName", "نام", o => o.FirstName),
            new PropertyMapping<InterdictOrder>("LastName", "نام خانوادگی", o => o.LastName),
            new PropertyMapping<InterdictOrder>("FatherName", "نام پدر", o => o.FatherName),
            new PropertyMapping<InterdictOrder>("NationalNo", "کد ملی", o => o.NationalNo),
            new PropertyMapping<InterdictOrder>("IdentityNo", "شماره شناسنامه", o => o.IdentityNo),
            new PropertyMapping<InterdictOrder>("PersonelCode", "کد کارمندی", o => o.PersonelCode),
            new PropertyMapping<InterdictOrder>("BirthDate", "تاریخ تولد", o => o.BirthDate?.ToString("yyyy/MM/dd")),
            new PropertyMapping<InterdictOrder>("EmployeeDate", "تاریخ استخدام", o => o.EmployeeDate?.ToString("yyyy/MM/dd")),
            new PropertyMapping<InterdictOrder>("OrderType", "نوع حکم", o => o.OrderType?.title),
            new PropertyMapping<InterdictOrder>("IssueType", "نوع صدور", o => o.IssueType?.title),
            new PropertyMapping<InterdictOrder>("Status", "وضعیت", o => o.Status?.title),
            new PropertyMapping<InterdictOrder>("MarriageStatus", "وضعیت تاهل", o => o.MarriageStatus?.title),
            new PropertyMapping<InterdictOrder>("InsuranceType", "نوع بیمه", o => o.InsuranceType?.title),
            new PropertyMapping<InterdictOrder>("EducationGrade", "مدرک تحصیلی", o => o.EducationGrade?.title),
            new PropertyMapping<InterdictOrder>("EffectiveEducationGrade", "مدرک موثر", o => o.EffectiveEducationGrade?.title),
            new PropertyMapping<InterdictOrder>("SponsorshipCount", "تعداد افراد تحت تکفل", o => o.SponsorshipCount?.ToString()),
            new PropertyMapping<InterdictOrder>("ChildCount", "تعداد فرزندان", o => o.ChildCount.ToString()),
            new PropertyMapping<InterdictOrder>("WifeCount", "تعداد همسر", o => o.WifeCount?.ToString()),
            new PropertyMapping<InterdictOrder>("YearCoefficient", "ضریب سنوات", o => o.YearCoefficient?.ToString()),
            new PropertyMapping<InterdictOrder>("IsWar", "جانباز", o => o.IsWar?.ToString()),
            new PropertyMapping<InterdictOrder>("IsCaptivity", "آزاده", o => o.IsCaptivity?.ToString()),
            new PropertyMapping<InterdictOrder>("IsBasij", "بسیجی", o => o.IsBasij?.ToString()),
            new PropertyMapping<InterdictOrder>("IsIsar", "ایثارگر", o => o.IsIsar?.ToString()),
            new PropertyMapping<InterdictOrder>("IsMartyrs", "شهید", o => o.IsMartyrs?.ToString()),
            new PropertyMapping<InterdictOrder>("IsWomenHead", "سرپرست خانوار", o => o.IsWomenHead.ToString()),
            new PropertyMapping<InterdictOrder>("WarDuration", "مدت جانبازی", o => o.WarDuration?.ToString()),
            new PropertyMapping<InterdictOrder>("CaptivityDuration", "مدت اسارت", o => o.CaptivityDuration?.ToString()),
            new PropertyMapping<InterdictOrder>("BasijDuration", "مدت بسیج", o => o.BasijDuration?.ToString()),
            new PropertyMapping<InterdictOrder>("IsarPercent", "درصد ایثارگری", o => o.IsarPercent?.ToString()),
            new PropertyMapping<InterdictOrder>("JobDegree", "ایجاده شغلی", o => o.JobDegree?.ToString()),
            new PropertyMapping<InterdictOrder>("GradScore", "امتیاز", o => o.GradScore?.ToString()),
            new PropertyMapping<InterdictOrder>("AccountNumber", "شماره حساب", o => o.AccountNumber),
            new PropertyMapping<InterdictOrder>("Description", "توضیحات", o => o.Description),
            new PropertyMapping<InterdictOrder>("SumWageFactors", "جمع ضرایب حقوقی", o => o.SumWageFactors?.ToString()),
            new PropertyMapping<InterdictOrder>("ExperienceRecorded", "سابقه ثبت شده", o => o.ExperienceRecorded),
            new PropertyMapping<InterdictOrder>("RetiredRecorded", "سابقه بازنشستگی", o => o.RetiredRecorded),
            new PropertyMapping<InterdictOrder>("YearRecorded", "سال ثبت", o => o.YearRecorded),
            new PropertyMapping<InterdictOrder>("HistoryOut", "مرخصی بدون حقوق", o => o.HistoryOut?.ToString()),
            new PropertyMapping<InterdictOrder>("HistoryStop", "توقف کار", o => o.HistoryStop?.ToString()),
        };
    }

    private List<PropertyMapping<RecruitOrder>> GetRecruitOrderPropertyMappings()
    {
        return new List<PropertyMapping<RecruitOrder>>
        {
            new PropertyMapping<RecruitOrder>("PayLocation", "محل پرداخت", o => o.PayLocation?.title),
            new PropertyMapping<RecruitOrder>("CostCenter", "مرکز هزینه", o => o.CostCenter?.title),
            new PropertyMapping<RecruitOrder>("OrganizationUnit", "واحد سازمانی", o => o.OrganizationUnit?.title),
            new PropertyMapping<RecruitOrder>("WorkPlace", "محل خدمت", o => o.WorkPlace?.title),
            new PropertyMapping<RecruitOrder>("Project", "پروژه", o => o.Project?.title),
            new PropertyMapping<RecruitOrder>("EmployeeStatus", "وضعیت استخدامی", o => o.EmployeeStatus?.title),
            new PropertyMapping<RecruitOrder>("EmployeeType", "نوع استخدام", o => o.EmployeeType?.title),
            // Compare by Job title, not by OrganizationJobId
            new PropertyMapping<RecruitOrder>("OrganizationJob", "شغل سازمانی", o => o.OrganizationJob != null && o.OrganizationJob.Job != null ? o.OrganizationJob.Job.title : null),
            new PropertyMapping<RecruitOrder>("OrganisationPosition", "پست سازمانی", o => o.OrganisationPosition?.title),
            new PropertyMapping<RecruitOrder>("CostCenterPercent", "درصد مرکز هزینه", o => o.CostCenterPercent?.ToString()),
        };
    }

    private bool AreValuesEqual(object? value1, object? value2)
    {
        if (value1 == null && value2 == null) return true;
        if (value1 == null || value2 == null) return false;
        return value1.Equals(value2);
    }

    private string? FormatValue(object? value)
    {
        if (value == null) return null;
        if (value is DateTime dateTime)
            return dateTime.ToString("yyyy/MM/dd");
        if (value is bool boolValue)
            return boolValue ? "بله" : "خیر";
        return value.ToString();
    }

    private class PropertyMapping<T>
    {
        public string PropertyName { get; }
        public string PropertyNameFa { get; }
        public Func<T, object?> GetValue { get; }

        public PropertyMapping(string propertyName, string propertyNameFa, Func<T, object?> getValue)
        {
            PropertyName = propertyName;
            PropertyNameFa = propertyNameFa;
            GetValue = getValue;
        }
    }

    public OperationResult UpdateReportDataSource()
    {
        try
        {
            var result = HR.SharedKernel.Sql.EmployeePropertyPopulationExecutor.Execute(
                _connectionString,
                new HR.SharedKernel.Sql.EmployeePropertyPopulationExecutor.ExecutionOptions(600));

            return OperationResult.Succeeded(payload: result);
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            return OperationResult.Failed(
                $"خطا در بروز رسانی منبع گزارش: {HR.SharedKernel.Sql.EmployeePropertyPopulationExecutor.FormatSqlException(ex)}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در بروز رسانی منبع گزارش: {ex.Message}");
        }
    }

    /// <summary>
    /// دریافت فهرست احکام صادره توسط کاربر جاری
    /// </summary>
    /// <param name="currentUserId">شناسه کاربر جاری</param>
    /// <returns>فهرست احکام صادره</returns>
    public OperationResult GetMyIssuedOrdersList(long currentUserId)
    {
        try
        {
            var orders = _unitOfWork.Context.InterdictOrders
                .Include(i => i.RecruitOrder)
                    .ThenInclude(r => r.Employee)
                .Include(i => i.RecruitOrder)
                    .ThenInclude(r => r.PayLocation)
                .Include(i => i.RecruitOrder)
                    .ThenInclude(r => r.CostCenter)
                .Include(i => i.RecruitOrder)
                    .ThenInclude(r => r.EmployeeStatus)
                .Include(i => i.OrderType)
                .Include(i => i.Status)
                .Include(i => i.IssueType)
                .Where(i => i.AspNetUsersId == currentUserId && !i.IsDeleted)
                .OrderByDescending(i => i.CreateDate)
                .Select(i => new MyIssuedOrdersListDTO
                {
                    Id = i.Id,
                    // From Employee
                    FirstName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.FirstName : null,
                    LastName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.LastName : null,
                    NationalNo = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.NationalNo : null,
                    // From RecruitOrder
                    PayLocation = i.RecruitOrder != null && i.RecruitOrder.PayLocation != null ? i.RecruitOrder.PayLocation.title : null,
                    CostCenter = i.RecruitOrder != null && i.RecruitOrder.CostCenter != null ? i.RecruitOrder.CostCenter.title : null,
                    EmployeeStatus = i.RecruitOrder != null && i.RecruitOrder.EmployeeStatus != null ? i.RecruitOrder.EmployeeStatus.title : null,
                    // From InterdictOrder
                    OrderType = i.OrderType != null ? i.OrderType.title : null,
                    StatusId = i.StatusId,
                    Status = i.Status != null ? i.Status.title : null,
                    IssueType = i.IssueType != null ? i.IssueType.title : null,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    CreateDate = i.CreateDate
                })
                .ToList();

            return OperationResult.Succeeded(payload: orders, rowCount: orders.Count);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت فهرست احکام: {ex.Message}");
        }
    }

    public OperationResult GetExpiringOrdersList(DateTime? endDateFilter, bool includeExpired = false, long? payLocationId = null, long? costCenterId = null)
    {
        // اگر تاریخ فیلتر مشخص نشده باشد، یک ماه آینده را به عنوان پیش‌فرض استفاده می‌کنیم
        DateTime targetDate = endDateFilter ?? DateTime.Now.AddMonths(1);
        DateTime now = DateTime.Now.Date; // فقط تاریخ، بدون ساعت

        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.Employee)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.PayLocation)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.CostCenter)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.EmployeeStatus)
            .Include(i => i.OrderType)
            .Include(i => i.Status)
            .Include(i => i.IssueType)
            .Where(i => i.StatusId == 9 && !i.IsDeleted && i.EndDate != null && i.EndDate.Value.Date <= targetDate.Date);

        // فیلتر بر اساس PayLocationId
        if (payLocationId.HasValue && payLocationId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder != null && i.RecruitOrder.PayLocationId == payLocationId.Value);
        }

        // فیلتر بر اساس CostCenterId
        if (costCenterId.HasValue && costCenterId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder != null && i.RecruitOrder.CostCenterId == costCenterId.Value);
        }

        // اگر includeExpired = false باشد، فقط احکامی که هنوز منقضی نشده‌اند را نمایش می‌دهیم
        if (!includeExpired)
        {
            query = query.Where(i => i.EndDate.Value.Date >= now);
        }

        var orders = query
            .OrderBy(i => i.EndDate)
            .Select(i => new MyIssuedOrdersListDTO
            {
                Id = i.Id,
                // From Employee
                FirstName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.FirstName : null,
                LastName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.LastName : null,
                NationalNo = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.NationalNo : null,
                // From RecruitOrder
                PayLocation = i.RecruitOrder != null && i.RecruitOrder.PayLocation != null ? i.RecruitOrder.PayLocation.title : null,
                CostCenter = i.RecruitOrder != null && i.RecruitOrder.CostCenter != null ? i.RecruitOrder.CostCenter.title : null,
                EmployeeStatus = i.RecruitOrder != null && i.RecruitOrder.EmployeeStatus != null ? i.RecruitOrder.EmployeeStatus.title : null,
                // From InterdictOrder
                OrderType = i.OrderType != null ? i.OrderType.title : null,
                Status = i.Status != null ? i.Status.title : null,
                IssueType = i.IssueType != null ? i.IssueType.title : null,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
                CreateDate = i.CreateDate
            })
            .ToList();

        return OperationResult.Succeeded(payload: orders, rowCount: orders.Count);
    }

    public OperationResult GetSettlementCartableList(long? payLocationId = null, long? costCenterId = null)
    {
        var now = DateTime.Now.Date;

        var nonEmployedMappingsQuery = _systemSettingContext.OrganisationEmployeeStatuses
            .AsNoTracking()
            .Where(o => o.IsDeleted != true
                && o.IsEmployed == false
                && (o.StartDate == null || o.StartDate.Value.Date <= now)
                && (o.EndDate == null || o.EndDate > now));

        if (payLocationId.HasValue && payLocationId.Value > 0)
        {
            nonEmployedMappingsQuery = nonEmployedMappingsQuery
                .Where(o => o.OrganisationChartId == payLocationId.Value);
        }

        var nonEmployedMappings = nonEmployedMappingsQuery
            .Select(o => new { o.EmployeeStatusId, o.OrganisationChartId })
            .ToList();

        if (nonEmployedMappings.Count == 0)
        {
            return OperationResult.Succeeded(payload: new List<MyIssuedOrdersListDTO>(), rowCount: 0);
        }

        var mappingSet = nonEmployedMappings
            .Select(m => (m.EmployeeStatusId, m.OrganisationChartId))
            .ToHashSet();

        var employeeStatusIds = nonEmployedMappings.Select(m => m.EmployeeStatusId).Distinct().ToList();
        var payLocationIds = nonEmployedMappings.Select(m => m.OrganisationChartId).Distinct().ToList();

        var query = _unitOfWork.Context.InterdictOrders
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.Employee)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.PayLocation)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.CostCenter)
            .Include(i => i.RecruitOrder)
                .ThenInclude(r => r.EmployeeStatus)
            .Include(i => i.OrderType)
            .Include(i => i.Status)
            .Include(i => i.IssueType)
            .Where(i => i.StatusId == 9
                && !i.IsDeleted
                && i.RecruitOrder != null
                && employeeStatusIds.Contains(i.RecruitOrder.EmployeeStatusId)
                && payLocationIds.Contains(i.RecruitOrder.PayLocationId));

        if (payLocationId.HasValue && payLocationId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder!.PayLocationId == payLocationId.Value);
        }

        if (costCenterId.HasValue && costCenterId.Value > 0)
        {
            query = query.Where(i => i.RecruitOrder!.CostCenterId == costCenterId.Value);
        }

        var orders = query
            .OrderByDescending(i => i.CreateDate)
            .AsEnumerable()
            .Where(i => mappingSet.Contains((i.RecruitOrder!.EmployeeStatusId, i.RecruitOrder.PayLocationId)))
            .Select(i => new MyIssuedOrdersListDTO
            {
                Id = i.Id,
                EmployeeId = i.RecruitOrder != null ? i.RecruitOrder.EmployeeId : null,
                FirstName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.FirstName : null,
                LastName = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.LastName : null,
                NationalNo = i.RecruitOrder != null && i.RecruitOrder.Employee != null ? i.RecruitOrder.Employee.NationalNo : null,
                PayLocation = i.RecruitOrder != null && i.RecruitOrder.PayLocation != null ? i.RecruitOrder.PayLocation.title : null,
                CostCenter = i.RecruitOrder != null && i.RecruitOrder.CostCenter != null ? i.RecruitOrder.CostCenter.title : null,
                EmployeeStatus = i.RecruitOrder != null && i.RecruitOrder.EmployeeStatus != null ? i.RecruitOrder.EmployeeStatus.title : null,
                OrderType = i.OrderType != null ? i.OrderType.title : null,
                Status = i.Status != null ? i.Status.title : null,
                IssueType = i.IssueType != null ? i.IssueType.title : null,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
                CreateDate = i.CreateDate
            })
            .ToList();

        return OperationResult.Succeeded(payload: orders, rowCount: orders.Count);
    }

    public OperationResult GetInterdictOrderArchiveStatus(long orderId)
    {
        var archive = unitOfWork.Context.InterdictOrderArchives
            .AsNoTracking()
            .Where(a => a.InterdictOrderId == orderId)
            .Select(a => new InterdictOrderArchiveStatusDto
            {
                HasRawPdf = a.PdfrawByteArray != null && a.PdfrawByteArray.Length > 0,
                HasFormattedPdf = a.PdfbyteArray != null && a.PdfbyteArray.Length > 0,
            })
            .FirstOrDefault();

        return OperationResult.Succeeded(payload: archive ?? new InterdictOrderArchiveStatusDto());
    }

    public OperationResult DownloadInterdictOrderArchivePdf(long orderId, bool isRaw)
    {
        var archive = unitOfWork.Context.InterdictOrderArchives
            .AsNoTracking()
            .Where(a => a.InterdictOrderId == orderId)
            .Select(a => new
            {
                a.PdfrawByteArray,
                a.PdfbyteArray,
            })
            .FirstOrDefault();

        if (archive == null)
        {
            return OperationResult.NotFound("آرشیو حکم یافت نشد");
        }

        var pdfBytes = isRaw ? archive.PdfrawByteArray : archive.PdfbyteArray;
        if (pdfBytes == null || pdfBytes.Length == 0)
        {
            return OperationResult.NotFound(isRaw ? "فایل PDF خام آرشیو موجود نیست" : "فایل PDF قالب‌دار آرشیو موجود نیست");
        }

        return OperationResult.Succeeded(payload: pdfBytes);
    }

    public OperationResult RebuildInterdictOrderArchive(long orderId)
    {
        var orderExists = unitOfWork.Context.InterdictOrders.AsNoTracking().Any(o => o.Id == orderId);
        if (!orderExists)
        {
            return OperationResult.NotFound("حکم یافت نشد");
        }

        var rawResult = DownloadOrderPDF(orderId, true);
        var formattedResult = DownloadOrderPDF(orderId, false);

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
                messages.Add(FormatArchivePdfError(rawResult.Message, isRaw: true));
            }

            if (formattedBytes == null)
            {
                messages.Add(FormatArchivePdfError(formattedResult.Message, isRaw: false));
            }

            return OperationResult.Failed(string.Join(" | ", messages.Distinct()));
        }

        var hadExistingArchive = InterdictOrderArchivePersistence.Exists(unitOfWork.Context, orderId);
        InterdictOrderArchivePersistence.Replace(
            unitOfWork.Context,
            orderId,
            rawBytes,
            formattedBytes);

        var status = new InterdictOrderArchiveStatusDto
        {
            HasRawPdf = rawBytes != null,
            HasFormattedPdf = formattedBytes != null,
        };

        var warnings = new List<string>();
        if (rawBytes == null)
        {
            warnings.Add(FormatArchivePdfError(rawResult.Message, isRaw: true));
        }

        if (formattedBytes == null)
        {
            warnings.Add(FormatArchivePdfError(formattedResult.Message, isRaw: false));
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

    private static string FormatArchivePdfError(string? message, bool isRaw)
    {
        var pdfType = isRaw ? "PDF خام" : "PDF قالب‌دار";
        if (string.IsNullOrWhiteSpace(message))
        {
            return $"امکان تولید {pdfType} وجود ندارد";
        }

        if (message.Contains("تنظیمات گزارش چاپی یافت نشد", StringComparison.Ordinal))
        {
            return isRaw
                ? "تنظیمات قالب چاپ (MRT) برای PDF خام این حکم یافت نشد. لطفاً در تنظیمات سازمان، قالب چاپ حکم خام را برای نوع استخدام و محل پرداخت مربوطه تعریف کنید."
                : "تنظیمات قالب چاپ (MRT) برای PDF قالب‌دار این حکم یافت نشد. لطفاً در تنظیمات سازمان، قالب چاپ حکم قالب‌دار را برای نوع استخدام و محل پرداخت مربوطه تعریف کنید.";
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
