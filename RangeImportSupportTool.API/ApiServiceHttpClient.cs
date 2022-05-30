using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RangeImportSupportTool.APIService
{
    /* 
     * Need to think about how we are going to populate the models and work out this class correctly
     * It isn't going to work the same as the static class made in the console application
     * we also need to sort out the api config and configuration manager set up
     * */
    public class ApiServiceHttpClient : HttpClient
    {

        public ApiServiceHttpClient()
        {
            this.BaseAddress = new Uri("https://nielsenbrandbank.freshservice.com/api/v2/");
        }
    }
}
