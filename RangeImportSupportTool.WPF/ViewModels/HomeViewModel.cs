using RangeImportSupportTool.APIService.Callers;
using RangeImportSupportTool.APIService.Downloaders;
using RangeImportSupportTool.APIService.Senders;
using RangeImportSupportTool.Domain;
using RangeImportSupportTool.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.WPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public GetRangeImportsCommand GetRangeImportsCommand { get; set; }
        public GetDownloadCommand GetDownloadCommand { get; set; }
        public CompletedImportCommand CompletedImportCommand { get; set; }

        private ObservableCollection<RangeImport> MasterRangeImportList { get; set; }
        public ObservableCollection<RangeImport> ExistingRangeImportList { get; set; } 
        public ObservableCollection<RangeImport> NewRangeImportList { get; set; } 
        public ObservableCollection<RangeImport> ManualCheckRangeImportsList { get; set; }

        public Uri TicketLinkUri { get; } = new Uri(ConfigurationManager.AppSettings.Get("LinkUri"));

        public string RangeResponse { get; set; } = String.Empty;

        public HomeViewModel()
        {
            GetRangeImportsCommand = new GetRangeImportsCommand(this);
            GetDownloadCommand = new GetDownloadCommand(this);
            CompletedImportCommand = new CompletedImportCommand(this);
        }

        #region RangeImports
        public async Task GetRangeImports()
        {
            if (ExistingRangeImportList != null || NewRangeImportList != null)
            {
                ExistingRangeImportList.Clear();
                NewRangeImportList.Clear();
                ManualCheckRangeImportsList.Clear();
            }

            TicketIdsCaller TicketIds = new();
            RangeImportApiCaller RangeImports = new();

            MasterRangeImportList = (ObservableCollection<RangeImport>) await TicketIds.GetTicketIds();
            MasterRangeImportList = (ObservableCollection<RangeImport>) await RangeImports.ReturnRangeImportModels(MasterRangeImportList);

            UpdateRangeImportLists(MasterRangeImportList);

            if (MasterRangeImportList.Count == 0)
            {
                RangeResponse = "No ranges to import.";
            }
            else
            {
                RangeResponse = String.Empty;
            }

            this.OnPropertyChanged(nameof(RangeResponse));
        }

        private void UpdateRangeImportLists(IList<RangeImport> rangeImportList)
        {
            ExistingRangeImportList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.IsNewReport == false && x.NumberOfReplies <= 2));
            NewRangeImportList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.IsNewReport == true && x.NumberOfReplies <= 2));
            ManualCheckRangeImportsList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.NumberOfReplies > 2));

            this.OnPropertyChanged(nameof(ExistingRangeImportList));
            this.OnPropertyChanged(nameof(NewRangeImportList));
            this.OnPropertyChanged(nameof(ManualCheckRangeImportsList));
        }

        #endregion

        #region Downloads
        public async Task GetDownload(RangeImport rangeImport)
        {
            FileDownloader fileDownload = new FileDownloader(rangeImport);

            await fileDownload.GetFile();
        }

        #endregion

        #region CompletionTasks

        public async Task SendCompletionMessage(RangeImport rangeImport)
        {
            CompleteMessageSender completeMessageSender = new(rangeImport);

            await completeMessageSender.SendReplyAndCompleteTicket();

            MasterRangeImportList.Remove(rangeImport);
            UpdateRangeImportLists(MasterRangeImportList);
        }

        #endregion
    }
}