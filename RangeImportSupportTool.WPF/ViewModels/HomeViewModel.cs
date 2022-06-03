using RangeImportSupportTool.Domain;
using RangeImportSupportTool.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.WPF.ViewModels
{
    public class HomeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public GetRangeImportsCommand GetRangeImportsCommand { get; set; }
        public ObservableCollection<RangeImport> RangeImportList { get; set; } = new ObservableCollection<RangeImport>();

        public HomeViewModel()
        {
            GetRangeImportsCommand = new GetRangeImportsCommand(this);
        }

        public ObservableCollection<RangeImport> OnExecute()
        {
            RangeImportList.Add(new RangeImport
            {
                Id = "1234", 
                TargetUsage = "Digital content",
                Action = "Overwrite",
                RetailerName = "Tesco",
                BatchName = "Main Range",
                BatchId = 1234,
                MatchedAfterDateRequired = false,
                UsePreferredSupplier = "No",
                TargetMarket = "UK", 
                IsNewReport = false,
                NumberOfReplies = 1
            });

            RangeImportList.Add(new RangeImport
            {
                Id = "123222",
                TargetUsage = "Digital content",
                Action = "Overwrite",
                RetailerName = "Tesco",
                BatchName = "Range",
                BatchId = 1232334,
                MatchedAfterDateRequired = false,
                UsePreferredSupplier = "No",
                TargetMarket = "UK",
                IsNewReport = false,
                NumberOfReplies = 1
            });

            RangeImportList.Add(new RangeImport
            {
                Id = "1378",
                TargetUsage = "Digital content",
                Action = "Overwrite",
                RetailerName = "Tesco",
                BatchName = "Main Range",
                BatchId = 15544234,
                MatchedAfterDateRequired = false,
                UsePreferredSupplier = "No",
                TargetMarket = "UK",
                IsNewReport = false,
                NumberOfReplies = 1
            });

            return RangeImportList; 
        }
    }
}
