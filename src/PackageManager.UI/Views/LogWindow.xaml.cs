using Neptuo;
using PackageManager.Logging.Serialization;
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
    public partial class LogWindow : Window
    {
        private readonly MemoryLogSerializer log;

        public LogWindow(MemoryLogSerializer log)
        {
            Ensure.NotNull(log, "log");
            this.log = log;

            InitializeComponent();
            RefreshContent();
        }

        private async void RefreshContent()
        {
            TextContent.Text = log.GetContent();
            if (String.IsNullOrEmpty(TextContent.Text))
                TextContent.Text = "No entries.";

            await Task.Delay(50);
            TextContent.ScrollToEnd();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            log.Clear();
            RefreshContent();
        }

        private void GoToBottom_Click(object sender, RoutedEventArgs e)
            => TextContent.ScrollToEnd();
    }
}
