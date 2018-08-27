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
        private readonly App application;

        public Navigator(App application)
        {
            Ensure.NotNull(application, "application");
            this.application = application;
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
    }
}
