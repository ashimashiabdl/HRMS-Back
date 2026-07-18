

using HR.SharedKernel.DTOs;
using HRMS.API.Infrastructure.SMS;

namespace HRMS.API.Infrastructure;

/// <summary>
/// فراهم کننده سرویس اول پیامک
/// </summary>
public interface ISMSProvider1
{
    /// <summary>
    /// نام فراهم کننده سرویس پیامک
    /// </summary>
    public string PrividerName { get; }
    /// <summary>
    /// ارسال پیامک
    /// </summary>
    /// <param name="Mobile">شماره موبایل</param>
    /// <param name="Message">متن پیام</param>
    /// <returns></returns>
    Task<SMSProviderResponse> SendSMS(string Mobile, string Message);
}
