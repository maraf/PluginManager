using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockPackageSourceBuilder : IPackageSourceBuilder
    {
        private readonly MockPackageSourceCollection service;
        private string name;
        private Uri uri;

        public MockPackageSourceBuilder(MockPackageSourceCollection service)
        {
            this.service = service;
        }

        public IPackageSourceBuilder Name(string name)
        {
            this.name = name;
            return this;
        }

        public IPackageSourceBuilder Uri(Uri uri)
        {
            this.uri = uri;
            return this;
        }

        public IPackageSource Save()
        {
            var source = new MockPackageSource(name, uri);
            service.all.Add(source);
            service.RaiseChanged();
            return source;
        }
    }
}
