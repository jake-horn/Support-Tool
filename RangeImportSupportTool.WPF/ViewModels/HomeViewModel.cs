using RangeImportSupportTool.APIService.Callers;
using RangeImportSupportTool.APIService.Downloaders;
using RangeImportSupportTool.APIService.Senders;
using RangeImportSupportTool.Domain;
using RangeImportSupportTool.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RangeImportSupportTool.WPF.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        // Commands
        public GetRangeImportsCommand GetRangeImportsCommand { get; set; }
        public GetDownloadCommand GetDownloadCommand { get; set; }
        public CompletedImportCommand CompletedImportCommand { get; set; }

        // Collections
        private ObservableCollection<RangeImport> MasterRangeImportList { get; set; }
        public ObservableCollection<RangeImport> ExistingRangeImportList { get; set; } 
        public ObservableCollection<RangeImport> NewRangeImportList { get; set; } 
        public ObservableCollection<RangeImport> ManualCheckRangeImportsList { get; set; }

        // Misc.
        public string RangeResponse { get; set; } = String.Empty;

        public HomeViewModel()
        {
            GetRangeImportsCommand = new GetRangeImportsCommand(this);
            GetDownloadCommand = new GetDownloadCommand(this);
            CompletedImportCommand = new CompletedImportCommand(this);
        }

        /// <summary>
        /// Primary method used for calling and populating the range import lists. 
        /// </summary>
        public async Task GetRangeImports()
        {
            // Clears the Master list if it isn't null so the lists are repopulated if required. 
            MasterRangeImportList?.Clear();

            TicketIdsCaller TicketIds = new();
            RangeImportApiCaller RangeImports = new();

            MasterRangeImportList = (ObservableCollection<RangeImport>) await TicketIds.GetTicketIds();
            await RangeImports.ReturnRangeImportModels(MasterRangeImportList);

            UpdateRangeImportLists(MasterRangeImportList);

            // To update the text on application if there are no imports to complete. 
            if (MasterRangeImportList.Count == 0)
            {
                RangeResponse = "No ranges to import.";
            }
            else
            {
                RangeResponse = "Range import tickets imported.";
            }

            this.OnPropertyChanged(nameof(RangeResponse));
        }

        /// <summary>
        /// Filters and updates the lists for the View to display correctly. 
        /// </summary>
        /// <param name="rangeImportList"></param>
        private void UpdateRangeImportLists(IList<RangeImport> rangeImportList)
        {
            ExistingRangeImportList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.IsNewReport == false && x.NumberOfReplies <= 2));
            NewRangeImportList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.IsNewReport == true && x.NumberOfReplies <= 2));
            ManualCheckRangeImportsList = new ObservableCollection<RangeImport>(rangeImportList.Where(x => x.NumberOfReplies > 2));

            this.OnPropertyChanged(nameof(ExistingRangeImportList));
            this.OnPropertyChanged(nameof(NewRangeImportList));
            this.OnPropertyChanged(nameof(ManualCheckRangeImportsList));
        }

        /// <summary>
        /// Downloads the file when the "Download" button is clicked. 
        /// </summary>
        /// <param name="rangeImport"></param>
        /// <returns></returns>
        public async Task GetDownload(RangeImport rangeImport)
        {
            FileDownloader fileDownload = new FileDownloader(rangeImport);

            await fileDownload.GetFiles();
        }

        /// <summary>
        /// Sends the completion message and completes the ticket, then removing the RangeImport from the MasterRangeImportList and updates the lists. 
        /// </summary>
        /// <param name="rangeImport"></param>
        /// <returns></returns>
        public async Task SendCompletionMessage(RangeImport rangeImport)
        {
            CompleteMessageSender completeMessageSender = new(rangeImport);

            await completeMessageSender.SendReplyAndCompleteTicket();

            MasterRangeImportList.Remove(rangeImport);
            UpdateRangeImportLists(MasterRangeImportList);
        }
    }
}