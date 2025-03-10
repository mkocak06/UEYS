using Core.Interfaces;
using Core.Models.ConfigModels;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Shared.Models.SMSModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SMSSender : ISMSSender
    {
        private readonly AppSettingsModel _appSettingsModel;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SMSSender(AppSettingsModel appSettingsModel)
        {
            _appSettingsModel = appSettingsModel;
        }
        public async Task SendMessageAsync(SMSModel model)
        {
            HttpClient client_auth = new();
            HttpClient client = new();

            var model_auth = new SMSSettings() { Username = _appSettingsModel.SMS.Username, Password = _appSettingsModel.SMS.Password };
            client_auth.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client_auth.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            string requestObject_auth = JsonConvert.SerializeObject(model_auth);
            var req_auth = new HttpRequestMessage(HttpMethod.Post, _appSettingsModel.SMS.TokenUrl)
            {
                Content = new StringContent
                (
                    requestObject_auth,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            var response_auth = await client.SendAsync(req_auth);

            string output_auth = await response_auth.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            string requestObject = JsonConvert.SerializeObject(model);
            var req = new HttpRequestMessage(HttpMethod.Post, _appSettingsModel.SMS.SMSUrl);
            var smsSettings = JsonConvert.DeserializeObject<SMSSettings>(output_auth);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", smsSettings.Token);
            req.Content = new StringContent(
                requestObject,
                Encoding.UTF8,
                "application/json"
            );
            var result = await client.SendAsync(req);

            var resultContent = await result.Content.ReadAsStringAsync();
            var responseInfo = JsonConvert.DeserializeObject<SMSResponseModel>(resultContent);
        }

        public async Task SendBulkMessageAsync(BulkSMSModel model)
        {
            HttpClient client_auth = new();
            HttpClient client = new();

            var model_auth = new SMSSettings() { Username = _appSettingsModel.SMS.Username, Password = _appSettingsModel.SMS.Password };
            client_auth.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client_auth.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            string requestObject_auth = JsonConvert.SerializeObject(model_auth);
            var req_auth = new HttpRequestMessage(HttpMethod.Post, _appSettingsModel.SMS.TokenUrl)
            {
                Content = new StringContent
                (
                    requestObject_auth,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            var response_auth = await client.SendAsync(req_auth);

            string output_auth = await response_auth.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            var groupedPhoneNumbers = model.PhoneNumbers
            .Select((num, index) => new { num, index })
            .GroupBy(x => x.index / 900)
            .Select(group => group.Select(x => x.num).ToList())
            .ToList();

            // Her grup için SMS gönderimi
            foreach (var group in groupedPhoneNumbers)
            {
                var smsler = new BulkSMSModel()
                {
                    SmsContent = model.SmsContent,
                    PhoneNumbers = group,
                };

                string requestObject = JsonConvert.SerializeObject(new List<BulkSMSModel>() { smsler });
                var req = new HttpRequestMessage(HttpMethod.Post, _appSettingsModel.SMS.BulkSMSUrl);
                var smsSettings = JsonConvert.DeserializeObject<SMSSettings>(output_auth);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", smsSettings.Token);
                req.Content = new StringContent(
                    requestObject,
                    Encoding.UTF8,
                    "application/json"
                );
                var result = await client.SendAsync(req);

                var resultContent = await result.Content.ReadAsStringAsync();
                var responseInfo = JsonConvert.DeserializeObject<List<SMSResponseModel>>(resultContent);
            }
        }
    }
}
