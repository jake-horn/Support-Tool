using Newtonsoft.Json;
using RangeImportSupportTool.Domain.Settings;
using RangeImportSupportTool.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public string SaveOutcome { get; set; } = String.Empty; 

        public SettingsViewModel()
        {
            SaveSettingsCommand = new SaveSettingsCommand(this);
        }

        public void SaveSettings(SettingsViewModel vm)
        {
            var apiKey = SettingsSaver.SaveSettings("ApiKey", vm.ApiKey);
            var downloadLocation = SettingsSaver.SaveSettings("RangeImportFolderLocation", vm.DownloadLocation);

            if (apiKey && downloadLocation)
            {
                SaveOutcome = "Settings saved successfully";
            }
            else
            {
                SaveOutcome = "Settings not saved successfully";
            }

            this.OnPropertyChanged(nameof(SaveOutcome));
        }
    }
}
