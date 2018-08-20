using Neptuo;
using PackageManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PackageManager.Services
{
    internal class Navigator
    {
        private readonly MainWindow wnd;

        public Navigator(MainWindow wnd)
        {
            Ensure.NotNull(wnd, "wnd");
            this.wnd = wnd;
        }

        public void Message(string title, string message)
        {
            MessageBox.Show(wnd, message, title, MessageBoxButton.OK);
        }
    }
}
