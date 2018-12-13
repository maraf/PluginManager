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
    public partial class Browser : UserControl, IAutoFocus
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
            UpdateInitialMessage(true);
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
                newValue.Search.Completed += OnSearchCompleted;
            }
        }

        private void tbxSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ViewModel.Search.CanExecute())
                    ViewModel.Search.Execute();
            }
        }

        private void OnSearchCompleted()
        {
            lvwPackages.Focus();
            lvwPackages.SelectedIndex = 0;
            UpdateInitialMessage(false);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void lvwPackages_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => RaiseCanExecuteChangedOnCommands();

        private void cbxVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => RaiseCanExecuteChangedOnCommands();

        private void UpdateInitialMessage(bool isInitial)
        {
            if (isInitial)
            {
                stpNothing.Visibility = Visibility.Collapsed;
                stpInitial.Visibility = Visibility.Visible;
            }
            else
            {
                stpNothing.Visibility = Visibility.Visible;
                stpInitial.Visibility = Visibility.Collapsed;
            }
        }

        private void RaiseCanExecuteChangedOnCommands()
        {
            ViewModel.Install.RaiseCanExecuteChanged();
            ViewModel.Uninstall.RaiseCanExecuteChanged();
            ViewModel.Reinstall.RaiseCanExecuteChanged();
        }

        void IAutoFocus.Focus()
            => tbxSearch.Focus();
    }
}
