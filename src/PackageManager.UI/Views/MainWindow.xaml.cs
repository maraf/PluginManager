using Neptuo;
using PackageManager.Services;
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
        private readonly ProcessService processes;
        private readonly Navigator navigator;

        public MainViewModel ViewModel
            => (MainViewModel)DataContext;

        internal MainWindow(MainViewModel viewModel, ProcessService processes, Navigator navigator)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(processes, "processes");
            Ensure.NotNull(navigator, "navigator");
            DataContext = viewModel;
            this.processes = processes;
            this.navigator = navigator;
            InitializeViewModel();

            InitializeComponent();
        }

        private void InitializeViewModel()
        {
            ViewModel.Browser.Install.Executing += OnBeforeChange;
            ViewModel.Browser.Uninstall.Executing += OnBeforeChange;
            ViewModel.Installed.Uninstall.Executing += OnBeforeChange;
            ViewModel.Updates.Update.Executing += OnBeforeChange;
        }

        private Task<bool> OnBeforeChange()
        {
            var context = processes.PrepareContextForProcessesKillBeforeChange();
            if (context.IsExecutable)
            {
                bool result = navigator.Confirm(
                    "PluginManager",
                    "Plugin Manager is going to write to files that are holded by other executables. " + Environment.NewLine +
                    "Do you want to kill all instances of these applications?"
                );

                if (result)
                    context.Execute();
            }

            return Task.FromResult(true);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (String.IsNullOrEmpty(ViewModel.PackageSourceUrl))
                PackageSource.Focus();
        }

        public void SelectUpdatesTab()
            => Tabs.SelectedIndex = 2;

        private void PackageSourceSettings_Click(object sender, RoutedEventArgs e) 
            => navigator.OpenPackageSources();
    }
}
