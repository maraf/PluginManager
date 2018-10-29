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
        internal INuGetPackageSourceProvider Provider { get; }
        internal List<NuGetPackageSource> Sources { get; }

        public event Action Changed;

        public IPackageSource Primary => Sources.FirstOrDefault(s => s.Name == Provider.ActivePackageSourceName);
        public IReadOnlyCollection<IPackageSource> All => Sources;

        public NuGetPackageSourceCollection(INuGetPackageSourceProvider provider)
        {
            Ensure.NotNull(provider, "provider");
            this.Provider = provider;

            provider.PackageSourcesChanged += OnProviderChanged;
            Sources = provider.LoadPackageSources().Select(s => new NuGetPackageSource(s)).ToList();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            Provider.PackageSourcesChanged -= OnProviderChanged;
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

        public IPackageSourceBuilder Add()
            => new NuGetPackageSourceBuilder(this);

        public IPackageSourceBuilder Edit(IPackageSource source)
            => new NuGetPackageSourceBuilder(this, EnsureType(source));

        public void Remove(IPackageSource source)
        {
            NuGetPackageSource target = EnsureType(source);
            if (Sources.Remove(target))
                SavePackageSources();
        }

        public void MarkAsPrimary(IPackageSource source)
        {
            if (Provider.ActivePackageSourceName == source?.Name)
                return;

            if (source == null)
                Provider.SaveActivePackageSource(null);
            else
                Provider.SaveActivePackageSource(UnWrap(source));
        }

        internal void SavePackageSources()
            => Provider.SavePackageSources(Sources.Select(s => s.Original));
    }
}
