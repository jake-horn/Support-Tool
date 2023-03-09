using Newtonsoft.Json.Linq;
using RangeImportSupportTool.Domain;
using Serilog;

namespace RangeImportSupportTool.APIService.Callers
{
    public class RangeImportApiCaller
    {

        public RangeImportApiCaller()
        {
        }

        public async Task ReturnRangeImportModels(IList<RangeImport> rangeImportList)
        {
            await Task.WhenAll(rangeImportList.Select(x => PopulateRangeImportModel(x)));
        }

        /// <summary>
        /// Populates the RangeImport model. 
        /// </summary>
        /// <param name="rangeImport"></param>
        /// <returns></returns>
        private async Task PopulateRangeImportModel(RangeImport rangeImport)
        {
            try
            {
                JObject requestedItemsJsonParse = await ConvertResponseToJObject($"{rangeImport.Id}/requested_items");
                JObject ticketInfoJsonParse = await ConvertResponseToJObject($"{rangeImport.Id}");
                JObject conversationInfoJsonParse = await ConvertResponseToJObject($"{rangeImport.Id}/conversations");

                // Sets up the list for the TargetUsage to be populated
                JArray targetUsageJArray = (JArray)requestedItemsJsonParse["requested_items"][0]["custom_fields"]["what_is_the_target_usage_of_the_products_in_this_range"];

                // Sets up the list for downloadItems to be populated
                JArray downloadItemsJArray = (JArray)ticketInfoJsonParse["ticket"]["attachments"];

                rangeImport.TargetUsage = targetUsageJArray.Select(c => (string)c).ToList();
                rangeImport.Action = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_tasks_do_you_require.[0]").Value<string>();
                rangeImport.RetailerName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_name").Value<string>();
                rangeImport.BatchName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_range_name").Value<string>();
                rangeImport.MatchedAfterDate = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.match_after_date_if_required").Value<string>();
                rangeImport.TargetMarket = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_target_market_is_this_for").Value<string>();
                rangeImport.UsePreferredSupplier = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.use_preferred_supplier_matching").Value<string>();
                rangeImport.PurposeId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.purpose_id_if_required").Value<string>();
                rangeImport.NumberOfReplies = conversationInfoJsonParse.SelectToken("meta.count").Value<int>();
                rangeImport.RequesterEmail = conversationInfoJsonParse.SelectToken("conversations.[0].to_emails.[0]").Value<string>();

                foreach (JObject item in downloadItemsJArray)
                {
                    rangeImport.DownloadLinkID.Add(item.GetValue("attachment_url").ToString());
                }

                // Check if the range import is a new batch or not
                if (rangeImport.Action.Equals("New report"))
                {
                    rangeImport.BatchId = 0;
                    rangeImport.IsNewReport = true;
                }
                else
                {
                    rangeImport.BatchId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.batch_id_if_existing").Value<int>();
                }

                // If matched after date is required, then amend the required value to true
                if (rangeImport.MatchedAfterDate != null)
                    rangeImport.MatchedAfterDateRequired = true;

                // If there are no downloads, then the ticket needs to be checked as no file has been provided. 
                if (rangeImport.DownloadLinkID.Count == 0)
                {
                    Log.Information($"BatchID {rangeImport.BatchId} has no file attached, manually check the ticket.");
                }

                if (rangeImport.UsePreferredSupplier is null)
                {
                    rangeImport.UsePreferredSupplier = "No"; 
                }
            }
            catch (Exception e)
            {
                Log.Error($"BatchID: {rangeImport.BatchId}. Exception: {e}");
            }
        }

        /// <summary>
        /// Converts the api response into a JObject to be used in populating the RangeImport model within the RangeImportApiCaller class. 
        /// </summary>
        /// <param name="baseAddressAppend">Completes the API address</param>
        /// <returns>JObject of the API response</returns>
        private async Task<JObject> ConvertResponseToJObject(string baseAddressAppend)
        {
            var requestedItemsResponse = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + baseAddressAppend);

            string requestedItemsResponseContent = await requestedItemsResponse.Content.ReadAsStringAsync();

            return JObject.Parse(requestedItemsResponseContent);
        }
    }
}
