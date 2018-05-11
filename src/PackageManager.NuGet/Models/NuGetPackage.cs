using Neptuo;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string Authors => source.Authors;
        public DateTime? Published => source.Published?.DateTime;
        public string Tags => source.Tags;

        public Uri IconUrl => source.IconUrl;
        public Uri ProjectUrl => source.ProjectUrl;
        public Uri LicenseUrl => source.LicenseUrl;

        public NuGetPackage(IPackageSearchMetadata source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public async Task<IPackageContent> DownloadAsync()
        {
            var providers = Repository.Provider.GetCoreV3();
            var repository = Repository.CreateSource(providers, "https://www.nuget.org/api/v2/");

            DownloadResource download = repository.GetResource<DownloadResource>();
            if (download == null)
            {
                Console.WriteLine($"Unnable to resolve '{nameof(DownloadResource)}'.");
                return null;
            }

            using (var sourceCacheContext = new SourceCacheContext() { NoCache = true })
            {
                var context = new PackageDownloadContext(sourceCacheContext, Path.GetTempPath(), true);
                var result = await download.GetDownloadResourceResultAsync(source.Identity, context, String.Empty, NullLogger.Instance, default);
                if (result.Status == DownloadResourceResultStatus.Cancelled)
                    throw new OperationCanceledException();
                else if (result.Status == DownloadResourceResultStatus.NotFound)
                    throw new Exception($"Package '{source.Identity.Id}-v{source.Identity.Version}' not found");

                var tempFilePath = $"{Path.GetTempFileName()}.nupkg";
                using (var fileStream = File.OpenWrite(tempFilePath))
                    await result.PackageStream.CopyToAsync(fileStream);

                return new LocalPackageContent(tempFilePath);
            }
        }
    }
}
