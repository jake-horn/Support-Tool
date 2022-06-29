using RangeImportSupportTool.WPF.Commands;
using RangeImportSupportTool.WPF.Models;
using RangeImportSupportTool.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RangeImportSupportTool.WPF.States.Navigators
{
    public class Navigator : ObservableObject, INavigator
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));

            }
        }

        public ICommand UpdateCurrentViewModel => new UpdateCurrentViewModelCommand(this);
    }
}
