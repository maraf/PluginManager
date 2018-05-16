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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackageManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Browser.Focus();
        }

        private int lastTabSelectedIndex;

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lastTabSelectedIndex == Tabs.SelectedIndex)
                return;

            lastTabSelectedIndex = Tabs.SelectedIndex;
            TabItem item = (TabItem)Tabs.Items[lastTabSelectedIndex];
            UIElement element = (UIElement)item.Content;

            if (element is IAutoFocus autoFocus)
                autoFocus.Focus();
            else
                element.Focus();
        }
    }
}
