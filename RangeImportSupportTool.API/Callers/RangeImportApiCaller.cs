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

        public async Task<IList<RangeImport>> ReturnRangeImportModels(IList<RangeImport> rangeImportList)
        {
            foreach (var ticket in rangeImportList)
            {
                try
                {
                    var requestedItemsResponse = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + $"{ticket.Id}/requested_items");
                    var ticketInfoResponse = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + $"{ticket.Id}");
                    var conversationInfoResponse = await ApiServiceHttpClient.HttpClientReturn().GetAsync(ApiServiceHttpClient.BaseAddress + $"{ticket.Id}/conversations");

                    string requestedItemsResponseContent = await requestedItemsResponse.Content.ReadAsStringAsync();
                    string ticketInfoResponseContent = await ticketInfoResponse.Content.ReadAsStringAsync();
                    string conversationInfoResponseContent = await conversationInfoResponse.Content.ReadAsStringAsync();

                    JObject requestedItemsJsonParse = JObject.Parse(requestedItemsResponseContent);
                    JObject ticketInfoJsonParse = JObject.Parse(ticketInfoResponseContent);
                    JObject conversationInfoJsonParse = JObject.Parse(conversationInfoResponseContent);

                    // Sets up the list for the TargetUsage to be populated, needs to be tidied up in future
                    JArray targetUsageJArray = (JArray)requestedItemsJsonParse["requested_items"][0]["custom_fields"]["what_is_the_target_usage_of_the_products_in_this_range"];
                    List<string> targetUsageList = targetUsageJArray.Select(c => (string)c).ToList();

                    RangeImport rangeImportModel = new()
                    {
                        Id = ticket.Id,
                        TargetUsage = targetUsageList, 
                        Action = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_tasks_do_you_require.[0]").Value<string>(),
                        RetailerName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_name").Value<string>(),
                        BatchName = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.retailer_range_name").Value<string>(),
                        DownloadLinkID = ticketInfoJsonParse.SelectToken("ticket.attachments.[0].attachment_url").Value<string>(),
                        MatchedAfterDate = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.match_after_date_if_required").Value<string>(),
                        TargetMarket = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.which_target_market_is_this_for").Value<string>(),
                        UsePreferredSupplier = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.use_preferred_supplier_matching").Value<string>(),
                        PurposeId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.purpose_id_if_required").Value<string>(),
                        NumberOfReplies = conversationInfoJsonParse.SelectToken("meta.count").Value<int>()
                    };

                    // Check if the range import is a new batch or not
                    if (rangeImportModel.Action.Equals("New report"))
                    {
                        rangeImportModel.BatchId = 0;
                        rangeImportModel.IsNewReport = true; 
                    }
                    else
                    {
                        rangeImportModel.BatchId = requestedItemsJsonParse.SelectToken("requested_items.[0].custom_fields.batch_id_if_existing").Value<int>();
                    }

                    // If matched after date is required, then amend the required value to true
                    if (rangeImportModel.MatchedAfterDate != null)
                        rangeImportModel.MatchedAfterDateRequired = true;

                    // A temporary measure to ensure that tickets without a file uploaded are not completely ignored by the application
                    // The NumberOfReplies set to 5 will move it to the "Manual Checks" section of the application, and the download is unavailable there
                    // Leading to no download being able to be completed, and the user can manually check for the issue
                    if (rangeImportModel.DownloadLinkID == null)
                    {
                        rangeImportModel.DownloadLinkID = "0";
                        rangeImportModel.NumberOfReplies = 5; 

                    }

                    _rangeImportList.Add(rangeImportModel);
                }
                catch (Exception e)
                {
                    // Logging/catching to be implemented later
                    // Need to implement error handling when the downloadlinkid is empty
                }
            }

            return _rangeImportList;
        }
    }
}
