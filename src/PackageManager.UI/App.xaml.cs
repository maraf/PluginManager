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
        public Args Args { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ParseParameters(e.Args);

            base.OnStartup(e);

            MainViewModel viewModel = new MainViewModel(
                new NuGetSearchService(Args.Path), 
                new Views.DesignData.MockInstallService()
            );
            viewModel.Browser.Source = "https://api.nuget.org/v3/index.json";

            MainWindow wnd = new MainWindow(viewModel);
            wnd.Show();
        }

        private void ParseParameters(string[] args)
        {
            Args = new Args();
            if (args.Length % 2 == 0)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    if (args[i] == "--path")
                        Args.Path = args[i + 1];
                    else if (args[i] == "--monikers")
                        Args.Monikers = args[i + 1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    else if (args[i] == "--dependencies")
                        Args.Dependencies = ParseDependencies(args[i + 1]);
                }
            }
        }

        private (string id, string version)[] ParseDependencies(string arg)
        {
            string[] dependencies = arg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            (string id, string version)[] result = new(string id, string version)[dependencies.Length];

            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependency = dependencies[i];

                string[] parts = dependency.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    result[i] = (parts[0], null);
                else
                    result[i] = (parts[0], parts[1]);
            }

            return result;
        }
    }
}
