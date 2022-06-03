using RangeImportSupportTool.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RangeImportSupportTool.WPF.Commands
{
    public class GetRangeImportsCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public HomeViewModel HomeViewModel;

        public GetRangeImportsCommand(HomeViewModel viewModel)
        {
            HomeViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true; 
        }

        public void Execute(object? parameter)
        {
            HomeViewModel.OnExecute();
        }
    }
}
