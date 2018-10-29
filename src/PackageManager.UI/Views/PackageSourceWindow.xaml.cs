using Neptuo;
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
using System.Windows.Shapes;

namespace PackageManager.Views
{
    public partial class PackageSourceWindow : Window
    {
        public PackageSourceViewModel ViewModel => (PackageSourceViewModel)DataContext;

        internal PackageSourceWindow(PackageSourceViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            DataContext = viewModel;

            InitializeComponent();
        }

        private void lvwSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            ViewModel.Edit.RaiseCanExecuteChanged();
            ViewModel.Remove.RaiseCanExecuteChanged();
            ViewModel.MoveUp.RaiseCanExecuteChanged();
            ViewModel.MoveDown.RaiseCanExecuteChanged();
        }
    }
}
