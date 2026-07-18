using HR.BaseInfo.infrastructure.Data;
using Microsoft.Extensions.Options;

namespace HRMS.API.Infrastructure.SMS;

public class KaveNegarSMSProvider(IOptions<KaveNegarSettingModel> config, BaseInfoContext dbContext) : ISMSProvider2, ISMSProvider1
{
    private readonly IOptions<KaveNegarSettingModel> _config = config;
    public string PrividerName
    {
        get
        {
            return "کاوه نگار";
        }
    }
    public async Task<SMSProviderResponse> SendSMS(string Mobile, string Message)
    {

        var response = new SMSProviderResponse();
        try
        {


            Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(_config.Value.ApiKey);
            var result = await api.Send("2000470051", Mobile, Message);
            if (result.Status == 1)
            {
                response.ResponseMessage = result.Message;
                response.IsSuccess = true;
                return response;
            }
            else
            {
                response.ResponseMessage = result.Message;
                response.IsSuccess = false;
                return response;
            }

        }
        catch (Exception ex)
        {
            response.ResponseMessage = ex.Message;
            response.IsSuccess = false;
            return response;
        }

    }
}