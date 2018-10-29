using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackageSourceBuilder : IPackageSourceBuilder
    {
        private readonly NuGetPackageSourceCollection service;
        private readonly NuGetPackageSource edit;

        private string name;
        private Uri uri;

        internal NuGetPackageSourceBuilder(NuGetPackageSourceCollection service)
        {
            this.service = service;
        }

        internal NuGetPackageSourceBuilder(NuGetPackageSourceCollection service, NuGetPackageSource edit)
        {
            this.service = service;
            this.edit = edit;
            this.name = edit.Name;
            this.uri = edit.Uri;
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
            if (edit == null)
            {
                var source = new NuGetPackageSource(new PackageSource(uri.ToString(), name));
                service.Sources.Add(source);
                service.SavePackageSources();
                return source;
            }
            else if(edit.Name == name)
            {
                edit.Original.Source = uri.ToString();
                service.SavePackageSources();
                return edit;
            }
            else
            {
                int index = service.Sources.IndexOf(edit);
                service.Sources.Remove(edit);

                var source = new NuGetPackageSource(new PackageSource(uri.ToString(), name));
                service.Sources.Insert(index, source);
                service.SavePackageSources();
                return source;
            }
        }
    }
}
