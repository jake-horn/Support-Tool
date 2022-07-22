using RangeImportSupportTool.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RangeImportSupportTool.WPF.Commands
{
    public class SaveSettingsCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public SettingsViewModel SettingsViewModel;

        public SaveSettingsCommand(SettingsViewModel viewModel)
        {
            SettingsViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true; 
        }

        public void Execute(object? parameter)
        {
            SettingsViewModel.SaveSettings(SettingsViewModel);
        }
    }
}
