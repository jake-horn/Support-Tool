using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RangeImportSupportTool.APIService
{
    /* 
     * Need to think about how we are going to populate the models and work out this class correctly
     * It isn't going to work the same as the static class made in the console application
     * we also need to sort out the api config and configuration manager set up
     * */
    public static class ApiServiceHttpClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static HttpClient HttpClientReturn(string apiKey)
        {
            var authToken = Encoding.ASCII.GetBytes(apiKey);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        public static async Task<string> GetApiResponse(string url, string apiKey)
        {
            try
            {
                var response = await HttpClientReturn(apiKey).GetAsync(url);

                return await response.Content.ReadAsStringAsync();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw new InvalidOperationException($"Invalid Operation Exception: {invalidOperationException}"); 
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new HttpRequestException($"Http Request Exception: {httpRequestException}");
            }
            catch (TaskCanceledException taskCanceledException)
            {
                throw new TaskCanceledException($"Task Cancelled Exception: {taskCanceledException}");
            }
        }
    }
}
