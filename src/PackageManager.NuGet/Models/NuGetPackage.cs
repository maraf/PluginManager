using Neptuo;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackage : IPackage
    {
        private readonly IPackageSearchMetadata source;

        public string Id => source.Identity.Id;
        public string Version => source.Identity.Version.ToFullString();
        public string Description => source.Description;
        public Uri IconUrl => source.IconUrl;

        public IReadOnlyCollection<IPackage> Dependecies => throw new NotImplementedException();

        public NuGetPackage(IPackageSearchMetadata source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }
    }
}
