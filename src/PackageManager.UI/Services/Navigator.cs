using Neptuo;
using Neptuo.Activators;
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
        private readonly App application;
        private readonly IFactory<PackageSourceWindow> packageSourceFactory;

        private PackageSourceWindow packageSource;

        public Navigator(App application, IFactory<PackageSourceWindow> packageSourceFactory)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(packageSourceFactory, "packageSourceFactory");
            this.application = application;
            this.packageSourceFactory = packageSourceFactory;
        }

        public void Notify(string title, string message, MessageType type = MessageType.Info)
            => MessageBox.Show(application.MainWindow, message, title, MessageBoxButton.OK, MapTypeToImage(type));

        public bool Confirm(string title, string message, MessageType type = MessageType.Info)
            => MessageBox.Show(application.MainWindow, message, title, MessageBoxButton.YesNo, MapTypeToImage(type)) == MessageBoxResult.Yes;

        private MessageBoxImage MapTypeToImage(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return MessageBoxImage.Information;
                case MessageType.Error:
                    return MessageBoxImage.Error;
                case MessageType.Warning:
                    return MessageBoxImage.Warning;
                default:
                    throw Ensure.Exception.NotSupported(type);
            }
        }

        public enum MessageType
        {
            Info,
            Error,
            Warning
        }

        public void OpenPackageSources()
        {
            if (packageSource == null)
            {
                packageSource = packageSourceFactory.Create();
                packageSource.Closed += OnPackageSourceClosed;
                packageSource.Owner = application.MainWindow;
                packageSource.ShowDialog();
            }
            else
            {
                packageSource.BringIntoView();
            }
        }

        private void OnPackageSourceClosed(object sender, EventArgs e)
        {
            packageSource.Closed -= OnPackageSourceClosed;
            packageSource = null;
        }
    }
}
