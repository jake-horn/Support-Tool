using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.APIService.Models
{
    public partial class TicketUpdateJsonModel
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("custom_fields")]
        public CustomFields CustomFields { get; set; }
    }

    public partial class CustomFields
    {
        [JsonProperty("root_cause_reason")]
        public string RootCauseReason { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("impacted_area")]
        public string ImpactedArea { get; set; }

        [JsonProperty("resolving_team")]
        public string ResolvingTeam { get; set; }
    }
}
