using Newtonsoft.Json.Linq;
using RangeImportSupportTool.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.APIService.Callers
{
    public class RangeImportApiCaller
    {
        private IList<RangeImport> _rangeImportList; 

        public RangeImportApiCaller()
        {
            _rangeImportList = new ObservableCollection<RangeImport>();
        }

        public async Task ReturnRangeImportModels(IList<RangeImport> rangeImportList)
        {
            await Task.WhenAll(rangeImportList.Select(x => PopulateModel(x)));
        }

        private async Task PopulateModel(RangeImport ticket)
        {
            try
            {
                JObject requestedItemsJsonParse = await ConvertResponseToJObject($"{ticket.Id}/requested_items");
                JObject ticketInfoJsonParse = await ConvertResponseToJObject($"{ticket.Id}");
                JObject conversationInfoJsonParse = await ConvertResponseToJObject($"{ticket.Id}/conversations");

                // Sets up the list for the TargetUsage to be populated, needs to be tidied up in future
                JArray targetUsageJArray = (JArray)requestedItemsJsonParse["requested_items"][0]["custom_fields"]["what_is_the_target_usage_of_the_products_in_this_range"];
                List<string> targetUsageList = targetUsageJArray.Select(c => (string)c).ToList();

                ticket.TargetUsage = targetUsageList;
                ticket.Action = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_tasks_do_you_require.[0]").Value<string>();
                ticket.RetailerName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_name").Value<string>();
                ticket.BatchName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_range_name").Value<string>();
                ticket.DownloadLinkID = ticketInfoJsonParse.SelectToken("ticket.attachments.[0].attachment_url").Value<string>();
                ticket.MatchedAfterDate = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.match_after_date_if_required").Value<string>();
                ticket.TargetMarket = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_target_market_is_this_for").Value<string>();
                ticket.UsePreferredSupplier = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.use_preferred_supplier_matching").Value<string>();
                ticket.PurposeId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.purpose_id_if_required").Value<string>();
                ticket.NumberOfReplies = conversationInfoJsonParse.SelectToken("meta.count").Value<int>();
                ticket.RequesterEmail = conversationInfoJsonParse.SelectToken("conversations.[0].to_emails.[0]").Value<string>();

                // Check if the range import is a new batch or not
                if (ticket.Action.Equals("New report"))
                {
                    ticket.BatchId = 0;
                    ticket.IsNewReport = true;
                }
                else
                {
                    ticket.BatchId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.batch_id_if_existing").Value<int>();
                }

                // If matched after date is required, then amend the required value to true
                if (ticket.MatchedAfterDate != null)
                    ticket.MatchedAfterDateRequired = true;

                // A temporary measure to ensure that tickets without a file uploaded are not completely ignored by the application
                // The NumberOfReplies set to 5 will move it to the "Manual Checks" section of the application, and the download is unavailable there
                // Leading to no download being able to be completed, and the user can manually check for the issue
                if (ticket.DownloadLinkID == null)
                {
                    ticket.DownloadLinkID = "0";
                    ticket.NumberOfReplies = 5;

                }
            }
            catch (Exception e)
            {
                // Logging/catching to be implemented later
                // Need to implement error handling when the downloadlinkid is empty
            }
        }

        private async Task<JObject> ConvertResponseToJObject(string baseAddressAppend)
        {
            var requestedItemsResponse = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + baseAddressAppend);

            string requestedItemsResponseContent = await requestedItemsResponse.Content.ReadAsStringAsync();

            return JObject.Parse(requestedItemsResponseContent);
        }
    }
}
