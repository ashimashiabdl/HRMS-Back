using HR.BaseInfo.infrastructure.Data;
using HRMS.API.Infrastructure;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace HRMS.API.Infrastructure.SMS;
/// <summary>
/// فراهم کننده سرویس پیامک آسانک
/// </summary>
/// <param name="config"></param>
public class AsanakSMSProvider(IOptions<AsanakSettingModel> config, BaseInfoContext dbContext) : ISMSProvider1, ISMSProvider2
{
    private readonly IOptions<AsanakSettingModel> _config = config;
    public string PrividerName
    {
        get
        {
            return "آسانک";
        }
    }
    public async Task<SMSProviderResponse> SendSMS(string Mobile, string Message)
    {
        SMSProviderResponse resp = new();
        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_config.Value.ServiceURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var nvc = new List<KeyValuePair<string, string>>
            {
                new("username", _config.Value.userName),
                new("password",  _config.Value.passWord),
                new("Source",  _config.Value.Source),
                new("Message", Message),
                new("destination", Mobile),
            };

            HttpResponseMessage response = await client.PostAsync("sendsms", new FormUrlEncodedContent(nvc));
            if (response.IsSuccessStatusCode)
            {
                resp.ResponseMessage = await response.Content.ReadAsStringAsync();
                resp.IsSuccess = true;
                return await Task.FromResult(resp);
            }
            else
            {
                resp.ResponseMessage = "IsSuccessStatusCode Is False";
                resp.IsSuccess = false;
                return await Task.FromResult(resp);
            }
        }
        catch (Exception ex)
        {
            resp.ResponseMessage = ex.Message;
            resp.IsSuccess = false;
            return await Task.FromResult(resp);
        }
    }
}