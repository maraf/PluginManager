using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    internal class MockPackage : IPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }

        public string Authors { get; set; }
        public DateTime? Published { get; set; }
        public string Tags { get; set; }

        public Uri IconUrl { get; set; }
        public Uri ProjectUrl { get; set; }
        public Uri LicenseUrl { get; set; }

        public Task<IPackageContent> DownloadAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
