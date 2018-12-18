using Neptuo.Logging;
using NuGet.Configuration;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PackageManager.Cli
{
    class Program : SelfUpdateService.IApplication, ProcessService.IApplication
    {
        static Task Main(string[] args) => new Program().MainAsync(args);

        public Args Args { get; private set; }

        SelfUpdateService.IArgs SelfUpdateService.IApplication.Args => Args;
        object ProcessService.IApplication.Args => Args;

        public async Task MainAsync(string[] args)
        {
            Args = new Args(args);

            if (!Directory.Exists(Args.Path))
            {
                Console.WriteLine("Missing argument '--path' - a target path to install packages to.");
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

        private UpdatesViewModel CreateUpdatesViewModel()
        {
            var log = new DefaultLog();

            var contentService = new NuGetPackageContentService(log);
            var packageSources = new NuGetPackageSourceCollection(new PackageSourceProvider(new Settings(Environment.CurrentDirectory)));
            var packageSourceSelector = new AllPackageSourceSelector(packageSources);
            var versionService = new NuGetPackageVersionService(contentService, log);

            var repositoryFactory = new NuGetSourceRepositoryFactory();
            var installService = new NuGetInstallService(repositoryFactory, log, Args.Path, contentService, versionService);
            var searchService = new NuGetSearchService(repositoryFactory, log, contentService, versionService);
            var selfPackageConfiguration = new SelfPackageConfiguration(Args.SelfPackageId);
            var selfUpdateService = new SelfUpdateService(this, new ProcessService(this, Array.Empty<string>()));

            UpdatesViewModel viewModel = new UpdatesViewModel(packageSourceSelector, installService, searchService, selfPackageConfiguration, selfUpdateService);
            return viewModel;
        }

        public void Shutdown() 
            => Environment.Exit(0);
    }
}
