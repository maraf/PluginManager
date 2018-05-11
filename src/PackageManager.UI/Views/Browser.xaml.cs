using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackageManager.Views
{
    public partial class Browser : UserControl
    {
        public BrowserViewModel ViewModel
        {
            get => (BrowserViewModel)DataContext;
        }

        public Browser()
        {
            InitializeComponent();
        }

        public new void Focus()
            => tbxSearch.Focus();

        private void tbxSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ViewModel.Search.CanExecute(null))
                    ViewModel.Search.Execute(null);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void lvwPackages_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ViewModel.Install.RaiseCanExecuteChanged();
    }
}
