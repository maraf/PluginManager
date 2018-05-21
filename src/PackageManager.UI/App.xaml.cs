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

            NuGetSourceRepositoryFactory repositoryFactory = new NuGetSourceRepositoryFactory();
            NuGetSearchService.IFilter filter = null;
            if (Args.Dependencies.Any())
                filter = new DependencyNuGetSearchFilter(Args.Dependencies);

            MainViewModel viewModel = new MainViewModel(
                new NuGetSearchService(repositoryFactory, filter),
                new NuGetInstallService(repositoryFactory, Args.Path)
            );
            viewModel.PackageSourceUrl = Args.PackageSourceUrl;

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
                    string name = args[i];
                    string value = args[i + 1];
                    ParseParameter(name, value);
                }
            }
        }

        private bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Args.Path = value;
                    return true;
                case "--monikers":
                    Args.Monikers = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    return true;
                case "--dependencies":
                    Args.Dependencies = ParseDependencies(value);
                    return true;
                case "--packagesource":
                    Args.PackageSourceUrl = value;
                    return true;
                default:
                    return false;
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
