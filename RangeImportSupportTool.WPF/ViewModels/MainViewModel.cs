using RangeImportSupportTool.WPF.States.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.WPF.ViewModels
{
    public class MainViewModel
    {
        public INavigator Navigator { get; set; } = new Navigator();

        public MainViewModel()
        {
            Navigator.UpdateCurrentViewModel.Execute(ViewType.Home);
        }
    }
}
