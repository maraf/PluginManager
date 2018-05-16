using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PackageManager.Views.Controls
{
    public class TabControl
    {
        public static int GetLastSelectedIndex(DependencyObject obj)
            => (int)obj.GetValue(LastSelectedIndexProperty);

        public static void SetLastSelectedIndex(DependencyObject obj, int value)
            => obj.SetValue(LastSelectedIndexProperty, value);

        public static readonly DependencyProperty LastSelectedIndexProperty = DependencyProperty.RegisterAttached(
            "LastSelectedIndex", 
            typeof(int), 
            typeof(TabControl), 
            new PropertyMetadata(-1)
        );


        public static bool GetIsAutoFocus(DependencyObject obj)
            => (bool)obj.GetValue(IsAutoFocusProperty);

        public static void SetIsAutoFocus(DependencyObject obj, bool value) 
            => obj.SetValue(IsAutoFocusProperty, value);

        public static readonly DependencyProperty IsAutoFocusProperty = DependencyProperty.RegisterAttached(
            "IsAutoFocus",
            typeof(bool),
            typeof(TabControl),
            new PropertyMetadata(false, OnIsAutoFocusChanged)
        );

        private static void OnIsAutoFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector view = (Selector)d;
            bool isAutoFocus = (bool)e.NewValue;
            if (isAutoFocus)
            {
                view.SelectionChanged += OnSelectionChanged;
                TabSelected(view, view.SelectedIndex);
            }
            else
            {
                view.SelectionChanged -= OnSelectionChanged;
            }
        }


        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector view = (Selector)sender;
            TabSelected(view, view.SelectedIndex);
        }

        private static void TabSelected(Selector sender, int index)
        {
            if (GetLastSelectedIndex(sender) == index)
                return;

            SetLastSelectedIndex(sender, index);

            ContentControl item = (ContentControl)sender.Items[index];
            UIElement element = (UIElement)item.Content;

            if (element is IAutoFocus autoFocus)
                autoFocus.Focus();
            else
                element.Focus();
        }
    }
}
