using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using HR.BaseInfo.Core.DTOs;
using HR.FormulaEngine.Core.Data;
using HR.FormulaEngine.Core.DTOs;
using HR.FormulaEngine.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Jace.Operations;
using Jace;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Share;
using System.Globalization;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Data.Entity.SqlServer;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using HR.BaseInfo.infrastructure.Services;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using HR.BaseInfo.Core.Entities;
using Hr.SystemSetting.Core.Entities;
using HR.Organisation.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.FormulaEngine.Infrastructure.Services;

public class FormulaService : BaseService<FormulaDefinition, FormulaEngineContext, FormulaDefinitionDTO>, IScopedServices
{

    private FormulaTableValueService _formulaTableValueService;
    private FormulaTableService _formulaTableService;


    private EmployeeTypeService _employeeTypeService;
    private FormulaOperandService _formulaOperandService;
    private FormulaDatabaseFunctionDefinitionService _formulaDatabaseFunctionDefinitionService;
    private BaseTableValueService _baseTableValueService;
    private WageItemService _wageItemService;
    private SettlementItemService _settlementItemService;
    private BaseInfoContext _formulaService;
    private FormulaEngineContext _formulaEngineContext;



    private SystemSettingContext _systemSettingContext;
    // private FormulaService _formulaService;
    private CoefficientService _coefficientService;

    private Dictionary<long, string?>? VariableFriendlyListFoDataBaseFunctions { get; set; }
    private Dictionary<string, string?>? VariableFriendlyListFoDataBaseFunctionsToAppend { get; set; }
    private readonly UserResolverService _userService;
    public FormulaService(IMapper mapper, FormulaEngineContext FormulaEngineContext, BaseInfoContext FormulaService, WageItemService WageItemService, SettlementItemService SettlementItemService, CoefficientService CoefficientService, SystemSettingContext SystemSettingContext, OrganisationChartService OrganisationChartService, EducationGradeService EducationGradeService, BaseTableValueService BaseTableValueService, EmployeeTypeService EmployeeTypeService, FormulaOperandService FormulaOperandService, FormulaDatabaseFunctionDefinitionService FormulaDatabaseFunctionDefinitionService, FormulaTableService FormulaTableService, FormulaTableValueService FormulaTableValueService, IUnitOfWork<FormulaEngineContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _formulaEngineContext = FormulaEngineContext;
        _wageItemService = WageItemService;
        _settlementItemService = SettlementItemService;
        _formulaService = FormulaService;
        _coefficientService = CoefficientService;
        _systemSettingContext = SystemSettingContext;
        //_organisationWageItemService = OrganisationWageItemService;
        //_organisationCoefficientService = OrganisationCoefficientService;
        //_organisationOrderTypeService = OrganisationOrderTypeService;
        _employeeTypeService = EmployeeTypeService;
        _formulaOperandService = FormulaOperandService;
        _formulaDatabaseFunctionDefinitionService = FormulaDatabaseFunctionDefinitionService;
        _formulaTableValueService = FormulaTableValueService;
        _formulaTableService = FormulaTableService;
        _baseTableValueService = BaseTableValueService;

        VariableFriendlyListFoDataBaseFunctions = new Dictionary<long, string?>();
        VariableFriendlyListFoDataBaseFunctionsToAppend = new Dictionary<string, string?>();
        _userService = userService;
    }

    public async Task LogFormulaHistoryAsync(long formulaDefinitionId, string? previousFormulaText)
    {
        var history = new FormulaDefinitionHistory
        {
            FormulaDefinitionId = formulaDefinitionId,
            PreviousFormulaText = previousFormulaText,
            IPAddress = _userService?.GetIP(),
            UserId = _userService?.GetUserId(),
            UserFullName = _userService?.fullname(),
            ChangeDateTime = DateTime.Now,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now
        };
        _unitOfWork.Context.FormulaDefinitionHistories.Add(history);
        await _unitOfWork.Save();
    }

    public IQueryable<FormulaDefinitionHistory> GetHistory(long formulaDefinitionId)
    {

        return _unitOfWork.Context.FormulaDefinitionHistories
            .AsNoTracking()
            .Where(h => h.FormulaDefinitionId == formulaDefinitionId)
            .OrderByDescending(h => h.ChangeDateTime);
    }

    public new OperationResult Get(long id)
    {
        try
        {
            var row = All().Include(i => i.OrganisationFormula.Formula).AsNoTracking().SingleOrDefault(i => i.Id == id);
            var record = _mapper.Map<FormulaDefinitionDTO>(row);
            if (record == null)
            {
                return OperationResult.NotFound();
            }
            else
            {
                return OperationResult.Succeeded(payload: record);
            }
        }
        catch (Exception ex)
        {

            throw new Exception(" خطا در Get " + ex.Message);
        }
    }




    public FormulaCalculateResponseDTO Calculate(FormulaCalculationRequestDTO req)
    {
        var ret = new FormulaCalculateResponseDTO { };
        ret.VariableFriendlyList = req.VariableFriendlyList;
        #region FillFrienlytextSources

        #endregion FillFrienlytextSources

        try
        {
            CalculationEngine engine = new CalculationEngine();
            engine.AddFunction("inOperator".Trim().ToLower(), inOperator);
            var currentFormula = _formulaEngineContext.FormulaDefinitions
                .AsNoTracking()
                .Where(i => i.Id == req.OrganisationFormulaId)
                .ToList();
            if (currentFormula != null)
            {
                if (currentFormula.Any())
                {
                    var formulaDefinition = currentFormula.Single();
                    string formulatext = "0";
                    if (formulaDefinition == null)
                    {


                    }
                    else
                    {
                        if (string.IsNullOrEmpty(formulaDefinition.FormulaText))
                        {

                        }
                        else
                        {
                            formulatext = Regex.Replace(formulaDefinition.FormulaText.Trim().ToLower(), @"\s+", " ");
                        }
                    }

                    if (req.BuildTreeTrace == true)
                    {
                        ret.FormulaFriendlyText = formulatext;
                        ret.FormulaHelpDesc = currentFormula.Single().Description;
                    }
                    var aperands = formulatext.Split('(', ')', ',', '*', '=', '>', '<', '/', '%', '!', '^', '+', '-');
                    Dictionary<string, double> VariableList = new();
                    aperands = aperands.Select(x => x.Trim()).Distinct().ToArray();
                    if (string.IsNullOrEmpty(ret.FormulaFriendlyText))
                    {
                        ret.FormulaFriendlyText = "";
                    }
                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.ToLower().Trim();

                 

                    

                    foreach (var item in aperands.Where(i => i.Trim() != ""))
                    {
                        string key = item.Trim().ToLower();
                        if (key.Contains("emptype_"))
                        {
                            if (req.BuildTreeTrace == true)
                            {
                                var employeeType = _employeeTypeService.GetIdAsync(Convert.ToInt64(key.Replace("emptype_", ""))).Result;
                                if (employeeType != null)
                                {
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " نوع استخدام " + employeeType.title);
                                }
                            }
                            VariableList.Add(key, Convert.ToDouble(key.Replace("emptype_", ""))); ;
                        }
                        if (key.Contains("subfrml_"))
                        {
                            if (req.BuildTreeTrace == true)
                            {
                                var orgFormula = _systemSettingContext.OrganisationFormulas.AsNoTracking().Single(i => i.Id == currentFormula.Single().Id);
                                var mainBaseFormula = _formulaService.Formulas.Find(orgFormula.FormulaId).title;
                                if (ret.FormulaTreeParser == null)
                                {
                                    ret.FormulaTreeParser = new FormulaExecutionTree()
                                    {
                                        FormulaName = mainBaseFormula,
                                        FormulaText = formulatext,
                                        FormulaHelpDesc = currentFormula.Single().Description
                                    };
                                    ret.FormulaTreeParser.ChildList = new List<FormulaExecutionTree>() { };
                                }
                            }


                            try
                            {
                                FormulaCalculationRequestDTO innerreq = new FormulaCalculationRequestDTO()
                                {
                                    BuildTreeTrace = req.BuildTreeTrace,
                                    VariableFriendlyList = req.VariableFriendlyList,

                                };
                                innerreq.VariableList = new Dictionary<string, double>();
                                foreach (var variable in req.VariableList)
                                {
                                    innerreq.VariableList.Add(variable.Key, variable.Value);
                                }
                                innerreq.OrganisationFormulaId = Convert.ToInt32(key.Replace("subfrml_", ""));
                                innerreq.StartDate = req.StartDate;
                                var formularesponse = Calculate(innerreq);
                                if (formularesponse.Succees == true)
                                {
                                    if (VariableList.Any(i => i.Key == key))
                                    {

                                    }
                                    else
                                    {
                                        VariableList.Add(key, formularesponse.Result);
                                        if (req.BuildTreeTrace == true)
                                        {
                                            var subformula = _systemSettingContext.OrganisationFormulas.AsNoTracking().Single(i => i.Id == Convert.ToInt32(key.Replace("subfrml_", "")));
                                            var baseFormula = _formulaService.Formulas.Find(subformula.FormulaId).title;
                                            ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " فرمول فرزند " + baseFormula);
                                            ret.FormulaTreeParser.ChildList.Add(formularesponse.FormulaTreeParser);
                                        }
                                    }
                                }
                                else
                                {
                                    ret.Succees = false;
                                    ret.ResponseMessage = " خطا در فرمول فرزند " + formularesponse.ResponseMessage + " - ";
                                    return ret;
                                }

                            }
                            catch (Exception ex)
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = " خطا در فرمول فرزند " + ex.Message + " - ";
                                return ret;
                            }
                        }

                        if (key.Contains("cof_"))
                        {
                            if (req.VariableList.Any(i => i.Key.ToLower().Trim() == key))
                            {
                                VariableList.Add(key, Convert.ToDouble(req.VariableList.Single(i => i.Key.Trim().ToLower() == key).Value));
                                if (req.BuildTreeTrace == true)
                                {
                                    var organisationCoefficient = _coefficientService.GetIdAsync(Convert.ToInt64(key.Replace("cof_", "")));
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " ضریب ( عامل ) " + organisationCoefficient.Result.title);
                                }
                            }
                            else
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = "ضریب ( عامل )   " + item + " در متن فرمول استفاده شده اما مقداری برای آن ارسال نشده است ";
                                return ret;
                            }
                        }
                        if (key.Contains("wf_"))
                        {
                            if (req.VariableList.Any(i => i.Key.ToLower().Trim() == key))
                            {
                                VariableList.Add(key, Convert.ToDouble(req.VariableList.Single(i => i.Key.Trim().ToLower() == key).Value));
                                if (req.BuildTreeTrace == true)
                                {
                                    var wage = _wageItemService.GetIdAsync(Convert.ToInt64(key.Replace("wf_", "")));
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " عامل حقوقی " + wage.Result.title);
                                }
                            }
                            else
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = "عامل حقوقی " + item + " در متن فرمول استفاده شده اما مقداری برای آن ارسال نشده است ";
                                return ret;
                            }
                        }
                        if (key.Contains("fi_"))
                        {
                            if (req.VariableList.Any(i => i.Key.ToLower().Trim() == key))
                            {
                                VariableList.Add(key, Convert.ToDouble(req.VariableList.Single(i => i.Key.Trim().ToLower() == key).Value));
                                if (req.BuildTreeTrace == true)
                                {
                                    var wage = _wageItemService.GetIdAsync(Convert.ToInt64(key.Replace("fi_", "")));
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " قلم حقوقی فیش " + wage.Result.title);
                                }
                            }
                            else
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = "  قلم فیش حقوقی  " + item + " در متن فرمول استفاده شده اما مقداری برای آن ارسال نشده است ";
                                return ret;
                            }
                        }
                        if (key.Contains("stl_"))
                        {
                            if (req.VariableList.Any(i => i.Key.ToLower().Trim() == key))
                            {
                                VariableList.Add(key, Convert.ToDouble(req.VariableList.Single(i => i.Key.Trim().ToLower() == key).Value));
                                if (req.BuildTreeTrace == true)
                                {
                                    var settlementItem = _settlementItemService.GetIdAsync(Convert.ToInt64(key.Replace("stl_", "")));
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " آیتم تسویه حساب " + settlementItem.Result.title);
                                }
                            }
                            else
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = " آیتم تسویه حساب " + item + " در متن فرمول استفاده شده اما مقداری برای آن ارسال نشده است ";
                                return ret;
                            }
                        }
                        if (key.Contains("basetableval_"))
                        {
                            if (VariableList.Any(i => i.Key.ToLower().Trim() == key))
                            {

                            }
                            else
                            {
                                VariableList.Add(key, Convert.ToDouble(key.Replace("basetableval_", "")));
                                if (req.BuildTreeTrace == true)
                                {
                                    var baseTableValue = _baseTableValueService.GetIdAsync(Convert.ToInt64(key.Replace("basetableval_", "")));
                                    ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, baseTableValue.Result.title);
                                }
                            }
                        }

                        if (key.Contains("operand_"))
                        {
                            var formulaOperand = _formulaOperandService
                                .All(ImpleDate: req.StartDate.Value)
                                .AsNoTracking()
                                .Where(i => i.Id == Convert.ToInt32(item.Replace("operand_", "")))
                                .ToList();
                            if (formulaOperand == null)
                            {
                                ret.Succees = false;
                                ret.ResponseMessage = "عملوند " + item + " استفاده شده ولی تعریف آن یافت نشد یا منقضی شده است ";
                                return ret;
                            }
                            else
                            {
                                if (formulaOperand.Any())
                                {
                                    var operandEnglishName = formulaOperand.Single().EnglishName.Trim().ToLower();
                                    if (req.VariableList.Any(i => i.Key.Trim().ToLower() == operandEnglishName))
                                    {
                                        VariableList.Add(key, Convert.ToDouble(req.VariableList.Single(i => i.Key.Trim().ToLower() == operandEnglishName).Value));
                                        if (req.BuildTreeTrace == true)
                                        {
                                            var formulaOperandf = _formulaOperandService.GetIdAsync(Convert.ToInt32(item.Replace("operand_", ""))).Result.title;
                                            ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, "  " + formulaOperandf);
                                        }
                                    }
                                    else
                                    {
                                        ret.Succees = false;
                                        ret.ResponseMessage = "عملوند " + item + " در متن فرمول استفاده شده اما مقداری برای آن ارسال نشده است ";
                                        return ret;
                                    }
                                }
                                else
                                {
                                    ret.Succees = false;
                                    ret.ResponseMessage = "عملوند " + item + " استفاده شده اما مقداری برای آن ارسال نشده است ";
                                    return ret;
                                }
                            }

                        }

                        if (key.Contains("tbl_"))
                        {
                            var currentFormulaBaseTable = _formulaTableService.GetIdAsync(Convert.ToUInt32(item.Replace("tbl_", ""))).Result;
                            if (currentFormulaBaseTable.TableTypeId == (long)Enums.TableType.Discrete)
                            {
                                bool isSingleValue = false;
                                if (currentFormulaBaseTable.RelatedContextField == "-1")
                                {
                                    isSingleValue = true;
                                }
                                if (!string.IsNullOrEmpty(currentFormulaBaseTable.RelatedContextField) || isSingleValue)
                                {
                                    if (req.VariableList.Any(i => i.Key.Trim().ToLower() == currentFormulaBaseTable.RelatedContextField.Trim().ToLower()) || isSingleValue)
                                    {
                                        var pc = new PersianCalendar();
                                        int shamsiYear = pc.GetYear(req.StartDate.Value);
                                        decimal DiscreteValue = 0;
                                        var DiscreteValueRef = req.VariableList.Where(i => i.Key.Trim().ToLower() == currentFormulaBaseTable.RelatedContextField.Trim().ToLower());
                                        if (!isSingleValue)
                                        {
                                            if (DiscreteValueRef == null)
                                            {
                                                ret.Succees = false;
                                                ret.ResponseMessage = "مقدار ستون متناظر جهت واکشی مقدار جدول پایه وارد نشده";
                                                return ret;
                                            }
                                            else
                                            {
                                                if (DiscreteValueRef.Any())
                                                {
                                                    if (DiscreteValueRef.Count() == 1)
                                                    {
                                                        DiscreteValue = Convert.ToDecimal(DiscreteValueRef.Single().Value);
                                                    }
                                                    else
                                                    {
                                                        ret.Succees = false;
                                                        ret.ResponseMessage = "مقدار ستون متناظر جهت واکشی مقدار جدول پایه بیش از یک رکورد یافت شد";
                                                        return ret;
                                                    }
                                                }
                                                else
                                                {
                                                    ret.Succees = false;
                                                    ret.ResponseMessage = "مقدار ستون متناظر جهت واکشی مقدار جدول پایه وارد نشده";
                                                    return ret;
                                                }
                                            }
                                        }
                                        var valueRow = _formulaTableValueService
                                            .All(ImpleDate: req.StartDate)
                                            .AsNoTracking()
                                            .Where(i => i.FormulaTableId == currentFormulaBaseTable.Id && i.Year == shamsiYear && (i.DiscreteValue == DiscreteValue || isSingleValue))
                                            .ToList();
                                        if (valueRow == null)
                                        {
                                            ret.Succees = false;
                                            ret.ResponseMessage = "مقدار جدول پایه فرمول با تنظیمات ارسالی یافت نشد";
                                            return ret;
                                        }
                                        else
                                        {
                                            if (valueRow.Any())
                                            {
                                                if (valueRow.Count() == 1)
                                                {
                                                    var singleRow = valueRow.Single();
                                                    if (singleRow.Resultvalue.HasValue)
                                                    {
                                                        VariableList.Add(key, Convert.ToDouble(singleRow.Resultvalue));
                                                        if (req.BuildTreeTrace == true)
                                                        {
                                                            ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " جدول پایه فرمول " + currentFormulaBaseTable.title);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ret.Result = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    ret.Succees = false;
                                                    ret.ResponseMessage = "بیش از یک مقدار برای جدول پایه مورد نظر یافت شد لطفا تنظیمات را اصلاح نمایید";
                                                    return ret;
                                                }

                                            }
                                            else
                                            {

                                                //TODO
                                                if (currentFormulaBaseTable.SetZeroIfNotFound == true)
                                                {
                                                    VariableList.Add(key, 0);
                                                    if (req.BuildTreeTrace == true)
                                                    {
                                                        ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " جدول پایه فرمول " + currentFormulaBaseTable.title);
                                                    }
                                                    ret.Result = 0;
                                                }
                                                else
                                                {
                                                    ret.Succees = false;
                                                    ret.ResponseMessage = "مقدار جدول پایه فرمول با تنظیمات ارسالی یافت نشد";
                                                    return ret;
                                                }



                                            }
                                        }

                                    }
                                    else
                                    {
                                        ret.Succees = false;
                                        ret.ResponseMessage = "مقدار جدول پایه فرمول با تنظیمات ارسالی یافت نشد";
                                        return ret;
                                    }
                                }
                                else
                                {
                                    ret.Succees = false;
                                    ret.ResponseMessage = "ستون متناظر در تعریف جدول پایه فرمول گسسته وارد نشده است";
                                    return ret;
                                }
                            }
                        }

                        if (key.Contains("systemfun_"))
                        {
                            engine.AddFunction(key, sqlfunctionRunner);
                            bool haspara = false;
                            if (formulatext.Contains(key + "("))
                            {
                                haspara = true;
                                formulatext = formulatext.Replace(key + "(", key + "(" + key.Replace("systemfun_", "") + ",");
                            }
                            if (formulatext.Contains(key + " ("))
                            {
                                haspara = true;
                                formulatext = formulatext.Replace(key + " (", key + "(" + key.Replace("systemfun_", "") + ",");
                            }
                            if (formulatext.Contains(key + "( "))
                            {
                                haspara = true;
                                formulatext = formulatext.Replace(key + "( ", key + "(" + key.Replace("systemfun_", "") + ",");
                            }
                            if (formulatext.Contains(key + " ( "))
                            {
                                haspara = true;
                                formulatext = formulatext.Replace(key + " ( ", key + "(" + key.Replace("systemfun_", "") + ",");
                            }
                            if (haspara == false)
                            {
                                formulatext = formulatext.Replace(key, key + "(" + key.Replace("systemfun_", "") + ")");
                            }
                            var id = Convert.ToInt64(key.Replace("systemfun_", ""));
                            if (req.BuildTreeTrace == true)
                            {

                                var formulaDatabaseFunctionDefinition = _formulaDatabaseFunctionDefinitionService.GetIdAsync(id).Result.title;
                                ret.FormulaFriendlyText = ret.FormulaFriendlyText.Replace(key, " تابع پایگاه داده  - " + formulaDatabaseFunctionDefinition);
                            }
                            if (VariableFriendlyListFoDataBaseFunctions.Any(i => i.Key == id))
                            {
                                //      VariableFriendlyListFoDataBaseFunctions.Single(i=>i.Key == id).Value = "محاسبه نشده";
                            }
                            else
                            {
                                VariableFriendlyListFoDataBaseFunctions.Add(id, "محاسبه نشده");
                            }

                        }
                    }
                    DateTime dt = DateTime.Now;
                    ret.Result = Math.Round(engine.Calculate(formulatext.Trim().ToLower(), VariableList));



                    if (req.BuildTreeTrace == true)
                    {
                        ret.FormulaText = formulatext;

                        if (VariableFriendlyListFoDataBaseFunctionsToAppend != null)
                        {
                            if (VariableFriendlyListFoDataBaseFunctionsToAppend.Any())
                            {
                                foreach (var item in VariableFriendlyListFoDataBaseFunctionsToAppend)
                                {
                                    var key = " تابع پایگاه داده : " + item.Key;

                                    if (ret.VariableFriendlyList != null)
                                    {
                                        if (!ret.VariableFriendlyList.Any(i => i.Key == key))
                                        {
                                            ret.VariableFriendlyList.Add(key, Convert.ToInt64(item.Value).ToString("#,##0"));
                                        }
                                    }
                                }
                            }

                        }
                    }
                    ret.SuccessRunTimeInmilliseconds = (int)(DateTime.Now - dt).TotalMilliseconds;
                    ret.Succees = true;
                    ret.ResponseMessage = "Ok";

                    if (req.BuildTreeTrace == true)
                    {
                        if (ret.FormulaTreeParser == null)
                        {
                            var orgFormula = _systemSettingContext.OrganisationFormulas.AsNoTracking().Single(i => i.Id == currentFormula.Single().Id);
                            var mainBaseFormula = _formulaService.Formulas.Find(orgFormula.FormulaId).title;

                            if (ret.FormulaTreeParser == null)
                            {
                                ret.FormulaTreeParser = new FormulaExecutionTree()
                                {
                                    FormulaName = mainBaseFormula,
                                    FormulaText = formulatext,
                                    FormulaHelpDesc = currentFormula.Single().Description
                                };
                                ret.FormulaTreeParser.ChildList = new List<FormulaExecutionTree>() { };
                            }
                        }
                        ret.FormulaTreeParser.FormulaFriendlyText = ret.FormulaFriendlyText;
                        ret.FormulaTreeParser.Result = ret.Result;
                        ret.FormulaTreeParser.SuccessRunTimeInmilliseconds = ret.SuccessRunTimeInmilliseconds;
                    }


                    return ret;

                }
                else
                {
                    ret.Succees = false;
                    ret.ResponseMessage = "شناسه فرمول ارسالی یافت نشد";
                    return ret;
                }
            }
            else
            {
                ret.Succees = false;
                ret.ResponseMessage = "شناسه فرمول ارسالی یافت نشد";
                return ret;
            }
        }
        catch (Exception ex)
        {
            ret.Succees = false;
            ret.ResponseMessage = ex.Message;
            return ret;
        }

    }

    public OperationResult GetFormulaCountsByOrganisation()
    {
        var query = _unitOfWork.Context.FormulaDefinitions
            .Include(d => d.OrganisationFormula)
            .ThenInclude(of => of.OrganisationChart)
            .AsNoTracking()
            .GroupBy(d => new
            {
                OrganisationChartId = d.OrganisationFormula != null ? (long?)d.OrganisationFormula.OrganisationChartId : null,
                Organisation = d.OrganisationFormula != null && d.OrganisationFormula.OrganisationChart != null ? d.OrganisationFormula.OrganisationChart.title : ""
            })
            .Select(g => new
            {
                OrganisationChartId = g.Key.OrganisationChartId,
                Organisation = g.Key.Organisation,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return OperationResult.Succeeded(payload: query);
    }
    private double inOperator(double[] a)
    {
        List<double> WithoutFirstArgumentList = new List<double>();
        for (int i = 1; i < a.Length; i++)
        {
            WithoutFirstArgumentList.Add(a[i]);
        }
        if (WithoutFirstArgumentList.Contains(a[0]))
        {
            return 1;
        }
        return 0;
    }
    private double sqlfunctionRunner(double[] a = null)
    {
        try
        {
            var currentSqlFunction = _formulaDatabaseFunctionDefinitionService.All().Where(i => i.Id == a[0]).ToList();
            if (currentSqlFunction == null)
            {
                throw new Exception("شناسه تابع پایگاه داده ارسالی یافت نشد");
            }
            else
            {
                if (currentSqlFunction.Any())
                {
                    var Sqlfunction = currentSqlFunction.SingleOrDefault();
                    List<JsonPara> parameters = new List<JsonPara>();
                    if (!string.IsNullOrEmpty(Sqlfunction.ParamsJson))
                    {
                        parameters = JsonConvert.DeserializeObject<List<JsonPara>>(Sqlfunction.ParamsJson);
                    }
                    if (Sqlfunction.FuctionTypeId == (long)Enums.SqlFunctionType.SqlFunction)
                    {
                        using (SqlConnection con = new SqlConnection(_connectionString))
                        {
                            using (var command = con.CreateCommand())
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "[" + Sqlfunction.Schema + "].[" + Sqlfunction.FunctionName + "]";
                                int index = 1;
                                foreach (var parameter in parameters)
                                {
                                    command.Parameters.AddWithValue("@" + parameter.ParamName, a[index]);
                                    index++;
                                }
                                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                                returnValue.Direction = ParameterDirection.ReturnValue;
                                con.Open();
                                command.ExecuteNonQuery();
                                con.Close();
                                if (!VariableFriendlyListFoDataBaseFunctions.Any(i => i.Key == Sqlfunction.Id))
                                {
                                    VariableFriendlyListFoDataBaseFunctionsToAppend.Add(Sqlfunction.title, returnValue.Value.ToString());
                                }
                                else
                                {
                                    VariableFriendlyListFoDataBaseFunctionsToAppend.Remove(Sqlfunction.title);
                                    VariableFriendlyListFoDataBaseFunctionsToAppend.Add(Sqlfunction.title, returnValue.Value.ToString());
                                }
                                if (returnValue.Value == DBNull.Value)
                                {
                                    return Convert.ToDouble(0);
                                }
                                else
                                {
                                    return Convert.ToDouble(returnValue.Value);
                                }

                            }
                        }
                    }

                    if (Sqlfunction.FuctionTypeId == (long)Enums.SqlFunctionType.DynamicQuery)
                    {
                        if (!string.IsNullOrEmpty(Sqlfunction.ParamsJson))
                        {
                            StringBuilder sb = new StringBuilder();
                            int index = 1;
                            foreach (var parameter in parameters)
                            {
                                sb.AppendLine(" DECLARE @" + parameter.ParamName.Trim().ToLower() + " FLOAT = " + a[index]);
                                index++;
                            }
                            if (!string.IsNullOrEmpty(Sqlfunction.Body))
                            {
                                sb.AppendLine(Sqlfunction.Body);
                                using (SqlConnection con = new SqlConnection(_connectionString))
                                {
                                    var SQL = sb.ToString();
                                    var sqlCmd = new SqlCommand(SQL, con);
                                    con.Open();
                                    using (SqlDataReader rdr = sqlCmd.ExecuteReader())
                                    {
                                        while (rdr.Read())
                                        {
                                            string resp = rdr["id"].ToString();
                                            if (!VariableFriendlyListFoDataBaseFunctions.Any(i => i.Key == Sqlfunction.Id))
                                            {
                                                VariableFriendlyListFoDataBaseFunctionsToAppend.Add(Sqlfunction.title, resp);
                                            }
                                            else
                                            {
                                                VariableFriendlyListFoDataBaseFunctionsToAppend.Remove(Sqlfunction.title);
                                                VariableFriendlyListFoDataBaseFunctionsToAppend.Add(Sqlfunction.title, resp);
                                            }
                                            return Convert.ToDouble(resp);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("بدنه تابع پایگاه داده ای پویا وارد نشده است");
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("شناسه تابع پایگاه داده ارسالی یافت نشد");
                }
            }
            throw new Exception("شناسه تابع پایگاه داده ارسالی یافت نشد");
        }
        catch (Exception ex)
        {

            throw;
        }

    }



    public bool Validate(FormulaDefinition entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
