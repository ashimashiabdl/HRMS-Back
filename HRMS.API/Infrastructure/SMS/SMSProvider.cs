
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;

namespace HRMS.API.Infrastructure.SMS;
public class SMSProvider(ISMSProvider1 ISMSProvider1, ISMSProvider2 ISMSProvider2, BaseInfoContext dbContext) : ISMSProvider
{
    private readonly ISMSProvider1 _firstProvider = ISMSProvider1;
    private readonly ISMSProvider2 _secondProvider = ISMSProvider2;
    private readonly BaseInfoContext _dbContext = dbContext;
    /// <summary>
    /// ارسال پیامک بر اساس فراهم کننده های اول و دوم
    /// </summary>
    /// <param name="Mobile">شماره موبایل</param>
    /// <param name="Message">متن پیام</param>
    /// <returns></returns>
    public async Task<bool> SendSMS(string Mobile, string Message, string? ValidationCode, long UserId)
    {
        SendedSMS logSMS = new()
        {
            UserId = UserId,
            IsDeleted = false,
            Provider1Name = _firstProvider.PrividerName,
            Provider2Name = _secondProvider.PrividerName,
            SMSBody = Message,
            MobileNumber = Mobile,
            Provider1ResponseDateTime = DateTime.MaxValue,
            Provider2ResponseDateTime = DateTime.MaxValue,
            Provider1SendDateTime = DateTime.MaxValue,
            Provider2SendDateTime = DateTime.MaxValue,
            ValidationCode = ValidationCode
        };
        if (string.IsNullOrEmpty(Message))
        {
            logSMS.Provider1Response = "متن پیام خالی است";
            logSMS.Provider2Response = "متن پیام خالی است";
            await _dbContext.SendedSMSs.AddAsync(logSMS);
            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        try
        {
            logSMS.Provider1SendDateTime = DateTime.Now;
            var resp = await _firstProvider.SendSMS(Mobile, Message);
            logSMS.Provider1ResponseDateTime = DateTime.Now;
            logSMS.Provider1Response = resp.ResponseMessage;
            if (resp.IsSuccess)
            {
                logSMS.IsSuccess = true;
            }
            else
            {
                logSMS.Provider1Response = logSMS.Provider1Response;
                logSMS.Provider2SendDateTime = DateTime.Now;
                var resp2 = await _secondProvider.SendSMS(Mobile, Message);
                logSMS.Provider2ResponseDateTime = DateTime.Now;
                logSMS.Provider2Response = resp2.ResponseMessage;
                if (resp.IsSuccess)
                {
                    logSMS.IsSuccess = true;
                }
            }
        }
        catch (Exception ex)
        {
            logSMS.Provider1Response = ex.Message;
            logSMS.Provider2SendDateTime = DateTime.Now;
            var resp = await _secondProvider.SendSMS(Mobile, Message);
            logSMS.Provider2ResponseDateTime = DateTime.Now;
            logSMS.Provider2Response = resp.ResponseMessage;
            if (resp.IsSuccess)
            {
                logSMS.IsSuccess = true;
            }
        }
        await _dbContext.SendedSMSs.AddAsync(logSMS);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}