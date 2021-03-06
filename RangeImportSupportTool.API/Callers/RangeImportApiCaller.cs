using Newtonsoft.Json.Linq;
using RangeImportSupportTool.Domain;

namespace RangeImportSupportTool.APIService.Callers
{
    public class RangeImportApiCaller
    {
        public RangeImportApiCaller() { }

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

                // Sets up the list for the TargetUsage to be populated, needs to be tidied up in future
                JArray targetUsageJArray = (JArray)requestedItemsJsonParse["requested_items"][0]["custom_fields"]["what_is_the_target_usage_of_the_products_in_this_range"];
                List<string> targetUsageList = targetUsageJArray.Select(c => (string)c).ToList();

                rangeImport.TargetUsage = targetUsageList;
                rangeImport.Action = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_tasks_do_you_require.[0]").Value<string>();
                rangeImport.RetailerName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_name").Value<string>();
                rangeImport.BatchName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_range_name").Value<string>();
                rangeImport.DownloadLinkID = ticketInfoJsonParse.SelectToken("ticket.attachments.[0].attachment_url").Value<string>();
                rangeImport.MatchedAfterDate = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.match_after_date_if_required").Value<string>();
                rangeImport.TargetMarket = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_target_market_is_this_for").Value<string>();
                rangeImport.UsePreferredSupplier = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.use_preferred_supplier_matching").Value<string>();
                rangeImport.PurposeId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.purpose_id_if_required").Value<string>();
                rangeImport.NumberOfReplies = conversationInfoJsonParse.SelectToken("meta.count").Value<int>();
                rangeImport.RequesterEmail = conversationInfoJsonParse.SelectToken("conversations.[0].to_emails.[0]").Value<string>();

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

                // A temporary measure to ensure that tickets without a file uploaded are not completely ignored by the application
                // The NumberOfReplies set to 5 will move it to the "Manual Checks" section of the application, and the download is unavailable there
                // Leading to no download being able to be completed, and the user can manually check for the issue
                if (rangeImport.DownloadLinkID == null)
                {
                    rangeImport.DownloadLinkID = "0";
                    rangeImport.NumberOfReplies = 5;

                }
            }
            catch (Exception e)
            {
                // Logging/catching to be implemented later
                // Need to implement error handling when the downloadlinkid is empty
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
