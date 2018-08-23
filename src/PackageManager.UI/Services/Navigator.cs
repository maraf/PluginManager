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

        public void Message(string title, string message, MessageType type = MessageType.Info)
            => MessageBox.Show(wnd, message, title, MessageBoxButton.OK, MapTypeToImage(type));

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
