using Neptuo;
using Neptuo.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class LocalPackageContent : IPackageContent
    {
        private readonly string filePath;

        public LocalPackageContent(string filePath)
        {
            Ensure.Condition.FileExists(filePath, "filePath");
            this.filePath = filePath;
        }

        public Task ExtractToAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Run(
                () =>
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    using (var zip = new ZipArchive(fileStream))
                    {
                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            // TODO: Supported target framework monikers.
                            if (entry.FullName.StartsWith("lib/net45") || entry.FullName.StartsWith("lib/net46") || entry.FullName.StartsWith("lib/netstandard2.0"))
                                entry.ExtractToFile(Path.Combine(path, entry.Name), true);
                        }
                    }
                }, 
                cancellationToken
            );
        }
    }
}
