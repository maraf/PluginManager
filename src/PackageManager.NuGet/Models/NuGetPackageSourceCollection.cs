using Neptuo;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackageSourceCollection : IPackageSourceCollection
    {
        private readonly PackageSourceProvider provider;
        private readonly List<NuGetPackageSource> sources;

        public IPackageSource Primary => throw new NotImplementedException();
        public IReadOnlyCollection<IPackageSource> All => sources;

        public NuGetPackageSourceCollection(PackageSourceProvider provider)
        {
            Ensure.NotNull(provider, "provider");
            this.provider = provider;

            sources = provider.LoadPackageSources().Select(s => new NuGetPackageSource(s)).ToList();
        }

        public void Add(IPackageSource source)
        {
            Ensure.NotNull(source, "source");
            throw new NotImplementedException();
        }

        public void MarkAsPrimary(IPackageSource source)
        {
            throw new NotImplementedException();
        }

        public void Remove(IPackageSource source)
        {
            Ensure.NotNull(source, "source");
            throw new NotImplementedException();
        }
    }
}
