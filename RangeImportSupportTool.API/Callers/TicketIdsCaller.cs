using Newtonsoft.Json.Linq;
using RangeImportSupportTool.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.APIService.Callers
{
    public class TicketIdsCaller
    {
        private IList<RangeImport> _rangeImportList { get; set; }

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
            using (HttpResponseMessage response = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ConfigurationManager.AppSettings.Get("TicketIdAddress")))
            {
                if(response.IsSuccessStatusCode)
                {
                    // Converts the response into a string
                    string responseString = await response.Content.ReadAsStringAsync();

                    // Converts the response to a JSON object
                    JObject parseJson = JObject.Parse(responseString);

                    // Adds each ticket to a JToken list
                    List<JToken> results = parseJson["tickets"].Children().ToList();

                    // Creates a RangeImport model for each item in the results list and adds to the _rangeImportList
                    foreach (JToken item in results)
                    {
                        RangeImport rangeImport = item.ToObject<RangeImport>();
                        _rangeImportList.Add(rangeImport);
                    }

                    return _rangeImportList;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
