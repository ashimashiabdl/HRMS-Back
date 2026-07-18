
using HR.SharedKernel.DTOs;
using HRMS.API.Infrastructure.SMS;

namespace HRMS.API.Infrastructure;

/// <summary>
/// فراهم کننده سرویس دوم پیامک
/// </summary>
public interface ISMSProvider2
{
    /// <summary>
    /// نام فراهم کننده سرویس پیامک
    /// </summary>
    string PrividerName { get; }
    /// <summary>
    /// ارسال پیامک
    /// </summary>
    /// <param name="Mobile">شماره موبایل</param>
    /// <param name="Message">متن پیام</param>
    /// <returns></returns>
    Task<SMSProviderResponse> SendSMS(string Mobile, string Message);
}
