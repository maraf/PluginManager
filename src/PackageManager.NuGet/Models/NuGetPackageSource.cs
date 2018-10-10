using Neptuo;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackageSource : IPackageSource
    {
        private readonly PackageSource source;

        public string Name => source.Name;
        public Uri Uri => source.SourceUri;

        public NuGetPackageSource(PackageSource source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }
    }
}
