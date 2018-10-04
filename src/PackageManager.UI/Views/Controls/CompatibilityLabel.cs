using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PackageManager.Views.Controls
{
    public class CompatibilityLabel : ContentControl
    {
        public bool IsCompatible
        {
            get => (bool)GetValue(IsCompatibleProperty);
            set => SetValue(IsCompatibleProperty, value);
        }

        public static readonly DependencyProperty IsCompatibleProperty = DependencyProperty.Register(
            "IsCompatible", 
            typeof(bool), 
            typeof(CompatibilityLabel), 
            new PropertyMetadata(true, OnIsCompatibleChanged)
        );

        private static void OnIsCompatibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompatibilityLabel view = (CompatibilityLabel)d;
            view.UpdateVisibility();
        }

        public CompatibilityLabel()
        {
            CreateContent();
            UpdateVisibility();
        }

        private void CreateContent()
        {
            Content = new TextBlock()
            {
                Text = " (Not compatible)",
                ToolTip = "Installed version is not compatible with current a version of the application.",
                Foreground = new SolidColorBrush(Colors.Red)
            };
        }

        private void UpdateVisibility()
            => Visibility = IsCompatible ? Visibility.Collapsed : Visibility.Visible;
    }
}
