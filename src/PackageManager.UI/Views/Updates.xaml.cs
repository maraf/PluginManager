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
    public partial class Updates : UserControl, IAutoFocus
    {
        public UpdatesViewModel ViewModel
        {
            get { return (UpdatesViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(UpdatesViewModel),
            typeof(Updates),
            new PropertyMetadata(null, OnViewModelChanged)
        );

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Updates view = (Updates)d;
            view.OnViewModelChanged((UpdatesViewModel)e.OldValue, (UpdatesViewModel)e.NewValue);
        }

        public Updates()
        {
            InitializeComponent();
        }

        private void OnViewModelChanged(UpdatesViewModel oldValue, UpdatesViewModel newValue)
        {
            if (oldValue != null)
                oldValue.Update.Completed += OnRefresh;

            MainPanel.DataContext = newValue;

            if (newValue != null)
                newValue.Update.Completed += OnRefresh;
        }

        private void OnRefresh()
            => ViewModel.Refresh.Execute();

        async void IAutoFocus.Focus()
        { 
            lvwPackages.Focus();
            await ViewModel.Refresh.ExecuteAsync();
        }
    }
}
