using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public static class ViewModelLocator
    {
        private static BrowserViewModel browser;

        public static BrowserViewModel Browser
        {
            get
            {
                if (browser == null)
                {
                    browser = new BrowserViewModel(new MockSearchService())
                    {
                        SearchText = "GitExtensions",
                        Source = "https://api.nuget.org/v3/index.json",
                    };
                    browser.Search.Execute(null);
                }

                return browser;
            }
        }
    }
}
