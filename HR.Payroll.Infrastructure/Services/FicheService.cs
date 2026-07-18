using AutoMapper;
using Hr.Employee.infrastructure.Data;
using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.infrastructure.Services;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Data;
using HR.FormulaEngine.Infrastructure.Services;
using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
using HR.Payroll.Core.Data;
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
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;


namespace HR.Payroll.Infrastructure.Services;

public class FicheService(IMapper mapper, InsuranceService InsuranceService, FormulaEngineContext FormulaEngineContext, BankAccountService BankAccountService, PersonnelLoanService PersonnelLoanService, BlackListService BlackListService, OrganisationCostCenterService OrganisationCostCenterService, PaymentPeriodService PaymentPeriodService, TaxTableService TaxTableService, TaxService TaxService, FormulaOperandService FormulaOperandService, BaseInfo.infrastructure.Services.CoefficientService CoefficientService, WageItemService wageItemService, SettlementItemService settlementItemService, OrganisationPositionService OrganisationPositionService, EmployeeStatusService EmployeeStatusService, OrderTypeService OrderTypeService, ProjectService ProjectService, JobService JobService, OrganizationJobService OrganizationJobService, OrganisationChartService OrganisationChartService, EmployeeTypeService employeeTypeService, OrganisationEmployeeTypeMRTService organisationEmployeeTypeMRTService, OrganisationMRTService organisationMRTService, OrganisationContext organisationContext, EmployeeContext employeeContext, IUnitOfWork<PayrollContext> unitOfWork, FormulaEngine.Infrastructure.Services.FormulaService FormulaService, IDapper dapper, IConfiguration configuration, UserResolverService userService, IServiceScopeFactory scopeFactory, ILogger<FicheService> logger) : BaseService<Fiche, PayrollContext, FicheDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly OrganisationEmployeeTypeMRTService _organisationEmployeeTypeMRTService = organisationEmployeeTypeMRTService;
    private readonly OrganisationMRTService _organisationMRTService = organisationMRTService;
    private readonly OrganisationContext _organisationContext = organisationContext;
    private readonly EmployeeContext _employeeContext = employeeContext;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly FormulaEngine.Infrastructure.Services.FormulaService _formulaService = FormulaService;
    private readonly EmployeeTypeService _employeeTypeService = employeeTypeService;
    private readonly OrganisationChartService _organisationChartService = OrganisationChartService;
    private readonly OrganizationJobService _organizationJobService = OrganizationJobService;
    private readonly JobService _jobService = JobService;
    private readonly ProjectService _projectService = ProjectService;
    private readonly OrderTypeService _orderTypeService = OrderTypeService;
    private readonly EmployeeStatusService _employeeStatusService = EmployeeStatusService;
    private readonly OrganisationPositionService _organisationPositionService = OrganisationPositionService;
    private readonly WageItemService _wageItemService = wageItemService;
    private readonly SettlementItemService _settlementItemService = settlementItemService;
    private readonly BaseInfo.infrastructure.Services.CoefficientService _coefficientService = CoefficientService;
    private readonly FormulaOperandService _formulaOperandService = FormulaOperandService;
    private readonly TaxService _taxService = TaxService;
    private readonly TaxTableService _taxTableService = TaxTableService;
    private readonly PaymentPeriodService _paymentPeriodService = PaymentPeriodService;
    private readonly OrganisationCostCenterService _organisationCostCenterService = OrganisationCostCenterService;
    private readonly BlackListService _blackListService = BlackListService;
    private readonly PersonnelLoanService _personnelLoanService = PersonnelLoanService;
    private readonly BankAccountService _bankAccountService = BankAccountService;
    private readonly InsuranceService _insuranceService = InsuranceService;
    private readonly FormulaEngineContext _formulaEngineContext = FormulaEngineContext;
    private readonly ILogger<FicheService> _logger = logger;

    /// <summary>
    /// این متد تمام فیش های دوره محاسبه حقوق ارسالی را حذف می کند
    /// </summary>
    /// <param name="_currentUserDefaultPaymentPeriod"></param>
    /// <returns></returns>
    public OperationResult DeleteDraftFichesByPaymentPeriodId(long _currentUserDefaultPaymentPeriod)
    {
        var currentPeriod = _unitOfWork.Context.PaymentPeriods.Find(_currentUserDefaultPaymentPeriod);

        if (currentPeriod == null)
        {
            return OperationResult.Failed("دوره پیش فرض یافت نشد");
        }
        else
        {
            if (currentPeriod.IsClosed)
            {
                return OperationResult.Failed("دوره پیش فرض بسته شده است و امکان حذف فیش ها وجود ندارد");
            }
        }
        if (_db.Set<BankDiskette>().Any(i => i.PaymentPeriodId == _currentUserDefaultPaymentPeriod && i.BankDisketteStatusId != (long)Enums.BankDisketteStatus.Deleted))
        {
            return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت بانک وجود دارد");
        }
        if (_db.Set<TaxDiskette>().Any(i => i.PaymentPeriodId == _currentUserDefaultPaymentPeriod && i.TaxDisketteStatusId != (long)Enums.TaxDisketteStatus.Deleted))
        {
            return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت دارایی وجود دارد");
        }
        if (_db.Set<InsuranceDiskette>().Any(i => i.PaymentPeriodId == _currentUserDefaultPaymentPeriod && i.InsuranceDisketteStatusId != (long)Enums.InsuranceDisketteStatus.Deleted))
        {
            return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت بیمه وجود دارد");
        }
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand("[Payroll].[Deletedraftfichesbypaymentperiodid]", con);
            cmd.Parameters.AddWithValue("@PaymentPeriodId", _currentUserDefaultPaymentPeriod);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            con.Close();
            return OperationResult.Succeeded(payload: 1);
        }
    }
    public string PersianDate(DateTime DateTime1)

    {

        PersianCalendar PersianCalendar1 = new PersianCalendar();

        return string.Format(@"{0}/{1}/{2}",

       PersianCalendar1.GetYear(DateTime1),

       PersianCalendar1.GetMonth(DateTime1),

       PersianCalendar1.GetDayOfMonth(DateTime1));



    }
    /// <summary>
    /// ارتباط با سرویس محاسباتی فرمول ها 
    /// </summary>
    /// <param name="OrganisationFormulaId"></param>
    /// <param name="req"></param>
    /// <returns></returns>
    public FormulaCalculateResponseDTO CommunicateWithFormula(long OrganisationFormulaId, CommunicateWithFormulaRequest req)
    {


        FormulaCalculationRequestDTO formulareq = new FormulaCalculationRequestDTO();
        formulareq.OrganisationFormulaId = OrganisationFormulaId;
        formulareq.VariableList = new Dictionary<string, double>
                                {
                                    { "EmployeeId", Convert.ToDouble(req.RecruitOrder.EmployeeId) },
                                    { "PayLocationId", Convert.ToDouble(req.RecruitOrder.PayLocationId) },
                                    { "NumericalDate", Convert.ToDouble(Utilities.ConvertDateToNumber(req.PaymentPeriod.StartDate.Value)) },
                                    { "EmployeeTypeId", Convert.ToDouble(req.RecruitOrder.EmployeeTypeId) }
                                };

        if (req.BuildTreeTrace == true)
        {

            formulareq.VariableFriendlyList = new Dictionary<string, string?>
                                {
                                    { "شناسه کارمندی", Convert.ToString(req.RecruitOrder.EmployeeId) },
                                    //{ "محل پرداخت", Convert.ToDouble(req.PayLocationId) },
                                    { "تاریخ عددی", Convert.ToString(Utilities.ConvertDateToNumber(req.PaymentPeriod.StartDate.Value)) },
                //{ "نوع استخدام", Convert.ToDouble(req.EmployeeTypeId) }
                                };
            var employeeType = _employeeTypeService.GetIdAsync(req.RecruitOrder.EmployeeTypeId).Result.title;

            formulareq.VariableFriendlyList.Add("نوع استخدام", employeeType);

            var PayLocation = _organisationChartService.GetIdAsync(req.RecruitOrder.PayLocationId).Result.title;

            formulareq.VariableFriendlyList.Add("محل پرداخت", PayLocation);

        }
        if (req.PaymentPeriod != null)
        {
            if (req.PaymentPeriod.Id > 0)
            {
                formulareq.VariableList.Add("PaymentPeriodId", Convert.ToDouble(req.PaymentPeriod.Id));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("دوره محاسبه فیش", Convert.ToString(req.PaymentPeriod.title));
                }
                formulareq.VariableList.Add("PaymentPeriodStartDate", Convert.ToDouble(Utilities.ConvertDateToNumberMiladi(req.PaymentPeriod.StartDate.Value)));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("تاریخ اغاز دوره فیش حقوقی", PersianDate(req.PaymentPeriod.StartDate.Value));
                }

                formulareq.VariableList.Add("PaymentPeriodEndDate", Convert.ToDouble(Utilities.ConvertDateToNumberMiladi(req.PaymentPeriod.EndDate.Value)));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("تاریخ پایان دوره فیش حقوقی", PersianDate(req.PaymentPeriod.EndDate.Value));
                }

            }
        }

        var pc = new PersianCalendar();
        int shamsiYear = pc.GetYear(req.PaymentPeriod.StartDate.Value);
        formulareq.VariableList.Add("shamsiYear", Convert.ToDouble(shamsiYear));
        if (req.BuildTreeTrace == true)
        {
            formulareq.VariableFriendlyList.Add("سال شمسی", Convert.ToString(shamsiYear));
        }

        var hasDeathDate = req.VariableList != null && req.VariableList.ContainsKey("DeathDate");
        var hasReleaseDate = req.VariableList != null && req.VariableList.ContainsKey("ReleaseDate");
        if (hasDeathDate)
        {
            formulareq.VariableList.Add("DeathDate", req.VariableList!["DeathDate"]);
        }
        if (hasReleaseDate)
        {
            formulareq.VariableList.Add("ReleaseDate", req.VariableList!["ReleaseDate"]);
        }

        if (!hasDeathDate || !hasReleaseDate)
        {
            var employeeIdForDates = req.RecruitOrder?.EmployeeId ?? 0;
            var employeeDateVars = employeeIdForDates > 0
                ? _employeeContext.Employees
                    .AsNoTracking()
                    .Where(e => e.Id == employeeIdForDates)
                    .Select(e => new { e.DeathDate, e.ReleaseDate })
                    .FirstOrDefault()
                : null;

            if (!hasDeathDate)
            {
                formulareq.VariableList.Add(
                    "DeathDate",
                    employeeDateVars?.DeathDate != null
                        ? Convert.ToDouble(Utilities.ConvertDateToNumber(employeeDateVars.DeathDate.Value))
                        : 0);
            }

            if (!hasReleaseDate)
            {
                formulareq.VariableList.Add(
                    "ReleaseDate",
                    employeeDateVars?.ReleaseDate != null
                        ? Convert.ToDouble(Utilities.ConvertDateToNumber(employeeDateVars.ReleaseDate.Value))
                        : 0);
            }
        }

        if (req.BuildTreeTrace == true)
        {
            formulareq.VariableFriendlyList.Add("تاریخ فوت", Convert.ToString(formulareq.VariableList["DeathDate"]));
            formulareq.VariableFriendlyList.Add("تاریخ پایان همکاری", Convert.ToString(formulareq.VariableList["ReleaseDate"]));
        }

        if (req.RecruitOrder.OrganizationJobId > 0)
        {
            var OrganizationJob = _organizationJobService.GetIdAsync(req.RecruitOrder.OrganizationJobId.Value).Result;
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
            formulareq.VariableList.Add("OrganizationJobId", Convert.ToDouble(req.RecruitOrder.OrganizationJobId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه شغل", Convert.ToString(req.RecruitOrder.OrganizationJobId));
                var JobId = _organizationJobService.GetIdAsync(req.RecruitOrder.OrganizationJobId.Value).Result.JobId;
                var jobTitle = _jobService.GetIdAsync(JobId.Value).Result.title;
                formulareq.VariableFriendlyList.Add("عنوان شغل", jobTitle);
            }


            if (OrganizationJob.JobNatureId > 0)
            {
                formulareq.VariableList.Add("JobNatureId", Convert.ToDouble(OrganizationJob.JobNatureId));

                if (req.BuildTreeTrace == true)
                {

                    var title = _jobService._unitOfWork.Context.BaseTableValues.Find(OrganizationJob.JobNatureId);

                    formulareq.VariableFriendlyList.Add("ماهیت شغل", title.title);
                }
            }


        }
        if (req.RecruitOrder.CostCenterId > 0)
        {
            formulareq.VariableList.Add("CostCenterId", Convert.ToDouble(req.RecruitOrder.CostCenterId));

            if (req.BuildTreeTrace == true)
            {
                var CostCenter = _organisationChartService.GetIdAsync(req.RecruitOrder.CostCenterId).Result.title;
                formulareq.VariableFriendlyList.Add("مرکز هزینه", CostCenter);
            }

        }
        if (req.RecruitOrder.WorkPlaceId > 0)
        {
            formulareq.VariableList.Add("WorkPlaceId", Convert.ToDouble(req.RecruitOrder.WorkPlaceId));
            if (req.BuildTreeTrace == true)
            {
                var WorkPlace = _organisationChartService.GetIdAsync(req.RecruitOrder.WorkPlaceId.Value).Result.title;
                formulareq.VariableFriendlyList.Add("محل خدمت", WorkPlace);
            }
        }


        if (req.RecruitOrder.OrganizationUnitId > 0)
        {
            formulareq.VariableList.Add("OrganizationUnitId", Convert.ToDouble(req.RecruitOrder.OrganizationUnitId));
            if (req.BuildTreeTrace == true)
            {
                var OrganizationUnit = _organisationChartService.GetIdAsync(req.RecruitOrder.OrganizationUnitId.Value).Result.title;
                formulareq.VariableFriendlyList.Add("واحد سازمانی", OrganizationUnit);
            }
        }
        if (req.RecruitOrder.ProjectId > 0)
        {
            formulareq.VariableList.Add("ProjectId", Convert.ToDouble(req.RecruitOrder.ProjectId));


            if (req.BuildTreeTrace == true)
            {
                var Project = _projectService.GetIdAsync(req.RecruitOrder.ProjectId.Value).Result.title;
                formulareq.VariableFriendlyList.Add("پروژه", Project);
            }
        }
        if (req.lastorderId > 0)
        {
            formulareq.VariableList.Add("lastorderId", Convert.ToDouble(req.lastorderId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه حکم ماقبل", req.lastorderId.ToString());
            }

            var lastInterdictObject = _db.Set<InterdictOrder>().Find(req.lastorderId);


            if (lastInterdictObject.EducationGradeId.HasValue)
            {
                formulareq.VariableList.Add("OrderEducationGradeId", Convert.ToDouble(lastInterdictObject.EducationGradeId));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add(" مقطع تحصیلی در حکم ", lastInterdictObject.EducationGradeId.ToString());
                }
            }


            if (lastInterdictObject.MarriageStatusId.HasValue)
            {
                formulareq.VariableList.Add("MarriageStatusId", lastInterdictObject.MarriageStatusId.Value);

                if (req.BuildTreeTrace == true)
                {
                    var MarriageStatus = _jobService._unitOfWork.Context.BaseTableValues.Find(lastInterdictObject.MarriageStatusId.Value);
                    formulareq.VariableFriendlyList.Add("وضعیت تاهل در حکم", MarriageStatus.title);
                }
            }
            else
            {
                formulareq.VariableList.Add("MarriageStatusId", 0);
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("وضعیت تاهل در حکم", "نا مشخص");
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
        if (req.InterdictOrder.OrderTypeId > 0)
        {
            formulareq.VariableList.Add("OrderTypeId", Convert.ToDouble(req.InterdictOrder.OrderTypeId));

            if (req.BuildTreeTrace == true)
            {
                var OrderType = _orderTypeService.GetIdAsync(req.InterdictOrder.OrderTypeId).Result.title;
                formulareq.VariableFriendlyList.Add("نوع حکم", OrderType);
            }

        }
        if (req.RecruitOrder.EmployeeStatusId > 0)
        {
            if (req.BuildTreeTrace == true)
            {
                var EmployeeStatus = _employeeStatusService.GetIdAsync(req.RecruitOrder.EmployeeStatusId).Result.title;
                formulareq.VariableFriendlyList.Add("وضعیت استخدامی", EmployeeStatus);
            }
            formulareq.VariableList.Add("EmployeeStatusId", Convert.ToDouble(req.RecruitOrder.EmployeeStatusId));
        }
        if (req.RecruitOrder.OrganisationPositionId > 0)
        {
            var organisationPosition = _organisationPositionService.All().Single(i => i.Id == req.RecruitOrder.OrganisationPositionId);
            if (!string.IsNullOrEmpty(organisationPosition.PositionCode))
            {
                formulareq.VariableList.Add("PositionCode", Convert.ToDouble(organisationPosition.PositionCode));
                if (req.BuildTreeTrace == true)
                {
                    formulareq.VariableFriendlyList.Add("کد پست سازمانی", organisationPosition.PositionCode);
                }
            }
            formulareq.VariableList.Add("OrganisationPositionId", Convert.ToDouble(req.RecruitOrder.OrganisationPositionId));
            formulareq.VariableList.Add("OrganisationPositionDetailId", Convert.ToDouble(req.RecruitOrder.OrganisationPositionId));
            if (req.BuildTreeTrace == true)
            {
                formulareq.VariableFriendlyList.Add("شناسه پست سازمانی ", req.RecruitOrder.OrganisationPositionId.ToString());
            }
        }

        var formulaImpleDate = req.FormulaImpleDate ?? req.PaymentPeriod.StartDate;
        formulareq.StartDate = formulaImpleDate;

        if (req.BuildTreeTrace == true)
        {
            formulareq.VariableFriendlyList.Add("تاریخ اجرا", formulaImpleDate.ToString());
        }
        if (req.InterdictOrderWageItems != null)
        {
            if (req.InterdictOrderWageItems.Any())
            {
                foreach (var item in req.InterdictOrderWageItems)
                {
                    formulareq.VariableList.Add("wf_" + item.WageItemId, Convert.ToDouble(item.Value));

                    if (req.BuildTreeTrace == true)
                    {
                        var separator = new System.Globalization.NumberFormatInfo()
                        {
                            NumberDecimalDigits = 0,
                            NumberGroupSeparator = ","
                        };
                        var wage = _wageItemService.GetIdAsync(item.WageItemId).Result.title;
                        formulareq.VariableFriendlyList.Add(" قلم حکم : " + wage, item.Value.ToString("N", separator));
                    }
                }
            }
        }

        if (req.InterdictOrderCoefficientItems != null)
        {
            if (req.InterdictOrderCoefficientItems.Any())
            {
                foreach (var item in req.InterdictOrderCoefficientItems)
                {
                    formulareq.VariableList.Add("cof_" + item.CoefficientId, Convert.ToDouble(item.OutPutFactValue));
                    if (req.BuildTreeTrace == true)
                    {
                        var separator = new System.Globalization.NumberFormatInfo()
                        {
                            NumberDecimalDigits = 3,
                            NumberGroupSeparator = ","
                        };
                        var Coefficient = _coefficientService.GetIdAsync(item.CoefficientId).Result.title;
                        formulareq.VariableFriendlyList.Add(" ضریب ( عامل ) حکم : " + Coefficient, item.OutPutFactValue.Value.ToString("N", separator));
                    }
                }
            }
        }
        if (req.FicheItems != null)
        {
            if (req.FicheItems.Any())
            {
                foreach (var item in req.FicheItems)
                {
                    formulareq.VariableList.Add("fi_" + item.WageItemId, Convert.ToDouble(item.Value));
                    if (req.BuildTreeTrace == true)
                    {
                        var separator = new System.Globalization.NumberFormatInfo()
                        {
                            NumberDecimalDigits = 3,
                            NumberGroupSeparator = ","
                        };
                        var ficheItem = _wageItemService.GetIdAsync(item.WageItemId).Result.title;
                        formulareq.VariableFriendlyList.Add("  قلم فیش حقوقی : " + ficheItem, item.Value.ToString("N", separator));
                    }
                }
            }
        }

        if (req.SettlementItems != null && req.SettlementItems.Count > 0)
        {
            foreach (var item in req.SettlementItems)
            {
                formulareq.VariableList.Add("stl_" + item.SettlementItemId, item.Value);
                if (req.BuildTreeTrace == true)
                {
                    var separator = new System.Globalization.NumberFormatInfo()
                    {
                        NumberDecimalDigits = 0,
                        NumberGroupSeparator = ","
                    };
                    var settlementItem = _settlementItemService.GetIdAsync(item.SettlementItemId).Result.title;
                    formulareq.VariableFriendlyList.Add(" آیتم تسویه حساب : " + settlementItem, item.Value.ToString("N", separator));
                }
            }
        }

        if (req.PersonnelFunction != null)
        {
            Type myType = req.PersonnelFunction.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            List<string> ingnoreList = new List<string>()
            {
                "id",
                "startdate",
                "enddate",
                "lastmodifieddate",
                "comment",
                "employee",
                "organisationchart",
                "organisationchartid",
                "receivedate",
                "isconfirmed",
                "arearsstatus",
                "confirmdate",
                "attendanceid",
                "costcenter",
                "organizationunit",
                "workplace",
                "costcenterid",
                "organizationunitid",
                "workplaceid",
            };

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(req.PersonnelFunction, null);
                if (propValue != null)
                {
                    var isNumeric = double.TryParse(propValue.ToString(), out double n);
                    if (isNumeric)
                    {
                        if (!formulareq.VariableList.Any(i => i.Key == prop.Name))
                        {
                            if (ingnoreList.Contains(prop.Name.Trim().ToLower()))
                            {
                                continue;
                            }
                            formulareq.VariableList.Add(prop.Name, Convert.ToInt64(propValue));
                            if (req.BuildTreeTrace == true)
                            {
                                var relatedOperand = _formulaOperandService.All().Where(i => i.EnglishName.Trim().ToLower() == prop.Name.Trim().ToLower());
                                if (relatedOperand == null)
                                {
                                    formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, propValue.ToString());
                                }
                                else
                                {
                                    if (relatedOperand.Any())
                                    {
                                        var setting = relatedOperand.Single();
                                        if (string.IsNullOrEmpty(setting.title))
                                        {
                                            formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, propValue.ToString());
                                        }
                                        else
                                        {
                                            formulareq.VariableFriendlyList.Add(" کارکرد : " + setting.title, propValue.ToString());
                                        }
                                    }
                                    else
                                    {
                                        formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, propValue.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ingnoreList.Contains(prop.Name.Trim().ToLower()))
                    {
                        continue;
                    }
                    if (!formulareq.VariableList.Any(i => i.Key == prop.Name))
                    {
                        formulareq.VariableList.Add(prop.Name, 0);

                        if (req.BuildTreeTrace == true)
                        {
                            var relatedOperand = _formulaOperandService.All().Where(i => i.EnglishName.Trim().ToLower() == prop.Name.Trim().ToLower());
                            if (relatedOperand == null)
                            {
                                formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, "0");
                            }
                            else
                            {
                                if (relatedOperand.Any())
                                {
                                    var setting = relatedOperand.Single();
                                    if (string.IsNullOrEmpty(setting.title))
                                    {
                                        formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, "0");
                                    }
                                    else
                                    {
                                        formulareq.VariableFriendlyList.Add(" کارکرد : " + setting.title, "0");
                                    }
                                }
                                else
                                {
                                    formulareq.VariableFriendlyList.Add(" کارکرد : " + prop.Name, "0");
                                }
                            }
                        }

                    }
                }
            }
        }
        if (req.VariableList != null)
        {
            foreach (var item in req.VariableList)
            {
                if (formulareq.VariableList.Any(i => i.Key == item.Key))
                {

                }
                else
                {
                    formulareq.VariableList.Add(item.Key, item.Value);
                    if (req.BuildTreeTrace == true)
                    {
                        string friendlyKey = item.Key;
                        if (item.Key.Trim().ToLower().Contains("operand_"))
                        {
                            var formulaOperand = _formulaOperandService.All(ImpleDate: formulareq.StartDate.Value).Where(i => i.Id == Convert.ToInt32(item.Key.Replace("operand_", "")));
                            if (formulaOperand == null)
                            {
                                //ret.Succees = false;
                                //ret.ResponseMessage = "عملوند " + item + " استفاده شده ولی تعریف آن یافت نشد یا منقضی شده است ";
                                //return ret;
                            }
                            else
                            {
                                if (formulaOperand.Any())
                                {
                                    friendlyKey = formulaOperand.Single().title;
                                }
                            }
                        }
                        var separator = new System.Globalization.NumberFormatInfo()
                        {
                            NumberDecimalDigits = 3,
                            NumberGroupSeparator = ","
                        };
                        formulareq.VariableFriendlyList.Add(friendlyKey, item.Value.ToString("N", separator));
                    }
                }
            }
        }
        formulareq.BuildTreeTrace = req.BuildTreeTrace;
        return _formulaService.Calculate(formulareq);

    }

    public void BatchCalculation()
    {
        try
        {
            _logger.LogInformation("شروع BatchCalculation - محاسبه گروهی فیش");

            // بهینه‌سازی عملکرد: پاک‌سازی ChangeTracker و غیرفعال‌سازی موقت AutoDetectChanges
            _unitOfWork.Context.ChangeTracker.Clear();
            var originalAutoDetectChanges = _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled;
            var originalQueryTracking = _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior;

            _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _logger.LogInformation("بهینه‌سازی ChangeTracker فعال شد: AutoDetectChanges=false, QueryTracking=NoTracking");

            try
            {
                var validStates = new List<long>()
                {
                    (long)Enums.BatchPayRollRequestState.Initial,
                    (long)Enums.BatchPayRollRequestState.TryAgain,
                };
                var validRequests = _unitOfWork.Context.BatchPayRollRequests
                    .Where(i => validStates.Contains(i.RequestStateId)
                                && i.PaymentPeriodId > 0
                                && i.RequestTypeId == (long)Enums.BatchPayRollRequestType.NormalFicheCalculation)
                    .ToList();

                _logger.LogInformation("تعداد {Count} درخواست محاسبه فیش یافت شد", validRequests.Count());

                foreach (var Request in validRequests)
                {
                    _logger.LogInformation("شروع پردازش درخواست {RequestId}", Request.Id);
                    try
                    {
                        Request.RequestStateId = (long)Enums.BatchPayRollRequestState.Running;
                        Request.LastPoolingTime = DateTime.Now;
                        Request.StartDate = DateTime.Now;
                        _unitOfWork.Context.Update(Request);
                        _unitOfWork.Context.SaveChanges();

                        var requestDetails = _db.Set<BatchPayRollRequestDetail>()
                            .Where(i => i.BatchPayRollRequestId == Request.Id)
                            .ToList();

                        if (requestDetails == null || requestDetails.Count == 0)
                        {
                            // هیچ ردیف جزئیاتی وجود ندارد؛ علت عدم محاسبه را روی خود درخواست ثبت می‌کنیم
                            _logger.LogWarning("درخواست {RequestId} هیچ ردیف جزئیات (Detail) ندارد؛ هیچ فیشی محاسبه نمی‌شود", Request.Id);
                            Request.ExeptionMessage = "هیچ ردیف جزئیاتی (کارمندی) برای این درخواست یافت نشد - هیچ فیشی محاسبه نشد";
                        }

                        foreach (var requestDetail in requestDetails)
                        {
                            // مرحله ۰: ثبت تضمین‌شده «شروع پردازش ردیف» با SQL مستقل از وضعیت EF
                            // تا اگر در ادامه کار گیر کرد یا پروسه از بین رفت، مشخص باشد کار تا کجا رسیده است
                            SafeLogBatchDetailProgress(requestDetail.Id, "شروع پردازش ردیف - آماده‌سازی محاسبه");

                            try
                            {
                                Request.PoolingEmployeeId = requestDetail.EmployeeId;
                                _unitOfWork.Context.Update(Request);
                                _unitOfWork.Context.SaveChanges();

                                requestDetail.LastTryDateTime = DateTime.Now;
                                // ثبت وضعیت «در حال پردازش» در حافظه تا ذخیره EF نیز پیام معنادار داشته باشد
                                requestDetail.FinalMessage = "در حال پردازش محاسبه فیش ...";
                                _unitOfWork.Context.Update(requestDetail);
                                _unitOfWork.Context.SaveChanges();

                                if (Request.PaymentPeriodId.HasValue)
                                {
                                    requestDetail.StartDate = DateTime.Now;
                                    _currentUserDefaultOrganId = Request.OrganisationChartId;
                                    //_IP = "SystemJOB";
                                    //_UserName = "HrBackGroundJob";

                                    // مرحله ۱: درست قبل از فراخوانی محاسبه - اگر CalculateFiche گیر کند یا
                                    // پاسخ ندهد، این پیام در جدول باقی می‌ماند و نقطه توقف را مشخص می‌کند
                                    SafeLogBatchDetailProgress(requestDetail.Id,
                                        "در حال محاسبه فیش (CalculateFiche فراخوانی شد - منتظر پاسخ) ...");

                                    // اجرای محاسبه با محدودیت زمانی در Scope مستقل؛ اگر به مهلت رسید،
                                    // به‌صورت امن رها می‌شود تا کل فرآیند گروهی متوقف نشود.
                                    var result = RunCalculateFicheWithTimeout(
                                        requestDetail.EmployeeId,
                                        Request.PaymentPeriodId.Value,
                                        Request.OrganisationChartId,
                                        out bool ficheTimedOut);

                                    requestDetail.EndDate = DateTime.Now;
                                    if (requestDetail.StartDate.HasValue && requestDetail.EndDate.HasValue)
                                    {
                                        requestDetail.RunTimeinMilliseconds =
                                            (requestDetail.EndDate.Value - requestDetail.StartDate.Value)
                                            .TotalMilliseconds;
                                    }

                                    if (ficheTimedOut)
                                    {
                                        // محاسبه این کارمند در مهلت مجاز تمام نشد؛ ثبت تضمین‌شده و عبور به کارمند بعدی
                                        var timeoutMsg = $"محاسبه فیش این کارمند در مهلت مجاز ({PerEmployeeCalculationTimeoutSeconds} ثانیه) به پایان نرسید و برای جلوگیری از توقف کل فرآیند رها شد (Timeout)";
                                        requestDetail.FinalMessage = timeoutMsg;
                                        _logger.LogWarning(
                                            "Timeout در محاسبه فیش برای کارمند {EmployeeId} در درخواست {RequestId} پس از {Timeout} ثانیه",
                                            requestDetail.EmployeeId, Request.Id, PerEmployeeCalculationTimeoutSeconds);
                                        SafeLogBatchDetailProgress(requestDetail.Id, timeoutMsg, markEnd: true);
                                        continue;
                                    }

                                    if (result == null)
                                    {
                                        // مرحله ۲: نتیجه null - حالت غیرمنتظره را هم لاگ می‌کنیم
                                        requestDetail.FinalMessage = "نتیجه محاسبه نامشخص بود (CalculateFiche مقدار null بازگرداند)";
                                    }
                                    else if (result.Success == true)
                                    {
                                        if (result.Payload is FicheDTO ficheDto && ficheDto.ExsitingFicheId > 0)
                                        {
                                            requestDetail.FicheId = ficheDto.ExsitingFicheId;
                                            requestDetail.FinalMessage = "ثبت موفق";
                                            requestDetail.DoDatetime = DateTime.Now;

                                            Request.SuccessCount = Request.SuccessCount + 1;
                                            _unitOfWork.Context.Update(Request);
                                            _unitOfWork.Context.SaveChanges();
                                        }
                                        else
                                        {
                                            requestDetail.FinalMessage = "محاسبه موفق بود ولی شماره فیش بازگشت داده نشد";
                                        }
                                    }
                                    else
                                    {
                                        requestDetail.FinalMessage = string.IsNullOrWhiteSpace(result.Message)
                                            ? "محاسبه ناموفق بود (بدون پیام مشخص)"
                                            : result.Message;
                                    }

                                    // مرحله ۳: ذخیره نتیجه نهایی ردیف - با Fallback تضمین‌شده در صورت خطای EF
                                    TrySaveBatchDetailResult(requestDetail);
                                }
                                else
                                {
                                    // PaymentPeriodId ندارد؛ علت را در ردیف ثبت می‌کنیم تا گنگ نماند
                                    requestDetail.FinalMessage = "دوره پرداخت (PaymentPeriodId) برای این درخواست تعیین نشده است";
                                    TrySaveBatchDetailResult(requestDetail);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "خطا در محاسبه فیش برای کارمند {EmployeeId} در درخواست {RequestId}",
                                    requestDetail.EmployeeId, Request.Id);

                                // ساخت پیام کامل شامل InnerException برای عیب‌یابی دقیق
                                var errorMessage = "خطا: " + (ex.Message ?? "نامشخص");
                                if (ex.InnerException != null)
                                {
                                    errorMessage += " | علت داخلی: " + ex.InnerException.Message;
                                }

                                // ابتدا تلاش با EF؛ در صورت خراب بودن Context، با SQL مستقیم تضمین می‌کنیم
                                if (!TrySetBatchDetailFinalMessageEf(requestDetail, errorMessage))
                                {
                                    SafeLogBatchDetailProgress(requestDetail.Id, errorMessage, markEnd: true);
                                }
                            }
                        }

                        Request.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                        Request.FinishDateTime = DateTime.Now;
                        Request.IsDone = true;
                        Request.EndDate = DateTime.Now;
                        _unitOfWork.Context.Update(Request);
                        _unitOfWork.Context.SaveChanges();
                        _logger.LogInformation("پایان موفق پردازش درخواست {RequestId} - {SuccessCount} فیش موفق",
                            Request.Id, Request.SuccessCount);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در پردازش درخواست {RequestId}", Request.Id);
                        try
                        {
                            Request.RequestStateId = (long)Enums.BatchPayRollRequestState.EndLoop;
                            Request.FinishDateTime = DateTime.Now;
                            _unitOfWork.Context.Update(Request);
                            _unitOfWork.Context.SaveChanges();
                        }
                        catch (Exception saveEx)
                        {
                            _logger.LogError(saveEx,
                                "خطا در ذخیره وضعیت نهایی درخواست {RequestId}", Request.Id);
                        }
                    }
                }

                _logger.LogInformation("پایان BatchCalculation - محاسبه گروهی فیش");
            }
            finally
            {
                // بازگرداندن تنظیمات ChangeTracker به حالت اولیه
                _unitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
                _unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
                _logger.LogInformation(
                    "تنظیمات ChangeTracker به حالت اولیه بازگشت: AutoDetectChanges={AutoDetect}, QueryTracking={QueryTracking}",
                    originalAutoDetectChanges, originalQueryTracking);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای کلی در BatchCalculation");
            throw;
        }
    }

    /// <summary>
    /// ثبت تضمین‌شده پیام پیشرفت/خطا در ردیف درخواست (Batch_PayRoll_Request_Detail) با استفاده از یک
    /// اتصال SQL مستقل. این متد عمداً مستقل از وضعیت EF/ChangeTracker است تا حتی اگر Context در حالت
    /// نامعتبر باشد یا محاسبه گیر کرده باشد، باز هم نقطه‌ای که کار به آن رسیده در جدول ثبت شود.
    /// این متد هرگز Exception پرتاب نمی‌کند تا روند اصلی متوقف نشود.
    /// </summary>
    private void SafeLogBatchDetailProgress(long detailId, string message, bool markEnd = false)
    {
        try
        {
            if (detailId <= 0)
            {
                return;
            }

            // جلوگیری از سرریز احتمالی ستون با کوتاه‌سازی پیام‌های بسیار طولانی
            if (!string.IsNullOrEmpty(message) && message.Length > 3900)
            {
                message = message.Substring(0, 3900);
            }

            var sql = markEnd
                ? @"UPDATE [Payroll].[Batch_PayRoll_Request_Detail]
                       SET [FinalMessage] = @msg, [LastTryDateTime] = @now, [LastModifiedDate] = @now, [EndDate] = @now
                     WHERE [Id] = @id"
                : @"UPDATE [Payroll].[Batch_PayRoll_Request_Detail]
                       SET [FinalMessage] = @msg, [LastTryDateTime] = @now, [LastModifiedDate] = @now
                     WHERE [Id] = @id";

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandTimeout = 30;
            cmd.Parameters.AddWithValue("@msg", (object)message ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@now", DateTime.Now);
            cmd.Parameters.AddWithValue("@id", detailId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // حتی اگر ثبت لاگ هم ناموفق بود، نباید روند اصلی را متوقف کند
            _logger.LogError(ex, "ثبت لاگ پیشرفت برای ردیف {DetailId} ناموفق بود", detailId);
        }
    }

    /// <summary>
    /// ذخیره نتیجه نهایی ردیف از طریق EF و در صورت خطا، Fallback تضمین‌شده با SQL مستقیم برای ثبت پیام.
    /// </summary>
    private void TrySaveBatchDetailResult(BatchPayRollRequestDetail requestDetail)
    {
        try
        {
            _unitOfWork.Context.Update(requestDetail);
            _unitOfWork.Context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ذخیره نتیجه ردیف {DetailId} با EF ناموفق بود؛ تلاش با SQL مستقیم", requestDetail?.Id);
            if (requestDetail != null)
            {
                var msg = string.IsNullOrWhiteSpace(requestDetail.FinalMessage)
                    ? "ذخیره نتیجه ناموفق بود"
                    : requestDetail.FinalMessage;
                SafeLogBatchDetailProgress(requestDetail.Id, msg, markEnd: true);
            }
        }
    }

    /// <summary>
    /// تلاش برای ثبت پیام نهایی روی ردیف با EF. در صورت موفقیت true و در صورت خطا false برمی‌گرداند
    /// تا فراخواننده بتواند به Fallback با SQL مستقیم سوییچ کند.
    /// </summary>
    private bool TrySetBatchDetailFinalMessageEf(BatchPayRollRequestDetail requestDetail, string message)
    {
        try
        {
            if (requestDetail == null)
            {
                return false;
            }

            requestDetail.FinalMessage = message;
            requestDetail.EndDate = DateTime.Now;
            _unitOfWork.Context.Update(requestDetail);
            _unitOfWork.Context.SaveChanges();
            return true;
        }
        catch (Exception saveEx)
        {
            _logger.LogError(saveEx, "ثبت پیام خطا روی ردیف {DetailId} با EF ناموفق بود", requestDetail?.Id);
            return false;
        }
    }

    /// <summary>
    /// حداکثر مهلت مجاز محاسبه فیش برای هر کارمند (بر حسب ثانیه). اگر محاسبه یک کارمند بیش از این مدت
    /// طول بکشد، به‌صورت امن رها می‌شود تا کل فرآیند گروهی بلاک نشود. مقدار به‌اندازه کافی بزرگ است که
    /// محاسبات سنگینِ معتبر قطع نشوند، ولی از گیر کردن نامحدود جلوگیری می‌کند.
    /// </summary>
    private const int PerEmployeeCalculationTimeoutSeconds = 300;

    /// <summary>
    /// اجرای <see cref="CalculateFiche"/> برای یک کارمند با محدودیت زمانی (Wall-clock Timeout) و در یک
    /// Scope/DbContext کاملاً مستقل.
    ///
    /// چرا Scope مستقل؟ چون <c>DbContext</c> مشترکِ حلقه اصلی Thread-Safe نیست؛ اگر محاسبه روی همان
    /// Context اجرا و سپس رها می‌شد، ادامه‌ی کار حلقه روی همان Context باعث خرابی می‌شد. با Scope جدا،
    /// در صورت Timeout می‌توان Task را به‌صورت امن رها کرد بدون اینکه روی حلقه‌ی اصلی اثر بگذارد.
    /// </summary>
    /// <param name="timedOut">در صورت رسیدن به مهلت، true می‌شود.</param>
    private OperationResult RunCalculateFicheWithTimeout(long employeeId, long paymentPeriodId, long organisationChartId, out bool timedOut)
    {
        timedOut = false;

        // Scope مستقل؛ تا پایان واقعی Task باز می‌ماند (در صورت Timeout هم بعداً در ContinueWith بسته می‌شود)
        var scope = _scopeFactory.CreateScope();
        OperationResult? capturedResult = null;
        Exception? capturedException = null;

        var calculationTask = Task.Run(() =>
        {
            try
            {
                var isolatedFicheService = scope.ServiceProvider.GetRequiredService<FicheService>();
                // انتقال تنها وضعیت لازم برای محاسبه به نمونه‌ی مستقل
                isolatedFicheService._currentUserDefaultOrganId = organisationChartId;

                capturedResult = isolatedFicheService.CalculateFiche(
                    employeeId,
                    paymentPeriodId,
                    organisationChartId,
                    false,
                    true,
                    false);
            }
            catch (Exception ex)
            {
                capturedException = ex;
            }
        });

        // Dispose کردن Scope فقط پس از پایان واقعی Task (چه به‌موقع تمام شود، چه بعد از Timeout رها شده باشد)
        _ = calculationTask.ContinueWith(_ =>
        {
            try { scope.Dispose(); }
            catch { /* نادیده گرفته می‌شود */ }
        }, TaskScheduler.Default);

        bool finishedInTime = calculationTask.Wait(TimeSpan.FromSeconds(PerEmployeeCalculationTimeoutSeconds));

        if (!finishedInTime)
        {
            // محاسبه هنوز در حال اجراست؛ آن را رها می‌کنیم. Task روی Context ایزوله‌ی خودش ادامه می‌یابد
            // و در نهایت (با تمام شدن یا Timeout سطح SQL) خاتمه یافته و Scope را Dispose می‌کند.
            timedOut = true;
            return null;
        }

        if (capturedException != null)
        {
            // خطا را با حفظ Stack اصلی به فراخواننده منتقل می‌کنیم تا در catch موجود لاگ و در ردیف ثبت شود
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(capturedException).Throw();
        }

        return capturedResult;
    }

    /// <summary>
    /// محاسبه فیش حقوقی
    /// </summary>
    /// <returns></returns>
    public OperationResult CalculateFiche(long EmployeeId, long PaymentPeriodId, long PayLocationId, bool BuildTreeTrace, bool SaveFiche, bool IsArear, long ArearsInterdictId = 0)
    {
        FicheDTO ret = new();
        var CurrentPeriod = _db.Set<PaymentPeriod>()
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == PaymentPeriodId);
        ret.PaymentPeriodId = PaymentPeriodId;

        var existingFicheId = _unitOfWork.Context.Fiches
            .AsNoTracking()
            .Where(i => i.EmployeeId == EmployeeId && i.PaymentPeriodId == PaymentPeriodId && i.IsDeleted != true)
            .Select(i => (long?)i.Id)
            .FirstOrDefault();

        if (existingFicheId.HasValue)
        {
            if (BuildTreeTrace == false)/// یعنی درخواست صدور گروهی است
            {
                if (IsArear == true)
                {

                }
                else
                {
                    return OperationResult.Failed("برای دوره مورد نظر فیش وجود دارد");
                }
            }

            ret.ExsitingFicheId = existingFicheId.Value;
            ret.HasExistingFiche = true;

            // بارگذاری اطلاعات مرخصی های فیش موجود
            ret.FicheLeaveItemDTOs = _unitOfWork.Context.FicheLeaveItems
                .AsNoTracking()
                .Include(i => i.LeaveType)
                .Include(i => i.PersonnelLeave)
                .Where(i => i.FicheId == ret.ExsitingFicheId && i.IsDeleted == false)
                .Select(item => new FicheLeaveItemDTO
                {
                    FicheId = item.FicheId,
                    LeaveTypeId = item.LeaveTypeId,
                    LeaveType = item.LeaveType != null ? item.LeaveType.title : null,
                    PersonnelLeaveId = item.PersonnelLeaveId,
                    PersonnelLeave = item.PersonnelLeave != null ? item.PersonnelLeave.Description : null,
                    LeaveAmountTicks = item.LeaveAmountTicks,
                    LeaveBalanceTicks = item.LeaveBalanceTicks
                })
                .ToList();
        }
        else
        {
            ret.HasExistingFiche = false;
        }

        if (CurrentPeriod == null)
        {
            return OperationResult.Failed("لطفا از بخش تنظیمات حساب کاربری دوره پیش فرض را انتخاب بفرمایید");
        }

        if (PayLocationId != CurrentPeriod.OrganisationChartId)
        {
            return OperationResult.NotFound("شناسه محل پرداخت و سازمان جاری مطابقت ندارد");
        }

        var isBlacklisted = _unitOfWork.Context.BlackLists
            .AsNoTracking()
            .Where(DateValidityExtension<BlackList>.GetDateValidationPredicate(IgnoreExpired: true, ImpleDate: CurrentPeriod.StartDate))
            .Any(i => i.EmployeeId == EmployeeId
                && i.OrganisationChartId == PayLocationId
                && i.WillBeCalculated != true);

        if (isBlacklisted)
        {
            return OperationResult.Failed("شخص مورد نظر داخل فهرست سیاه محاسبه می باشد");
        }

        PersonnelFunction PersonnelFunction = null;
        InterdictOrder InterdictOrder = null;
        RecruitOrder RecruitOrder = null;

        #region انتخاب حکم و ردیف کارکرد

        long lastOrderId = 0;

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();

            if (IsArear)
            {
                lastOrderId = ArearsInterdictId;
            }
            else
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[Order].[GetLastOrderByImpleDate]";
                    command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    command.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = CurrentPeriod.StartDate.GetValueOrDefault().Date;
                    command.Parameters.AddWithValue("@CorrectionOrderId", 0);
                    command.Parameters.AddWithValue("@IsForFiche", true);
                    command.Parameters.AddWithValue("@PayLocationId", PayLocationId);
                    command.Parameters.AddWithValue("@PaymentPeriodId", PaymentPeriodId);
                    SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    command.ExecuteNonQuery();
                    lastOrderId = Convert.ToInt32(returnValue.Value);
                }

                if (lastOrderId <= 0)
                {
                    return OperationResult.NotFound("حکم فعال یافت نشد");
                }
            }

            using (SqlCommand cmd = new SqlCommand("[Payroll].[Get_Personnel_Function_ForFiche]", con))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                cmd.Parameters.AddWithValue("@PaymentPeriodId", PaymentPeriodId);
                cmd.Parameters.AddWithValue("@PayLocationId", PayLocationId);
                cmd.CommandType = CommandType.StoredProcedure;

                using SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    PersonnelFunction = rdr.ConvertToObject<PersonnelFunction>();
                }
            }
        }

        if (PersonnelFunction == null || PersonnelFunction.Id == 0)
        {
            return OperationResult.NotFound(" کارکردی برای دوره ارسالی یافت نشد");
        }
        else
        {
            ret.PersonnelFunction = _mapper.Map<PersonnelFunctionDTO>(PersonnelFunction);
            if (ret.PersonnelFunction.PersonnelFunctionDay > 0)
            {

            }
            else
            {
                return OperationResult.NotFound("روز های کاری کارکرد ارسال شد نا معتبر می باشد");
            }

            if (ret.PersonnelFunction.PersonnelIllnessDay > 3)
            {
                return OperationResult.NotFound("روز های استعلاجی نمی تواند بیشتر از 3 روز باشد");
            }

            ret.PersonnelFunctionId = PersonnelFunction.Id;
        }

        var orderData = (
            from interdictOrder in _unitOfWork.Context.InterdictOrders.AsNoTracking()
            join recruitOrder in _unitOfWork.Context.RecruitOrders.AsNoTracking()
                on interdictOrder.RecruitOrderId equals recruitOrder.Id
            where interdictOrder.Id == lastOrderId
            select new { InterdictOrder = interdictOrder, RecruitOrder = recruitOrder }
        ).FirstOrDefault();

        if (orderData == null)
        {
            return OperationResult.NotFound(" حکم حقوقی یافت نشد ");
        }

        InterdictOrder = orderData.InterdictOrder;
        RecruitOrder = orderData.RecruitOrder;
        ret.InterdictOrderId = InterdictOrder.Id;
        ret.InterdictOrderSerial = InterdictOrder.Serial;

        List<InterdictOrderCoefficientItem> InterdictOrderCoefficientItem = _unitOfWork.Context.InterdictOrderCoefficientItems
            .AsNoTracking()
            .Where(i => i.InterdictOrderId == lastOrderId)
            .ToList();

        #endregion انتخاب حکم و ردیف کارکرد
        #region واکشی تنظیمات

        List<OrganisationEmployeeTypeFicheItemDTO> executAbleSetting;
        (bool flowControl, OperationResult retvalue) = GetComputeSettings(EmployeeId, ret, CurrentPeriod, RecruitOrder, out executAbleSetting);
        if (!flowControl)
        {
            return retvalue;
        }

        List<InterdictOrderWageItem> orderWageItems = new List<InterdictOrderWageItem>();
        List<OrganisationEmployeeTypeOrderTypeWageItemDTO> orderWageSettingList = new();
        List<OrganisationEmployeeTypeOrderTypeCoefficientDTO> orderCoefficientSettingList = new();
        var impleDate = CurrentPeriod.StartDate.GetValueOrDefault().Date;

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Order].Interdict_Order_WageItem WHERE InterdictOrderId = @InterdictOrderId", con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@InterdictOrderId", SqlDbType.BigInt).Value = lastOrderId;
                using SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    orderWageItems.Add(rdr.ConvertToObject<InterdictOrderWageItem>());
                }
            }

            using (SqlCommand cmd = new SqlCommand("[Order].[GetCurrentPayLocationEmployeeTypeOrderTypeWageItems]", con))
            {
                cmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = RecruitOrder.PayLocationId;
                cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = RecruitOrder.EmployeeTypeId;
                cmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = InterdictOrder.OrderTypeId;
                cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = impleDate;
                cmd.CommandType = CommandType.StoredProcedure;
                using SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    orderWageSettingList.Add(rdr.ConvertToObject<OrganisationEmployeeTypeOrderTypeWageItemDTO>());
                }
            }

            using (SqlCommand cmd = new SqlCommand("[Order].[GetCurrentPayLocationEmployeeTypeOrderTypeCoefficients]", con))
            {
                cmd.Parameters.Add("@PayLocationId", SqlDbType.BigInt).Value = RecruitOrder.PayLocationId;
                cmd.Parameters.Add("@EmployeeTypeId", SqlDbType.BigInt).Value = RecruitOrder.EmployeeTypeId;
                cmd.Parameters.Add("@OrderTypeId", SqlDbType.BigInt).Value = InterdictOrder.OrderTypeId;
                cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = impleDate;
                cmd.CommandType = CommandType.StoredProcedure;
                using SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    orderCoefficientSettingList.Add(rdr.ConvertToObject<OrganisationEmployeeTypeOrderTypeCoefficientDTO>());
                }
            }
        }

        if (orderWageItems.Count == 0)
        {
            return OperationResult.NotFound(" آیتم حقوی حکم یافت نشد ");
        }

        var hasPositiveWageItem = false;
        for (var wageItemIndex = 0; wageItemIndex < orderWageItems.Count; wageItemIndex++)
        {
            if (orderWageItems[wageItemIndex].Value > 0)
            {
                hasPositiveWageItem = true;
                break;
            }
        }

        if (!hasPositiveWageItem)
        {
            return OperationResult.NotFound(" آیتم حقوی حکم یافت نشد ");
        }

        /// پر کردن عامل های نوع استخدام که موجود نیستند
        var orderWageItemsByWageItemId = orderWageItems.ToDictionary(i => i.WageItemId);
        foreach (var item in orderWageSettingList)
        {
            if (orderWageItemsByWageItemId.TryGetValue(item.WageItemId, out var existingWageItem))
            {
                if (item.IsSanavatINC)
                {
                    ret.IncAmount = existingWageItem.Value;
                }
            }
            else
            {
                var missingWageItem = new InterdictOrderWageItem()
                {
                    WageItemId = item.WageItemId,
                    Value = 0,
                    WageItem = new BaseInfo.Core.Entities.WageItem()
                    {
                        Id = item.WageItemId,
                        title = item.WageItem
                    }
                };
                orderWageItems.Add(missingWageItem);
                orderWageItemsByWageItemId[item.WageItemId] = missingWageItem;
            }
        }

        var existingCoefficientIds = new HashSet<long>(InterdictOrderCoefficientItem.Select(i => i.CoefficientId));
        foreach (var item in orderCoefficientSettingList)
        {
            if (existingCoefficientIds.Add(item.CoefficientId))
            {
                InterdictOrderCoefficientItem.Add(new InterdictOrderCoefficientItem()
                {
                    CoefficientId = item.CoefficientId,
                    OutPutFactValue = 0,
                    Coefficient = new BaseInfo.Core.Entities.Coefficient
                    {
                        Id = item.CoefficientId,
                        title = item.Coefficient
                    }
                });
            }
        }

        #endregion واکشی تنظیمات

        var organisationLoanWageItemIds = _unitOfWork.Context.LoanTypes
            .AsNoTracking()
            .Where(lt => lt.OrganisationChartId == PayLocationId && lt.IsDeleted != true)
            .Select(lt => lt.WageItemId)
            .Distinct()
            .ToHashSet();

        #region CalculateItems

        int index = 1;
        List<FicheItem> runTimeFicheItems = new List<FicheItem>(executAbleSetting.Count);
        executAbleSetting = executAbleSetting.OrderBy(i => i.Priority).ToList();

        var formulaRequest = new CommunicateWithFormulaRequest
        {
            BuildTreeTrace = BuildTreeTrace,
            DoFinalCalc = true,
            InterdictOrder = InterdictOrder,
            InterdictOrderCoefficientItems = InterdictOrderCoefficientItem,
            InterdictOrderWageItems = orderWageItems,
            lastorderId = lastOrderId,
            PaymentPeriod = CurrentPeriod,
            PersonnelFunction = PersonnelFunction,
            RecruitOrder = RecruitOrder,
            FicheItems = runTimeFicheItems,
        };

        foreach (var wageRow in executAbleSetting)
        {
            FicheItem runTimerow = new FicheItem()
            {
                WageItemId = wageRow.WageItemId,
            };
            try
            {
                wageRow.Index = index;
                switch (wageRow.EnterTypeId)
                {
                    case (long)Enums.PayRollEnterTypeId.EqualTolastRec:

                        if (orderWageItemsByWageItemId.TryGetValue(wageRow.WageItemId, out var orderWageItem))
                        {
                            wageRow.Amount = orderWageItem.Value;
                        }
                        wageRow.IsRowSuccess = true;
                        runTimerow.Value = wageRow.Amount;
                        break;
                    case (long)Enums.PayRollEnterTypeId.fixValue:

                        wageRow.Amount = wageRow.FixValue.Value;
                        wageRow.IsRowSuccess = true;
                        runTimerow.Value = wageRow.Amount;
                        break;

                    case (long)Enums.PayRollEnterTypeId.UseFormula:
                        formulaRequest.VariableList = null;
                        if (wageRow.OriginId == (long)Enums.OriginOfFicheItem.PersonnelFicheItem && wageRow.FixValue != null)
                        {
                            formulaRequest.VariableList = new Dictionary<string, double>
                                {
                                    { "operand_10179", Convert.ToDouble(wageRow.FixValue.Value) } // مقدار عامل حقوقی اختصاصی فرد (عامل جاری )
                                };
                        }
                        else if (wageRow.OriginId == (long)Enums.OriginOfFicheItem.CostCenter && wageRow.FixValue != null)
                        {
                            formulaRequest.VariableList = new Dictionary<string, double>
                                {
                                    { "operand_10180", Convert.ToDouble(wageRow.FixValue.Value) } //مقدار عامل حقوقی مرکز هزینه (عامل جاری )
                                };
                        }

                        var formulaRespone = CommunicateWithFormula(wageRow.OrganisationFormulaId.Value, formulaRequest);
                        if (BuildTreeTrace == true)
                        {
                            wageRow.FormulaFriendlyText = formulaRespone.FormulaFriendlyText;
                            wageRow.VariableFriendlyList = formulaRespone.VariableFriendlyList;
                            wageRow.FormulaText = formulaRespone.FormulaText;
                            wageRow.FormulaTreeParser = formulaRespone.FormulaTreeParser;
                            wageRow.FormulaHelpDesc = formulaRespone.FormulaHelpDesc;
                        }
                        wageRow.Amount = formulaRespone.Result;
                        wageRow.IsRowSuccess = formulaRespone.Succees;
                        wageRow.SuccessRunTimeInmilliseconds = formulaRespone.SuccessRunTimeInmilliseconds;
                        if (!formulaRespone.Succees)
                        {
                            wageRow.formularowmessage = formulaRespone.ResponseMessage + " ";
                            wageRow.IsRowSuccess = false;
                            //  return OperationResult.Succeeded(payload: wageRow);
                        }
                        else
                        {
                            wageRow.formularowmessage = "Ok";
                        }
                        runTimerow.Value = wageRow.Amount;
                        break;


                    default:
                        if (organisationLoanWageItemIds.Contains(wageRow.WageItemId))
                        {
                            wageRow.Amount = 0;
                            wageRow.IsRowSuccess = true;
                            wageRow.formularowmessage = "Ok";
                            runTimerow.Value = 0;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                return OperationResult.Failed("خطا در محاسبه عامل : " + wageRow.WageItem, payload: ret);
            }

            index++;
            runTimeFicheItems.Add(runTimerow);
        }



        #endregion CalculateItems
        #region Karane

        var karaneItems = _db.Set<OrganisationEmployeeTypeCoefficientBonusWageItem>()
            .AsNoTracking()
            .Include(i => i.WageItem)
            .Where(i => i.OrganisationChartId == RecruitOrder.PayLocationId && i.EmployeeTypeId == RecruitOrder.EmployeeTypeId)
            .ToList();

        if (karaneItems.Count > 0)
        {
            var employeeBonuses = _db.Set<PaymentPeriodEmployeeBonus>()
                .AsNoTracking()
                .Where(i => i.PaymentPeriodId == PaymentPeriodId && i.EmployeeId == RecruitOrder.EmployeeId)
                .ToDictionary(i => i.CoefficientId, i => i.Value);

            var positiveCoefficientsById = new Dictionary<long, double>();
            foreach (var coefficientItem in InterdictOrderCoefficientItem)
            {
                if (coefficientItem.OutPutFactValue > 0)
                {
                    positiveCoefficientsById[coefficientItem.CoefficientId] = coefficientItem.OutPutFactValue ?? 0;
                }
            }

            int? karanePriority = null;
            for (var settingIndex = executAbleSetting.Count - 1; settingIndex >= 0; settingIndex--)
            {
                var settingRow = executAbleSetting[settingIndex];
                if (settingRow.PaymentTypeId == (long)Enums.PaymentType.Deduction)
                {
                    karanePriority = settingRow.Priority - 1;
                    break;
                }
            }

            if (!karanePriority.HasValue)
            {
                karanePriority = executAbleSetting.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Deduction).OrderBy(i => i.Priority).Last().Priority - 1;
            }

            foreach (var karaneItem in karaneItems)
            {
                if (!positiveCoefficientsById.TryGetValue(karaneItem.CoefficientId, out var zarib))
                {
                    continue;
                }

                if (!employeeBonuses.TryGetValue(karaneItem.CoefficientId, out var value))
                {
                    continue;
                }

                executAbleSetting.Add(new OrganisationEmployeeTypeFicheItemDTO()
                {
                    EmployeeTypeId = RecruitOrder.EmployeeTypeId,
                    Amount = zarib * value,
                    IsDaily = false,
                    DailyCovered = false,
                    IsInsuranceCovered = false,
                    IsTaxCovered = false,
                    Description = "کارانه",
                    EnterType = "کارانه",
                    Origin = "کارانه",
                    ShowZeroInFiche = false,
                    WageItemId = karaneItem.WageItemId,
                    IsRowSuccess = true,
                    WageItem = karaneItem.WageItem.title,
                    OrganisationFormula = zarib + " ضرب در " + value,
                    EnterTypeId = (long)Enums.EnterTypeId.UseFormula,
                    PaymentType = "پرداختی",
                    PaymentTypeId = (long)Enums.PaymentType.Payment,
                    Priority = karanePriority.Value
                });
            }
        }

        #endregion Karane
        #region Eydi

        var eydiSanavatSetting = _db.Set<CalclulationSetting>()
            .AsNoTracking()
            .Include(i => i.RewardFormula.Formula)
            .Include(i => i.SanavatFormula.Formula)
            .FirstOrDefault(i => i.OrganisationChartId == RecruitOrder.PayLocationId);

        if (eydiSanavatSetting == null)
        {
            return OperationResult.Failed(" تنظیمات محاسبه عیدی و سنوات یافت نشد ");
        }

        var singleSetting = eydiSanavatSetting;

        if (singleSetting.RewardAndSanavatStoreTypeId == (long)Enums.EydiSanavatCalculationType.Monthly)
        {
            var RewardRespone = CommunicateWithFormula(singleSetting.RewardFormulaId.Value, formulaRequest);
            if (BuildTreeTrace == true)
            {
                ret.EydiFormulaFriendlyText = RewardRespone.FormulaFriendlyText;
                ret.EydiVariableFriendlyList = RewardRespone.VariableFriendlyList;
                ret.EydiFormulaText = RewardRespone.FormulaText;
                ret.EydiFormulaTreeParser = RewardRespone.FormulaTreeParser;
                ret.EydiFormulaHelpDesc = RewardRespone.FormulaHelpDesc;
                ret.EydiFormula = singleSetting.RewardFormula.Formula.title;
            }
            ret.BillEydiOpadashAmount = Convert.ToInt64(RewardRespone.Result);
            ret.EydiIsRowSuccess = RewardRespone.Succees;
            ret.EydiSuccessRunTimeInmilliseconds = RewardRespone.SuccessRunTimeInmilliseconds;
            if (!RewardRespone.Succees)
            {
                ret.Eydiformularowmessage = RewardRespone.ResponseMessage + " ";
                ret.EydiIsRowSuccess = false;
                //  return OperationResult.Succeeded(payload: wageRow);
            }
            else
            {
                ret.Eydiformularowmessage = "Ok";
            }

        }
        if (singleSetting.RewardAndSanavatStoreTypeId == (long)Enums.EydiSanavatCalculationType.YearlYOrTermination)
        {

        }

        #endregion Eydi
        #region Sanavat

        if (singleSetting.RewardAndSanavatStoreTypeId == (long)Enums.EydiSanavatCalculationType.Monthly)
        {
            var RewardRespone = CommunicateWithFormula(singleSetting.SanavatFormulaId.Value, formulaRequest);
            if (BuildTreeTrace == true)
            {
                ret.SanavatFormulaFriendlyText = RewardRespone.FormulaFriendlyText;
                ret.SanavatVariableFriendlyList = RewardRespone.VariableFriendlyList;
                ret.SanavatFormulaText = RewardRespone.FormulaText;
                ret.SanavatFormulaTreeParser = RewardRespone.FormulaTreeParser;
                ret.SanavatFormulaHelpDesc = RewardRespone.FormulaHelpDesc;
                ret.SanavatFormula = singleSetting.SanavatFormula.Formula.title;
            }
            ret.BillBazkharidSanavatAmount = Convert.ToInt64(RewardRespone.Result);
            ret.SanavatIsRowSuccess = RewardRespone.Succees;
            ret.SanavatSuccessRunTimeInmilliseconds = RewardRespone.SuccessRunTimeInmilliseconds;
            if (!RewardRespone.Succees)
            {
                ret.Sanavatformularowmessage = RewardRespone.ResponseMessage + " ";
                ret.SanavatIsRowSuccess = false;
                //  return OperationResult.Succeeded(payload: wageRow);
            }
            else
            {
                ret.Sanavatformularowmessage = "Ok";
            }

        }
        if (singleSetting.RewardAndSanavatStoreTypeId == (long)Enums.EydiSanavatCalculationType.YearlYOrTermination)
        {

        }

        #endregion Sanavat
        ret.OrganisationEmployeeTypeFicheItem = executAbleSetting;
        var ficheItemsByWageItemId = BuildFicheItemsByWageItemId(ret.OrganisationEmployeeTypeFicheItem);
        #region Arear

        var calculatedArears = _unitOfWork.Context.ArearFiches
            .AsNoTracking()
            .Include(i => i.PaymentPeriod)
            .Where(i => i.PaymentPeriodIntendToPayId == ret.PaymentPeriodId && i.EmployeeId == EmployeeId)
            .ToList();

        if (calculatedArears.Count > 0)
        {
            var arearFicheIds = calculatedArears.Select(i => i.Id).ToList();
            var arearsChangedItemsByFicheId = _unitOfWork.Context.ArearsChangedFicheItems
                .AsNoTracking()
                .Include(i => i.WageItem)
                .Where(i => arearFicheIds.Contains(i.ArearFicheId))
                .ToList()
                .GroupBy(i => i.ArearFicheId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var arearsBaseSettingsByWageItemId = ret.OrganisationEmployeeTypeFicheItem
                .Where(i => i.Id == null)
                .GroupBy(i => i.WageItemId)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var arearFiche in calculatedArears)
            {
                if (!arearsChangedItemsByFicheId.TryGetValue(arearFiche.Id, out var arearsChangedItems) || arearsChangedItems.Count == 0)
                {
                    continue;
                }

                int maxIndex = ret.OrganisationEmployeeTypeFicheItem.Count(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment) + 1;
                var relatedPeriod = arearFiche.PaymentPeriod;

                foreach (var item in arearsChangedItems)
                {
                    arearsBaseSettingsByWageItemId.TryGetValue(item.WageItemId, out var setting);

                    var arearItem = new OrganisationEmployeeTypeFicheItemDTO()
                    {
                        WageItemId = item.WageItemId,
                        PaymentTypeId = (long)Enums.PaymentType.Payment,
                        PaymentType = "پرداختی",
                        IsRowSuccess = true,
                        ArearsCovered = false,
                        IsInsuranceCovered = setting?.IsInsuranceCovered,
                        Continuous = false,
                        IsTaxCovered = setting?.IsTaxCovered,
                        Id = item.WageItemId,
                        IsArear = true,
                        ArearPaymentPeriodId = relatedPeriod!.Id,
                        ShowZeroInFiche = true,
                        Amount = (item.CurrentAmount - item.LastAmount) > 0 ? (item.CurrentAmount - item.LastAmount) : 0,
                        EnterType = "سیستمی",
                        OrganisationFormula = (item.CurrentAmount - item.LastAmount) > 0 ? "" : " معوقه منفی : " + (item.CurrentAmount - item.LastAmount),
                        Index = maxIndex,
                        Origin = "محاسبه معوقه",
                        WageItem = "معوقه " + (item == null ? "" : (item.WageItem == null ? "" : item.WageItem.title)) + " " + relatedPeriod!.title
                    };
                    ret.OrganisationEmployeeTypeFicheItem.Add(arearItem);
                    ficheItemsByWageItemId[arearItem.WageItemId] = arearItem;
                    maxIndex++;
                }
            }
        }

        #endregion Arear
        #region Tax


        DateTime dt = DateTime.Now;

        List<PaymentPeriod> LastPeriodsOfCurrentYear = _paymentPeriodService.GetCurrentYearPeriods(CurrentPeriod.ShamsiYear, CurrentPeriod.ShamsiMonth).ToList();

        // Old tax formula
        var taxResult = CalculateTax(new TaxCalculationRequest(EmployeeId, CurrentPeriod, RecruitOrder, LastPeriodsOfCurrentYear, PersonnelFunction, InterdictOrder)
        {
            OrganisationEmployeeTypeFicheItem = ret.OrganisationEmployeeTypeFicheItem
        });

        var taxFreeRecord = _formulaEngineContext.FormulaTableValues
            .AsNoTracking()
            .Where(i => i.DiscreteValue == EmployeeId && i.FormulaTableId == 12)
            .Select(i => new { i.Resultvalue })
            .FirstOrDefault();

        if (taxFreeRecord?.Resultvalue == 1)
        {
            taxResult.CurrentTax = 0;
        }



        //var taxResult = CalculateTaxNew(new TaxCalculationRequest(EmployeeId, CurrentPeriod, RecruitOrder, LastPeriodsOfCurrentYear, PersonnelFunction, InterdictOrder)
        //    {
        //        OrganisationEmployeeTypeFicheItem = ret.OrganisationEmployeeTypeFicheItem
        //    });

        ret.TaxCoveredSum = taxResult.TaxCoveredSum;
        ret.CurrentTax = taxResult.CurrentTax;

        if (ficheItemsByWageItemId.TryGetValue(taxResult.RelatedTaxWageItemId, out var taxWageItem))
        {
            taxWageItem.Amount = taxResult.CurrentTax;
            taxWageItem.PaymentTypeId = (long)Enums.PaymentType.Deduction;
            taxWageItem.IsRowSuccess = true;
            taxWageItem.SuccessRunTimeInmilliseconds = (DateTime.Now - dt).Microseconds;
        }

        if (taxResult.Success == true)
        {
            ret.SumCashTaxCoveredAndCountinious = taxResult.SumCashTaxCoveredAndCountinious;
            ret.SumNonCashTaxCoveredAndCountinious = taxResult.SumNonCashTaxCoveredAndCountinious;
            ret.SumNonCashTaxCoveredAndNotCountinious = taxResult.SumNonCashTaxCoveredAndNotCountinious;
            ret.SumCashTaxCoveredAndNotCountinious = taxResult.SumCashTaxCoveredAndNotCountinious;
        }
        else
        {
            return OperationResult.Failed(taxResult.Message == null ? "خط در محاسبه مالیات" : " خطا در محاسبه مالیات " + taxResult.Message);
        }

        #endregion Tax
        #region Loan
        ret.PersonnelLoanPayments = new List<PersonnelLoanPayment>();
        var personLoans = _unitOfWork.Context.PersonnelLoans
            .AsNoTracking()
            .Include(i => i.StartDeductPaymentPeriod)
            .Where(pl => pl.EmployeeId == EmployeeId
                && pl.IsDeleted != true
                && pl.StartDeductPaymentPeriod.ShamsiYear <= CurrentPeriod.ShamsiYear
                && ((pl.StartDeductPaymentPeriod.ShamsiYear == CurrentPeriod.ShamsiYear && pl.StartDeductPaymentPeriod.ShamsiMonth <= CurrentPeriod.ShamsiMonth)
                    || pl.StartDeductPaymentPeriod.ShamsiYear < CurrentPeriod.ShamsiYear))
            .ToList();

        var loanTypesById = _unitOfWork.Context.LoanTypes
            .AsNoTracking()
            .Where(t => t.OrganisationChartId == PayLocationId && t.IsDeleted != true)
            .ToDictionary(t => t.Id);

        personLoans = personLoans
            .Where(pl => loanTypesById.ContainsKey(pl.LoanTypeId))
            .ToList();

        ApplyInactiveLoanWageItemSuppression(
            personLoans,
            loanTypesById,
            organisationLoanWageItemIds,
            ret.OrganisationEmployeeTypeFicheItem,
            ficheItemsByWageItemId);

        var activePersonLoans = personLoans
            .Where(l => l.IsActive
                && loanTypesById.TryGetValue(l.LoanTypeId, out var lt)
                && lt.IsActive == true)
            .ToList();

        if (activePersonLoans.Count > 0)
        {
            var loanIds = activePersonLoans.Select(l => l.Id).ToList();
            var previousLoanPaymentSums = _unitOfWork.Context.PersonnelLoanPayments
                .AsNoTracking()
                .Where(i => loanIds.Contains(i.PersonnelLoanId) && i.FicheId != null && i.Fiche.PaymentPeriodId != CurrentPeriod.Id)
                .GroupBy(i => i.PersonnelLoanId)
                .Select(g => new { PersonnelLoanId = g.Key, Sum = g.Sum(i => i.PaymentAmount) })
                .ToDictionary(x => x.PersonnelLoanId, x => x.Sum);

            foreach (var PersonnelLoan in activePersonLoans)
            {
                if (!loanTypesById.TryGetValue(PersonnelLoan.LoanTypeId, out var loanType))
                {
                    continue;
                }

                if (!TryGetLoanWageItem(ret.OrganisationEmployeeTypeFicheItem, ficheItemsByWageItemId, loanType.WageItemId, out var relatedWageItem))
                {
                    continue;
                }

                DateTime PersonnelLoandt = DateTime.Now;
                previousLoanPaymentSums.TryGetValue(PersonnelLoan.Id, out var currentLoanPaymentSum);
                bool hasLastPayment = currentLoanPaymentSum > 0;
                        if (hasLastPayment)
                        {

                            long totalDebt = PersonnelLoan.AllAmount.Value - currentLoanPaymentSum;
                            if (totalDebt > PersonnelLoan.InstallmentAmount)
                            {
                                if (PersonnelLoan.InstallmentAmount.HasValue)
                                {
                                    relatedWageItem.Amount = PersonnelLoan.InstallmentAmount.Value;
                                }
                                else
                                {
                                    relatedWageItem.Amount = 0;
                                }
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainLoanAmount = Convert.ToInt64(PersonnelLoan.AllAmount.Value - currentLoanPaymentSum - relatedWageItem.Amount);
                                relatedWageItem.PersonnelLoanId = PersonnelLoan.Id;
                                relatedWageItem.IsInactiveLoanSuppressed = false;
                                relatedWageItem.OrganisationFormula = "کسر قسط وام";
                                ret.PersonnelLoanPayments.Add(new PersonnelLoanPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    PersonnelLoanId = PersonnelLoan.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                            else
                            {
                                relatedWageItem.Amount = totalDebt;
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainLoanAmount = Convert.ToInt64(PersonnelLoan.AllAmount.Value - currentLoanPaymentSum - relatedWageItem.Amount);
                                relatedWageItem.PersonnelLoanId = PersonnelLoan.Id;
                                relatedWageItem.IsInactiveLoanSuppressed = false;
                                relatedWageItem.OrganisationFormula = "کسر قسط وام";
                                ret.PersonnelLoanPayments.Add(new PersonnelLoanPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    PersonnelLoanId = PersonnelLoan.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                        }
                        else
                        {
                            if (PersonnelLoan.RemainingCrumbsAtFirst == true)
                            {
                                if (PersonnelLoan.AllAmount.HasValue && PersonnelLoan.InstallmentAmount.HasValue)
                                {
                                    long RemainingCrumb = PersonnelLoan.AllAmount.Value % PersonnelLoan.InstallmentAmount.Value;

                                    if (RemainingCrumb > 0)
                                    {
                                        relatedWageItem.Amount = RemainingCrumb;
                                        relatedWageItem.IsRowSuccess = true;
                                        relatedWageItem.RemainLoanAmount = PersonnelLoan.AllAmount.Value - RemainingCrumb;
                                        relatedWageItem.PersonnelLoanId = PersonnelLoan.Id;
                                relatedWageItem.IsInactiveLoanSuppressed = false;
                                        relatedWageItem.SuccessRunTimeInmilliseconds = (DateTime.Now - PersonnelLoandt).Milliseconds;
                                        relatedWageItem.OrganisationFormula = "کسر قسط وام";
                                        ret.PersonnelLoanPayments.Add(new PersonnelLoanPayment()
                                        {
                                            PaymentAmount = RemainingCrumb,
                                            PersonnelLoanId = PersonnelLoan.Id,
                                            CreateDate = DateTime.Now,
                                            PaymentTypeId = 6,// واریز نقدی,
                                            PaymentDate = DateTime.Now,
                                            IPAddress = "",
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (PersonnelLoan.InstallmentAmount.HasValue)
                                {
                                    relatedWageItem.Amount = PersonnelLoan.InstallmentAmount.Value;
                                }
                                else
                                {
                                    relatedWageItem.Amount = 0;
                                }
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainLoanAmount = Convert.ToInt64(PersonnelLoan.AllAmount.Value - relatedWageItem.Amount);
                                relatedWageItem.PersonnelLoanId = PersonnelLoan.Id;
                                relatedWageItem.IsInactiveLoanSuppressed = false;
                                relatedWageItem.IsInactiveLoanSuppressed = false;
                                relatedWageItem.OrganisationFormula = "کسر قسط وام";
                                ret.PersonnelLoanPayments.Add(new PersonnelLoanPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    PersonnelLoanId = PersonnelLoan.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                        }
            }
        }

        #endregion Loan
        #region Deduction

        ret.EmployeeDeductionPayments = new List<EmployeeDeductionPayment>();
        var personDeductions = _unitOfWork.Context.EmployeeDeductions
            .AsNoTracking()
            .Include(i => i.StartDeductPaymentPeriod)
            .Where(i => i.EmployeeId == EmployeeId && i.IsActive == true && i.StartDeductPaymentPeriod.ShamsiYear <= CurrentPeriod.ShamsiYear && ((i.StartDeductPaymentPeriod.ShamsiYear == CurrentPeriod.ShamsiYear && i.StartDeductPaymentPeriod.ShamsiMonth <= CurrentPeriod.ShamsiMonth) || (i.StartDeductPaymentPeriod.ShamsiYear < CurrentPeriod.ShamsiYear)))
            .ToList();

        if (personDeductions.Count > 0)
        {
            var deductionTypeIds = personDeductions.Select(d => d.DeductionTypeId).Distinct().ToList();
            var deductionTypesById = _unitOfWork.Context.DeductionTypes
                .AsNoTracking()
                .Where(t => deductionTypeIds.Contains(t.Id))
                .ToDictionary(t => t.Id);

            var deductionIds = personDeductions.Select(d => d.Id).ToList();
            var previousDeductionPaymentSums = _unitOfWork.Context.EmployeeDeductionPayments
                .AsNoTracking()
                .Where(i => deductionIds.Contains(i.EmployeeDeductionId) && i.FicheId != null && i.Fiche.PaymentPeriodId != CurrentPeriod.Id)
                .GroupBy(i => i.EmployeeDeductionId)
                .Select(g => new { EmployeeDeductionId = g.Key, Sum = g.Sum(i => i.PaymentAmount) })
                .ToDictionary(x => x.EmployeeDeductionId, x => x.Sum);

            foreach (var PersonnelDeduction in personDeductions)
            {
                if (!deductionTypesById.TryGetValue(PersonnelDeduction.DeductionTypeId, out var DeductionType))
                {
                    continue;
                }

                if (!ficheItemsByWageItemId.TryGetValue(DeductionType.WageItemId, out var relatedWageItem))
                {
                    continue;
                }

                DateTime PersonnelDeductiondt = DateTime.Now;
                previousDeductionPaymentSums.TryGetValue(PersonnelDeduction.Id, out var currentDeductionPaymentSum);
                bool hasLastPayment = currentDeductionPaymentSum > 0;
                        if (hasLastPayment)
                        {

                            long totalDebt = PersonnelDeduction.AllAmount.Value - currentDeductionPaymentSum;
                            if (totalDebt > PersonnelDeduction.InstallmentAmount)
                            {
                                if (PersonnelDeduction.InstallmentAmount.HasValue)
                                {
                                    relatedWageItem.Amount = PersonnelDeduction.InstallmentAmount.Value;
                                }
                                else
                                {
                                    relatedWageItem.Amount = 0;
                                }
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainLoanAmount = Convert.ToInt64(PersonnelDeduction.AllAmount.Value - currentDeductionPaymentSum - relatedWageItem.Amount);
                                relatedWageItem.EmployeeDeductionId = PersonnelDeduction.Id;
                                relatedWageItem.OrganisationFormula = "کسر قسط کسورات";
                                ret.EmployeeDeductionPayments.Add(new EmployeeDeductionPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    EmployeeDeductionId = PersonnelDeduction.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                            else
                            {
                                relatedWageItem.Amount = totalDebt;
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainDeductionAmount = Convert.ToInt64(PersonnelDeduction.AllAmount.Value - currentDeductionPaymentSum - relatedWageItem.Amount);
                                relatedWageItem.EmployeeDeductionId = PersonnelDeduction.Id;
                                relatedWageItem.OrganisationFormula = "کسر قسط کسورات";
                                ret.EmployeeDeductionPayments.Add(new EmployeeDeductionPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    EmployeeDeductionId = PersonnelDeduction.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                        }
                        else
                        {
                            if (PersonnelDeduction.RemainingCrumbsAtFirst == true)
                            {
                                if (PersonnelDeduction.AllAmount.HasValue && PersonnelDeduction.InstallmentAmount.HasValue)
                                {
                                    long RemainingCrumb = PersonnelDeduction.AllAmount.Value % PersonnelDeduction.InstallmentAmount.Value;

                                    if (RemainingCrumb > 0)
                                    {
                                        relatedWageItem.Amount = RemainingCrumb;
                                        relatedWageItem.IsRowSuccess = true;
                                        relatedWageItem.RemainDeductionAmount = PersonnelDeduction.AllAmount.Value - RemainingCrumb;
                                        relatedWageItem.EmployeeDeductionId = PersonnelDeduction.Id;
                                        relatedWageItem.SuccessRunTimeInmilliseconds = (DateTime.Now - PersonnelDeductiondt).Milliseconds;
                                        relatedWageItem.OrganisationFormula = "کسر قسط کسورات";
                                        ret.EmployeeDeductionPayments.Add(new EmployeeDeductionPayment()
                                        {
                                            PaymentAmount = RemainingCrumb,
                                            EmployeeDeductionId = PersonnelDeduction.Id,
                                            CreateDate = DateTime.Now,
                                            PaymentTypeId = 6,// واریز نقدی,
                                            PaymentDate = DateTime.Now,
                                            IPAddress = "",
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (PersonnelDeduction.InstallmentAmount.HasValue)
                                {
                                    relatedWageItem.Amount = PersonnelDeduction.InstallmentAmount.Value;
                                }
                                else
                                {
                                    relatedWageItem.Amount = 0;
                                }
                                relatedWageItem.IsRowSuccess = true;
                                relatedWageItem.RemainDeductionAmount = Convert.ToInt64(PersonnelDeduction.AllAmount.Value - relatedWageItem.Amount);
                                relatedWageItem.EmployeeDeductionId = PersonnelDeduction.Id;
                                relatedWageItem.OrganisationFormula = "کسر قسط کسورات";
                                ret.EmployeeDeductionPayments.Add(new EmployeeDeductionPayment()
                                {
                                    PaymentAmount = Convert.ToInt64(relatedWageItem.Amount),
                                    EmployeeDeductionId = PersonnelDeduction.Id,
                                    CreateDate = DateTime.Now,
                                    PaymentTypeId = 6,// واریز نقدی,
                                    PaymentDate = DateTime.Now,
                                    IPAddress = "",
                                });
                            }
                        }
            }
        }


        #endregion

        #region صندوق

        var currentEmployeeFunds = _unitOfWork.Context.EmployeeFunds
            .AsNoTracking()
            .Where(i => i.EmployeeId == EmployeeId && i.IsActive == true && i.IsDeleted != true)
            .ToList();

        if (currentEmployeeFunds.Count > 0)
        {
            var fundTypeIds = currentEmployeeFunds.Select(i => i.FundTypeId).Distinct().ToList();
            var fundSettingsByTypeId = _unitOfWork.Context.OrganisationEmployeeTypeFundTypeDefinitions
                .AsNoTracking()
                .Where(i =>
                    i.OrganisationChartId == RecruitOrder.PayLocationId &&
                    i.EmployeeTypeId == RecruitOrder.EmployeeTypeId &&
                    fundTypeIds.Contains(i.FundTypeId) &&
                    i.IsDeleted != true)
                .ToList()
                .GroupBy(i => i.FundTypeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var currentEmployeeFund in currentEmployeeFunds)
            {
                if (!fundSettingsByTypeId.TryGetValue(currentEmployeeFund.FundTypeId, out var currentFundSetting))
                {
                    continue;
                }

                if (currentFundSetting.Count == 1)
                {
                    var validSetting = currentFundSetting[0];
                }
                else if (currentFundSetting.Count > 1)
                {
                    return OperationResult.NotFound("بیش از یک رکورد تنظیمات برای نوع صندوق مورد نظر ثبت شده است");
                }
            }
        }

        #endregion صندوق


        #region Leaves -  مرخصی ها

        var currentEmployeeTypeSettings = _unitOfWork.Context.OrganisationEmployeeTypeLeaves
            .AsNoTracking()
            .Where(i => i.OrganisationChartId == RecruitOrder.PayLocationId && i.EmployeeTypeId == RecruitOrder.EmployeeTypeId)
            .ToList();

        if (currentEmployeeTypeSettings.Count > 0)
        {
            var allowedLeaveTypeIds = currentEmployeeTypeSettings.Select(i => i.LeaveTypeId).ToHashSet();
            var leaveSettingsByTypeId = currentEmployeeTypeSettings.ToDictionary(i => i.LeaveTypeId);

            var EmployeeLeavesInCurrentPeriod = _unitOfWork.Context.PersonnelLeaves
                .AsNoTracking()
                .Include(i => i.LeaveType)
                .Where(i => i.OrganisationChartId == RecruitOrder.PayLocationId
                    && i.EmployeeId == RecruitOrder.EmployeeId
                    && i.PaymentPeriodId == PaymentPeriodId
                    && allowedLeaveTypeIds.Contains(i.LeaveTypeId))
                .ToList();

            var lastImportedLeaves = EmployeeLeavesInCurrentPeriod
                .GroupBy(i => i.LeaveTypeId)
                .Select(g => g.OrderBy(i => i.CreateDate).Last())
                .ToList();

            ret.FicheLeaveItemDTOs = [];
            /// وقتی ترو هست یعنی اولین دوره محاسبه برای این فرد در سال جاری هست و باید بر اساس سهمیه ها محاسبه شود
            bool needtoBeInitial = CurrentPeriod.ShamsiMonth == 1;
            var lastPeriod = _unitOfWork.Context.PaymentPeriods
                .AsNoTracking()
                .SingleOrDefault(i => i.ShamsiYear == CurrentPeriod.ShamsiYear && i.ShamsiMonth == CurrentPeriod.ShamsiMonth - 1);

            long? lastPeriodFicheId = null;
            if (lastPeriod != null)
            {
                lastPeriodFicheId = _unitOfWork.Context.Fiches
                    .AsNoTracking()
                    .Where(i => i.PaymentPeriodId == lastPeriod.Id && i.EmployeeId == EmployeeId)
                    .OrderBy(i => i.Id)
                    .Select(i => (long?)i.Id)
                    .LastOrDefault();
            }

            if (!needtoBeInitial)
            {
                needtoBeInitial = !lastPeriodFicheId.HasValue
                    || !_unitOfWork.Context.FicheLeaveItems.AsNoTracking().Any(i => i.FicheId == lastPeriodFicheId.Value);
            }
                if (needtoBeInitial)
                {
                    var entitlementsByLeaveTypeId = _unitOfWork.Context.EmployeeLeaveEntitlements
                        .AsNoTracking()
                        .Where(i => i.EmployeeId == RecruitOrder.EmployeeId && i.Year == CurrentPeriod.ShamsiYear)
                        .ToDictionary(i => i.LeaveTypeId);

                    foreach (var leave in lastImportedLeaves)
                    {
                        if (!entitlementsByLeaveTypeId.TryGetValue(leave.LeaveTypeId, out var entitlement))
                        {
                            continue;
                        }

                        var setting = leaveSettingsByTypeId[leave.LeaveTypeId];

                        var toAddEntity = new FicheLeaveItemDTO()
                        {
                            LeaveTypeId = leave.LeaveTypeId,
                            LeaveType = leave.LeaveType?.title,
                            PersonnelLeaveId = leave.Id,
                            PersonnelLeave = leave.Description
                        };
                        if (setting.IsDailyOrHourMinute)
                        {
                            toAddEntity.LeaveAmountTicks = new TimeSpan(Convert.ToInt32(leave.Day), 0, 0, 0).Ticks;
                            toAddEntity.LeaveBalanceTicks = new TimeSpan(Convert.ToInt32(entitlement.LeaveAmount), 0, 0, 0).Ticks - toAddEntity.LeaveAmountTicks;
                        }
                        else
                        {
                            toAddEntity.LeaveAmountTicks = new TimeSpan(0, Convert.ToInt32(leave.Hour), Convert.ToInt32(leave.Minute), 0, 0).Ticks;
                            toAddEntity.LeaveBalanceTicks = new TimeSpan(0, Convert.ToInt32(entitlement.LeaveAmount), 0, 0, 0).Ticks - toAddEntity.LeaveAmountTicks;
                        }

                        ret.FicheLeaveItemDTOs.Add(toAddEntity);
                    }
                }
                else
                {
                    // برای دوره‌های غیر اولیه، از مانده دوره قبل استفاده می‌کنیم

                    if (lastPeriodFicheId.HasValue)
                    {
                        var lastPeriodLeaves = _unitOfWork.Context.FicheLeaveItems
                            .AsNoTracking()
                            .Include(i => i.LeaveType)
                            .Where(i => i.FicheId == lastPeriodFicheId.Value && i.IsDeleted == false)
                            .ToList();

                        if (lastPeriodLeaves.Count > 0)
                        {
                            var lastPeriodLeavesByTypeId = lastPeriodLeaves.ToDictionary(i => i.LeaveTypeId);
                            foreach (var Leave in lastImportedLeaves)
                            {
                                if (!lastPeriodLeavesByTypeId.TryGetValue(Leave.LeaveTypeId, out var lastLeave))
                                {
                                    continue;
                                }

                                var setting = leaveSettingsByTypeId[lastLeave.LeaveTypeId];

                                var toAddEntity = new FicheLeaveItemDTO()
                                {
                                    LeaveTypeId = Leave.LeaveTypeId,
                                    LeaveType = Leave.LeaveType?.title,
                                    PersonnelLeaveId = Leave.Id,
                                    PersonnelLeave = Leave.Description
                                };

                                if (setting.IsDailyOrHourMinute)
                                {
                                    // مانده دوره قبل منهای مرخصی مصرف شده در این دوره
                                    toAddEntity.LeaveAmountTicks = new TimeSpan(Convert.ToInt32(Leave.Day), 0, 0, 0).Ticks;
                                    toAddEntity.LeaveBalanceTicks = lastLeave.LeaveBalanceTicks - toAddEntity.LeaveAmountTicks;
                                }
                                else
                                {
                                    toAddEntity.LeaveAmountTicks = new TimeSpan(0, Convert.ToInt32(Leave.Hour), Convert.ToInt32(Leave.Minute), 0, 0).Ticks;
                                    toAddEntity.LeaveBalanceTicks = lastLeave.LeaveBalanceTicks - toAddEntity.LeaveAmountTicks;
                                }

                                ret.FicheLeaveItemDTOs.Add(toAddEntity);
                            }
                        }
                    }
                }
        }


        #endregion Leaves -  مرخصی ها


        var arersCount = ret.OrganisationEmployeeTypeFicheItem.Count(i => i.IsArear == true);

        foreach (var item in ret.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Deduction))
        {
            item.Index = item.Index + arersCount;
        }

        ret.OrganisationEmployeeTypeFicheItem = ret.OrganisationEmployeeTypeFicheItem.OrderBy(i => i.Index).ToList();

        ComputeFicheAmountTotals(
            ret.OrganisationEmployeeTypeFicheItem,
            out var totalPaymentAmount,
            out var insuranceTotalPaymentAmountDsw,
            out var paymentInsuranceCovered,
            out var deductionSumAmount,
            out var dailyFunctionAmount,
            out var spouseAmount,
            out var sumCashTaxCoveredAndContinuous,
            out var secondaryTotalPaymentAmount,
            out var secondaryDeductionSumAmount,
            out var secondaryBillEydiOpadashAmount,
            out var secondaryBillBazkharidSanavatAmount);

        ret.TotalPaymentAmount = totalPaymentAmount;
        ret.InsuranceTotalPaymentAmount_DSW = insuranceTotalPaymentAmountDsw;
        ret.PaymentInsuranceCovered = paymentInsuranceCovered;
        ret.DeductionSumAmount = deductionSumAmount;
        ret.DailyFunctionAmount = dailyFunctionAmount;
        ret.SpouseAmount = spouseAmount;
        // سهم قلم‌های فرعی فقط محاسباتی است و در موجودیت Fiche ذخیره نمی‌شود
        ret.SecondaryTotalPaymentAmount = secondaryTotalPaymentAmount;
        ret.SecondaryDeductionSumAmount = secondaryDeductionSumAmount;
        ret.SecondaryPayableAmount = secondaryTotalPaymentAmount - secondaryDeductionSumAmount;
        ret.SecondaryTotalAmount = Convert.ToInt64(secondaryTotalPaymentAmount);
        ret.SecondaryDeductedAmount = secondaryDeductionSumAmount;
        ret.SecondaryPurePaymentAmount = Convert.ToInt64(ret.SecondaryPayableAmount);
        ret.SecondaryBillEydiOpadashAmount = secondaryBillEydiOpadashAmount;
        ret.SecondaryBillBazkharidSanavatAmount = secondaryBillBazkharidSanavatAmount;

        ret.PayableAmount = ret.TotalPaymentAmount - ret.DeductionSumAmount;
        try
        {
            ret.PayableAmountSTR = (ret.PayableAmount > 0 ? "" : " منفی ") + HR.SharedKernel.Persian_Number_To_String.GET_Number_To_PersianString(Convert.ToInt32(ret.PayableAmount).ToString());
        }
        catch (Exception exx)
        {

        }

        /// جمع مزایای مستمر نقدی و مشمول مالیات
        ret.SumCashTaxCoveredAndCountinious = sumCashTaxCoveredAndContinuous;
        /// جمع مزایای مستمر غیر نقدی و مشمول مالیات
        //ret.SumNonCashTaxCoveredAndCountinious = Convert.ToInt64(ret.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.Continuous == true && i.IsTaxCovered == true && i.IsVirtual != true).Sum(i => i.Amount));

        ApplyInactiveLoanWageItemSuppression(
            personLoans,
            loanTypesById,
            organisationLoanWageItemIds,
            ret.OrganisationEmployeeTypeFicheItem,
            ficheItemsByWageItemId);

        ret.OrganisationEmployeeTypeFicheItem = ret.OrganisationEmployeeTypeFicheItem
            .Where(i => i.IsInactiveLoanSuppressed || i.ShowZeroInFiche == true || (i.ShowZeroInFiche != true && i.Amount > 0))
            .ToList();

        foreach (var item in ret.OrganisationEmployeeTypeFicheItem.Where(i =>
            i.ShowZeroInFiche == true
            && i.Amount == 0
            && !i.IsInactiveLoanSuppressed
            && !organisationLoanWageItemIds.Contains(i.WageItemId)))
        {
            item.formularowmessage = "محاسبه نشده";
        }


        #region Fill&SaveFiche




        if (SaveFiche)
        {
            if (!ret.HasExistingFiche)
            {
                Fiche newFiche = new Fiche()
                {
                    EmployeeId = EmployeeId,
                    Description = PersonnelFunction.Description,
                    PersonnelFunctionId = ret.PersonnelFunctionId,
                    InterdictOrderId = ret.InterdictOrderId,
                    CreateDate = DateTime.Now,
                    PaymentPeriodId = ret.PaymentPeriodId,
                    EmployeeTypeId = RecruitOrder.EmployeeTypeId,
                    StartDate = CurrentPeriod.StartDate,
                    EndDate = CurrentPeriod.EndDate,
                    OrganisationChartId = RecruitOrder.PayLocationId,
                    DeductedAmount = ret.DeductionSumAmount,
                    PurePaymentAmount = Convert.ToInt64(ret.PayableAmount),
                    IPAddress = "",
                    PaymentTax = Convert.ToInt64(ret.TaxCoveredSum),
                    PaymentInsuranceCovered = Convert.ToInt64(ret.PaymentInsuranceCovered),
                    DailyFunctionAmount = ret.DailyFunctionAmount,
                    TotalAmount = Convert.ToInt64(ret.TotalPaymentAmount),
                    InsuranceTotal_DSW = Convert.ToInt64(ret.InsuranceTotalPaymentAmount_DSW),
                    BillEydiOpadashAmount = ret.BillEydiOpadashAmount,
                    BillBazkharidSanavatAmount = ret.BillBazkharidSanavatAmount,
                    FicheStatusId = (long)Enums.FicheStatus.Initial,
                    SumCashTaxCoveredAndCountinious = ret.SumCashTaxCoveredAndCountinious,
                    SumNonCashTaxCoveredAndCountinious = ret.SumNonCashTaxCoveredAndCountinious,
                    SumNonCashTaxCoveredAndNotCountinious = ret.SumNonCashTaxCoveredAndNotCountinious,
                    SumCashTaxCoveredAndNotCountinious = ret.SumCashTaxCoveredAndNotCountinious,
                    CostCenterId = RecruitOrder.CostCenterId,
                    SpouseAmount = ret.SpouseAmount,
                    IncAmount = ret.IncAmount,
                    IsActiveInsurance = true

                };
                if (newFiche.CostCenterId > 0)
                {
                    var costCenterSetting = _organisationCostCenterService.All().Where(i => i.OrganisationChartId == newFiche.OrganisationChartId && i.CostCenterId == newFiche.CostCenterId);
                    if (costCenterSetting == null)
                    {

                    }
                    else
                    {
                        if (costCenterSetting.Any())
                        {
                            newFiche.PeymanRowId = costCenterSetting.Single().PeymanRowId.HasValue ? costCenterSetting.Single().PeymanRowId.Value : null;
                        }
                    }
                }
                //var property = _db.Set<PersonnelProperty>().Where(i => i.EmployeeId == EmployeeId);
                //if (property == null)
                //{
                //    return OperationResult.Failed("مشخصات فردی یافت نشد");
                //}
                //else
                //{
                //    if (property.Any())
                //    {
                //        var single = property.Single();
                //        if (string.IsNullOrEmpty(single.BankAccountNo))
                //        {
                //            return OperationResult.Failed("شماره حساب جهت پرداخت حقوق یافت نشد");
                //        }
                //        newFiche.BankAccountNo = single.BankAccountNo;
                //        //  newFiche.InsuranceNo = single.InsuranceNo;
                //    }
                //    else
                //    {
                //        return OperationResult.Failed("مشخصات فردی یافت نشد");
                //    }
                //}

                #region خواندن اطلاعات بیمه از پرونده
                var insuranceRecors = _insuranceService.All().Where(i => i.EmployeeId == EmployeeId);
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
                                ret.InsuranceNo = insuranceRecors.Single(i => i.IsLast == true).InsuranceNumber;
                                newFiche.InsuranceNo = insuranceRecors.Single(i => i.IsLast == true).InsuranceNumber;
                            }
                        }
                    }
                }
                #endregion خواندن اطلاعات بیمه از پرونده

                #region خواندن اطلاعات حساب بانکی از پرونده


                var bankAccountRecords = _bankAccountService.All().Where(i => i.EmployeeId == EmployeeId);
                if (bankAccountRecords == null)
                {
                    return OperationResult.Failed("شماره حساب بانکی یافت نشد");
                }
                else
                {
                    if (bankAccountRecords.Any())
                    {
                        if (bankAccountRecords.Any(i => i.Status == true))
                        {
                            if (bankAccountRecords.Count(i => i.Status == true) == 1)
                            {
                                newFiche.BankAccountNo = bankAccountRecords.Single(i => i.Status == true).AccountNumber;
                            }
                        }
                        else
                        {
                            return OperationResult.Failed("شماره حساب بانکی یافت نشد");
                        }
                    }
                    else
                    {
                        return OperationResult.Failed("شماره حساب بانکی یافت نشد");
                    }
                }

                #endregion خواندن اطلاعات حساب بانکی از پرونده

                //


                try
                {
                    _unitOfWork.CreateTransaction();
                    newFiche.title = "";
                    _unitOfWork.Context.Fiches.Add(newFiche);
                    _unitOfWork.Context.SaveChanges();

                    #region loan
                    foreach (var item in ret.PersonnelLoanPayments)
                    {
                        item.FicheId = newFiche.Id;
                        item.IsPaid = true;
                        item.IPAddress = "";
                        item.title = "پرداخت وام";
                        _unitOfWork.Context.PersonnelLoanPayments.Add(item);
                    }
                    _unitOfWork.Context.SaveChanges();

                    #endregion loan


                    #region Deduction

                    foreach (var item in ret.EmployeeDeductionPayments)
                    {
                        item.FicheId = newFiche.Id;
                        item.IsPaid = true;
                        item.IPAddress = "";
                        item.title = "پرداخت کسورات";
                        _unitOfWork.Context.EmployeeDeductionPayments.Add(item);
                    }
                    _unitOfWork.Context.SaveChanges();

                    #endregion Deduction


                    #region PersonnelPayment

                    foreach (var item in ret.PersonnelPayments)
                    {
                        if (ret.OrganisationEmployeeTypeFicheItem != null)
                        {
                            if (ret.OrganisationEmployeeTypeFicheItem.Any(i => i.PersonnelFicheItemId == item.PersonnelFicheItemId))
                            {
                                var singleRecord = ret.OrganisationEmployeeTypeFicheItem.Single(i => i.PersonnelFicheItemId == item.PersonnelFicheItemId);
                                item.Amount = Convert.ToInt64(singleRecord.Amount);
                                item.PersonnelFicheItemId = item.PersonnelFicheItemId;
                                item.PaymentTypeId = (long)Enums.PaymentTypeTable15Tax.Cash_Deposit;
                                item.PaymentDate = DateTime.Now;
                                item.FicheId = newFiche.Id;
                                item.EmployeeId = EmployeeId;

                                item.IPAddress = "";
                                item.title = "پرداخت فردی";
                                _unitOfWork.Context.PersonnelPayments.Add(item);
                                _unitOfWork.Context.SaveChanges();
                                singleRecord.PersonnelPaymentId = item.Id;
                            }
                        }
                    }
                    _unitOfWork.Context.SaveChanges();

                    #endregion PersonnelPayment

                    foreach (var item in ret.OrganisationEmployeeTypeFicheItem.Where(i => i.IsVirtual != true))
                    {
                        FicheItem row = new FicheItem()
                        {
                            FicheId = newFiche.Id,
                            CreateDate = DateTime.Now,
                            IPAddress = "",
                            Value = item.Amount,
                            IsArear = item.IsArear,
                            IsSubItem = item.IsSubItem,
                            PaymentTypeId = item.PaymentTypeId.Value,
                            WageItemId = item.WageItemId,
                            IsDeleted = false,
                            ArearPaymentPeriodId = item.ArearPaymentPeriodId,
                            RemainLoanAmount = item.RemainLoanAmount,
                            PersonnelLoanId = item.PersonnelLoanId,
                            EmployeeDeductionId = item.EmployeeDeductionId,
                            RemainDeductionAmount = item.RemainDeductionAmount,
                            PersonnelPaymentId = item.PersonnelPaymentId
                        };

                        if (item.IsRowSuccess == true)
                        {

                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            return OperationResult.NotFound("همه ردیف های محاسبه قلم موفقیت آمیز نوده اند - " + item.WageItem);
                        }

                        _unitOfWork.Context.FicheItems.Add(row);
                    }

                    #region Leaves - مرخصی ها 

                    if (ret.FicheLeaveItemDTOs != null)
                    {
                        foreach (var item in ret.FicheLeaveItemDTOs)
                        {
                            FicheLeaveItem row = new FicheLeaveItem()
                            {
                                FicheId = newFiche.Id,
                                CreateDate = DateTime.Now,
                                title = item.LeaveType,
                                IPAddress = "",
                                IsDeleted = false,
                                LeaveTypeId = item.LeaveTypeId,
                                PersonnelLeaveId = item.PersonnelLeaveId,
                                LeaveAmountTicks = item.LeaveAmountTicks,
                                LeaveBalanceTicks = item.LeaveBalanceTicks,
                            };
                            if (string.IsNullOrEmpty(row.title))
                            {
                                row.title = "";
                            }
                            _unitOfWork.Context.FicheLeaveItems.Add(row);
                        }
                    }

                    #endregion Leaves - مرخصی ها 

                    _unitOfWork.Context.SaveChanges();
                    _unitOfWork.Commit();
                    ret.ExsitingFicheId = newFiche.Id;
                    ret.HasExistingFiche = true;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed(ex.Message, payload: ret);
                }
            }
        }
        #endregion Fill&SaveFiche

        return OperationResult.Succeeded(payload: ret);


    }

    private const string InactiveLoanFormulaLabel = "وام غیرفعال";

    private static bool TryGetLoanWageItem(
        IList<OrganisationEmployeeTypeFicheItemDTO> ficheItems,
        Dictionary<long, OrganisationEmployeeTypeFicheItemDTO> ficheItemsByWageItemId,
        long wageItemId,
        out OrganisationEmployeeTypeFicheItemDTO wageItem)
    {
        if (ficheItemsByWageItemId.TryGetValue(wageItemId, out wageItem!))
        {
            return true;
        }

        wageItem = ficheItems.FirstOrDefault(i => i.WageItemId == wageItemId);
        return wageItem != null;
    }

    private static void SuppressInactiveLoanWageItem(OrganisationEmployeeTypeFicheItemDTO wageItem)
    {
        wageItem.Amount = 0;
        wageItem.IsRowSuccess = true;
        wageItem.RemainLoanAmount = 0;
        wageItem.PersonnelLoanId = null;
        wageItem.IsInactiveLoanSuppressed = true;
        wageItem.OrganisationFormula = InactiveLoanFormulaLabel;
        wageItem.formularowmessage = "Ok";
    }

    private static void ApplyInactiveLoanWageItemSuppression(
        IReadOnlyCollection<PersonnelLoan> personLoansInPeriod,
        IReadOnlyDictionary<long, LoanType> loanTypesById,
        HashSet<long> organisationLoanWageItemIds,
        IList<OrganisationEmployeeTypeFicheItemDTO> ficheItems,
        Dictionary<long, OrganisationEmployeeTypeFicheItemDTO> ficheItemsByWageItemId)
    {
        if (organisationLoanWageItemIds.Count == 0)
        {
            return;
        }

        var wageItemIdsToSuppress = new HashSet<long>();

        if (personLoansInPeriod.Count > 0)
        {
            foreach (var group in personLoansInPeriod
                .Where(pl => loanTypesById.ContainsKey(pl.LoanTypeId))
                .GroupBy(pl => loanTypesById[pl.LoanTypeId].WageItemId))
            {
                var hasActiveLoan = group.Any(pl =>
                    pl.IsActive
                    && loanTypesById.TryGetValue(pl.LoanTypeId, out var loanType)
                    && loanType.IsActive == true);

                if (!hasActiveLoan)
                {
                    wageItemIdsToSuppress.Add(group.Key);
                }
            }
        }

        foreach (var wageItemId in wageItemIdsToSuppress.Where(organisationLoanWageItemIds.Contains))
        {
            foreach (var wageItem in ficheItems.Where(i => i.WageItemId == wageItemId))
            {
                SuppressInactiveLoanWageItem(wageItem);
            }

            if (ficheItemsByWageItemId.TryGetValue(wageItemId, out var mappedWageItem)
                && !ficheItems.Contains(mappedWageItem))
            {
                SuppressInactiveLoanWageItem(mappedWageItem);
            }
        }
    }

    private static Dictionary<long, OrganisationEmployeeTypeFicheItemDTO> BuildFicheItemsByWageItemId(
        IEnumerable<OrganisationEmployeeTypeFicheItemDTO> items)
    {
        var lookup = new Dictionary<long, OrganisationEmployeeTypeFicheItemDTO>();
        foreach (var item in items)
        {
            lookup[item.WageItemId] = item;
        }

        return lookup;
    }

    private static void ComputeFicheAmountTotals(
        IList<OrganisationEmployeeTypeFicheItemDTO> items,
        out double totalPaymentAmount,
        out double insuranceTotalPaymentAmountDsw,
        out double paymentInsuranceCovered,
        out long deductionSumAmount,
        out long dailyFunctionAmount,
        out long spouseAmount,
        out long sumCashTaxCoveredAndContinuous,
        out double secondaryTotalPaymentAmount,
        out long secondaryDeductionSumAmount,
        out long secondaryBillEydiOpadashAmount,
        out long secondaryBillBazkharidSanavatAmount)
    {
        totalPaymentAmount = 0;
        insuranceTotalPaymentAmountDsw = 0;
        paymentInsuranceCovered = 0;
        deductionSumAmount = 0;
        dailyFunctionAmount = 0;
        spouseAmount = 0;
        sumCashTaxCoveredAndContinuous = 0;
        secondaryTotalPaymentAmount = 0;
        secondaryDeductionSumAmount = 0;
        secondaryBillEydiOpadashAmount = 0;
        secondaryBillBazkharidSanavatAmount = 0;

        var paymentTypePayment = (long)Enums.PaymentType.Payment;
        var paymentTypeDeduction = (long)Enums.PaymentType.Deduction;

        foreach (var item in items)
        {
            if (item.IsVirtual)
            {
                continue;
            }

            // قلم فرعی در جمع پرداخت/کسور (با احتساب) لحاظ می‌شود؛
            // Secondary* فقط سهم اقلام فرعی را برای نمایش محاسباتی نگه می‌دارد (ذخیره نمی‌شود).
            if (item.PaymentTypeId == paymentTypePayment)
            {
                totalPaymentAmount += item.Amount;

                if (item.IsSubItem)
                {
                    secondaryTotalPaymentAmount += item.Amount;
                    if (item.IsAnnualEydi)
                    {
                        secondaryBillEydiOpadashAmount += (long)item.Amount;
                    }
                    if (item.IsSeverancePay || item.IsServiceBuyback)
                    {
                        secondaryBillBazkharidSanavatAmount += (long)item.Amount;
                    }
                    continue;
                }

                if (!item.IsChildItem)
                {
                    insuranceTotalPaymentAmountDsw += item.Amount;
                }
                if (item.IsInsuranceCovered == true)
                {
                    paymentInsuranceCovered += item.Amount;
                }

                if (item.IsEmployerItem != true)
                {
                    if (item.DailyCovered == true)
                    {
                        dailyFunctionAmount += (long)item.Amount;
                    }

                    if (item.IsSpouse)
                    {
                        spouseAmount += (long)item.Amount;
                    }
                }

                if (item.Continuous == true && item.IsTaxCovered == true)
                {
                    sumCashTaxCoveredAndContinuous += (long)item.Amount;
                }
            }
            else if (item.PaymentTypeId == paymentTypeDeduction && item.IsEmployerItem != true)
            {
                deductionSumAmount += (long)item.Amount;
                if (item.IsSubItem)
                {
                    secondaryDeductionSumAmount += (long)item.Amount;
                }
            }
        }
    }

    /// <summary>
    /// سهم قلم‌های فرعی را از ردیف‌های فیش محاسبه می‌کند (ذخیره نمی‌شود).
    /// </summary>
    public static void ApplyComputedSubItemAmounts(
        FicheDTO fiche,
        IEnumerable<FicheItemDTO> wages,
        IList<OrganisationEmployeeTypeFicheItemDTO>? settings = null)
    {
        var paymentTypePayment = (long)Enums.PaymentType.Payment;
        var paymentTypeDeduction = (long)Enums.PaymentType.Deduction;
        var settingsByWageId = settings?
            .GroupBy(i => i.WageItemId)
            .ToDictionary(g => g.Key, g => g.First())
            ?? new Dictionary<long, OrganisationEmployeeTypeFicheItemDTO>();

        double secondaryPayment = 0;
        long secondaryDeduction = 0;
        long secondaryEydi = 0;
        long secondarySanavat = 0;

        foreach (var item in wages.Where(i => i.IsSubItem))
        {
            if (item.PaymentTypeId == paymentTypePayment)
            {
                secondaryPayment += item.Value;
                if (settingsByWageId.TryGetValue(item.WageItemId, out var setting))
                {
                    if (setting.IsAnnualEydi)
                    {
                        secondaryEydi += (long)item.Value;
                    }
                    if (setting.IsSeverancePay || setting.IsServiceBuyback)
                    {
                        secondarySanavat += (long)item.Value;
                    }
                }
            }
            else if (item.PaymentTypeId == paymentTypeDeduction && item.IsEmployerItem != true)
            {
                secondaryDeduction += (long)item.Value;
            }
        }

        fiche.SecondaryTotalPaymentAmount = secondaryPayment;
        fiche.SecondaryDeductionSumAmount = secondaryDeduction;
        fiche.SecondaryPayableAmount = secondaryPayment - secondaryDeduction;
        fiche.SecondaryTotalAmount = Convert.ToInt64(secondaryPayment);
        fiche.SecondaryDeductedAmount = secondaryDeduction;
        fiche.SecondaryPurePaymentAmount = Convert.ToInt64(fiche.SecondaryPayableAmount);
        fiche.SecondaryBillEydiOpadashAmount = secondaryEydi;
        fiche.SecondaryBillBazkharidSanavatAmount = secondarySanavat;
    }

    private static void ApplyTaxCategoryFlags(OrganisationEmployeeTypeFicheItemDTO target, OrganisationFicheItem source)
    {
        target.IsGrossContinuousCashCurrentMonth = source.IsGrossContinuousCashCurrentMonth;
        target.IsContinuousCashArearsNoTax = source.IsContinuousCashArearsNoTax;
        target.IsEmployeeHousingDeductionCurrentMonth = source.IsEmployeeHousingDeductionCurrentMonth;
        target.IsEmployeeCarDeductionCurrentMonth = source.IsEmployeeCarDeductionCurrentMonth;
        target.IsContinuousNonCashOtherBenefitsCost = source.IsContinuousNonCashOtherBenefitsCost;
        target.IsContinuousNonCashArearsNoTax = source.IsContinuousNonCashArearsNoTax;
        target.IsConsultingFeesAndSimilar = source.IsConsultingFeesAndSimilar;
        target.IsResearchContracts = source.IsResearchContracts;
        target.IsOvertime = source.IsOvertime;
        target.IsTravelExpense = source.IsTravelExpense;
        target.IsMissionAllowance = source.IsMissionAllowance;
        target.IsKaraneh = source.IsKaraneh;
        target.IsBonusExceptYearEndServiceEndProductivity = source.IsBonusExceptYearEndServiceEndProductivity;
        target.IsYearEndBonus = source.IsYearEndBonus;
        target.IsAnnualEydi = source.IsAnnualEydi;
        target.IsEndOfServiceBonus = source.IsEndOfServiceBonus;
        target.IsDismissalCompensation = source.IsDismissalCompensation;
        target.IsServiceBuyback = source.IsServiceBuyback;
        target.IsSeverancePay = source.IsSeverancePay;
        target.IsUnusedLeavePay = source.IsUnusedLeavePay;
        target.IsNonContinuousCashCurrentMonth = source.IsNonContinuousCashCurrentMonth;
        target.IsNonContinuousCashArearsNoTax = source.IsNonContinuousCashArearsNoTax;
        target.IsNonContinuousNonCashCostCurrentMonth = source.IsNonContinuousNonCashCostCurrentMonth;
        target.IsNonContinuousNonCashArearsNoTax = source.IsNonContinuousNonCashArearsNoTax;
        target.IsMedicalInsuranceArticle137 = source.IsMedicalInsuranceArticle137;
        target.IsLifeInsuranceArticle137 = source.IsLifeInsuranceArticle137;
        target.IsTeachingResearchFees = source.IsTeachingResearchFees;
        target.IsOnCallPay = source.IsOnCallPay;
        target.IsWelfareMotivationProductivity = source.IsWelfareMotivationProductivity;
        target.IsWorkEffortExcludingWageSalaryBonus = source.IsWorkEffortExcludingWageSalaryBonus;
        target.IsTaxableContinuousCash = source.IsTaxableContinuousCash;
        target.IsTaxableNonContinuousCash = source.IsTaxableNonContinuousCash;
        target.IsTaxableContinuousNonCash = source.IsTaxableContinuousNonCash;
        target.IsTaxableNonContinuousNonCash = source.IsTaxableNonContinuousNonCash;
        target.IsSpecialTax = source.IsSpecialTax;
        target.IsTaxDiscount = source.IsTaxDiscount;
    }

    private static void ApplyTaxCategoryFlags(OrganisationEmployeeTypeFicheItemDTO target, OrganisationEmployeeTypeFicheItemDTO source)
    {
        target.IsGrossContinuousCashCurrentMonth = source.IsGrossContinuousCashCurrentMonth;
        target.IsContinuousCashArearsNoTax = source.IsContinuousCashArearsNoTax;
        target.IsEmployeeHousingDeductionCurrentMonth = source.IsEmployeeHousingDeductionCurrentMonth;
        target.IsEmployeeCarDeductionCurrentMonth = source.IsEmployeeCarDeductionCurrentMonth;
        target.IsContinuousNonCashOtherBenefitsCost = source.IsContinuousNonCashOtherBenefitsCost;
        target.IsContinuousNonCashArearsNoTax = source.IsContinuousNonCashArearsNoTax;
        target.IsConsultingFeesAndSimilar = source.IsConsultingFeesAndSimilar;
        target.IsResearchContracts = source.IsResearchContracts;
        target.IsOvertime = source.IsOvertime;
        target.IsTravelExpense = source.IsTravelExpense;
        target.IsMissionAllowance = source.IsMissionAllowance;
        target.IsKaraneh = source.IsKaraneh;
        target.IsBonusExceptYearEndServiceEndProductivity = source.IsBonusExceptYearEndServiceEndProductivity;
        target.IsYearEndBonus = source.IsYearEndBonus;
        target.IsAnnualEydi = source.IsAnnualEydi;
        target.IsEndOfServiceBonus = source.IsEndOfServiceBonus;
        target.IsDismissalCompensation = source.IsDismissalCompensation;
        target.IsServiceBuyback = source.IsServiceBuyback;
        target.IsSeverancePay = source.IsSeverancePay;
        target.IsUnusedLeavePay = source.IsUnusedLeavePay;
        target.IsNonContinuousCashCurrentMonth = source.IsNonContinuousCashCurrentMonth;
        target.IsNonContinuousCashArearsNoTax = source.IsNonContinuousCashArearsNoTax;
        target.IsNonContinuousNonCashCostCurrentMonth = source.IsNonContinuousNonCashCostCurrentMonth;
        target.IsNonContinuousNonCashArearsNoTax = source.IsNonContinuousNonCashArearsNoTax;
        target.IsMedicalInsuranceArticle137 = source.IsMedicalInsuranceArticle137;
        target.IsLifeInsuranceArticle137 = source.IsLifeInsuranceArticle137;
        target.IsTeachingResearchFees = source.IsTeachingResearchFees;
        target.IsOnCallPay = source.IsOnCallPay;
        target.IsWelfareMotivationProductivity = source.IsWelfareMotivationProductivity;
        target.IsWorkEffortExcludingWageSalaryBonus = source.IsWorkEffortExcludingWageSalaryBonus;
        target.IsTaxableContinuousCash = source.IsTaxableContinuousCash;
        target.IsTaxableNonContinuousCash = source.IsTaxableNonContinuousCash;
        target.IsTaxableContinuousNonCash = source.IsTaxableContinuousNonCash;
        target.IsTaxableNonContinuousNonCash = source.IsTaxableNonContinuousNonCash;
        target.IsSpecialTax = source.IsSpecialTax;
        target.IsTaxDiscount = source.IsTaxDiscount;
    }

    private static void ApplyOrganisationCoverageFields(OrganisationEmployeeTypeFicheItemDTO target, OrganisationFicheItem source, bool includeEnterType, bool includePriority, bool includeFixValue)
    {
        target.PaymentTypeId = source.PaymentTypeId;
        target.PaymentType = source.PaymentType!.title;
        target.Continuous = source.Continuous;
        target.ShowZeroInFiche = source.ShowZeroInFiche;
        target.IsVirtual = source.IsVirtual;
        target.IsInsuranceCovered = source.IsInsuranceCovered;
        target.IsTaxCovered = source.IsTaxCovered;
        target.RetiredCovered = source.RetiredCovered;
        target.DailyCovered = source.DailyCovered;
        target.IsDaily = source.IsDaily;
        if (includePriority)
        {
            target.Priority = source.Priority;
        }
        target.Description = source.Description;
        target.IsFixed = source.IsFixed;
        target.OnceInFiche = source.OnceInFiche;
        target.IsTaminInsurance = source.IsTaminInsurance;
        target.ArearsCovered = source.ArearsCovered;
        target.ZeroNegativeArears = source.ZeroNegativeArears;
        target.CurrentYearArearsCovered = source.CurrentYearArearsCovered;
        target.IsEmployerItem = source.IsEmployerItem;
        target.RetiredCover = source.RetiredCover;
        target.IsSpouse = source.IsSpouse;
        target.IsChildItem = source.IsChildItem;
        target.IsMainTaxItem = source.IsMainTaxItem;
        target.IsSubItem = source.IsSubItem;
        if (includeEnterType)
        {
            target.EnterTypeId = source.EnterTypeId;
            target.EnterType = source.EnterType!.title;
            target.OrganisationFormulaId = source.OrganisationFormulaId;
        }
        if (includeFixValue)
        {
            target.FixValue = source.FixValue;
        }
        ApplyTaxCategoryFlags(target, source);
        if (source.OrganisationFormula?.Formula != null)
        {
            target.OrganisationFormula = source.OrganisationFormula.Formula.title;
        }
        target.OrganisationCheckFormulaId = source.OrganisationCheckFormulaId;
        if (source.OrganisationCheckFormula?.Formula != null)
        {
            target.OrganisationCheckFormula = source.OrganisationCheckFormula.Formula.title;
        }
    }

    private static void ApplyEmployeeTypeCoverageFields(OrganisationEmployeeTypeFicheItemDTO target, OrganisationEmployeeTypeFicheItemDTO source, bool includePriority)
    {
        target.PaymentTypeId = source.PaymentTypeId;
        target.Continuous = source.Continuous;
        target.ShowZeroInFiche = source.ShowZeroInFiche;
        target.IsVirtual = source.IsVirtual;
        target.IsInsuranceCovered = source.IsInsuranceCovered;
        target.IsTaxCovered = source.IsTaxCovered;
        target.RetiredCovered = source.RetiredCovered;
        target.DailyCovered = source.DailyCovered;
        target.IsDaily = source.IsDaily;
        if (includePriority)
        {
            target.Priority = source.Priority;
        }
        target.Description = source.Description;
        target.IsFixed = source.IsFixed;
        target.OnceInFiche = source.OnceInFiche;
        target.IsTaminInsurance = source.IsTaminInsurance;
        target.ArearsCovered = source.ArearsCovered;
        target.ZeroNegativeArears = source.ZeroNegativeArears;
        target.CurrentYearArearsCovered = source.CurrentYearArearsCovered;
        target.IsEmployerItem = source.IsEmployerItem;
        target.RetiredCover = source.RetiredCover;
        target.IsSpouse = source.IsSpouse;
        target.IsChildItem = source.IsChildItem;
        target.IsMainTaxItem = source.IsMainTaxItem;
        target.IsSubItem = source.IsSubItem;
        target.EnterTypeId = source.EnterTypeId;
        target.EnterType = source.EnterType;
        target.OrganisationFormulaId = source.OrganisationFormulaId;
        ApplyTaxCategoryFlags(target, source);
        if (source.OrganisationFormula != null)
        {
            target.OrganisationFormula = source.OrganisationFormula;
        }
        target.OrganisationCheckFormulaId = source.OrganisationCheckFormulaId;
        if (source.OrganisationCheckFormula != null)
        {
            target.OrganisationCheckFormula = source.OrganisationCheckFormula;
        }
    }

    private static bool TryAddExecutableSetting(
        List<OrganisationEmployeeTypeFicheItemDTO> executAbleSetting,
        HashSet<long> addedWageItemIds,
        OrganisationEmployeeTypeFicheItemDTO toAdd)
    {
        if (!addedWageItemIds.Add(toAdd.WageItemId))
        {
            return false;
        }

        executAbleSetting.Add(toAdd);
        return true;
    }

    public (bool flowControl, OperationResult value) GetComputeSettings(long EmployeeId, FicheDTO ret, PaymentPeriod CurrentPeriod, RecruitOrder RecruitOrder, out List<OrganisationEmployeeTypeFicheItemDTO> executAbleSetting)
    {
        var organisationFicheItemPredicate = DateValidityExtension<OrganisationFicheItem>.GetDateValidationPredicate()
            .And(i => i.OrganisationChartId == RecruitOrder.PayLocationId);
        var employeeTypeFicheItemPredicate = DateValidityExtension<OrganisationEmployeeTypeFicheItem>.GetDateValidationPredicate()
            .And(i => i.EmployeeTypeId == RecruitOrder.EmployeeTypeId && i.OrganisationChartId == RecruitOrder.PayLocationId);
        var personnelFicheItemPredicate = DateValidityExtension<PersonnelFicheItem>.GetDateValidationPredicate(ImpleDate: CurrentPeriod.StartDate)
            .And(i => i.EmployeeId == RecruitOrder.EmployeeId && i.OrganisationChartId == RecruitOrder.PayLocationId);
        var costCenterFicheItemPredicate = DateValidityExtension<CostCenterFicheItem>.GetDateValidationPredicate(ImpleDate: CurrentPeriod.StartDate)
            .And(i => i.CostCenterId == RecruitOrder.CostCenterId && i.OrganisationChartId == RecruitOrder.PayLocationId);

        var OrganisationFicheItemSetting = _db.Set<OrganisationFicheItem>()
            .AsNoTracking()
            .Include(i => i.EnterType)
            .Include(i => i.OrganisationFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.OrganisationCheckFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.PaymentType)
            .Include(i => i.WageItem)
            .Where(organisationFicheItemPredicate)
            .ToList();

        var employeeTypeSetting = _mapper.Map<List<OrganisationEmployeeTypeFicheItemDTO>>(_db.Set<OrganisationEmployeeTypeFicheItem>()
            .AsNoTracking()
            .Include(i => i.EnterType)
            .Include(i => i.OrganisationFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.OrganisationCheckFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.PaymentType)
            .Include(i => i.WageItem)
            .Where(employeeTypeFicheItemPredicate)
            .ToList());

        var PersonnelFicheItemSetting = _db.Set<PersonnelFicheItem>()
            .AsNoTracking()
            .Include(i => i.EnterType)
            .Include(i => i.OrganisationFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.OrganisationCheckFormula!)
                .ThenInclude(f => f.Formula)
            .Include(i => i.WageItem)
            .Where(personnelFicheItemPredicate)
            .ToList();

        var CostCenterFicheItemSetting = _db.Set<CostCenterFicheItem>()
            .AsNoTracking()
            .Include(i => i.WageItem)
            .Where(costCenterFicheItemPredicate)
            .ToList();

        var organisationFicheItemsByWageItemId = OrganisationFicheItemSetting.ToDictionary(i => i.WageItemId);
        var personnelFicheItemsByWageItemId = PersonnelFicheItemSetting.ToDictionary(i => i.WageItemId);
        var costCenterFicheItemsByWageItemId = CostCenterFicheItemSetting.ToDictionary(i => i.WageItemId);

        executAbleSetting = new List<OrganisationEmployeeTypeFicheItemDTO>();
        var addedWageItemIds = new HashSet<long>();

        foreach (var item in employeeTypeSetting)
        {
            if (addedWageItemIds.Contains(item.WageItemId))
            {
                continue;
            }

            try
            {
                if (!organisationFicheItemsByWageItemId.TryGetValue(item.WageItemId, out var organisationFicheItem))
                {
                    continue;
                }

                OrganisationEmployeeTypeFicheItemDTO toAdd = new OrganisationEmployeeTypeFicheItemDTO();
                if (item.IsForOtherSources == true)
                {
                    if (costCenterFicheItemsByWageItemId.TryGetValue(item.WageItemId, out var CostCenterFicheItem))
                    {
                        toAdd.Origin = "عوامل اختصاصی مرکز هزینه";
                        toAdd.OriginId = (int)Enums.OriginOfFicheItem.CostCenter;
                        toAdd.WageItemId = CostCenterFicheItem.WageItemId;
                        toAdd.WageItem = CostCenterFicheItem.WageItem!.title;

                        if (CostCenterFicheItem.Amount.HasValue)
                        {
                            toAdd.FixValue = Convert.ToInt32(CostCenterFicheItem.Amount);
                        }
                        toAdd.Priority = CostCenterFicheItem.PriorityNo;
                        if (item.UseDefaultOrganSetting == true)
                        {
                            ApplyOrganisationCoverageFields(toAdd, organisationFicheItem, includeEnterType: true, includePriority: false, includeFixValue: false);
                        }
                        else
                        {
                            ApplyEmployeeTypeCoverageFields(toAdd, item, includePriority: false);
                        }

                        TryAddExecutableSetting(executAbleSetting, addedWageItemIds, toAdd);
                        continue;
                    }

                    if (personnelFicheItemsByWageItemId.TryGetValue(item.WageItemId, out var PersonnelFicheItem))
                    {
                        toAdd.PersonnelFicheItemId = PersonnelFicheItem.Id;
                        toAdd.WageItemId = PersonnelFicheItem.WageItemId;
                        toAdd.WageItem = PersonnelFicheItem.WageItem!.title;
                        toAdd.EnterTypeId = PersonnelFicheItem.EnterTypeId;
                        toAdd.EnterType = PersonnelFicheItem.EnterType!.title;
                        toAdd.OrganisationCheckFormulaId = PersonnelFicheItem.OrganisationCheckFormulaId;
                        toAdd.OrganisationFormulaId = PersonnelFicheItem.OrganisationFormulaId;
                        if (PersonnelFicheItem.OrganisationFormula?.Formula != null)
                        {
                            toAdd.OrganisationFormula = PersonnelFicheItem.OrganisationFormula.Formula.title;
                        }
                        toAdd.Origin = "عوامل اختصاصی فرد";
                        toAdd.OriginId = (int)Enums.OriginOfFicheItem.PersonnelFicheItem;
                        toAdd.FixValue = PersonnelFicheItem.Value;
                        if (item.UseDefaultOrganSetting == true)
                        {
                            ApplyOrganisationCoverageFields(toAdd, organisationFicheItem, includeEnterType: false, includePriority: true, includeFixValue: false);
                        }
                        else
                        {
                            ApplyEmployeeTypeCoverageFields(toAdd, item, includePriority: true);
                        }

                        if (TryAddExecutableSetting(executAbleSetting, addedWageItemIds, toAdd))
                        {
                            ret.PersonnelPayments.Add(
                                new PersonnelPayment()
                                {
                                    EmployeeId = EmployeeId,
                                    Amount = Convert.ToInt64(toAdd.Amount),
                                    PersonnelFicheItemId = PersonnelFicheItem.Id,
                                    PaymentDate = DateTime.Now,
                                    PaymentTypeId = toAdd.PaymentTypeId!.Value,
                                    title = "",
                                    CreateDate = DateTime.Now,
                                    IPAddress = "",
                                });
                        }
                    }

                    continue;
                }

                if (item.UseDefaultOrganSetting == true)
                {
                    toAdd.WageItemId = organisationFicheItem.WageItemId;
                    toAdd.WageItem = organisationFicheItem.WageItem!.title;
                    toAdd.EnterTypeId = organisationFicheItem.EnterTypeId;
                    toAdd.EnterType = organisationFicheItem.EnterType!.title;
                    toAdd.OrganisationCheckFormulaId = organisationFicheItem.OrganisationCheckFormulaId;
                    toAdd.OrganisationFormulaId = organisationFicheItem.OrganisationFormulaId;
                    ApplyOrganisationCoverageFields(toAdd, organisationFicheItem, includeEnterType: false, includePriority: true, includeFixValue: true);
                    toAdd.Origin = "نوع استخدام";
                    toAdd.OriginId = (int)Enums.OriginOfFicheItem.EmploymentType;
                }
                else
                {
                    toAdd = item;
                }

                TryAddExecutableSetting(executAbleSetting, addedWageItemIds, toAdd);
            }
            catch (Exception)
            {
                return (flowControl: false, value: OperationResult.Failed("خطا در محاسبه قلم " + item.WageItem, payload: ret));
            }
        }

        return (flowControl: true, value: null!);
    }

    /// <summary>
    /// محاسبه مالیات - بر اساس سند سال 1402
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private TaxCalculationResult CalculateTaxNew(TaxCalculationRequest req)
    {
        TaxCalculationResult ret = new TaxCalculationResult();
        try
        {
            var periodList = req.LastPeriodsOfCurrentYear.Select(i => i.Id).ToList();
            var lastFiches = _unitOfWork.Context.Fiches.Where(i => i.EmployeeId == req._employeeId && periodList.Contains(i.PaymentPeriodId)).ToList();
            #region calculate A
            // جمع حقوق و دستمزد مستمر نقدی ماه جاری دریافتی از کل کارفرمایان ( پس از کسر مزایای نقدی معاف مستمر موضوع ماده 91 ق.م.م و سایر معافیت های قابل کسر ) 
            // ابتدا قلم های مشمول مالیات مستمر نقدی دوره جاری را استخراج می کنیم
            long SumCashTaxCoveredAndCountinious = 0;
            if (req.OrganisationEmployeeTypeFicheItem != null)
            {
                var currentSumCashTaxCoveredAndCountiniousItems = req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.Continuous == true && i.IsTaxCovered == true && i.IsVirtual != true);
                SumCashTaxCoveredAndCountinious = Convert.ToInt64(currentSumCashTaxCoveredAndCountiniousItems.Sum(i => i.Amount));

                var mazayaNaghdiMoafMostamarMade91 = Convert.ToInt64(req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.Continuous == true && i.IsTaxCovered != true && i.IsVirtual != true).Sum(i => i.Amount));

                ret.SumCashTaxCoveredAndCountinious = SumCashTaxCoveredAndCountinious - mazayaNaghdiMoafMostamarMade91;
            }

            if (req._paymentPeriod.ShamsiMonth > 1)
            {
                // جمع حقوق و دستمزد مستمر نقدی تا پایان ماه قبل دریافتی از کل کارفرمایان
                foreach (var item in req.LastPeriodsOfCurrentYear.OrderByDescending(i => i.ShamsiMonth))
                {
                    var fiche = lastFiches.Where(i => i.PaymentPeriodId == item.Id);

                    if (fiche == null)
                    {
                        break;
                    }
                    else
                    {
                        if (fiche.Any())
                        {
                            SumCashTaxCoveredAndCountinious = SumCashTaxCoveredAndCountinious + (fiche.Single().SumCashTaxCoveredAndCountinious.HasValue ? 0 : fiche.Single().SumCashTaxCoveredAndCountinious.Value);
                        }
                    }

                }
            }

            long A = SumCashTaxCoveredAndCountinious * (12 / req._paymentPeriod.ShamsiMonth);

            #endregion calculate A
            #region calculate B


            long SumNonCashTaxCoveredAndCountinious = 0;

            //جمع مزایای مستمر غیر نقدی ماه جاری دریافتی از کل کارفرمایان
            var TaxNonCashPaymentsContinuous = _unitOfWork.Context.TaxNonCashPayments.Where(i => i.PaymentPeriodId == req._paymentPeriod.Id && i.Continuous == true && i.EmployeeId == req._employeeId);

            if (TaxNonCashPaymentsContinuous == null)
            {

            }
            else
            {
                if (TaxNonCashPaymentsContinuous.Any())
                {
                    // جمع مزایای مستمر غیر نقدی ماه جاری دریافتی از کل کارفرمایان
                    foreach (var TaxNonCashPayment in TaxNonCashPaymentsContinuous)
                    {
                        SumNonCashTaxCoveredAndCountinious = SumNonCashTaxCoveredAndCountinious + Convert.ToInt64(TaxNonCashPayment.Value);
                    }
                }
            }

            ret.SumNonCashTaxCoveredAndCountinious = SumNonCashTaxCoveredAndCountinious;
            if (req._paymentPeriod.ShamsiMonth > 1)
            {
                // جمع حقوق و دستمزد مستمر غیر نقدی تا پایان ماه قبل دریافتی از کل کارفرمایان
                foreach (var item in req.LastPeriodsOfCurrentYear.OrderByDescending(i => i.ShamsiMonth))
                {
                    var fiche = lastFiches.Where(i => i.PaymentPeriodId == item.Id);

                    if (fiche == null)
                    {
                        break;
                    }
                    else
                    {
                        if (fiche.Any())
                        {
                            SumNonCashTaxCoveredAndCountinious = SumNonCashTaxCoveredAndCountinious + (fiche.Single().SumNonCashTaxCoveredAndCountinious.HasValue ? 0 : fiche.Single().SumCashTaxCoveredAndCountinious.Value);
                        }
                    }
                }
            }

            long B = SumNonCashTaxCoveredAndCountinious * (12 / req._paymentPeriod.ShamsiMonth);


            #endregion calculate B 
            #region calculate C

            long SumNonCashTaxCoveredAndNotCountinious = 0;
            var NonCashTaxCoveredAndNotCountinious = _unitOfWork.Context.TaxNonCashPayments.Where(i => i.PaymentPeriodId == req._paymentPeriod.Id && i.Continuous == false && i.EmployeeId == req._employeeId).ToList();
            if (NonCashTaxCoveredAndNotCountinious == null)
            {

            }
            else
            {
                if (NonCashTaxCoveredAndNotCountinious.Any())
                {
                    // جمع مزایای غیر مستمر غیر نقدی ماه جاری دریافتی از کل کارفرمایان
                    foreach (var TaxNonCashPayment in NonCashTaxCoveredAndNotCountinious)
                    {
                        SumNonCashTaxCoveredAndNotCountinious = SumNonCashTaxCoveredAndNotCountinious + Convert.ToInt64(TaxNonCashPayment.Value);
                    }
                }
            }

            ret.SumNonCashTaxCoveredAndNotCountinious = SumNonCashTaxCoveredAndNotCountinious;
            if (req._paymentPeriod.ShamsiMonth > 1)
            {
                // جمع حقوق و دستمزد غیر مستمر غیر نقدی تا پایان ماه قبل دریافتی از کل کارفرمایان
                foreach (var item in req.LastPeriodsOfCurrentYear.OrderByDescending(i => i.ShamsiMonth))
                {
                    var fiche = lastFiches.Where(i => i.PaymentPeriodId == item.Id);

                    if (fiche == null)
                    {
                        break;
                    }
                    else
                    {
                        if (fiche.Any())
                        {
                            SumNonCashTaxCoveredAndNotCountinious = SumNonCashTaxCoveredAndNotCountinious + (fiche.Single().SumNonCashTaxCoveredAndNotCountinious.HasValue ? 0 : fiche.Single().SumNonCashTaxCoveredAndNotCountinious.Value);
                        }
                    }
                }
            }

            long C = SumNonCashTaxCoveredAndNotCountinious;


            #endregion calculate C
            #region calculate D


            long SumCashTaxCoveredAndNotCountinious = 0;
            if (req.OrganisationEmployeeTypeFicheItem != null)
            {
                var currentSumCashTaxCoveredAndNotCountiniousItems = req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.Continuous != true && i.IsTaxCovered == true && i.IsVirtual != true);
                SumCashTaxCoveredAndNotCountinious = Convert.ToInt64(currentSumCashTaxCoveredAndNotCountiniousItems.Sum(i => i.Amount));
                var mazayaNaghdiMoafGheirMostamar = req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.Continuous != true && i.IsTaxCovered != true && i.IsVirtual != true).Sum(i => i.Amount);
                ret.SumCashTaxCoveredAndNotCountinious = Convert.ToInt64(SumCashTaxCoveredAndNotCountinious - mazayaNaghdiMoafGheirMostamar);
            }

            if (req._paymentPeriod.ShamsiMonth > 1)
            {
                // جمع حقوق و دستمزد غیر مستمر نقدی تا پایان ماه قبل دریافتی از کل کارفرمایان
                foreach (var item in req.LastPeriodsOfCurrentYear.OrderByDescending(i => i.ShamsiMonth))
                {
                    var fiche = lastFiches.Where(i => i.PaymentPeriodId == item.Id);

                    if (fiche == null)
                    {
                        break;
                    }
                    else
                    {
                        if (fiche.Any())
                        {
                            SumCashTaxCoveredAndNotCountinious = SumCashTaxCoveredAndNotCountinious + (fiche.Single().SumCashTaxCoveredAndNotCountinious.HasValue ? 0 : fiche.Single().SumCashTaxCoveredAndNotCountinious.Value);
                        }
                    }

                }
            }

            long D = SumCashTaxCoveredAndNotCountinious;

            #endregion calculate D
            var tax = _taxService.All(ImpleDate: req._paymentPeriod.StartDate).Where(i => i.EmployeeTypeId == req._recruitOrder.EmployeeTypeId);
            long MoafiatMozoBand13Made91 = 0;
            if (tax == null)
            {
                ret.Success = false;
                ret.Message = "جدول مالیاتی یافت نشد";
                return ret;
            }
            else
            {
                if (tax.Any())
                {
                    if (tax.Count() > 1)
                    {
                        ret.Success = false;
                        ret.Message = "بیش از یک جدول مالیاتی به ازای یک نوع استخدام یافت شد";
                        return ret;
                    }
                    if (tax.Count() == 1)
                    {
                        var taxEmployeeTypeSetting = tax.SingleOrDefault();
                        var taxTable = _taxTableService.All().Where(i => i.TaxId == taxEmployeeTypeSetting.Id);
                        var exemptionRow = taxTable.Where(i => i.TaxPercent == 0);
                        if (exemptionRow == null)
                        {

                        }
                        else
                        {
                            if (exemptionRow.Any())
                            {
                                var exemptionSingleRow = exemptionRow.SingleOrDefault();
                                MoafiatMozoBand13Made91 = exemptionSingleRow.ToValue * 2;
                            }
                        }
                    }
                }
                else
                {
                    ret.Success = false;
                    ret.Message = "جدول مالیاتی یافت نشد";
                    return ret;
                }
            }



            // جمع مزایای مستمر غیر نقدی سالانه ( پس از کسر معافیت موضوع بند 13 ماده 91 ق.م.م
            long S = B - MoafiatMozoBand13Made91;

            long H = 0;
            long F = 0;
            if (S < 0)
            {
                H = A;
                F = S + C;

                if (F < 0)
                {
                    F = 0;
                }
            }
            else
            {
                F = C;
                // جمع حقوق و مزایای مستمر نقدی و غیر نقدی سالانه
                H = A + S;
            }

            // جمع حقوق  ومزایای مستمر نقدی و غیر نقدی سالانه و مزایای غیر مستمر نقدی و غیر نقدی تا پایان ماه جاری
            long E = H + F + D;

            if (tax == null)
            {
                ret.Success = false;
                ret.Message = "جدول مالیاتی یافت نشد";
                return ret;
            }
            else
            {
                if (tax.Any())
                {
                    if (tax.Count() > 1)
                    {
                        ret.Success = false;
                        ret.Message = "بیش از یک جدول مالیاتی به ازای یک نوع استخدام یافت شد";
                        return ret;
                    }
                    if (tax.Count() == 1)
                    {
                        var taxEmployeeTypeSetting = tax.SingleOrDefault();
                        ret.RelatedTaxWageItemId = taxEmployeeTypeSetting.WageItemId;
                        var taxTable = _taxTableService.All().Where(i => i.TaxId == taxEmployeeTypeSetting.Id);
                        ret.TaxCoveredSum = req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.IsVirtual != true && i.IsEmployerItem != true && i.IsTaxCovered == true).Sum(i => i.Amount);

                        long sumOfYearlyTaxCovered = Convert.ToInt64(ret.TaxCoveredSum);

                        foreach (var item in req.LastPeriodsOfCurrentYear.OrderByDescending(i => i.ShamsiMonth))
                        {
                            var fiche = lastFiches.Where(i => i.PaymentPeriodId == item.Id);
                            if (fiche == null)
                            {
                                break;
                            }
                            else
                            {
                                if (fiche.Any())
                                {
                                    sumOfYearlyTaxCovered = sumOfYearlyTaxCovered + fiche.Single().PaymentTax;
                                }
                            }

                        }

                        foreach (var item in taxTable.OrderBy(i => i.TaxPercent))
                        {
                            if (sumOfYearlyTaxCovered > (item.ToValue * req._paymentPeriod.ShamsiMonth))
                            {
                                double currentRowExtraValue = ((item.ToValue * req._paymentPeriod.ShamsiMonth) - (item.FromValue * req._paymentPeriod.ShamsiMonth)) * Convert.ToDouble(Convert.ToDouble(item.TaxPercent) / 100);
                                ret.CurrentTax = ret.CurrentTax + currentRowExtraValue;
                            }
                            else
                            {
                                var currentRowExtraValue = (sumOfYearlyTaxCovered - (item.FromValue * req._paymentPeriod.ShamsiMonth)) * Convert.ToDouble(Convert.ToDouble(item.TaxPercent) / 100);
                                ret.CurrentTax = ret.CurrentTax + currentRowExtraValue;
                                break;
                            }
                        }
                        ret.CurrentTax = Math.Round(ret.CurrentTax);

                        //  کل مالیات متغلق مستمر سالانه و مالیات غیر مستمر تا پایان ماه جاری
                        long G = Convert.ToInt64(E - ret.CurrentTax);
                        // مالیات حقوق و مزایای مستمر نقدی و غیر نقدی سالانه
                        long K = Convert.ToInt64(H - ret.CurrentTax);


                        // مالیات حقوق و مزایای مستمر نقدی و غیر نقدی تا پایان ماه جاری
                        var division = req._paymentPeriod.ShamsiMonth / 12.0;
                        long P = Convert.ToInt64(K * division);
                        // مالیات مزایای غیر مستمر نقدی و غیر نقدی تا پایان ماه جاری
                        long R = G - K;
                        // جمع مالیات متعلق تا پایان ماه جاری
                        long M = P + R;
                        ret.CurrentTax = M / 12;
                        ret.Success = true;
                        return ret;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ret.Message = ex.Message;
            ret.Success = false;
        }
        return ret;
    }



    /// <summary>
    /// محاسبه مالیات
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private TaxCalculationResult CalculateTax(TaxCalculationRequest req)
    {
        TaxCalculationResult ret = new TaxCalculationResult();
        try
        {
            var tax = _taxService.All(ImpleDate: req._paymentPeriod.StartDate)
                .Where(i => i.EmployeeTypeId == req._recruitOrder.EmployeeTypeId)
                .ToList();
            if (tax == null)
            {
                ret.Success = false;
                ret.Message = "جدول مالیاتی یافت نشد";
                return ret;
            }
            else
            {
                if (tax.Any())
                {
                    if (tax.Count() > 1)
                    {
                        ret.Success = false;
                        ret.Message = "بیش از یک جدول مالیاتی به ازای یک نوع استخدام یافت شد";
                        return ret;
                    }
                    if (tax.Count() == 1)
                    {
                        var taxEmployeeTypeSetting = tax.SingleOrDefault();
                        ret.RelatedTaxWageItemId = taxEmployeeTypeSetting.WageItemId;
                        var taxTable = _taxTableService.All()
                            .Where(i => i.TaxId == taxEmployeeTypeSetting.Id)
                            .OrderBy(i => i.TaxPercent)
                            .ToList();
                        ret.TaxCoveredSum = req.OrganisationEmployeeTypeFicheItem.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment && i.IsVirtual != true && i.IsEmployerItem != true && i.IsTaxCovered == true).Sum(i => i.Amount);
                        var periodList = _paymentPeriodService
                            .GetCurrentYearPeriods(req._paymentPeriod.ShamsiYear, req._paymentPeriod.ShamsiMonth)
                            .ToList();

                        foreach (var item in taxTable)
                        {
                            if (ret.TaxCoveredSum > item.ToValue)
                            {
                                double currentRowExtraValue = (item.ToValue - item.FromValue) * Convert.ToDouble(Convert.ToDouble(item.TaxPercent) / 100);
                                ret.CurrentTax = ret.CurrentTax + currentRowExtraValue;
                            }
                            else
                            {
                                var currentRowExtraValue = (ret.TaxCoveredSum - item.FromValue) * Convert.ToDouble(Convert.ToDouble(item.TaxPercent) / 100);
                                ret.CurrentTax = ret.CurrentTax + currentRowExtraValue;
                                break;
                            }
                        }
                        ret.CurrentTax = Math.Round(ret.CurrentTax);
                        foreach (var item in periodList)
                        {
                            var relatedPeriodFiceh = All(IgnoreExpired: false)
                                .Where(i => i.EmployeeId == req._employeeId && i.PaymentPeriodId == item.Id)
                                .ToList();
                            if (relatedPeriodFiceh == null)
                            {
                                break;
                            }
                            else
                            {
                                if (relatedPeriodFiceh.Any())
                                {

                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        ret.Success = true;
                        return ret;
                    }
                }
                else
                {
                    ret.Success = false;
                    ret.Message = "جدول مالیاتی یافت نشد";
                    return ret;
                }
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CalculateTax for EmployeeId {EmployeeId} and PaymentPeriodId {PaymentPeriodId}",
                req._employeeId, req._paymentPeriod?.Id);
            ret.Success = false;
            ret.Message = $"خطای بحرانی در محاسبه مالیات: {ex.Message}";
        }
        return ret;
    }
    /// <summary>
    /// حذف موردی فیش حقوقی
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult DeleteRecord(long Id, bool IgnoreCalculate = false)
    {
        try
        {
            var fiche = _unitOfWork.Context.Fiches.Find(Id);
            if (fiche != null)
            {
                if (fiche.FicheStatusId == (long)Enums.FicheStatus.Initial)
                {

                }
                else
                {
                    return OperationResult.Failed("وضعیت فیش صدور اولیه نمی باشد و امکان حذف وجود ندارد");
                }

                if (_db.Set<BankDiskette>().Any(i => i.PaymentPeriodId == fiche.PaymentPeriodId && i.BankDisketteStatusId != (long)Enums.BankDisketteStatus.Deleted))
                {
                    return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت بانک وجود دارد");
                }
                if (_db.Set<TaxDiskette>().Any(i => i.PaymentPeriodId == fiche.PaymentPeriodId && i.TaxDisketteStatusId != (long)Enums.TaxDisketteStatus.Deleted))
                {
                    return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت دارایی وجود دارد");
                }
                if (_db.Set<InsuranceDiskette>().Any(i => i.PaymentPeriodId == fiche.PaymentPeriodId && i.InsuranceDisketteStatusId != (long)Enums.InsuranceDisketteStatus.Deleted))
                {
                    return OperationResult.Failed("برای دوره فیش مورد نظر دیسکت بیمه وجود دارد");
                }
                var paymentPeriod = _db.Set<PaymentPeriod>().Find(fiche.PaymentPeriodId);
                if (paymentPeriod.IsClosed)
                {
                    return OperationResult.Failed("دوره فیش مورد نظر بسته شده است و امکان حذف وجود ندارد");
                }
                _unitOfWork.CreateTransaction();
                var FicheItems = _unitOfWork.Context.FicheItems.Where(i => i.FicheId == Id);
                foreach (var item in FicheItems)
                {
                    _unitOfWork.Context.Remove(item);
                }


                var FicheLeaveItems = _unitOfWork.Context.FicheLeaveItems.Where(i => i.FicheId == Id);
                foreach (var item in FicheLeaveItems)
                {
                    _unitOfWork.Context.Remove(item);
                }


                var BankDisketteItems = _unitOfWork.Context.BankDisketteItems.Where(i => i.FicheId == Id);
                foreach (var item in BankDisketteItems)
                {
                    _unitOfWork.Context.Remove(item);
                }

                var TaxDisketteWhs = _unitOfWork.Context.TaxDisketteWHs.Where(i => i.FicheId == Id);
                foreach (var item in TaxDisketteWhs)
                {
                    _unitOfWork.Context.Remove(item);
                }

                var TaxDisketteWPs = _unitOfWork.Context.TaxDisketteWPs.Where(i => i.FicheId == Id);
                foreach (var item in TaxDisketteWPs)
                {
                    _unitOfWork.Context.Remove(item);
                }

                var InsuranceDisketteItems = _unitOfWork.Context.InsuranceDisketteItems.Where(i => i.FicheId == Id);
                foreach (var item in InsuranceDisketteItems)
                {
                    _unitOfWork.Context.Remove(item);
                }

                var PersonnelLoanPayments = _unitOfWork.Context.PersonnelLoanPayments.Where(i => i.FicheId == Id);
                foreach (var item in PersonnelLoanPayments)
                {
                    _unitOfWork.Context.Remove(item);
                }

                var EmployeeDeductionPayments = _unitOfWork.Context.EmployeeDeductionPayments.Where(i => i.FicheId == Id);
                foreach (var item in EmployeeDeductionPayments)
                {
                    _unitOfWork.Context.Remove(item);
                }


                var PersonnelPayments = _unitOfWork.Context.PersonnelPayments.Where(i => i.FicheId == Id);
                foreach (var item in PersonnelPayments)
                {
                    _unitOfWork.Context.Remove(item);
                }

                _unitOfWork.Context.Remove(fiche);
                _unitOfWork.Context.SaveChanges();
                _unitOfWork.Commit();

                if (IgnoreCalculate == true)
                {
                    return OperationResult.Succeeded();
                }
                return CalculateFiche(fiche.EmployeeId, fiche.PaymentPeriodId, fiche.OrganisationChartId, true, false, false);
            }
            return OperationResult.Failed("فیش مورد نظر یافت نشد");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed(ex.Message);
        }
    }

    public OperationResult DownloadFichePDF(long ficheId, bool isRaw, bool isArear = false)
    {
        long interdictOrderId;
        long employeeTypeId;
        long payLocationId;
        long employeeId;

        if (isArear)
        {
            var arearFiche = _unitOfWork.Context.ArearFiches
                .AsNoTracking()
                .Include(i => i.InterdictOrder)
                .ThenInclude(i => i!.RecruitOrder)
                .SingleOrDefault(i => i.Id == ficheId);

            if (arearFiche?.InterdictOrder?.RecruitOrder == null)
            {
                return OperationResult.NotFound("فیش معوق یا حکم مرتبط یافت نشد");
            }

            interdictOrderId = arearFiche.InterdictOrderId;
            employeeTypeId = arearFiche.InterdictOrder.RecruitOrder.EmployeeTypeId;
            payLocationId = arearFiche.InterdictOrder.RecruitOrder.PayLocationId;
            employeeId = arearFiche.InterdictOrder.RecruitOrder.EmployeeId;
        }
        else
        {
            var fiche = _unitOfWork.Context.Fiches
                .AsNoTracking()
                .Include(i => i.InterdictOrder)
                .ThenInclude(i => i!.RecruitOrder)
                .SingleOrDefault(i => i.Id == ficheId);

            if (fiche?.InterdictOrder?.RecruitOrder == null)
            {
                return OperationResult.NotFound("فیش حقوقی یا حکم مرتبط یافت نشد");
            }

            interdictOrderId = fiche.InterdictOrderId;
            employeeTypeId = fiche.InterdictOrder.RecruitOrder.EmployeeTypeId;
            payLocationId = fiche.InterdictOrder.RecruitOrder.PayLocationId;
            employeeId = fiche.InterdictOrder.RecruitOrder.EmployeeId;
        }

        var mrtMappings = _organisationEmployeeTypeMRTService.All().Where(i =>
            i.EmployeeTypeId == employeeTypeId
            && i.OrganisationChartId == payLocationId
            && i.IsRaw == isRaw
            && i.IsManager == false
            && i.SettingTypeId == (long)Enums.MRTtype.Fiche);

        if (!mrtMappings.Any())
        {
            return OperationResult.NotFound("تنظیمات گزارش چاپی فیش حقوقی یافت نشد");
        }

        if (mrtMappings.Count() > 1)
        {
            return OperationResult.NotFound("بیش از یک رکورد تنظیمات چاپ فیش یافت شد؛ لطفا تاریخ تنظیمات را بررسی بفرمایید");
        }

        var mrt = _organisationMRTService.GetIdAsync(mrtMappings.Single().OrganisationMRTId).Result;
        if (mrt == null)
        {
            return OperationResult.NotFound("قالب چاپ فیش حقوقی یافت نشد");
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

            var fichePdf = new FichePrint().GetFichePrint(
                ficheId,
                interdictOrderId,
                isArear,
                mrt,
                orgImg,
                employeeImg,
                absolutePath,
                _connectionString);

            return OperationResult.Succeeded(payload: fichePdf);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطا در تولید PDF فیش: " + ex.Message);
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

    public bool Validate(Fiche entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
