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
            get { return (BrowserViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(BrowserViewModel),
            typeof(Browser),
            new PropertyMetadata(null, OnViewModelChanged)
        );

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Browser view = (Browser)d;
            view.OnViewModelChanged((BrowserViewModel)e.OldValue, (BrowserViewModel)e.NewValue);
        }

        public Browser()
        {
            InitializeComponent();
        }

        private void OnViewModelChanged(BrowserViewModel oldValue, BrowserViewModel newValue)
        {
            if (oldValue != null)
            {
                oldValue.Install.Completed -= RaiseCanExecuteChangedOnCommands;
                oldValue.Uninstall.Completed -= RaiseCanExecuteChangedOnCommands;
            }

            MainPanel.DataContext = newValue;

            if (newValue != null)
            {
                newValue.Install.Completed += RaiseCanExecuteChangedOnCommands;
                newValue.Uninstall.Completed += RaiseCanExecuteChangedOnCommands;
            }
        }

        public new void Focus()
            => tbxSearch.Focus();

        private void tbxSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ViewModel.Search.CanExecute())
                    ViewModel.Search.Execute();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void lvwPackages_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => RaiseCanExecuteChangedOnCommands();

        private void RaiseCanExecuteChangedOnCommands()
        {
            ViewModel.Install.RaiseCanExecuteChanged();
            ViewModel.Uninstall.RaiseCanExecuteChanged();
        }
    }
}
