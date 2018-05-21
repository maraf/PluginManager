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
        private SourceRepository repository;

        public SourceRepository Create(string packageSourceUrl)
        {
            if (repository == null || repository.PackageSource.Source != packageSourceUrl)
                repository = Repository.CreateSource(Repository.Provider.GetCoreV3(), packageSourceUrl);

            return repository;
        }
    }
}
