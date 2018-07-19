using PackageManager.Services;
using PackageManager.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PackageManager.Cli
{
    class Program
    {
        public static Args Args { get; private set; }

        static async Task Main(string[] args)
        {
            Args = new Args(args);

            if (!Directory.Exists(Args.Path))
            {
                Console.WriteLine("Missing argument '--path' - a target path to install packages to.");
                return;
            }

            if (!Uri.IsWellFormedUriString(Args.PackageSourceUrl, UriKind.Absolute))
            {
                Console.WriteLine("Missing argument '--packagesource' - a package repository URL.");
                return;
            }

            if (Args.IsUpdateCount)
            {
                UpdatesViewModel viewModel = CreateUpdatesViewModel();
                await viewModel.Refresh.ExecuteAsync();
                Console.WriteLine(viewModel.Packages.Count);
            }
            else if (Args.IsUpdatePackage)
            {
                UpdatesViewModel viewModel = CreateUpdatesViewModel();
                await viewModel.Refresh.ExecuteAsync();

                PackageUpdateViewModel packageModel = viewModel.Packages.FirstOrDefault(p => p.Latest.Id == Args.PackageId);
                if (packageModel != null && viewModel.Update.CanExecute(packageModel))
                {
                    await viewModel.Update.ExecuteAsync(packageModel);
                    Console.WriteLine($"Updated '{packageModel.Latest.Id}' to version '{packageModel.Latest.Version}'.");
                    Environment.ExitCode = 0;
                }
                else
                {
                    Console.WriteLine($"There is no available update for package '{Args.PackageId}'.");
                    Environment.ExitCode = 1;
                }
            }
        }

        private static UpdatesViewModel CreateUpdatesViewModel()
        {
            var repositoryFactory = new NuGetSourceRepositoryFactory();
            var installService = new NuGetInstallService(repositoryFactory, Args.Path);
            var searchService = new NuGetSearchService(repositoryFactory);
            var selfPackageConfiguration = new SelfPackageConfiguration(Args.SelfPackageId);

            UpdatesViewModel viewModel = new UpdatesViewModel(Args, installService, searchService, selfPackageConfiguration);
            return viewModel;
        }
    }
}
