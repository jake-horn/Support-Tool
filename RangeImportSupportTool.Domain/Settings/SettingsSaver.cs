using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.Domain.Settings
{
    public static class SettingsSaver
    {
        public static bool SaveSettings(string key, string newValue)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                if (!settings[$"{key}"].Value.Equals(newValue))
                {
                    settings[$"{key}"].Value = newValue;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                return true; 
            }
            catch (ConfigurationErrorsException)
            {
                return false; 
            }
        }
    }
}
