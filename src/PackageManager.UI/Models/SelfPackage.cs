using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    internal class SelfPackage : IPackage
    {
        public string Id { get; }
        public string Version { get; }
        public string Description { get; }
        public string Authors { get; }
        public DateTime? Published { get; }
        public string Tags { get; }
        public Uri IconUrl { get; }
        public Uri ProjectUrl { get; }
        public Uri LicenseUrl { get; }

        public SelfPackage(string id)
        {
            Ensure.NotNull(id, "id");
            Id = id;
            Version = VersionInfo.Version;
        }

        public Task<IPackageContent> GetContentAsync(CancellationToken cancellationToken) 
            => throw Ensure.Exception.NotSupported();
    }
}
