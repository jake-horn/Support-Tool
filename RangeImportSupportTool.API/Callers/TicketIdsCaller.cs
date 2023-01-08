using Newtonsoft.Json.Linq;
using RangeImportSupportTool.Domain;
using System.Collections.ObjectModel;
using Serilog; 

namespace RangeImportSupportTool.APIService.Callers
{
    public class TicketIdsCaller
    {
        private IList<RangeImport> _rangeImportList { get; set; }
        private readonly string _baseAddressAppend = @"filter?query=""tag: RI AND status: 2""";

        public TicketIdsCaller()
        {
            _rangeImportList = new ObservableCollection<RangeImport>();
        }

        /// <summary>
        /// Calls the API and populates RangeImport models with the ticket Id. 
        /// </summary>
        /// <returns>ObservableCollection of RangeImports with the Id property populated.</returns>
        /// <exception cref="Exception">Throws exception if the response is not a success code</exception>
        public async Task<IList<RangeImport>> GetTicketIds()
        {
            using HttpResponseMessage response = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + _baseAddressAppend);

            if (response.IsSuccessStatusCode)
            {
                JObject parseJson = JObject.Parse(await response.Content.ReadAsStringAsync());

                IList<JToken> results = parseJson["tickets"].Children().ToList();

                foreach (JToken item in results)
                {
                    try
                    {
                        RangeImport rangeImport = item.ToObject<RangeImport>();
                        _rangeImportList.Add(rangeImport);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error: {ex}");
                    }

                }

                return _rangeImportList;
            }
            else
            {
                Log.Error($"Exception: {response.ReasonPhrase}");
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
