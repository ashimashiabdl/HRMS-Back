using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class PaymentPeriodService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<PaymentPeriod, PayrollContext, PaymentPeriodDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    /// <summary>
    /// گرفتن تمام دوره های سال جاری
    /// </summary>
    /// <param name="ShamsiYear"></param>
    /// <returns></returns>
    public IQueryable<PaymentPeriod> GetCurrentYearPeriods(int ShamsiYear , int ShamsiMonth)
    {
        return All(IgnoreExpired:false).OrderByDescending(i => i.ShamsiMonth).Where(i => i.ShamsiYear == ShamsiYear && i.ShamsiMonth < ShamsiMonth);
    }
    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All(false).OrderByDescending(i => i.ShamsiYear).ThenByDescending(i=>i.ShamsiMonth).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title +  " ( " + (i.IsClosed == true ? "بسته" : "باز") + " ) " 
        }));
    }

    /// <summary>
    /// بستن دوره حقوقی و تغییر وضعیت تمام فیش‌های متصل به «پرداخت شده»
    /// </summary>
    public OperationResult ClosePeriodAndMarkFichesPayed(long paymentPeriodId)
    {
        var period = _unitOfWork.Context.PaymentPeriods.Find(paymentPeriodId);
        if (period == null)
        {
            return OperationResult.Failed("دوره حقوقی یافت نشد.");
        }

        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("[Payroll].[ClosePaymentPeriodAndMarkFichesPayed]", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@PaymentPeriodId", paymentPeriodId);
        cmd.Parameters.AddWithValue("@PayedFicheStatusId", (long)Enums.FicheStatus.Payed);
        var updatedCountParam = cmd.Parameters.Add("@UpdatedFicheCount", SqlDbType.Int);
        updatedCountParam.Direction = ParameterDirection.Output;
        con.Open();
        cmd.ExecuteNonQuery();

        var updatedCount = updatedCountParam.Value is int count ? count : Convert.ToInt32(updatedCountParam.Value ?? 0);
        return OperationResult.Succeeded(
            $"{updatedCount} فیش به وضعیت پرداخت شده تغییر یافت و دوره بسته شد.",
            updatedCount);
    }
}