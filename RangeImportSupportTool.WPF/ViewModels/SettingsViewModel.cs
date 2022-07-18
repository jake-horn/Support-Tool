using RangeImportSupportTool.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.WPF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        // Commands
        public SaveSettingsCommand SaveSettingsCommand { get; set; }

        // Properties
        public string ApiKey { get; set; } = ConfigurationManager.AppSettings.Get("ApiKey");
        public string DownloadLocation { get; set; } = ConfigurationManager.AppSettings.Get("RangeImportFolderLocation");

        public SettingsViewModel()
        {
            SaveSettingsCommand = new SaveSettingsCommand(this);
        }

        public void SaveSettings()
        {

        }
    }
}
