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
    public class Button
    {
        public static double? GetImageSize(DependencyObject obj)
        {
            return (double?)obj.GetValue(ImageSizeProperty);
        }

        public static void SetImageSize(DependencyObject obj, double? value)
        {
            obj.SetValue(ImageSizeProperty, value);
        }

        public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.RegisterAttached(
            "ImageSize",
            typeof(double?),
            typeof(Button),
            new PropertyMetadata(null, OnImageSizeChanged)
        );

        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double? size = GetImageSize(d);
            if (size != null)
            {
                ContentControl view = (ContentControl)d;
                if ((view.Content is Image image) || (view.Content is StackPanel panel && (image = panel.Children.OfType<Image>().FirstOrDefault()) != null))
                    image.Width = image.Height = size.Value;
            }
        }

        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached(
            "Image",
            typeof(ImageSource),
            typeof(Button),
            new PropertyMetadata(null, OnImageChanged)
        );

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl view = (ContentControl)d;
            UpdateContent(view, null, view.Content as TextBlock);
        }


        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(Button),
            new PropertyMetadata(null, OnTextChanged)
        );

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl view = (ContentControl)d;
            UpdateContent(view, view.Content as Image, null);
        }


        private static void UpdateContent(ContentControl view, Image image, TextBlock text)
        {
            if (image == null)
            {
                ImageSource imageSource = GetImage(view);
                if (imageSource != null)
                {
                    image = new Image()
                    {
                        SnapsToDevicePixels = true,
                        Source = GetImage(view)
                    };
                }
            }

            if (text == null)
            {
                string textSource = GetText(view);
                if (textSource != null)
                {
                    text = new TextBlock()
                    {
                        Text = GetText(view)
                    };
                }
            }

            if (image != null && text != null)
            {
                var panel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
                view.Content = panel;

                image.Width = image.Height = GetImageSize(view) ?? 16;
                panel.Children.Add(image);

                text.Margin = new Thickness(4, 0, 0, 0);
                panel.Children.Add(text);
            }
            else if (image != null)
            {
                if (view.Style == null)
                    view.Style = (Style)Application.Current.Resources["ImageButtonStyle"];

                image.Width = image.Height = GetImageSize(view) ?? 22;
                view.Content = image;
            }
            else if (text != null)
            {
                view.Content = text;
            }
        }
    }
}
