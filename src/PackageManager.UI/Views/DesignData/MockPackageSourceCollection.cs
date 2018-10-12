using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockPackageSourceCollection : IPackageSourceCollection
    {
        private readonly List<MockPackageSource> all = new List<MockPackageSource>();
        private IPackageSource primary;

        public IPackageSource Primary => primary;
        public IReadOnlyCollection<IPackageSource> All => all;

        public IPackageSource Add(string name, Uri uri)
        {
            var source = new MockPackageSource()
            {
                Name = name,
                Uri = uri
            };
            all.Add(source);
            return source;
        }

        public void MarkAsPrimary(IPackageSource source) => primary = source;
        public void Remove(IPackageSource source) => all.Remove((MockPackageSource)source);
    }
}
