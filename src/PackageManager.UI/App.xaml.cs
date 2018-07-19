using NuGet.Frameworks;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels;
using PackageManager.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PackageManager
{
    public partial class App : Application
    {
        public Args Args { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Args = new Args(e.Args);
            if (!Directory.Exists(Args.Path))
            {
                MessageBox.Show("Missing argument '--path' - a target path to install packages to.", "Packages");
                Shutdown();
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
                new NuGetInstallService(repositoryFactory, Args.Path, frameworkFilter),
                new SelfPackageConfiguration(Args.SelfPackageId)
            );
            viewModel.PackageSourceUrl = Args.PackageSourceUrl;

            MainWindow wnd = new MainWindow(viewModel);
            wnd.Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is UnauthorizedAccessException)
            {
                e.Handled = true;
                RestartAsAdministrator();
            }
        }

        private void RestartAsAdministrator()
        {
            Process current = Process.GetCurrentProcess();
            ProcessStartInfo processStart = new ProcessStartInfo(
                current.MainModule.FileName,
                Args.ToString()
            );

            processStart.Verb = "runas";
            Process.Start(processStart);

            Shutdown();
        }
    }
}
