using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Neptuo.Activators;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using Neptuo.Logging.Serialization.Formatters;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using PackageManager.Exceptions;
using PackageManager.Logging.Serialization;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels;
using PackageManager.Views;
using PackageManager.Views.Converters;

namespace PackageManager
{
    public partial class App : Application, SelfUpdateService.IApplication, ProcessService.IApplication, IFactory<PackageSourceWindow>, IFactory<LogWindow>
    {
        public Args Args { get; private set; }
        internal ProcessService ProcessService { get; private set; }
        internal IExceptionHandler ExceptionHandler { get; private set; }
        internal Navigator Navigator { get; private set; }
        internal ILogFactory LogFactory { get; private set; }
        internal IPackageSourceCollection PackageSources { get; private set; }
        internal MemoryLogSerializer MemoryLogSerializer { get; private set; }

        SelfUpdateService.IArgs SelfUpdateService.IApplication.Args => Args;
        object ProcessService.IApplication.Args => Args;

        protected override void OnStartup(StartupEventArgs e)
        {
            MemoryLogSerializer = new MemoryLogSerializer(new DefaultLogFormatter());

            LogFactory = new DefaultLogFactory("root")
                .AddConsole()
                .AddSerializer(MemoryLogSerializer);

            ILog log = LogFactory.Scope("Startup");
            log.Debug($"Startup arguments: {Environment.NewLine}{String.Join(" ", e.Args)}");

            Args = new Args(e.Args);

            ProcessService = new ProcessService(this, Args.ProcessNamesToKillBeforeChange ?? new string[0]);
            Navigator = new Navigator(this, this, this);
            BuildExceptionHandler();

            if (!Directory.Exists(Args.Path))
            {
                Navigator.Notify("Packages", "Missing argument '--path' - a target path to install packages to.", Navigator.MessageType.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            PackageSources = new NuGetPackageSourceCollection(new PackageSourceProvider(new Settings(Args.Path)));

            IReadOnlyCollection<NuGetFramework> frameworks = ParseMonikers(Args.Monikers);
            NuGetSourceRepositoryFactory repositoryFactory = new NuGetSourceRepositoryFactory();
            INuGetPackageFilter packageFilter = null;
            if (Args.Dependencies.Any())
                packageFilter = new DependencyNuGetPackageFilter(Args.Dependencies, frameworks);

            NuGetPackageContent.IFrameworkFilter frameworkFilter = null;
            if (Args.Monikers.Any())
                frameworkFilter = new NuGetFrameworkFilter(frameworks);

            var selfPackageConfiguration = new SelfPackageConfiguration(Args.SelfPackageId);

            SelfPackageConverter.Configuration = selfPackageConfiguration;

            var contentService = new NuGetPackageContentService(log, frameworkFilter);
            var versionService = new NuGetPackageVersionService(contentService, log, packageFilter, frameworkFilter);
            var searchService = new NuGetSearchService(repositoryFactory, LogFactory.Scope("Search"), contentService, versionService, packageFilter, frameworkFilter);
            var installService = new NuGetInstallService(repositoryFactory, LogFactory.Scope("Install"), Args.Path, contentService, versionService, packageFilter, frameworkFilter);
            var selfUpdateService = new SelfUpdateService(this, ProcessService);

            EnsureSelfPackageInstalled(installService);

            var viewModel = new MainViewModel(
                PackageSources,
                searchService,
                installService,
                selfPackageConfiguration,
                selfUpdateService
            );

            var wnd = new MainWindow(viewModel, ProcessService, Navigator);

            wnd.Show();

            if (Args.IsSelfUpdate)
                RunSelfUpdate(wnd);
        }

        private void BuildExceptionHandler()
        {
            var exceptionBuilder = new ExceptionHandlerBuilder();

            exceptionBuilder
                .Handler(new LogExceptionHandler(LogFactory.Scope("Exception")));

            var unauthorized = new UnauthorizedExceptionHandler(ProcessService);

            exceptionBuilder
                .Filter<UnauthorizedAccessException>()
                .Handler(unauthorized);

            exceptionBuilder
                .Filter<PackagesConfigWriterException>()
                .Filter(e => e.InnerException is FileNotFoundException)
                .Handler(unauthorized);

            exceptionBuilder
                .Filter<NuGetConfigurationException>()
                .Filter(e => e.InnerException is UnauthorizedAccessException)
                .Handler(unauthorized);

            exceptionBuilder
                .Filter<FatalProtocolException>()
                .Handler(new NuGetFatalProtocolExceptionHandler(Navigator));

            var packageContent = new PackageInstallExceptionHandler(Navigator);
            exceptionBuilder
                .Handler<PackageFileExtractionException>(packageContent)
                .Handler<PackageFileRemovalException>(packageContent);

            exceptionBuilder
                .Handler(new RestartAsAdministratorCancelledExceptionHandler(Navigator, this));

            exceptionBuilder
                .Handler(new MessageExceptionHandler(Navigator))
                .Handler(new ShutdownExceptionHandler(this));

            ExceptionHandler = exceptionBuilder;
        }

        private void EnsureSelfPackageInstalled(NuGetInstallService installService)
        {
            if (Args.SelfPackageId != null)
            {
                SelfPackage package = new SelfPackage(Args.SelfPackageId);
                if (!installService.IsInstalled(package))
                {
                    if (installService.IsInstalled(package.Id))
                        installService.Uninstall(package.Id);

                    installService.Install(package);
                }
            }
        }

        private void RunSelfUpdate(MainWindow wnd)
        {
            UpdatesViewModel updates = wnd.ViewModel.Updates;
            updates.IsPrereleaseIncluded = true;
            updates.Refresh.Completed += async () =>
            {
                bool canUpdate = false;
                PackageUpdateViewModel package = updates.Packages.FirstOrDefault(p => p.Current.Id == Args.SelfPackageId);
                if (package != null)
                {
                    if (package.Target.Version == Args.SelfUpdateVersion)
                    {
                        canUpdate = true;
                    }
                    else
                    {
                        if (package.Current.LoadVersions.CanExecute())
                            package.Current.LoadVersions.Execute();

                        PackageViewModel version = package.Current.Versions.FirstOrDefault(p => p.Version == Args.SelfUpdateVersion);
                        if (version != null)
                        {
                            package.Target = version.Model;
                            canUpdate = true;
                        }
                        else
                        {
                            Navigator.Notify("Self Update Error", $"Unnable to find update package (version {Args.SelfUpdateVersion}) for PackageManager.", Navigator.MessageType.Error);
                        }
                    }
                }
                else
                {
                    Navigator.Notify("Self Update Error", $"Unnable to find update package for PackageManager.", Navigator.MessageType.Error);
                }

                if (canUpdate)
                    await updates.Update.ExecuteAsync(package);

                Shutdown();
            };
            wnd.SelectUpdatesTab();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionHandler.Handle(e.Exception);
            e.Handled = true;
        }

        private IReadOnlyCollection<NuGetFramework> ParseMonikers(IReadOnlyCollection<string> values)
        {
            List<NuGetFramework> result = new List<NuGetFramework>();
            foreach (string value in values)
            {
                NuGetFramework framework = NuGetFramework.Parse(value, DefaultFrameworkNameProvider.Instance);
                result.Add(framework);
            }

            if (result.Count == 0)
            {
                result.Add(NuGetFramework.AnyFramework);
                result.Add(FrameworkConstants.CommonFrameworks.Net461);
            }

            return result;
        }

        PackageSourceWindow IFactory<PackageSourceWindow>.Create()
            => new PackageSourceWindow(new PackageSourceViewModel(PackageSources));

        LogWindow IFactory<LogWindow>.Create()
            => new LogWindow(MemoryLogSerializer);
    }
}
