using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default);
    }
}
