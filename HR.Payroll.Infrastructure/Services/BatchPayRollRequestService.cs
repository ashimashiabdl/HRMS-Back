using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace HR.Payroll.Infrastructure.Services;

public class BatchPayRollRequestService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<BatchPayRollRequest, PayrollContext, BatchPayRollRequestDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly UserResolverService _userResolverService = userService;

    public OperationResult UpdateRequestState(long BatchPayRollRequestId, Enums.BatchPayRollRequestState NewState)
    {
        var request = _unitOfWork.Context.BatchPayRollRequests.Find(BatchPayRollRequestId);

        if (request == null)
        {
            return OperationResult.NotFound();
        }

        if (request.RequestStateId == (long)Enums.BatchPayRollRequestState.Deleted)
        {
            return OperationResult.Failed("وضعیت درخواست حذف شده می باشد، امکان تغییر وضعیت وجود ندارد");
        }

        // شروع تراکنش برای بروزرسانی همزمان درخواست و دیسکت‌های متناظر
        _unitOfWork.CreateTransaction();
        try
        {
            // بروزرسانی وضعیت درخواست
            request.RequestStateId = (int)NewState;
            request.LastModifiedDate = DateTime.Now;
            _unitOfWork.Context.Update(request);

            // بروزرسانی وضعیت دیسکت بانک متناظر
            if (request.BankDisketteId.HasValue)
            {
                var bankDiskette = _unitOfWork.Context.BankDiskettes.Find(request.BankDisketteId.Value);
                if (bankDiskette != null)
                {
                    var newStatus = MapRequestStateToBankDisketteStatus(NewState, bankDiskette.BankDisketteStatusId);
                    bankDiskette.BankDisketteStatusId = newStatus;
                    bankDiskette.LastModifiedDate = DateTime.Now;
                    _unitOfWork.Context.Update(bankDiskette);
                }
            }

            // بروزرسانی وضعیت دیسکت مالیات متناظر
            if (request.TaxDisketteId.HasValue)
            {
                var taxDiskette = _unitOfWork.Context.TaxDiskettes.Find(request.TaxDisketteId.Value);
                if (taxDiskette != null)
                {
                    var newStatus = MapRequestStateToTaxDisketteStatus(NewState, taxDiskette.TaxDisketteStatusId);
                    taxDiskette.TaxDisketteStatusId = newStatus;
                    taxDiskette.LastModifiedDate = DateTime.Now;
                    _unitOfWork.Context.Update(taxDiskette);
                }
            }

            // بروزرسانی وضعیت دیسکت بیمه متناظر
            if (request.InsuranceDisketteId.HasValue)
            {
                var insuranceDiskette = _unitOfWork.Context.InsuranceDiskettes.Find(request.InsuranceDisketteId.Value);
                if (insuranceDiskette != null)
                {
                    var newStatus = MapRequestStateToInsuranceDisketteStatus(NewState, insuranceDiskette.InsuranceDisketteStatusId);
                    insuranceDiskette.InsuranceDisketteStatusId = newStatus;
                    insuranceDiskette.LastModifiedDate = DateTime.Now;
                    _unitOfWork.Context.Update(insuranceDiskette);
                }
            }

            _unitOfWork.Context.SaveChanges();
            _unitOfWork.Commit();
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی وضعیت: {ex.Message}");
        }
    }

    /// <summary>
    /// نگاشت وضعیت درخواست به وضعیت دیسکت بانک
    /// </summary>
    private long MapRequestStateToBankDisketteStatus(Enums.BatchPayRollRequestState requestState, long currentDisketteStatus)
    {
        return requestState switch
        {
            // فقط برای TryAgain دیسکت را به Initial برمی‌گردانیم
            Enums.BatchPayRollRequestState.TryAgain => (long)Enums.BankDisketteStatus.Initial,
            // برای Deleted و CancelByUser دیسکت را Deleted می‌کنیم
            Enums.BatchPayRollRequestState.Deleted => (long)Enums.BankDisketteStatus.Deleted,
            Enums.BatchPayRollRequestState.CancelByUser => (long)Enums.BankDisketteStatus.Deleted,
            // برای سایر حالات، وضعیت فعلی دیسکت را حفظ می‌کنیم
            _ => currentDisketteStatus
        };
    }

    /// <summary>
    /// نگاشت وضعیت درخواست به وضعیت دیسکت مالیات
    /// </summary>
    private long MapRequestStateToTaxDisketteStatus(Enums.BatchPayRollRequestState requestState, long currentDisketteStatus)
    {
        return requestState switch
        {
            // فقط برای TryAgain دیسکت را به Initial برمی‌گردانیم
            Enums.BatchPayRollRequestState.TryAgain => (long)Enums.TaxDisketteStatus.Initial,
            // برای Deleted و CancelByUser دیسکت را Deleted می‌کنیم
            Enums.BatchPayRollRequestState.Deleted => (long)Enums.TaxDisketteStatus.Deleted,
            Enums.BatchPayRollRequestState.CancelByUser => (long)Enums.TaxDisketteStatus.Deleted,
            // برای سایر حالات، وضعیت فعلی دیسکت را حفظ می‌کنیم
            _ => currentDisketteStatus
        };
    }

    /// <summary>
    /// نگاشت وضعیت درخواست به وضعیت دیسکت بیمه
    /// </summary>
    private long MapRequestStateToInsuranceDisketteStatus(Enums.BatchPayRollRequestState requestState, long currentDisketteStatus)
    {
        return requestState switch
        {
            // فقط برای TryAgain دیسکت را به Initial برمی‌گردانیم
            Enums.BatchPayRollRequestState.TryAgain => (long)Enums.InsuranceDisketteStatus.Initial,
            // برای Deleted و CancelByUser دیسکت را Deleted می‌کنیم
            Enums.BatchPayRollRequestState.Deleted => (long)Enums.InsuranceDisketteStatus.Deleted,
            Enums.BatchPayRollRequestState.CancelByUser => (long)Enums.InsuranceDisketteStatus.Deleted,
            // برای سایر حالات، وضعیت فعلی دیسکت را حفظ می‌کنیم
            _ => currentDisketteStatus
        };
    }
    public OperationResult GetCurrentPeriodEligibleEmployees(long PaymentPeriodId)
    {
        try
        {
            List<GetCurrentPeriodEligibleEmployees_Result> EligibleList = new List<GetCurrentPeriodEligibleEmployees_Result>();

            if (PaymentPeriodId > 0)
            {

            }
            else
            {
                return OperationResult.Succeeded(payload: EligibleList);
            }
            var selectedPeriod = _db.Set<PaymentPeriod>().Find(PaymentPeriodId);
            if (selectedPeriod == null)
            {
                return OperationResult.Succeeded(payload: EligibleList);
            }
            else
            {
                if (selectedPeriod.IsClosed == true)
                {
                    return OperationResult.Failed("دوره انتخابی بسته است و امکان صدور وجود ندارد");
                }
            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[Payroll].[GetCurrentPeriodEligibleEmployees]", con);
                cmd.Parameters.AddWithValue("@PaymentPeriodId", PaymentPeriodId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                int index = 1;
                while (rdr.Read())
                {
                    var toAdd = rdr.ConvertToObject<GetCurrentPeriodEligibleEmployees_Result>();
                    toAdd.Id = index;
                    EligibleList.Add(toAdd);
                    index++;
                }
                con.Close();
            }
            return OperationResult.Succeeded(payload: EligibleList);
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// ایجاد درخواست صدور گروهی
    /// </summary>
    /// <param name="entityToCreate"></param>
    /// <returns></returns>
    public new OperationResult CreateForAsync(BatchPayRollRequestDTO entityToCreate)
    {
        try
        {
            if (_currentUserDefaultOrganId > 0)
            {
                BatchPayRollRequest toAdd = new BatchPayRollRequest();
                toAdd.OrganisationChartId = _currentUserDefaultOrganId;
                toAdd.UserId = entityToCreate.UserId;
                if (entityToCreate.PaymentPeriodId.HasValue)
                {
                    var res = GetCurrentPeriodEligibleEmployees(entityToCreate.PaymentPeriodId.Value);
                    List<GetCurrentPeriodEligibleEmployees_Result> Result = (List<GetCurrentPeriodEligibleEmployees_Result>)res.Payload;
                    if (Result.Count() > 0)
                    {
                        toAdd.RequestStateId = (long)Enums.BatchPayRollRequestState.Initial;
                        toAdd.RequestTypeId = (long)Enums.BatchPayRollRequestType.NormalFicheCalculation;
                        toAdd.PaymentPeriodId = entityToCreate.PaymentPeriodId.Value;
                        // Capture the current user's display name for auditing and UI display
                        var creatorName = _userResolverService.fullname();
                        toAdd.Username = string.IsNullOrWhiteSpace(creatorName) ? _userResolverService.GetUser() : creatorName;
                        toAdd.RequsetDescription = entityToCreate.RequsetDescription;
                        toAdd.EmployeeCount = Result.Count();
                        toAdd.CreateDate = DateTime.Now;
                        toAdd.IPAddress = "";
                        _unitOfWork.CreateTransaction();
                        try
                        {
                            _unitOfWork.Context.Add(toAdd);
                            _unitOfWork.Context.SaveChanges();

                            foreach (var item in Result)
                            {
                                BatchPayRollRequestDetail row = new BatchPayRollRequestDetail();
                                row.BatchPayRollRequestId = toAdd.Id;
                                row.EmployeeId = item.EmployeeId;
                                row.IPAddress = "";
                                row.CreateDate = DateTime.Now;

                                _unitOfWork.Context.Add(row);
                                _unitOfWork.Context.SaveChanges();
                            }
                            _unitOfWork.Save();
                            _unitOfWork.Commit();
                            return OperationResult.Succeeded(payload: toAdd.Id);
                        }
                        catch (Exception ex)
                        {
                            _unitOfWork.Rollback();
                            return OperationResult.Failed();
                        }
                    }
                    else
                    {
                        return OperationResult.Failed("هیچ فردی جهت محاسبه یافت نشد");
                    }
                }
                else
                {
                    return OperationResult.Failed("دوره محاسبه ارسال نشده است");
                }

            }
            else
            {
                return OperationResult.Failed("سازمان پیش فرض مشخض نشده است");
            }
        }
        catch (Exception ex)
        {

            return OperationResult.Failed();
        }

    }
    public new OperationResult DeleteRecord(long Id)
    {
        try
        {
            var request = _unitOfWork.Context.BatchPayRollRequests.Find(Id);
            if (request == null)
            {
                return OperationResult.NotFound();
            }
            if (request.RequestStateId == (long)Enums.BatchPayRollRequestState.Deleted)
            {
                return OperationResult.Failed("وضعیت درخواست مورد نظر حذف شده می باشد");
            }
            else
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("[Payroll].[DeleteDraftFichesByBatchPayRollRequestId]", con);
                    cmd.Parameters.AddWithValue("@BatchPayRollRequestId", Id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                    }
                    con.Close();
                }
                return OperationResult.Succeeded();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public bool Validate(BatchPayRollRequest entity, object etc = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// دریافت یک رکورد با فیلتر بر اساس UserId
    /// اگر کاربر ادمین باشد، همه رکوردها را می‌بیند
    /// در غیر این صورت فقط رکوردهای خودش را می‌بیند
    /// </summary>
    public OperationResult Get(long id, long currentUserId, bool isAdmin)
    {
        var Properties = typeof(BatchPayRollRequest).GetProperties();
        var all = All(false);
        
        foreach (var Property in Properties)
        {
            if (Property.PropertyType.BaseType == typeof(BatchPayRollRequest).BaseType)
            {
                all = all.Include(Property.Name);
            }
        }

        // اگر کاربر ادمین نیست، فقط رکوردهای خودش را ببیند
        if (!isAdmin)
        {
            all = all.Where(i => i.UserId == currentUserId);
        }

        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<BatchPayRollRequestDTO>(row);
        
        if (record == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            PopulateRequestMetadata(record);
            return OperationResult.Succeeded(payload: record);
        }
    }

    /// <summary>
    /// دریافت فهرست صفحه‌بندی شده با فیلتر بر اساس UserId
    /// اگر کاربر ادمین باشد، همه رکوردها را می‌بیند
    /// در غیر این صورت فقط رکوردهای خودش را می‌بیند
    /// </summary>
    public OperationResult GetPagedData(int currentPage, int pageSize, string filter, string activeSortColumn, string Sortdirection, bool IgnoreExpired, long currentUserId, bool isAdmin)
    {
        var Properties = typeof(BatchPayRollRequest).GetProperties();
        var all = All(IgnoreExpired);
        
        foreach (var Property in Properties)
        {
            if (typeof(BatchPayRollRequest).Name == "OrganisationMRT")
            {
                break;
            }
            if (Property.PropertyType.BaseType == typeof(BatchPayRollRequest).BaseType)
            {
                all = all.Include(Property.Name);
            }
        }

        // اگر کاربر ادمین نیست، فقط رکوردهای خودش را ببیند
        if (!isAdmin)
        {
            all = all.Where(i => i.UserId == currentUserId);
            all = all.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
        }

        int rowCount = 0;
        var FlatList = _mapper.Map<List<BatchPayRollRequestDTO>>(PagerUtility<BatchPayRollRequest>.GetPagedData(all, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection)) ?? new List<BatchPayRollRequestDTO>();
        FlatList.ForEach(PopulateRequestMetadata);
        return OperationResult.Succeeded(payload: FlatList, rowCount: rowCount);
    }

    private static void PopulateRequestMetadata(BatchPayRollRequestDTO? dto)
    {
        if (dto == null)
        {
            return;
        }

        dto.RequestState = GetRequestStateTitle(dto.RequestStateId);
        dto.RequestType = GetRequestTypeTitle(dto.RequestTypeId);
    }

    private static string GetRequestStateTitle(long? requestStateId)
    {
        return requestStateId switch
        {
            (long)Enums.BatchPayRollRequestState.Initial => "ایجاد اولیه",
            (long)Enums.BatchPayRollRequestState.EndLoop => "پایان حلقه",
            (long)Enums.BatchPayRollRequestState.TryAgain => "تلاش مجدد",
            (long)Enums.BatchPayRollRequestState.CancelByUser => "انصراف کاربر",
            (long)Enums.BatchPayRollRequestState.Running => "درحال اجرا",
            (long)Enums.BatchPayRollRequestState.Deleted => "حذف شده",
            _ => string.Empty
        };
    }

    private static string GetRequestTypeTitle(long? requestTypeId)
    {
        return requestTypeId switch
        {
            (long)Enums.BatchPayRollRequestType.NormalFicheCalculation => "صدور فیش حقوقی عادی گروهی",
            (long)Enums.BatchPayRollRequestType.ArearsFicheCalculation => "صدور معوقه گروهی",
            (long)Enums.BatchPayRollRequestType.BankDisketteCalculation => "تهیه دیسکت بانک گروهی",
            (long)Enums.BatchPayRollRequestType.InsuranceDisketteCalculation => "تهیه دیسکت بیمه",
            (long)Enums.BatchPayRollRequestType.TaxDisketteCalculation => "تهیه دیسکت مالیات ( دارایی )",
            _ => string.Empty
        };
    }
}
