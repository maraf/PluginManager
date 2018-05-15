using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neptuo;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using PackageManager.Models;

namespace PackageManager.Services
{
    public class NuGetSearchService : ISearchService
    {
        public string Path { get; private set; }

        public NuGetSearchService(string path)
        {
            Ensure.NotNull(path, "path");
            Path = path;
        }

        private SearchOptions EnsureOptions(SearchOptions options)
        {
            if (options == null)
            {
                options = new SearchOptions()
                {
                    PageIndex = 0,
                    PageSize = 10
                };
            }

            return options;
        }

        public async Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            options = EnsureOptions(options);

            var providers = Repository.Provider.GetCoreV3();
            var repository = Repository.CreateSource(providers, packageSourceUrl);

            PackageSearchResource search = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            if (search == null)
                return Enumerable.Empty<IPackage>();

            var result = await search.SearchAsync(searchText, new SearchFilter(false), options.PageIndex * options.PageSize, options.PageSize, NullLogger.Instance, cancellationToken);

            return result.Select(p => new NuGetPackage(p, repository));
        }
    }
}
