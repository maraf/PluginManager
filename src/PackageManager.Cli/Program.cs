using PackageManager.Services;
using PackageManager.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PackageManager.Cli
{
    class Program
    {
        public static Args Args { get; private set; }

        static async Task Main(string[] args)
        {
            if (ParseParameters(args))
            {
                if (Args.IsUpdateCount)
                {
                    var repositoryFactory = new NuGetSourceRepositoryFactory();
                    var installService = new NuGetInstallService(repositoryFactory, Args.Path);
                    var searchService = new NuGetSearchService(repositoryFactory);

                    UpdatesViewModel viewModel = new UpdatesViewModel(Args, installService, searchService);
                    await viewModel.Refresh.ExecuteAsync();
                    Console.WriteLine(viewModel.Packages.Count);
                    UpdatesViewModel viewModel = new UpdatesViewModel(Args, installService, searchService);

                }
            }
        }

        private static bool ParseParameters(string[] args)
        {
            Args = new Args();

            int skipped = 0;
            if (args[0] == "update")
            {
                skipped++;
                if (args[1] == "--count")
                {
                    Args.IsUpdateCount = true;
                    skipped++;
                }
            }

            if ((args.Length - skipped) % 2 == 0)
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
                Console.WriteLine("Missing argument '--path' - a target path to install packages to.");
                return false;
            }

            if (!Uri.IsWellFormedUriString(Args.PackageSourceUrl, UriKind.Absolute))
            {
                Console.WriteLine("Missing argument '--packagesource' - a package repository URL.");
                return false;
            }

            return true;
        }

        private static bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Args.Path = value;
                    return true;
                case "--packagesource":
                    Args.PackageSourceUrl = value;
                    return true;
                default:
                    return false;
            }
        }
    }
}
