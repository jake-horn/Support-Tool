using RangeImportSupportTool.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeImportSupportTool.APIService.Downloaders
{
    public class FileDownloader
    {
        private RangeImport _rangeImport;

        public FileDownloader(RangeImport rangeImport)
        {
            _rangeImport = rangeImport;
        }

        public async Task GetFiles()
        {
            for(int i = 0; i < _rangeImport.DownloadLinkID.Count; i++)
            {
                using (FileStream fileStream = File.Create(ConfigurationManager.AppSettings.Get("RangeImportFolderLocation") + $"{_rangeImport.BatchId}" + $"({i})" + ".xlsx"))
                {
                    var request = await ApiServiceHttpClient.HttpClientReturn().GetAsync(_rangeImport.DownloadLinkID[i]);
                    var response = await request.Content.ReadAsStreamAsync();

                    await response.CopyToAsync(fileStream);
                }
            }
        }
    }
}
