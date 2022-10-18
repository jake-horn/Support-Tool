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
        public FileDownloader()
        {
            
        }

        public async Task GetFiles(RangeImport rangeImport)
        {
            for(int i = 0; i < rangeImport.DownloadLinkID.Count; i++)
            {
                using (FileStream fileStream = File.Create(ConfigurationManager.AppSettings.Get("RangeImportFolderLocation") + $"{rangeImport.BatchId}" + $"({i})" + ".xlsx"))
                {
                    var request = await ApiServiceHttpClient.HttpClientReturn().GetAsync(rangeImport.DownloadLinkID[i]);
                    var response = await request.Content.ReadAsStreamAsync();

                    await response.CopyToAsync(fileStream);
                }
            }
        }
    }
}
