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

        public event Action Changed;

        private void RaiseChanged() => Changed?.Invoke();

        public IPackageSource Add(string name, Uri uri)
        {
            var source = new MockPackageSource(name, uri);
            all.Add(source);
            RaiseChanged();
            return source;
        }

        public void MarkAsPrimary(IPackageSource source)
        {
            primary = source;
            RaiseChanged();
        }

        public void Remove(IPackageSource source)
        {
            all.Remove((MockPackageSource)source);
            RaiseChanged();
        }
    }
}
