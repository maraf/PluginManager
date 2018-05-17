using Neptuo.Activators;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class NuGetSourceRepositoryFactory : IFactory<SourceRepository, string>
    {
        public SourceRepository Create(string packageSourceUrl)
        {
            var providers = Repository.Provider.GetCoreV3();
            var repository = Repository.CreateSource(providers, packageSourceUrl);
            return repository;
        }
    }
}
