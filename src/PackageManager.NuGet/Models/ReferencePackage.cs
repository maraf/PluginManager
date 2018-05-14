using Neptuo;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class ReferencePackage : IPackage
    {
        private readonly PackageReference source;

        public ReferencePackage(PackageReference source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public string Id => source.PackageIdentity.Id;
        public string Version => source.PackageIdentity.Version.ToFullString();
        public string Description => null;

        public string Authors => null;
        public DateTime? Published => null;
        public string Tags => null;

        public Uri IconUrl => null;
        public Uri ProjectUrl => null;
        public Uri LicenseUrl => null;

        public Task<IPackageContent> DownloadAsync(CancellationToken cancellationToken)
            => throw Ensure.Exception.NotSupported();
    }
}
