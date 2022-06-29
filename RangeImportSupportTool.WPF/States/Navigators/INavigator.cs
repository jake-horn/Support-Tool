using RangeImportSupportTool.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RangeImportSupportTool.WPF.States.Navigators
{
    public enum ViewType
    {
        Home,
        Settings
    }

    public interface INavigator
    {
        ViewModelBase CurrentViewModel { get; set; }
        ICommand UpdateCurrentViewModel { get; }
    }
}
