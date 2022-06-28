using RangeImportSupportTool.APIService.Callers;
using RangeImportSupportTool.APIService.Downloaders;
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
    public class HomeViewModel : ViewModelBase
    {
        public GetRangeImportsCommand GetRangeImportsCommand { get; set; }
        public GetDownloadCommand GetDownloadCommand { get; set; }
        public ObservableCollection<RangeImport> ExistingRangeImportList { get; set; } 
        public ObservableCollection<RangeImport> NewRangeImportList { get; set; } 

        public HomeViewModel()
        {
            GetRangeImportsCommand = new GetRangeImportsCommand(this);
            GetDownloadCommand = new GetDownloadCommand(this);
        }

        public async Task GetRangeImports()
        {
            if (ExistingRangeImportList != null || NewRangeImportList != null)
            {
                ExistingRangeImportList.Clear();
                NewRangeImportList.Clear();
            }

            TicketIdsCaller TicketIds = new();
            RangeImportApiCaller RangeImports = new();

            ObservableCollection<RangeImport> list = (ObservableCollection<RangeImport>) await TicketIds.GetTicketIds();
            list = (ObservableCollection<RangeImport>) await RangeImports.ReturnRangeImportModels(list);

            UpdateExistingRangeImportList(list);
            UpdateNewRangeImportList(list);
        }

        private void UpdateExistingRangeImportList(IList<RangeImport> rangeImportList)
        {
            var filteredList = rangeImportList.Where(x => x.IsNewReport == false);

            ExistingRangeImportList = new ObservableCollection<RangeImport>(filteredList);

            this.OnPropertyChanged(nameof(ExistingRangeImportList));
        }

        private void UpdateNewRangeImportList(IList<RangeImport> rangeImportList)
        {
            var filteredList = rangeImportList.Where(x => x.IsNewReport == true);

            NewRangeImportList = new ObservableCollection<RangeImport>(filteredList);

            this.OnPropertyChanged(nameof(NewRangeImportList));
        }

        public async Task GetDownload(RangeImport rangeImport)
        {
            FileDownloader fileDownload = new FileDownloader(rangeImport);

            await fileDownload.GetFile();
        }
    }
}
