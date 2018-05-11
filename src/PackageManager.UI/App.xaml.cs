using PackageManager.Services;
using PackageManager.ViewModels;
using PackageManager.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PackageManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainViewModel viewModel = new MainViewModel(new NuGetSearchService(), new Views.DesignData.MockInstallService());
            viewModel.Browser.Source = "https://api.nuget.org/v3/index.json";

            MainWindow wnd = new MainWindow(viewModel);
            wnd.Show();
        }
    }
}
