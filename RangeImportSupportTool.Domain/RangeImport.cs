using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.Domain
{
    public class RangeImport
    {
        public string Id { get; set; }

        public List<string> TargetUsage { get; set; }

        public string? Action { get; set; }

        public string? RetailerName { get; set; }

        public string? BatchName { get; set; }

        public int? BatchId { get; set; }

        public List<string>? DownloadLinkID { get; set; } = new List<string>();

        public bool MatchedAfterDateRequired { get; set; } = false;

        public string? MatchedAfterDate { get; set; }

        public string? UsePreferredSupplier { get; set; } 

        public string? TargetMarket { get; set; }

        public string? PurposeId { get; set; }

        public bool IsNewReport { get; set; } = false;

        public int? NumberOfReplies { get; set; }

        public string? RequesterEmail { get; set; }
    }
}
