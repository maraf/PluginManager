using Neptuo;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public partial class NuGetPackageContent : IPackageContent
    {
        private readonly PackageReaderBase reader;
        private readonly IFrameworkFilter filter;

        public NuGetPackageContent(PackageReaderBase reader, IFrameworkFilter filter = null)
        {
            Ensure.NotNull(reader, "reader");

            if (filter == null)
                filter = AnyFrameworkFilter.Instance;

            this.reader = reader;
            this.filter = filter;
        }

        private async Task<IEnumerable<string>> EnumerateFiles(CancellationToken cancellationToken)
        {
            foreach (FrameworkSpecificGroup group in await reader.GetLibItemsAsync(cancellationToken))
            {
                if (filter.IsPassed(group))
                    return group.Items;
            }

            return Enumerable.Empty<string>();
        }

        private string MapPackageFilePath(string rootPath, string packagePath)
            => Path.Combine(rootPath, Path.GetFileName(packagePath));

        public Task ExtractToAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    string ExtractFile(string sourceFile, string targetPath, Stream sourceContent)
                    {
                        string result = MapPackageFilePath(path, targetPath);
                        using (FileStream targetContent = new FileStream(result, FileMode.OpenOrCreate))
                            sourceContent.CopyTo(targetContent);

                        return result;
                    }

                    var packagePaths = await EnumerateFiles(cancellationToken);
                    await reader.CopyFilesAsync(path, packagePaths, ExtractFile, NullLogger.Instance, cancellationToken);
                },
                cancellationToken
            );
        }

        public Task RemoveFromAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    var packagePaths = await EnumerateFiles(cancellationToken);
                    foreach (string packagePath in packagePaths)
                    {
                        string filePath = MapPackageFilePath(path, packagePath);
                        File.Delete(filePath);
                    }
                },
                cancellationToken
            );
        }
    }
}
