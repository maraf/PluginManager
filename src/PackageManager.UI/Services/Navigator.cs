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
        private readonly IFactory<LogWindow> logFactory;

        private PackageSourceWindow packageSource;
        private LogWindow log;

        public Navigator(App application, IFactory<PackageSourceWindow> packageSourceFactory, IFactory<LogWindow> logFactory)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(packageSourceFactory, "packageSourceFactory");
            Ensure.NotNull(logFactory, "logFactory");
            this.application = application;
            this.packageSourceFactory = packageSourceFactory;
            this.logFactory = logFactory;
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

        public void OpenLog()
        {
            if (log == null)
            {
                log = logFactory.Create();
                log.Closed += OnLogClosed;
                log.Owner = application.MainWindow;
                log.ShowDialog();
            }
            else
            {
                log.BringIntoView();
            }
        }

        private void OnLogClosed(object sender, EventArgs e)
        {
            log.Closed -= OnLogClosed;
            log = null;
        }
    }
}
