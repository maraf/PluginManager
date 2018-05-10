using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class BrowserViewModel : ObservableObject
    {
        private string source;
        public string Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
