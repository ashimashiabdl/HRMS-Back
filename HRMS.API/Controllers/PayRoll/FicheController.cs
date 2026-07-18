using HR.SharedKernel.Attribute;
using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Share;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using HRMS.API.Cache;
using System.ComponentModel;
using Dapper;
using System.Data;

namespace HRMS.API.Controllers.PayRoll;

[Route("api/Fiche")]
[ControllerGroup("PayRoll", "حقوق و دستمزد")]
[DisplayName("فیش حقوقی")]
public class FicheController : AppBaseController
{
    private readonly FicheService _FicheService;
    private readonly ArearFicheService _arearFicheService;
    private readonly ArearsChangedFicheItemService _arearsChangedFicheItemService;
    private readonly IDapper _dapper;
    public FicheController(FicheService Service, ArearsChangedFicheItemService ArearsChangedFicheItemService, ArearFicheService ArearFicheService, ILogger<FicheController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService) : base(UserResolverService, logger, accessor, mapper, dapper)
    {

        _arearsChangedFicheItemService = ArearsChangedFicheItemService;
        _arearFicheService = ArearFicheService;
        _FicheService = Service;
        _dapper = dapper;

        _FicheService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _arearFicheService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        ArearsChangedFicheItemService._currentUserDefaultOrganId = currentUserDefaultOrganId;


    }

    [HttpGet, Route("GetFicheCount")]
    [CustomAccessKey(AccessKey: "GetFicheCount")]
    public IActionResult GetFicheCount()
    {
        // دریافت PayLocationId های کاربر جاری از جدول UserPayLocation
        var query = @"SELECT DISTINCT [PayLocationId] 
                      FROM [Identity].[User_PayLocation] 
                      WHERE [UserId] = @userId 
                        AND [IsDeleted] = 0 
                        AND ([StartDate] IS NULL OR [StartDate] <= CAST(GETDATE() AS DATE))
                        AND ([EndDate] IS NULL OR [EndDate] > CAST(GETDATE() AS DATE))";
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", currentUserId, DbType.Int64);
        
        var payLocationIds = Task.FromResult(_dapper.GetAll<long>(query, parameters, commandType: CommandType.Text)).Result;
        
        long Count = 0;
        if (payLocationIds != null && payLocationIds.Any())
        {
            Count = _FicheService.All(false).Where(i => payLocationIds.Contains(i.OrganisationChartId)).Count();
        }
        
        return this.AppOk(OperationResult.Succeeded(payload: Count));
    }

    [HttpGet("PrintPdf/{Id}/{id1}")]
    [CustomAccessKey(AccessKey: "PrintPdf")]
    public IActionResult PrintPdf(long Id, int id1, [FromQuery] bool isArear = false)
    {
        var resp = _FicheService.DownloadFichePDF(Id, id1 > 0, isArear);
        if (resp.Success == true && resp.Payload != null)
        {
            var stream = new MemoryStream(resp.Payload);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = "fiche.pdf",
            };
        }

        return BadRequest(resp);
    }

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "Get")]

    public IActionResult Get(int id)
    {
        var fiche = _mapper.Map<FicheDTO>(_FicheService.All(IgnoreExpired: false).Include(i => i.PaymentPeriod).Include(i => i.OrganisationChart).Include(i => i.CostCenter).Include(i => i.PeymanRow).Include(i => i.Employee).Include(i => i.InterdictOrder).Include(i => i.EmployeeType).Include(i => i.PersonnelFunction).Include(i => i.FicheStatus).Where(i => i.Id == id).Single());
        var wages = _mapper.Map<List<FicheItemDTO>>(_FicheService._db.Set<FicheItem>()
            .Include(i => i.WageItem)
            .Include(i => i.ArearPaymentPeriod)
            .Where(i => i.FicheId == id));
        fiche.PersonnelLoanPaymentDTOs = _mapper.Map<List<PersonnelLoanPaymentDTO>>(_FicheService._unitOfWork.Context.PersonnelLoanPayments
            .Include(i => i.PersonnelLoan)
            .Include(i => i.PersonnelLoan == null ? null : i.PersonnelLoan.LoanType)
            .Where(i => i.FicheId == id));
        if (fiche.PersonnelLoanPaymentDTOs != null)
        {
            if (fiche.PersonnelLoanPaymentDTOs.Any())
            {
                foreach (var PersonnelLoanPayment in fiche.PersonnelLoanPaymentDTOs)
                {
                    if (wages.Any(i => i.PersonnelLoanId == PersonnelLoanPayment.PersonnelLoanId))
                    {
                        PersonnelLoanPayment.RemainLoanAmount = wages.Single(i => i.PersonnelLoanId == PersonnelLoanPayment.PersonnelLoanId).RemainLoanAmount;
                    }
                }
            }
        }

        // Load leave items
        fiche.FicheLeaveItemDTOs = _mapper.Map<List<FicheLeaveItemDTO>>(_FicheService._unitOfWork.Context.Set<FicheLeaveItem>()
            .Include(i => i.LeaveType)
            .Include(i => i.PersonnelLeave)
            .Where(i => i.FicheId == id)
            .ToList());

        var period = _FicheService._unitOfWork.Context.PaymentPeriods.Find(fiche.PaymentPeriodId);
        List<OrganisationEmployeeTypeFicheItemDTO> executAbleSetting = [];
        var currentFicheSetting = _FicheService.GetComputeSettings(fiche.EmployeeId, new FicheDTO(), period, new HR.Order.Core.Data.RecruitOrder() { PayLocationId = period.OrganisationChartId, EmployeeTypeId = fiche.EmployeeTypeId }, out executAbleSetting);
        fiche.DeductionFicheItems = [];
        fiche.DeductionSumAmount = 0;
        foreach (var item in wages)
        {
            if (executAbleSetting.Any(i => i.WageItemId == item.WageItemId && i.IsEmployerItem != true && i.PaymentTypeId == (long)Enums.PaymentType.Deduction))
            {
                item.IsEmployerItem = false;
                fiche.DeductionFicheItems.Add(item);
              //  fiche.DeductionSumAmount += (long)item.Value;
            }
        }


        //    fiche.DeductionFicheItems = wages.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Deduction ).ToList();
        fiche.PaymentFicheItems = wages.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment).ToList();
        FicheService.ApplyComputedSubItemAmounts(fiche, wages, executAbleSetting);
        fiche.PayableAmountSTR = HR.SharedKernel.Persian_Number_To_String.GET_Number_To_PersianString(fiche.PurePaymentAmount.ToString());
        return this.AppOk(OperationResult.Succeeded(payload: fiche));
    }
    [HttpGet, Route("GetArear/{id}")]
    [CustomAccessKey(AccessKey: "GetArear")]
    public IActionResult GetArear(int id)
    {
        var fiche = _mapper.Map<ArearFicheDTO>(_arearFicheService.All(IgnoreExpired: false).Include(i => i.PaymentPeriod).Include(i => i.PaymentPeriodIntendToPay).Include(i => i.OrganisationChart).Include(i => i.CostCenter).Include(i => i.PeymanRow).Include(i => i.Employee).Include(i => i.InterdictOrder).Include(i => i.EmployeeType).Include(i => i.PersonnelFunction).Include(i => i.FicheStatus).Where(i => i.Id == id).Single());
        var wages = _mapper.Map<List<ArearFicheItemDTO>>(_arearFicheService._db.Set<ArearFicheItem>().Include(i => i.WageItem).Where(i => i.ArearFicheId == id));
        //fiche.PersonnelLoanPaymentDTOs = _mapper.Map<List<PersonnelLoanPaymentDTO>>(_arearFicheService._unitOfWork.Context.PersonnelLoanPayments
        //    .Include(i => i.PersonnelLoan)
        //    .Include(i => i.PersonnelLoan == null ? null : i.PersonnelLoan.LoanType)
        //    .Where(i => i.FicheId == id));
        //if (fiche.PersonnelLoanPaymentDTOs != null)
        //{
        //    if (fiche.PersonnelLoanPaymentDTOs.Any())
        //    {
        //        foreach (var PersonnelLoanPayment in fiche.PersonnelLoanPaymentDTOs)
        //        {
        //            if (wages.Any(i => i.PersonnelLoanId == PersonnelLoanPayment.PersonnelLoanId))
        //            {
        //                PersonnelLoanPayment.RemainLoanAmount = wages.Single(i => i.PersonnelLoanId == PersonnelLoanPayment.PersonnelLoanId).RemainLoanAmount;
        //            }
        //        }
        //    }
        //}

        // Load leave items - Find the original Fiche for this employee and period
        var originalFiche = _arearFicheService._unitOfWork.Context.Set<Fiche>()
            .FirstOrDefault(f => f.EmployeeId == fiche.EmployeeId && f.PaymentPeriodId == fiche.PaymentPeriodId);
        
        if (originalFiche != null)
        {
            fiche.FicheLeaveItemDTOs = _mapper.Map<List<FicheLeaveItemDTO>>(_arearFicheService._unitOfWork.Context.Set<FicheLeaveItem>()
                .Include(i => i.LeaveType)
                .Include(i => i.PersonnelLeave)
                .Where(i => i.FicheId == originalFiche.Id)
                .ToList());
        }

        fiche.DeductionFicheItems = wages.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Deduction).ToList();
        fiche.PaymentFicheItems = wages.Where(i => i.PaymentTypeId == (long)Enums.PaymentType.Payment).ToList();
        fiche.PayableAmountSTR = HR.SharedKernel.Persian_Number_To_String.GET_Number_To_PersianString(fiche.PurePaymentAmount.ToString());
        return this.AppOk(OperationResult.Succeeded(payload: fiche));
    }
    [HttpDelete, Route("DeleteDraftFichesByPaymentPeriodId")]
    [CustomAccessKey(AccessKey: "DeleteDraftFichesByPaymentPeriodId")]
    public IActionResult DeleteDraftFichesByPaymentPeriodId()
    {
        var result = _FicheService.DeleteDraftFichesByPaymentPeriodId(currentUserDefaultPaymentPeiodId);
        return this.AppOk(result);
    }

    /// <summary>
    /// محاسبه فیش موردی برای فقط یک نفر
    /// </summary>
    /// <param name="id">شناسه کارمند (EmployeeId)</param>
    /// <param name="id1"></param>
    /// <returns></returns>
    [HttpGet, Route("CalculateFiche/{id}/{id1}")]
    [CustomAccessKey(AccessKey: "CalculateFiche")]
    [EmployeeAccess(idPropertyName: "id")]
    public IActionResult CalculateFiche(int id, int id1)
    {
        return this.AppOk(_FicheService.CalculateFiche(id, currentUserDefaultPaymentPeiodId, currentUserDefaultOrganId, true, id1 > 0 ? true : false, false));
    }


    [HttpGet, Route("BatchCalculation")]
    [CustomAccessKey(AccessKey: "BatchCalculation")]
    public IActionResult BatchCalculation()
    {
        _FicheService.BatchCalculation();
        return this.AppOk(1);
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "GetPagedData")]

    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null)
    {
        if (EmployeeId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var Filtered = _FicheService._db.Set<Fiche>()
            .Include(i => i.PaymentPeriod)
            .Include(i => i.FicheStatus)
            // .Include(i => i.FicheType)

            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeId == EmployeeId);
        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "id";
        }
        var resp = _FicheService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, EmployeeId: EmployeeId, CustomDataSource: Filtered);
        return this.AppOk(resp);
    }
    [HttpGet, Route("GetArearFichePagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "GetArearFichePagedData")]

    public IActionResult GetArearFichePagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? EmployeeId = null, [FromQuery] long? PaymentPeriodId = null, [FromQuery] long? PaymentPeriodIntendToPayId = null)
    {

        if (PaymentPeriodId == null)
        {
            PaymentPeriodId = 0;
        }
        if (PaymentPeriodIntendToPayId == null)
        {
            PaymentPeriodIntendToPayId = 0;
        }

        if (EmployeeId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var Filtered = _arearFicheService._unitOfWork.Context.ArearFiches
            .Include(i => i.PaymentPeriod)
            .Include(i => i.PaymentPeriodIntendToPay)
            .Include(i => i.FicheStatus)
            .Include(i => i.InterdictOrder)
            .Include(i => i.ArearsChangedFicheItems)
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.EmployeeId == EmployeeId && (PaymentPeriodId == 0 || i.PaymentPeriodId == PaymentPeriodId) && (PaymentPeriodIntendToPayId == 0 || i.PaymentPeriodIntendToPayId == PaymentPeriodIntendToPayId));
        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "id";
        }
        var resp = _arearFicheService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: Filtered);
        return this.AppOk(resp);
    }
    [HttpGet, Route("GetArearArearsChangedFicheItem/{currentPage}/{pageSize}/{ArearFicheId}")]
    [CustomAccessKey(AccessKey: "GetArearArearsChangedFicheItem")]

    public IActionResult GetArearArearsChangedFicheItem(int currentPage = 0, int pageSize = 10, long ArearFicheId = 0)
    {

        if (ArearFicheId > 0)
        {

        }
        else
        {
            return this.AppNotFound();
        }
        var Filtered = _arearFicheService._unitOfWork.Context.ArearsChangedFicheItems
        .Include(i => i.WageItem)
        .Where(i => i.ArearFicheId == ArearFicheId);

        var resp = _arearsChangedFicheItemService.GetPagedData(currentPage: currentPage, pageSize: pageSize, "", "Id", "", false, CustomDataSource: Filtered);
        return this.AppOk(resp);
    }
    [HttpGet, Route("getCurrentPeriodAllFiche/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}/{EmployeeId?}")]
    [CustomAccessKey(AccessKey: "getCurrentPeriodAllFiche")]

    public IActionResult getCurrentPeriodAllFiche(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "")
    {
        var Filtered = _FicheService._db.Set<Fiche>()
            .Include(i => i.PaymentPeriod)
            .Include(i => i.Employee)
            .Include(i => i.InterdictOrder)
            .Include(i => i.FicheStatus)
            // .Include(i => i.FicheType)
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.PaymentPeriodId == currentUserDefaultPaymentPeiodId);
        if (string.IsNullOrEmpty(activeSortColumn))
        {
            activeSortColumn = "id";
        }
        var resp = _FicheService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, false, CustomDataSource: Filtered);
        return this.AppOk(resp);
    }


    [HttpDelete("Delete/{id}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> Delete(int id)
    {
        return this.AppOk(await Task.Run(() => _FicheService.DeleteRecord(id)));
    }
    [HttpDelete("DeleteIgnoreReCalc/{id}")]
    [CustomAccessKey(AccessKey: "delete")]

    public async Task<IActionResult> DeleteIgnoreReCalc(int id)
    {
        return this.AppOk(await Task.Run(() => _FicheService.DeleteRecord(id, true)));
    }
}
