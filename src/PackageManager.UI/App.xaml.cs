using NuGet.Frameworks;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels;
using PackageManager.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
            if (!ParseParameters(e.Args))
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);

            NuGetSourceRepositoryFactory repositoryFactory = new NuGetSourceRepositoryFactory();
            NuGetSearchService.IFilter searchFilter = null;
            if (Args.Dependencies.Any())
                searchFilter = new DependencyNuGetSearchFilter(Args.Dependencies, Args.Monikers);

            NuGetPackageContent.IFrameworkFilter frameworkFilter = null;
            if (Args.Monikers.Any())
                frameworkFilter = new NuGetFrameworkFilter(Args.Monikers);

            MainViewModel viewModel = new MainViewModel(
                new NuGetSearchService(repositoryFactory, searchFilter, frameworkFilter),
                new NuGetInstallService(repositoryFactory, Args.Path, frameworkFilter)
            );
            viewModel.PackageSourceUrl = Args.PackageSourceUrl;

            MainWindow wnd = new MainWindow(viewModel);
            wnd.Show();
        }

        private bool ParseParameters(string[] args)
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

            if (!Directory.Exists(Args.Path))
            {
                MessageBox.Show("Missing argument '--path' - a target path to install packages to.", "Packages");
                return false;
            }

            if (Args.Monikers.Count == 0)
            {
                Args.Monikers = new List<NuGetFramework>()
                {
                    NuGetFramework.AnyFramework,
                    FrameworkConstants.CommonFrameworks.Net461
                };
            }

            return true;
        }

        private bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Args.Path = value;
                    return true;
                case "--monikers":
                    Args.Monikers = ParseMonikers(value);
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
                    result[i] = (parts[0], parts[1][0] == 'v' ? parts[1].Substring(1) : parts[1]);
            }

            return result;
        }

        private IReadOnlyCollection<NuGetFramework> ParseMonikers(string arg)
        {
            string[] values = arg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<NuGetFramework> result = new List<NuGetFramework>();
            foreach (string value in values)
            {
                NuGetFramework framework = NuGetFramework.Parse(value, DefaultFrameworkNameProvider.Instance);
                result.Add(framework);
            }

            return result;
        }
    }
}
