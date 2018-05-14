using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class MainViewModel
    {
        public BrowserViewModel Browser { get; }
        public InstalledViewModel Installed { get; }

        public MainViewModel(ISearchService search, IInstallService install)
        {
            Browser = new BrowserViewModel(search, install);
            Installed = new InstalledViewModel(install);
        }
    }
}
