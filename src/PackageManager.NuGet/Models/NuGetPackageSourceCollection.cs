using Neptuo;
using Neptuo.Activators;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INuGetPackageSourceProvider = NuGet.Configuration.IPackageSourceProvider;

namespace PackageManager.Models
{
    public class NuGetPackageSourceCollection : DisposableBase, IPackageSourceCollection
    {
        private readonly INuGetPackageSourceProvider provider;
        private readonly List<NuGetPackageSource> sources;

        public event Action Changed;

        public IPackageSource Primary => sources.FirstOrDefault(s => s.Name == provider.ActivePackageSourceName);
        public IReadOnlyCollection<IPackageSource> All => sources;

        public NuGetPackageSourceCollection(INuGetPackageSourceProvider provider)
        {
            Ensure.NotNull(provider, "provider");
            this.provider = provider;

            provider.PackageSourcesChanged += OnProviderChanged;
            sources = provider.LoadPackageSources().Select(s => new NuGetPackageSource(s)).ToList();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            provider.PackageSourcesChanged -= OnProviderChanged;
        }

        private void OnProviderChanged(object sender, EventArgs e) 
            => Changed?.Invoke();

        private NuGetPackageSource EnsureType(IPackageSource source, string argumentName = null)
        {
            Ensure.NotNull(source, argumentName ?? "source");
            if (source is NuGetPackageSource target)
                return target;

            throw new InvalidPackageSourceImplementationException();
        }

        private PackageSource UnWrap(IPackageSource source, string argumentName = null) 
            => EnsureType(source, argumentName).Original;

        public IPackageSource Add(string name, Uri uri)
        {
            var source = new NuGetPackageSource(new PackageSource(uri.ToString(), name));
            sources.Add(source);
            provider.SavePackageSources(sources.Select(s => s.Original));
            return source;
        }

        public void Remove(IPackageSource source)
        {
            NuGetPackageSource target = EnsureType(source);
            if (sources.Remove(target))
                provider.SavePackageSources(sources.Select(s => s.Original));
        }

        public void MarkAsPrimary(IPackageSource source)
        {
            if (provider.ActivePackageSourceName == source?.Name)
                return;

            if (source == null)
                provider.SaveActivePackageSource(null);
            else
                provider.SaveActivePackageSource(UnWrap(source));
        }
    }
}
