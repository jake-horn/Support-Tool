using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RangeImportSupportTool.APIService.Callers;
using RangeImportSupportTool.Domain;
using RangeImportSupportTool.WPF.ViewModels;
using Serilog;
using System;
using System.Configuration;
using System.Windows;

namespace RangeImportSupportTool.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .UseSerilog((host, loggerConfiguration) =>
                {
                    loggerConfiguration.WriteTo.File(ConfigurationManager.AppSettings.Get("RangeImportFolderLocation") + "logs.txt", rollingInterval: RollingInterval.Day);
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Window window = new MainWindow();

            window.DataContext = new MainViewModel();

            window.Show();

            base.OnStartup(e);
        }
    }
}