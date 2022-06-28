using RangeImportSupportTool.Domain;
using RangeImportSupportTool.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RangeImportSupportTool.WPF.Commands
{
    public class GetDownloadCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public HomeViewModel HomeViewModel;

        public GetDownloadCommand(HomeViewModel viewModel)
        {
            HomeViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true; 
        }

        public void Execute(object? parameter)
        {
            HomeViewModel.GetDownload((RangeImport)parameter);
        }
    }
}
