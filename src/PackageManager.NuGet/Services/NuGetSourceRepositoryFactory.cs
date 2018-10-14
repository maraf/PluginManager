using Neptuo;
using Neptuo.Activators;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class NuGetSourceRepositoryFactory : IFactory<SourceRepository, IPackageSource>
    {
        public SourceRepository Create(IPackageSource packageSource)
        {
            Ensure.NotNull(packageSource, "packageSource");

            if (packageSource is NuGetPackageSource nuget)
                return Repository.CreateSource(Repository.Provider.GetCoreV3(), nuget.Original);

            return Repository.CreateSource(Repository.Provider.GetCoreV3(), packageSource.Uri.ToString());
        }
    }
}
