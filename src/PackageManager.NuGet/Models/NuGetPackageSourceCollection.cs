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

        private NuGetPackageSource EnsureType(IPackageSource source, string argumentName = null)
        {
            Ensure.NotNull(source, argumentName ?? "source");
            if (source is NuGetPackageSource target)
                return target;

            throw new InvalidPackageSourceImplementationException();
        }

        private PackageSource UnWrap(IPackageSource source, string argumentName = null) => EnsureType(source, argumentName).Original;

        public void Add(IPackageSource source)
        {
            NuGetPackageSource target = EnsureType(source);
            sources.Add(target);
            provider.SavePackageSources(sources.Select(s => s.Original));
        }

        public void Remove(IPackageSource source)
        {
            NuGetPackageSource target = EnsureType(source);
            if (sources.Remove(target))
                provider.SavePackageSources(sources.Select(s => s.Original));
        }

        public void MarkAsPrimary(IPackageSource source)
        {
            if (source == null)
                provider.SaveActivePackageSource(null);
            else
                provider.SaveActivePackageSource(UnWrap(source));
        }
    }
}
