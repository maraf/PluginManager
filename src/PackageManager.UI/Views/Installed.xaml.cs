using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
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
    public partial class Installed : UserControl, IAutoFocus
    {
        public InstalledViewModel ViewModel
        {
            get { return (InstalledViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(InstalledViewModel),
            typeof(Installed),
            new PropertyMetadata(null, OnViewModelChanged)
        );

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Installed view = (Installed)d;
            view.OnViewModelChanged((InstalledViewModel)e.OldValue, (InstalledViewModel)e.NewValue);
        }

        public Installed()
        {
            InitializeComponent();
        }

        private void OnViewModelChanged(InstalledViewModel oldValue, InstalledViewModel newValue)
        {
            if (oldValue != null)
                oldValue.Uninstall.Completed += OnRefresh;

            MainPanel.DataContext = newValue;

            if (newValue != null)
                newValue.Uninstall.Completed += OnRefresh;
        }

        private void OnRefresh()
            => ViewModel.Refresh.Execute();

        void IAutoFocus.Focus()
        {
            ViewModel.Refresh.Execute();
            lvwPackages.Focus();
        }
    }
}
