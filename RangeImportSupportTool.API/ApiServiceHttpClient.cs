using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RangeImportSupportTool.APIService
{
    public static class ApiServiceHttpClient
    {
        private static readonly HttpClient _httpClient = new();
        public static Uri BaseAddress { get; } = new(ConfigurationManager.AppSettings.Get("BaseUri"));

        public static HttpClient HttpClientReturn()
        {
            string credentials = ConfigurationManager.AppSettings.Get("ApiKey");
            var authToken = Encoding.ASCII.GetBytes(credentials);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }
    }
}
