using Neptuo;
using NuGet.Common;
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

        private async Task<(string frameworkFolderName, IEnumerable<string> packagePaths)> EnumerateFiles(CancellationToken cancellationToken)
        {
            foreach (FrameworkSpecificGroup group in await reader.GetLibItemsAsync(cancellationToken))
            {
                if (filter.IsPassed(group))
                    return (group.TargetFramework.GetShortFolderName(), group.Items);
            }

            return (null, Enumerable.Empty<string>());
        }

        private string MapPackageFilePath(string rootPath, string frameworkFolderName, string packagePath)
        {
            // TODO: Not sure if 100% reliable.

            if (packagePath.StartsWith(rootPath, StringComparison.InvariantCultureIgnoreCase))
                packagePath = packagePath.Substring(rootPath.Length + 1);

            if (packagePath.StartsWith("lib", StringComparison.InvariantCultureIgnoreCase))
                packagePath = packagePath.Substring("lib".Length + 1);

            if (frameworkFolderName != null && packagePath.StartsWith(frameworkFolderName, StringComparison.InvariantCultureIgnoreCase))
                packagePath = packagePath.Substring(frameworkFolderName.Length + 1);

            return Path.Combine(rootPath, packagePath);
        }

        public Task ExtractToAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    var content = await EnumerateFiles(cancellationToken);
                    string ExtractFile(string sourceFile, string targetPath, Stream sourceContent)
                    {
                        string result = MapPackageFilePath(path, content.frameworkFolderName, targetPath);
                        using (FileStream targetContent = new FileStream(result, FileMode.OpenOrCreate))
                            sourceContent.CopyTo(targetContent);

                        return result;
                    }

                    await reader.CopyFilesAsync(path, content.packagePaths, ExtractFile, NullLogger.Instance, cancellationToken);
                },
                cancellationToken
            );
        }

        public Task RemoveFromAsync(string path, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    var content = await EnumerateFiles(cancellationToken);
                    foreach (string packagePath in content.packagePaths)
                    {
                        string filePath = MapPackageFilePath(path, content.frameworkFolderName, packagePath);
                        File.Delete(filePath);
                    }
                },
                cancellationToken
            );
        }
    }
}
