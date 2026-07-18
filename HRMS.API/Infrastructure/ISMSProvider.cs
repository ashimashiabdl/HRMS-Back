

namespace HRMS.API.Infrastructure;
/// <summary>
/// انواع فراهم کننده پیامک
/// </summary>
public interface ISMSProvider
{

    /// <summary>
    /// ارسال پیامک
    /// </summary>
    /// <param name="Mobile">شماره موبایل</param>
    /// <param name="Message">متن پیام</param>
    /// <returns></returns>
    Task<bool> SendSMS(string Mobile, string Message, string? ValidationCode, long UserId);
}
